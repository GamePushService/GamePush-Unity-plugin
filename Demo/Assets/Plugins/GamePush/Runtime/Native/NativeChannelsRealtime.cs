using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    /// <summary>
    /// Keeps an open chat live. The GamePush GraphQL API exposes no subscription token for chat
    /// channels (unlike multiplayer, which hands them out via ConnectPlayerMultiplayer), so new
    /// messages are picked up by re-reading the newest page on an interval. Watch/Stop are the seam:
    /// swapping in a Centrifugo subscription later only touches this file.
    /// </summary>
    public static class NativeChannelsRealtime
    {
        const float DefaultInterval = 3f;

        static NativeChannelsPump _pump;
        static NativeChatScope _scope;
        static int _target;
        static string _tags = "";
        static readonly HashSet<string> Seen = new HashSet<string>();
        static bool _busy;

        public static event Action<NativeChatMessage> MessageReceived;

        public static bool IsWatching => _pump != null && _pump.isActiveAndEnabled;

        public static void Watch(NativeChatScope scope, int target, string tags,
            IEnumerable<NativeChatMessage> known)
        {
            _scope = scope;
            _target = target;
            _tags = tags ?? "";
            Seen.Clear();
            if (known != null)
            {
                foreach (var message in known)
                {
                    if (!string.IsNullOrEmpty(message?.id))
                        Seen.Add(message.id);
                }
            }

            if (_pump == null)
            {
                var host = NativeMainThread.Instance;
                if (host == null)
                    return;
                _pump = host.gameObject.GetComponent<NativeChannelsPump>()
                        ?? host.gameObject.AddComponent<NativeChannelsPump>();
            }
            _pump.Interval = DefaultInterval;
            _pump.enabled = true;
        }

        public static void Stop()
        {
            if (_pump != null)
                _pump.enabled = false;
            Seen.Clear();
            _busy = false;
        }

        /// <summary>Registers a locally sent message so the poll does not echo it back.</summary>
        public static void Acknowledge(NativeChatMessage message)
        {
            if (!string.IsNullOrEmpty(message?.id))
                Seen.Add(message.id);
        }

        internal static void Poll()
        {
            if (_busy || !NativeCore.Ready)
                return;
            _busy = true;
            NativeChannels.FetchMessagesForOverlay(_scope, _target, _tags, 25, 0, page =>
            {
                _busy = false;
                if (page?.items == null)
                    return;
                // The API returns newest first; replay in chronological order.
                for (var i = page.items.Count - 1; i >= 0; i--)
                {
                    var message = page.items[i];
                    if (string.IsNullOrEmpty(message.id) || !Seen.Add(message.id))
                        continue;
                    GP_Channels.NativeFireIncomingMessage(message.json);
                    MessageReceived?.Invoke(message);
                }
            }, _ => _busy = false);
        }
    }

    [DisallowMultipleComponent]
    public sealed class NativeChannelsPump : MonoBehaviour
    {
        public float Interval = 3f;
        float _next;

        void OnEnable() => _next = Time.unscaledTime + Interval;

        void Update()
        {
            if (Time.unscaledTime < _next)
                return;
            _next = Time.unscaledTime + Interval;
            NativeChannelsRealtime.Poll();
        }
    }
}
