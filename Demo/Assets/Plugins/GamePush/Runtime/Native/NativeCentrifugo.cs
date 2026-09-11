using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using GamePush;

#if !(UNITY_WEBGL && !UNITY_EDITOR)
using Centrifugal.Centrifuge;
#endif

namespace GamePush.Native
{
    public sealed class NativeCentrifugo : IDisposable
    {
        public event Action Connected;
        public event Action Reconnected;
        public event Action Reconnecting;
        public event Action<string> Disconnected;
        bool _everConnected;

        public NativeSubscription Subscribe(string channel, string token)
        {
            return new NativeSubscription(this, channel, token);
        }

        public void Connect(string endpoint, string token)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GpWebSocketBridge.Connect(endpoint, token, OnWsOpen, OnWsClose, OnWsReconnecting);
#else
            GP_Logger.Info("Centrifugo", "connect " + endpoint + " tokenLen=" + (token == null ? 0 : token.Length));
            EnsureNativeClient(endpoint, token);
            _client.Connect();
#endif
        }

#if !(UNITY_WEBGL && !UNITY_EDITOR)
        CentrifugeClient _client;
        public CentrifugeClient Client => _client;

        void EnsureNativeClient(string endpoint, string token)
        {
            if (_client != null)
                return;
            var options = new CentrifugeClientOptions
            {
                Token = token ?? "",
                MinReconnectDelay = TimeSpan.FromMilliseconds(500),
                MaxReconnectDelay = TimeSpan.FromSeconds(20)
            };
            _client = new CentrifugeClient(endpoint, options);
            _client.Connected += (s, e) => NativeMainThread.Run(() =>
            {
                if (_everConnected)
                    Reconnected?.Invoke();
                else
                {
                    _everConnected = true;
                    Connected?.Invoke();
                }
            });
            _client.Disconnected += (s, e) =>
            {
                var code = ReadDisconnectCode(e);
                var reason = ReadDisconnectReason(e);
                NativeMainThread.Run(() =>
                {
                    if (IsReconnectable(code, reason))
                    {
                        GP_Logger.Warn("Centrifugo", code + " " + reason);
                        Reconnecting?.Invoke();
                    }
                    else
                    {
                        GP_Logger.Error("Centrifugo", code + " " + reason);
                        Disconnected?.Invoke(reason);
                    }
                });
            };
            _client.Error += (s, e) =>
            {
                var message = FormatError(e);
                NativeMainThread.Run(() =>
                {
                    if (IsTransientTransportMessage(message))
                        GP_Logger.Info("Centrifugo", message);
                    else
                        GP_Logger.Warn("Centrifugo", message);
                });
            };
        }

        static string FormatError(object args)
        {
            if (args == null)
                return "error";
            try
            {
                var type = args.GetType();
                var error = type.GetProperty("Error")?.GetValue(args)
                            ?? type.GetProperty("Exception")?.GetValue(args);
                if (error is Exception exception)
                    return exception.Message;
                var message = type.GetProperty("Message")?.GetValue(args);
                var code = type.GetProperty("Code")?.GetValue(args);
                if (message != null)
                    return code != null ? code + " " + message : message.ToString();
            }
            catch
            {
                /* fall through */
            }
            return args.ToString();
        }

        static uint ReadDisconnectCode(object args)
        {
            if (args == null)
                return 0;
            try
            {
                var value = args.GetType().GetProperty("Code")?.GetValue(args);
                if (value is uint unsigned)
                    return unsigned;
                if (value is int signed && signed >= 0)
                    return (uint)signed;
                if (value != null && uint.TryParse(value.ToString(), out var parsed))
                    return parsed;
            }
            catch
            {
                /* fall through */
            }
            return 0;
        }

        static string ReadDisconnectReason(object args)
        {
            if (args == null)
                return "disconnected";
            try
            {
                var reason = args.GetType().GetProperty("Reason")?.GetValue(args) as string;
                if (!string.IsNullOrEmpty(reason))
                    return reason;
            }
            catch
            {
                /* fall through */
            }
            return args.ToString();
        }

        static bool IsReconnectable(uint code, string reason)
        {
            if (code >= 3500 && code < 4000)
                return false;
            if (code >= 100 && code <= 111)
                return false;
            if (code >= 3000 && code < 3500)
                return true;
            if (code >= 4000 && code < 4500)
                return true;
            return IsTransientTransportMessage(reason);
        }

        static bool IsTransientTransportMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return false;
            var lower = message.ToLowerInvariant();
            return lower.IndexOf("connection closed", StringComparison.Ordinal) >= 0
                   || lower.IndexOf("reconnect", StringComparison.Ordinal) >= 0
                   || lower.IndexOf("no pong", StringComparison.Ordinal) >= 0
                   || lower.IndexOf("going away", StringComparison.Ordinal) >= 0;
        }
#endif

        void OnWsOpen()
        {
            NativeMainThread.Run(() =>
            {
                if (_everConnected)
                    Reconnected?.Invoke();
                else
                {
                    _everConnected = true;
                    Connected?.Invoke();
                }
            });
        }

        void OnWsReconnecting()
        {
            NativeMainThread.Run(() => Reconnecting?.Invoke());
        }

        void OnWsClose(string reason) => NativeMainThread.Run(() => Disconnected?.Invoke(reason));

        public void Dispose()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GpWebSocketBridge.Close();
#else
            try { _client?.Disconnect(); }
            catch { /* ignore */ }
            try { (_client as IDisposable)?.Dispose(); }
            catch { /* ignore */ }
            _client = null;
#endif
        }
    }

    public sealed class NativeSubscription
    {
        readonly NativeCentrifugo _client;
        readonly string _channel;
        readonly string _token;
        public event Action<byte[]> Publication;
        public event Action Subscribed;
        public bool IsSubscribed { get; private set; }

        public void MarkUnsubscribed() => IsSubscribed = false;

        public NativeSubscription(NativeCentrifugo client, string channel, string token)
        {
            _client = client;
            _channel = channel;
            _token = token;
        }

        public void Subscribe()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GpWebSocketBridge.Subscribe(_channel, _token, data =>
            {
                NativeMainThread.Run(() => Publication?.Invoke(data));
            }, () =>
            {
                IsSubscribed = true;
                NativeMainThread.Run(() => Subscribed?.Invoke());
            });
#else
            var options = new CentrifugeSubscriptionOptions { Token = _token ?? "" };
            var sub = _client.Client.NewSubscription(_channel, options);
            sub.Publication += (s, e) =>
            {
                var bytes = e.Data.ToArray();
                NativeMainThread.Run(() => Publication?.Invoke(bytes));
            };
            sub.Subscribed += (s, e) =>
            {
                IsSubscribed = true;
                NativeMainThread.Run(() => Subscribed?.Invoke());
            };
            sub.Subscribe();
            _native = sub;
#endif
        }

        public void Publish(byte[] data, Action<bool> onDone = null)
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GpWebSocketBridge.Publish(_channel, data);
            onDone?.Invoke(true);
#else
            if (_native == null || data == null)
            {
                onDone?.Invoke(false);
                return;
            }
            var task = _native.PublishAsync(data);
            task.ContinueWith(completed =>
            {
                NativeMainThread.Run(() =>
                {
                    if (completed.IsFaulted)
                    {
                        var error = completed.Exception?.GetBaseException()?.Message ?? "publish failed";
                        GP_Logger.Info("Centrifugo", "publish: " + error);
                        onDone?.Invoke(false);
                    }
                    else
                        onDone?.Invoke(true);
                });
            });
#endif
        }

        public void Unsubscribe()
        {
#if UNITY_WEBGL && !UNITY_EDITOR
            GpWebSocketBridge.Unsubscribe(_channel);
#else
            _native?.Unsubscribe();
#endif
            IsSubscribed = false;
        }

#if !(UNITY_WEBGL && !UNITY_EDITOR)
        CentrifugeSubscription _native;
#endif
    }

#if UNITY_WEBGL && !UNITY_EDITOR
    static class GpWebSocketBridge
    {
        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void GP_NativeWs_Connect(string endpoint, string token);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void GP_NativeWs_Subscribe(string channel, string token);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void GP_NativeWs_Publish(string channel, string base64);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void GP_NativeWs_Unsubscribe(string channel);

        [System.Runtime.InteropServices.DllImport("__Internal")]
        static extern void GP_NativeWs_Close();

        static Action _onOpen;
        static Action<string> _onClose;
        static Action _onReconnecting;
        static readonly Dictionary<string, Action<byte[]>> Pubs = new Dictionary<string, Action<byte[]>>();
        static readonly Dictionary<string, Action> Subs = new Dictionary<string, Action>();

        public static void Connect(string endpoint, string token, Action onOpen, Action<string> onClose,
            Action onReconnecting = null)
        {
            _onOpen = onOpen;
            _onClose = onClose;
            _onReconnecting = onReconnecting;
            EnsureReceiver();
            GP_NativeWs_Connect(endpoint, token);
        }

        public static void Subscribe(string channel, string token, Action<byte[]> onPub, Action onSub)
        {
            Pubs[channel ?? ""] = onPub;
            Subs[channel ?? ""] = onSub;
            GP_NativeWs_Subscribe(channel, token);
        }

        public static void Publish(string channel, byte[] data)
        {
            GP_NativeWs_Publish(channel, Convert.ToBase64String(data ?? Array.Empty<byte>()));
        }

        public static void Unsubscribe(string channel)
        {
            Pubs.Remove(channel ?? "");
            Subs.Remove(channel ?? "");
            GP_NativeWs_Unsubscribe(channel);
        }

        public static void Close() => GP_NativeWs_Close();

        static void EnsureReceiver()
        {
            var go = NativeMainThread.Instance.gameObject;
            if (go.GetComponent<GpWebSocketReceiver>() == null)
                go.AddComponent<GpWebSocketReceiver>();
        }

        public static void HandleOpen() => _onOpen?.Invoke();
        public static void HandleClose(string reason) => _onClose?.Invoke(reason);
        public static void HandleReconnecting(string _) => _onReconnecting?.Invoke();

        public static void HandleMessage(string payload)
        {
            if (string.IsNullOrEmpty(payload))
                return;
            var split = payload.IndexOf('|');
            var channel = split > 0 ? payload.Substring(0, split) : "";
            var b64 = split > 0 ? payload.Substring(split + 1) : payload;
            var data = string.IsNullOrEmpty(b64) ? Array.Empty<byte>() : Convert.FromBase64String(b64);
            if (Pubs.TryGetValue(channel, out var pub))
                pub(data);
            else
            {
                foreach (var pair in Pubs)
                    pair.Value(data);
            }
        }

        public static void HandleSubscribed(string channel)
        {
            if (Subs.TryGetValue(channel ?? "", out var sub))
                sub();
            else
            {
                foreach (var pair in Subs)
                    pair.Value();
            }
        }
    }

    public sealed class GpWebSocketReceiver : MonoBehaviour
    {
        public void OnNativeWsOpen(string _) => GpWebSocketBridge.HandleOpen();
        public void OnNativeWsClose(string reason) => GpWebSocketBridge.HandleClose(reason);
        public void OnNativeWsReconnecting(string reason) => GpWebSocketBridge.HandleReconnecting(reason);
        public void OnNativeWsMessage(string payload) => GpWebSocketBridge.HandleMessage(payload);
        public void OnNativeWsSubscribed(string channel) => GpWebSocketBridge.HandleSubscribed(channel);
    }
#endif
}
