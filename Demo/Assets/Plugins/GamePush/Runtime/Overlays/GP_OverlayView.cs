using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays
{
    /// <summary>
    /// Base for every overlay screen. Sits on the prefab root and owns the show/hide animation,
    /// the backdrop that blocks clicks on the game and the wiring to the host stack.
    /// </summary>
    public abstract class GP_OverlayView : MonoBehaviour
    {
        [Header("Common")]
        public RectTransform panel;
        public Image backdrop;
        public Button closeButton;
        public Button backdropButton;
        public TMP_Text titleLabel;
        public TMP_Text statusLabel;
        public CanvasGroup canvasGroup;

        [Tooltip("Close when the Android back button or Escape is pressed.")]
        public bool closeOnBack = true;

        [Tooltip("Close when the darkened area outside the panel is clicked.")]
        public bool closeOnBackdrop = true;

        public float fadeDuration = 0.15f;

        GP_OverlayHost _host;
        GP_OverlayKind _kind;
        bool _closing;
        float _fade;

        public GP_OverlayKind Kind => _kind;
        public GP_OverlayHost Host => _host;
        public GP_OverlaySkin Skin => GP_OverlaySkin.Instance;
        public bool CloseOnBack => closeOnBack;

        public event Action Closed;

        internal void Attach(GP_OverlayHost host, GP_OverlayKind kind)
        {
            _host = host;
            _kind = kind;

            var rect = transform as RectTransform;
            if (rect != null)
            {
                rect.anchorMin = Vector2.zero;
                rect.anchorMax = Vector2.one;
                rect.offsetMin = Vector2.zero;
                rect.offsetMax = Vector2.zero;
            }

            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>() ?? gameObject.AddComponent<CanvasGroup>();

            if (closeButton != null)
                closeButton.onClick.AddListener(Close);
            if (backdropButton != null && closeOnBackdrop)
                backdropButton.onClick.AddListener(Close);

            var responsive = panel != null ? panel.GetComponent<GP_OverlayResponsive>() : null;
            if (responsive != null)
                responsive.Configure(Skin.Find(kind), kind, GP_OverlayFit.IsMobileLayout());

            FitHeaderText();
            PinStatusTo(StatusHost);
            ApplyChrome(Skin);
        }

        /// <summary>
        /// Re-applies panel size and inner layout after the canvas scaler changes
        /// (rotation, window resize).
        /// </summary>
        internal void RefreshLayout()
        {
            var responsive = panel != null ? panel.GetComponent<GP_OverlayResponsive>() : null;
            if (responsive != null)
                responsive.Configure(Skin.Find(_kind), _kind, GP_OverlayFit.IsMobileLayout());
            OnViewportChanged();
        }

        protected virtual void OnViewportChanged()
        {
            var grids = GetComponentsInChildren<GP_FlexibleGrid>(true);
            for (var i = 0; i < grids.Length; i++)
                grids[i].Invalidate();
        }

        void FitHeaderText()
        {
            EllipsisSingleLine(titleLabel);
        }

        protected static void EllipsisSingleLine(TMP_Text text)
        {
            if (text == null)
                return;
            text.textWrappingMode = TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Ellipsis;
        }

        /// <summary>
        /// Paints palette-tagged surfaces from the current skin so header-colored rails,
        /// composers and similar chrome follow palette edits without a prefab rebuild.
        /// </summary>
        public void ApplyChrome(GP_OverlaySkin skin)
        {
            PinStatusTo(StatusHost);
            if (skin == null)
                return;
            GP_OverlayTone.Apply(gameObject, skin);
        }

        /// <summary>
        /// Split layouts (chat feed, feedback list, achievement grid) override this so loading
        /// and error copy sit in the content column instead of the whole window.
        /// </summary>
        protected virtual Transform StatusHost => null;

        protected void PinStatusTo(Transform host)
        {
            if (statusLabel == null || host == null)
                return;
            var rect = statusLabel.rectTransform;
            if (rect.parent != host)
                rect.SetParent(host, false);
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            rect.SetAsLastSibling();
            var ignore = statusLabel.GetComponent<LayoutElement>() ??
                         statusLabel.gameObject.AddComponent<LayoutElement>();
            ignore.ignoreLayout = true;
        }

        public abstract void Bind(object args);

        public virtual void PlayShow()
        {
            _fade = 0f;
            if (canvasGroup != null)
                canvasGroup.alpha = fadeDuration > 0f ? 0f : 1f;
        }

        public void Close()
        {
            if (_closing)
                return;
            _closing = true;
            OnClosing();
            _host?.Detach(this);
            Closed?.Invoke();
            Destroy(gameObject);
        }

        protected virtual void OnClosing()
        {
        }

        protected virtual void Update()
        {
            if (canvasGroup == null || fadeDuration <= 0f || _fade >= 1f)
                return;
            // unscaled so the fade still runs while the overlay pauses the game
            _fade = Mathf.Clamp01(_fade + Time.unscaledDeltaTime / fadeDuration);
            canvasGroup.alpha = _fade;
        }

        protected void SetTitle(string text)
        {
            if (titleLabel != null)
                titleLabel.text = text ?? "";
        }

        protected void SetStatus(string text)
        {
            if (statusLabel == null)
                return;
            statusLabel.text = text ?? "";
            statusLabel.gameObject.SetActive(!string.IsNullOrEmpty(text));
        }

        protected void ShowLoading() => SetStatus(GP_OverlayStrings.Loading);

        protected void ShowEmpty() => SetStatus(GP_OverlayStrings.Empty);

        protected void ShowError(string error)
        {
            SetStatus(string.IsNullOrEmpty(error) ? GP_OverlayStrings.Error : GP_OverlayStrings.Error + "\n" + error);
        }
    }
}
