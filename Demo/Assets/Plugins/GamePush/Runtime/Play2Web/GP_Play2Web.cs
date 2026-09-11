#if UNITY_EDITOR
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using GamePush.Data;
using UnityEngine;

namespace GamePush
{
    public static class GP_Play2Web
    {
        public const string PortPref = "GamePush.Play2Web.Port";
        public const string WantedPref = "GamePush.Play2Web.Wanted";
        public const int DefaultPort = 8765;

        static readonly ConcurrentQueue<Inbound> _inbound = new ConcurrentQueue<Inbound>();
        static readonly Dictionary<string, string> _cache = new Dictionary<string, string>(StringComparer.Ordinal);
        static readonly object _cacheLock = new object();
        static int _overlayDepth;

        public static event Action<bool> OverlayVisibilityChanged;
        public static event Action<string> Log;
        public static event Action FlushToBrowser;

        static bool _savedRunInBackground;
        static bool _restoreRunInBackground;

        public static bool Requested
        {
            get => PlayerPrefs.GetInt(WantedPref, 0) != 0;
            set
            {
                if (value)
                    PlayerPrefs.SetInt(WantedPref, 1);
                else
                    PlayerPrefs.DeleteKey(WantedPref);
                PlayerPrefs.Save();
            }
        }

        public static bool Enabled => Application.isPlaying && (Requested || ProjectData.SDK_LIVE);

        public static bool IsReady { get; private set; }

        public static bool OverlayVisible => _overlayDepth > 0;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void ResetStatics()
        {
            IsReady = false;
            _overlayDepth = 0;
            _restoreRunInBackground = false;
            lock (_cacheLock) _cache.Clear();
            while (_inbound.TryDequeue(out _)) { }
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Boot()
        {
            if (!Enabled)
                return;
            _savedRunInBackground = Application.runInBackground;
            _restoreRunInBackground = true;
            Application.runInBackground = true;
            var runner = new GameObject("GP_Play2WebRunner");
            UnityEngine.Object.DontDestroyOnLoad(runner);
            runner.hideFlags = HideFlags.HideAndDontSave;
            runner.AddComponent<GP_Play2WebRunner>();
        }

        internal static void Disconnect()
        {
            IsReady = false;
            if (_restoreRunInBackground)
            {
                Application.runInBackground = _savedRunInBackground;
                _restoreRunInBackground = false;
            }
        }

        public static bool Call(string method, params object[] args)
        {
            if (!Enabled)
                return false;
            Send(new Outbound
            {
                type = "call",
                method = method,
                args = ToStrings(args)
            });
            return true;
        }

        public static bool TryGet(string key, out string value)
        {
            value = null;
            if (!Enabled)
                return false;
            lock (_cacheLock)
            {
                if (_cache.TryGetValue(key, out value) && value != null)
                    return true;
                foreach (var alt in Alternates(key))
                {
                    if (_cache.TryGetValue(alt, out value) && value != null)
                        return true;
                }
            }
            value = null;
            return false;
        }

        static IEnumerable<string> Alternates(string key)
        {
            if (string.IsNullOrEmpty(key))
                yield break;
            if (key.StartsWith("GP_"))
                yield return key.Substring(3);
            else
                yield return "GP_" + key;
            yield return key.Replace("_", "");
            if (key.StartsWith("GP_"))
                yield return key.Substring(3).Replace("_", "");
        }

        public static bool TryGetBool(string key, out bool value)
        {
            value = false;
            if (!TryGet(key, out var raw))
                return false;
            value = raw == "true" || raw == "True" || raw == "1";
            return true;
        }

        public static bool TryGetInt(string key, out int value)
        {
            value = 0;
            if (!TryGet(key, out var raw))
                return false;
            return int.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        public static bool TryGetFloat(string key, out float value)
        {
            value = 0;
            if (!TryGet(key, out var raw))
                return false;
            return float.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out value);
        }

        public static string GetString(string key, string fallback = "")
        {
            return TryGet(key, out var value) ? value : fallback;
        }

        public static bool GetBool(string key, bool fallback = false)
        {
            return TryGetBool(key, out var value) ? value : fallback;
        }

        public static int GetInt(string key, int fallback = 0)
        {
            return TryGetInt(key, out var value) ? value : fallback;
        }

        public static float GetFloat(string key, float fallback = 0)
        {
            return TryGetFloat(key, out var value) ? value : fallback;
        }

        public static void Pump()
        {
            while (GP_Play2WebBus.TryTakeToUnity(out var json))
            {
                var inbound = ParseInbound(json);
                if (inbound != null)
                    _inbound.Enqueue(inbound);
            }
            while (_inbound.TryDequeue(out var msg))
                Handle(msg);
        }

        // JsonUtility cannot round-trip an event whose `arg` is itself JSON
        // (`[{...}]` or `{"id":1,...}`). It fails the whole envelope, type stays
        // empty, and channel create/list callbacks never reach GP_Channels.
        static Inbound ParseInbound(string json)
        {
            if (string.IsNullOrEmpty(json))
                return null;
            var inbound = JsonUtility.FromJson<Inbound>(json) ?? new Inbound();
            if (string.IsNullOrEmpty(inbound.type))
                inbound.type = ExtractJsonString(json, "type") ?? "";
            if (string.IsNullOrEmpty(inbound.method))
                inbound.method = ExtractJsonString(json, "method") ?? "";
            var arg = ExtractJsonString(json, "arg");
            if (arg != null)
                inbound.arg = arg;
            return inbound;
        }

        static string ExtractJsonString(string json, string key)
        {
            var needle = "\"" + key + "\":";
            var start = json.IndexOf(needle, StringComparison.Ordinal);
            if (start < 0)
                return null;
            start += needle.Length;
            while (start < json.Length && char.IsWhiteSpace(json[start]))
                start++;
            if (start >= json.Length || json[start] != '"')
                return null;
            start++;
            var text = new StringBuilder();
            for (var i = start; i < json.Length; i++)
            {
                var c = json[i];
                if (c == '\\' && i + 1 < json.Length)
                {
                    var next = json[++i];
                    switch (next)
                    {
                        case '"': text.Append('"'); break;
                        case '\\': text.Append('\\'); break;
                        case '/': text.Append('/'); break;
                        case 'n': text.Append('\n'); break;
                        case 'r': text.Append('\r'); break;
                        case 't': text.Append('\t'); break;
                        case 'u':
                            if (i + 4 < json.Length &&
                                int.TryParse(json.Substring(i + 1, 4), NumberStyles.HexNumber,
                                    CultureInfo.InvariantCulture, out var code))
                            {
                                text.Append((char)code);
                                i += 4;
                            }
                            break;
                        default: text.Append(next); break;
                    }
                    continue;
                }
                if (c == '"')
                    return text.ToString();
                text.Append(c);
            }
            return text.ToString();
        }

        static void Handle(Inbound msg)
        {
            if (msg == null || string.IsNullOrEmpty(msg.type))
                return;
            switch (msg.type)
            {
                case "ready":
                    ApplyEntries(msg.entries);
                    IsReady = true;
                    EmitLog("SDK ready");
                    break;
                case "snapshot":
                    ApplyEntries(msg.entries);
                    break;
                case "event":
                    if (ShowsOverlay(msg.method) || ShowsOverlayEvent(msg.method))
                        PushOverlay();
                    if (HidesOverlayEvent(msg.method))
                        PopOverlay();
                    DispatchUnity(msg.method, msg.arg);
                    break;
                case "overlay":
                    SetOverlay(msg.show);
                    break;
                case "log":
                    EmitLog(msg.arg);
                    break;
                case "warn":
                    EmitWarn(msg.arg);
                    break;
            }
        }

        static void ApplyEntries(SnapshotEntry[] entries)
        {
            if (entries == null)
                return;
            lock (_cacheLock)
            {
                foreach (var entry in entries)
                {
                    if (entry == null || string.IsNullOrEmpty(entry.k))
                        continue;
                    _cache[entry.k] = entry.v ?? "";
                }
            }
        }

        static void DispatchUnity(string method, string arg)
        {
            if (string.IsNullOrEmpty(method))
                return;
            var sdk = GameObject.Find("GamePushSDK");
            if (sdk == null)
                return;
            if (string.IsNullOrEmpty(arg))
                sdk.SendMessage(method, SendMessageOptions.DontRequireReceiver);
            else
                sdk.SendMessage(method, arg, SendMessageOptions.DontRequireReceiver);
        }

        static void Send(Outbound outbound)
        {
            GP_Play2WebBus.SendToBrowser(ToJson(outbound));
            FlushToBrowser?.Invoke();
        }

        static string ToJson(Outbound outbound)
        {
            var sb = new StringBuilder(64);
            sb.Append("{\"type\":");
            AppendJsonString(sb, outbound.type);
            sb.Append(",\"method\":");
            AppendJsonString(sb, outbound.method);
            sb.Append(",\"args\":[");
            var args = outbound.args ?? Array.Empty<string>();
            for (var i = 0; i < args.Length; i++)
            {
                if (i > 0)
                    sb.Append(',');
                AppendJsonString(sb, args[i]);
            }
            sb.Append("]}");
            return sb.ToString();
        }

        static void AppendJsonString(StringBuilder sb, string value)
        {
            sb.Append('"');
            if (!string.IsNullOrEmpty(value))
            {
                for (var i = 0; i < value.Length; i++)
                {
                    var c = value[i];
                    switch (c)
                    {
                        case '"': sb.Append("\\\""); break;
                        case '\\': sb.Append("\\\\"); break;
                        case '\n': sb.Append("\\n"); break;
                        case '\r': sb.Append("\\r"); break;
                        case '\t': sb.Append("\\t"); break;
                        default:
                            if (c < 32)
                                sb.AppendFormat("\\u{0:x4}", (int)c);
                            else
                                sb.Append(c);
                            break;
                    }
                }
            }
            sb.Append('"');
        }

        static string[] ToStrings(object[] args)
        {
            if (args == null || args.Length == 0)
                return Array.Empty<string>();
            var result = new string[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                var value = args[i];
                if (value == null)
                    result[i] = "";
                else if (value is bool b)
                    result[i] = b ? "True" : "False";
                else if (value is IFormattable formattable)
                    result[i] = formattable.ToString(null, CultureInfo.InvariantCulture);
                else
                    result[i] = value.ToString();
            }
            return result;
        }

        static void PushOverlay()
        {
            _overlayDepth++;
            if (_overlayDepth == 1)
                OverlayVisibilityChanged?.Invoke(true);
        }

        static void PopOverlay()
        {
            if (_overlayDepth <= 0)
                return;
            _overlayDepth--;
            if (_overlayDepth == 0)
                OverlayVisibilityChanged?.Invoke(false);
        }

        static void SetOverlay(bool show)
        {
            if (show)
            {
                if (_overlayDepth == 0)
                    OverlayVisibilityChanged?.Invoke(true);
                _overlayDepth = Math.Max(_overlayDepth, 1);
            }
            else
            {
                _overlayDepth = 0;
                OverlayVisibilityChanged?.Invoke(false);
            }
        }

        static bool ShowsOverlay(string method)
        {
            if (string.IsNullOrEmpty(method))
                return false;
            var n = method.Replace("_", "");
            return n.IndexOf("ShowFullscreen", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("ShowRewarded", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("ShowPreloader", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("ShowConfirm", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("PlayerLogin", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("PaymentsPurchase", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("PaymentsSubscribe", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("OpenChat", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("AchievementsOpen", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("LeaderboardOpen", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("LeaderboardScopedOpen", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("GamesCollectionsOpen", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("DocumentsOpen", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("FullscreenOpen", StringComparison.OrdinalIgnoreCase) >= 0
                   || n.IndexOf("FeedbacksOpen", StringComparison.OrdinalIgnoreCase) >= 0;
        }

        static bool ShowsOverlayEvent(string method)
        {
            if (string.IsNullOrEmpty(method))
                return false;
            return method.IndexOf("Start", StringComparison.Ordinal) >= 0
                   || method == "CallOnOpenChat"
                   || method == "CallAchievementsOpen"
                   || method == "CallLeaderboardOpen"
                   || method == "CallGamesCollectionsOpen"
                   || method == "CallOnDocumentsOpen"
                   || method == "CallPaymentsOpen"
                   || method == "CallFullscreenOpen"
                   || method == "CallWindowsShowConfirm"
                   || method == "CallOnPause"
                   || method == "CallFeedbacksOpenList"
                   || method == "CallFeedbacksOpenFeedback";
        }

        static bool HidesOverlayEvent(string method)
        {
            if (string.IsNullOrEmpty(method))
                return false;
            return method.IndexOf("Close", StringComparison.Ordinal) >= 0
                   || method == "CallOnResume"
                   || method == "CallPaymentsClose"
                   || method == "CallFullscreenClose"
                   || method == "CallFeedbacksOpenListError"
                   || method == "CallFeedbacksOpenFeedbackError";
        }

        static void EmitLog(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            Debug.Log("[Play2Web] " + message);
            Log?.Invoke(message);
        }

        static void EmitWarn(string message)
        {
            if (string.IsNullOrEmpty(message))
                return;
            Debug.LogWarning("[Play2Web] " + message);
            Log?.Invoke(message);
        }

        [Serializable]
        class Outbound
        {
            public string type;
            public string method;
            public string[] args;
        }

        [Serializable]
        class Inbound
        {
            public string type;
            public string method;
            public string arg;
            public bool show;
            public SnapshotEntry[] entries;
        }

        [Serializable]
        public class SnapshotEntry
        {
            public string k;
            public string v;
        }
    }

    sealed class GP_Play2WebRunner : MonoBehaviour
    {
        void Update() => GP_Play2Web.Pump();

        void OnDestroy() => GP_Play2Web.Disconnect();
    }
}
#endif
