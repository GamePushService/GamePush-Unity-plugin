using UnityEngine;

namespace GamePush.Overlays.Widgets
{
    /// <summary>
    /// Sizes an overlay panel against the safe area. Phones fill the safe area (sheet),
    /// desktop keeps a centred modal, and Document fills height and width on PC and phones.
    /// </summary>
    [ExecuteAlways]
    [RequireComponent(typeof(RectTransform))]
    [DisallowMultipleComponent]
    public sealed class GP_OverlayResponsive : MonoBehaviour
    {
        [Tooltip("Maximum width/height ratio of the panel. 1 means 'fit inside a square'.")]
        public float maxAspect = 1f;

        public Vector2 minSize = new Vector2(320f, 320f);
        public Vector2 maxSize = new Vector2(1400f, 1800f);

        [Tooltip("Extra margin inside the safe area, in reference units.")]
        public Vector2 padding = new Vector2(48f, 48f);

        public GP_OverlaySizeMode sizeMode = GP_OverlaySizeMode.Modal;

        RectTransform _rect;
        RectTransform _parent;
        Rect _lastSafeArea;
        Vector2 _lastParentSize;

        RectTransform Rect => _rect != null ? _rect : _rect = (RectTransform)transform;

        public void Configure(GP_OverlayPrefabEntry entry)
        {
            if (entry != null)
            {
                maxAspect = entry.maxAspect > 0f ? entry.maxAspect : maxAspect;
                minSize = entry.minSize;
                maxSize = entry.maxSize;
            }
            padding = sizeMode == GP_OverlaySizeMode.Sheet
                ? GP_OverlayFit.SheetPadding
                : GP_OverlaySkin.Instance.screenPadding;
            Apply();
        }

        public void Configure(GP_OverlayPrefabEntry entry, GP_OverlayKind kind, bool mobileLayout)
        {
            sizeMode = GP_OverlayFit.Resolve(kind, mobileLayout);
            Configure(entry);
        }

        void OnEnable() => Apply();

        void OnRectTransformDimensionsChange() => Apply();

        void Update()
        {
            // Safe area changes (rotation, notch) do not always raise a dimensions change.
            if (Screen.safeArea != _lastSafeArea)
                Apply();
        }

        public void Apply()
        {
            if (Rect == null)
                return;
            _parent = Rect.parent as RectTransform;
            if (_parent == null)
                return;

            _lastSafeArea = Screen.safeArea;
            var parentSize = _parent.rect.size;
            if (parentSize.x <= 0f || parentSize.y <= 0f)
                return;
            _lastParentSize = parentSize;

            // Screen.safeArea is in pixels; convert the insets into canvas units.
            var scaleX = Screen.width > 0 ? parentSize.x / Screen.width : 1f;
            var scaleY = Screen.height > 0 ? parentSize.y / Screen.height : 1f;
            var safe = Screen.safeArea;
            var insetX = (safe.xMin + (Screen.width - safe.xMax)) * scaleX;
            var insetY = (safe.yMin + (Screen.height - safe.yMax)) * scaleY;
            var offsetX = (safe.xMin - (Screen.width - safe.xMax)) * scaleX * 0.5f;
            var offsetY = (safe.yMin - (Screen.height - safe.yMax)) * scaleY * 0.5f;

            var availableWidth = Mathf.Max(0f, parentSize.x - insetX - padding.x * 2f);
            var availableHeight = Mathf.Max(0f, parentSize.y - insetY - padding.y * 2f);

            var wideThreshold = GP_OverlaySkin.Instance.wideThreshold;
            var size = CalculatePanelSize(new Vector2(availableWidth, availableHeight), minSize, maxSize,
                maxAspect, wideThreshold, sizeMode);
            var width = size.x;
            var height = size.y;

            Rect.anchorMin = new Vector2(0.5f, 0.5f);
            Rect.anchorMax = new Vector2(0.5f, 0.5f);
            Rect.pivot = new Vector2(0.5f, 0.5f);
            Rect.sizeDelta = new Vector2(width, height);
            Rect.anchoredPosition = new Vector2(offsetX, offsetY);

            var mode = GetComponent<GP_OverlayLayoutMode>();
            if (mode != null)
                mode.Evaluate(width, height);
        }

        /// <summary>Applies the same sizing math to a simulated viewport without reading Screen.safeArea.</summary>
        public void ApplyForPreview(Vector2 viewportSize)
        {
            if (Rect == null || viewportSize.x <= 0f || viewportSize.y <= 0f)
                return;
            var available = new Vector2(
                Mathf.Max(0f, viewportSize.x - padding.x * 2f),
                Mathf.Max(0f, viewportSize.y - padding.y * 2f));
            var size = CalculatePanelSize(available, minSize, maxSize, maxAspect,
                GP_OverlaySkin.Instance.wideThreshold, sizeMode);

            Rect.anchorMin = new Vector2(0.5f, 0.5f);
            Rect.anchorMax = new Vector2(0.5f, 0.5f);
            Rect.pivot = new Vector2(0.5f, 0.5f);
            Rect.sizeDelta = size;
            Rect.anchoredPosition = Vector2.zero;

            var mode = GetComponent<GP_OverlayLayoutMode>();
            if (mode != null)
                mode.Evaluate(size.x, size.y);
        }

        public static Vector2 CalculatePanelSize(Vector2 available, Vector2 minimum, Vector2 maximum,
            float aspectLimit, float wideThreshold)
        {
            return CalculatePanelSize(available, minimum, maximum, aspectLimit, wideThreshold,
                GP_OverlaySizeMode.Modal);
        }

        public static Vector2 CalculatePanelSize(Vector2 available, Vector2 minimum, Vector2 maximum,
            float aspectLimit, float wideThreshold, GP_OverlaySizeMode mode)
        {
            available.x = Mathf.Max(0f, available.x);
            available.y = Mathf.Max(0f, available.y);

            Vector2 size;
            switch (mode)
            {
                case GP_OverlaySizeMode.Sheet:
                case GP_OverlaySizeMode.Document:
                    size = CalculateSheetSize(available);
                    break;
                default:
                    size = CalculateModalSize(available, minimum, maximum, aspectLimit, wideThreshold);
                    break;
            }

            return ClampToAvailable(size, available);
        }

        static Vector2 CalculateSheetSize(Vector2 available)
        {
            var width = available.x;
            var height = available.y;
            if (height > 0f && width / height > GP_OverlayFit.UltraWideAspect)
                width = height * GP_OverlayFit.UltraWideAspect;
            return new Vector2(width, height);
        }

        static Vector2 CalculateModalSize(Vector2 available, Vector2 minimum, Vector2 maximum,
            float aspectLimit, float wideThreshold)
        {
            var height = Mathf.Clamp(available.y, Mathf.Min(minimum.y, available.y), maximum.y);
            var width = Mathf.Clamp(available.x, Mathf.Min(minimum.x, available.x), maximum.x);

            // Wide screens should use a wide modal instead of a tall portrait modal with empty
            // margins. Portrait keeps the available height; landscape approaches aspectLimit.
            if (available.y > 0f && available.x / available.y >= wideThreshold && aspectLimit >= wideThreshold)
            {
                height = Mathf.Max(Mathf.Min(minimum.y, available.y),
                    Mathf.Min(height, width / Mathf.Max(aspectLimit, 0.01f)));
            }
            if (aspectLimit > 0f)
                width = Mathf.Min(width, height * aspectLimit);
            return new Vector2(width, height);
        }

        static Vector2 ClampToAvailable(Vector2 size, Vector2 available)
        {
            return new Vector2(
                Mathf.Min(size.x, available.x),
                Mathf.Min(size.y, available.y));
        }
    }
}
