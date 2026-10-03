using UnityEngine;
using GamePush.Native;

namespace GamePush.Overlays
{
    public enum GP_OverlaySizeMode
    {
        Modal,
        Sheet,
        Dialog,
        Document
    }

    /// <summary>
    /// Picks how an overlay panel fills the screen. List screens become sheets on phones;
    /// Document fills the safe area on PC and phones; confirm/ads stay compact dialogs.
    /// </summary>
    public static class GP_OverlayFit
    {
        public static readonly Vector2 SheetPadding = new Vector2(20f, 20f);
        public const float UltraWideAspect = 2f;

        /// <summary>
        /// Picks a canvas reference that matches the screen orientation so fonts and icons
        /// stay near their designed size instead of shrinking when a portrait reference is
        /// matched to a landscape screen.
        /// </summary>
        public static Vector2 ReferenceFor(Vector2 designed, int screenWidth, int screenHeight)
        {
            var shortSide = Mathf.Min(designed.x, designed.y);
            var longSide = Mathf.Max(designed.x, designed.y);
            if (shortSide <= 0f || longSide <= 0f)
                return designed;
            return screenWidth >= screenHeight
                ? new Vector2(longSide, shortSide)
                : new Vector2(shortSide, longSide);
        }

        public static float MatchWidthOrHeight(int screenWidth, int screenHeight, Vector2 reference)
        {
            if (reference.y <= 0f)
                return 0f;
            var screenAspect = screenWidth / (float)Mathf.Max(1, screenHeight);
            var referenceAspect = reference.x / reference.y;
            return screenAspect > referenceAspect ? 1f : 0f;
        }

        public static Vector2 CanvasLogicalSize(Vector2 designed, int screenWidth, int screenHeight)
        {
            var reference = ReferenceFor(designed, screenWidth, screenHeight);
            var match = MatchWidthOrHeight(screenWidth, screenHeight, reference);
            var scale = Mathf.Lerp(
                screenWidth / Mathf.Max(1f, reference.x),
                screenHeight / Mathf.Max(1f, reference.y),
                match);
            if (scale <= 0.0001f)
                return reference;
            return new Vector2(screenWidth / scale, screenHeight / scale);
        }

        public static bool IsSheet(GP_OverlayKind kind)
        {
            switch (kind)
            {
                case GP_OverlayKind.Achievements:
                case GP_OverlayKind.Leaderboard:
                case GP_OverlayKind.Chat:
                case GP_OverlayKind.Feedbacks:
                case GP_OverlayKind.GamesCollections:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsDialog(GP_OverlayKind kind)
        {
            switch (kind)
            {
                case GP_OverlayKind.Confirm:
                case GP_OverlayKind.AdCountdown:
                case GP_OverlayKind.AdFailed:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsMobileLayout()
        {
            if (Application.isMobilePlatform)
                return true;
            if (GamePushHost.UseNativeCore)
                return false;
            return GP_Settings.instance != null && GP_Device.IsMobile();
        }

        public static bool IsMobilePreview(Vector2Int logical)
        {
            var shortSide = Mathf.Min(logical.x, logical.y);
            var longSide = Mathf.Max(logical.x, logical.y);
            return shortSide <= 540 && longSide <= 960;
        }

        public static GP_OverlaySizeMode Resolve(GP_OverlayKind kind, bool mobile)
        {
            if (kind == GP_OverlayKind.Document)
                return GP_OverlaySizeMode.Document;
            if (IsDialog(kind))
                return GP_OverlaySizeMode.Dialog;
            if (mobile && IsSheet(kind))
                return GP_OverlaySizeMode.Sheet;
            return GP_OverlaySizeMode.Modal;
        }
    }
}
