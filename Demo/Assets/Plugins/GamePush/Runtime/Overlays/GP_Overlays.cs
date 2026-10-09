using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Overlays
{
    public enum GP_OverlayKind
    {
        Confirm,
        Achievements,
        Leaderboard,
        Chat,
        Document,
        GamesCollections,
        Feedbacks,
        AdCountdown,
        AdFailed
    }

    /// <summary>
    /// Entry point for the native (Android / Windows) overlays. Every GP_* module routes its
    /// Open() here when running on NativeCore; returning false lets the caller fall back to
    /// its previous behaviour.
    /// </summary>
    public static class GP_Overlays
    {
        static readonly Dictionary<GP_OverlayKind, GameObject> Overrides =
            new Dictionary<GP_OverlayKind, GameObject>();

        public static event Action<GP_OverlayKind> OnOpen;
        public static event Action<GP_OverlayKind> OnClose;
        public static event Action<bool> OnAnyOpenChanged;

        public static bool IsAnyOpen => GP_OverlayHost.Instance != null && GP_OverlayHost.Instance.IsAnyOpen;
        public static int OpenCount => GP_OverlayHost.Instance != null ? GP_OverlayHost.Instance.OpenCount : 0;

        public static bool Available => ProjectData.NATIVE_OVERLAYS && GP_OverlayHost.Instance != null;

        public static bool Open(GP_OverlayKind kind, object args = null)
        {
            if (!ProjectData.NATIVE_OVERLAYS)
                return false;
            var host = GP_OverlayHost.Instance;
            if (host == null)
            {
                GP_Logger.Warn("OVERLAYS", "GP_OverlayHost is missing, overlay " + kind + " was skipped");
                return false;
            }
            return host.Show(kind, args);
        }

        public static void Close(GP_OverlayKind kind) => GP_OverlayHost.Instance?.Hide(kind);

        public static void CloseTop() => GP_OverlayHost.Instance?.HideTop();

        public static void CloseAll() => GP_OverlayHost.Instance?.HideAll();

        public static bool IsOpen(GP_OverlayKind kind) =>
            GP_OverlayHost.Instance != null && GP_OverlayHost.Instance.IsOpen(kind);

        /// <summary>Replaces the prefab used for a kind at runtime, before the next Open().</summary>
        public static void SetPrefab(GP_OverlayKind kind, GameObject prefab)
        {
            if (prefab == null)
                Overrides.Remove(kind);
            else
                Overrides[kind] = prefab;
        }

        public static GameObject GetPrefabOverride(GP_OverlayKind kind) =>
            Overrides.TryGetValue(kind, out var prefab) ? prefab : null;

        internal static void RaiseOpen(GP_OverlayKind kind)
        {
            OnOpen?.Invoke(kind);
            OnAnyOpenChanged?.Invoke(true);
        }

        internal static void RaiseClose(GP_OverlayKind kind, bool anyLeft)
        {
            OnClose?.Invoke(kind);
            if (!anyLeft)
                OnAnyOpenChanged?.Invoke(false);
        }
    }

    public sealed class GP_ConfirmArgs
    {
        public string title = "";
        public string text = "";
        public string confirmLabel = "";
        public string cancelLabel = "";
        public bool invertButtonColors;
        public bool hideCancelButton;
        public Action<bool> onResult;
    }

    public sealed class GP_LeaderboardArgs
    {
        public bool scoped;
        public string idOrTag = "";
        public string variant = "";
        public string orderBy = "score";
        public string order = "DESC";
        public int limit = 10;
        public int showNearest = 5;
        public string withMe = "none";
        public string includeFields = "";
        public string displayFields = "";
    }

    public sealed class GP_ChatArgs
    {
        public Native.NativeChatScope scope = Native.NativeChatScope.Channel;
        public int target = -10;
        public string tags = "";
    }

    public sealed class GP_DocumentArgs
    {
        public string type = Native.NativeDocuments.DefaultType;
        public string format = "TXT";
    }

    public sealed class GP_GamesCollectionsArgs
    {
        public string idOrTag = "";
    }

    public sealed class GP_FeedbacksArgs
    {
        public string type = "";
        public string status = "";
        public string feedbackId = "";
    }

    public sealed class GP_AdCountdownArgs
    {
        public float seconds = 3f;
        public Action onDone;
    }

    public sealed class GP_AdFailedArgs
    {
        public string text = "";
    }
}
