using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Native;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    public sealed class GP_DocumentView : GP_OverlayView
    {
        public const float ReaderFontSize = 42f;

        public ScrollRect scrollRect;
        public TMP_Text contentLabel;

        [Tooltip("When positive, caps the text column. 0 uses the viewport width.")]
        public LayoutElement contentLayout;

        public float maxTextWidth;
        float _lastViewportWidth = -1f;

        public override void Bind(object args)
        {
            var data = args as GP_DocumentArgs ?? new GP_DocumentArgs();
            SetTitle(GP_OverlayStrings.Document);
            ShowLoading();
            ApplyReaderStyle();
            if (contentLabel != null)
                contentLabel.text = "";

            ApplyTextWidth();

            NativeDocuments.FetchForOverlay(data.type, data.format, OnLoaded, ShowError);
        }

        protected override void OnViewportChanged()
        {
            base.OnViewportChanged();
            _lastViewportWidth = -1f;
            ApplyTextWidth();
        }

        protected override void Update()
        {
            base.Update();
            ApplyTextWidth();
        }

        void ApplyReaderStyle()
        {
            if (contentLabel == null)
                return;
            contentLabel.fontSize = ReaderFontSize;
            contentLabel.textWrappingMode = TextWrappingModes.Normal;
            contentLabel.overflowMode = TextOverflowModes.Overflow;
            if (scrollRect != null)
                scrollRect.horizontal = false;
        }

        void ApplyTextWidth()
        {
            if (contentLayout == null)
                return;
            var viewportWidth = scrollRect != null && scrollRect.viewport != null
                ? scrollRect.viewport.rect.width
                : 0f;
            if (viewportWidth <= 0f || Mathf.Approximately(viewportWidth, _lastViewportWidth))
                return;
            _lastViewportWidth = viewportWidth;
            var width = Mathf.Max(1f, viewportWidth - 64f);
            if (maxTextWidth > 0f)
                width = Mathf.Min(width, maxTextWidth);
            contentLayout.preferredWidth = width;
            contentLayout.flexibleWidth = 0f;
        }

        void OnLoaded(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                ShowEmpty();
                return;
            }
            SetStatus(null);
            if (contentLabel != null)
            {
                contentLabel.text = content;
                GP_OverlayTone.Paint(contentLabel, Skin, GP_OverlayColorRole.Text);
            }
            ApplyReaderStyle();
            ApplyTextWidth();
            if (scrollRect != null)
            {
                Canvas.ForceUpdateCanvases();
                scrollRect.verticalNormalizedPosition = 1f;
            }
        }
    }
}
