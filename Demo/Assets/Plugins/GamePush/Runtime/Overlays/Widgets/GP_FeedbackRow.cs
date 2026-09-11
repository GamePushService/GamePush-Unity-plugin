using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamePush.Overlays.Widgets
{
    public sealed class GP_FeedbackRow : MonoBehaviour
    {
        public Image background;
        public TMP_Text textLabel;
        public TMP_Text statusLabel;
        public TMP_Text dateLabel;
        public Image selectedBar;
        public Button button;

        public void Bind(FeedbackData feedback, int index, bool selected, Action onClick)
        {
            var skin = GP_OverlaySkin.Instance;

            if (background != null)
                GP_OverlayTone.Paint(background, skin, GP_OverlayTone.RowRole(index));

            if (selectedBar != null)
            {
                selectedBar.gameObject.SetActive(selected);
                GP_OverlayTone.Paint(selectedBar, skin, GP_OverlayColorRole.Accent);
            }

            if (textLabel != null)
            {
                textLabel.text = string.IsNullOrEmpty(feedback.text) ? GP_OverlayStrings.NewFeedback : feedback.text;
                GP_OverlayTone.Paint(textLabel, skin, GP_OverlayColorRole.Text);
            }

            if (statusLabel != null)
            {
                statusLabel.text = feedback.status ?? "";
                GP_OverlayTone.Paint(statusLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            if (dateLabel != null)
            {
                dateLabel.text = FormatDate(feedback.createdAt);
                GP_OverlayTone.Paint(dateLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            if (button == null)
                return;
            button.onClick.RemoveAllListeners();
            if (onClick != null)
                button.onClick.AddListener(() => onClick());
        }

        static string FormatDate(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return "";
            return DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var utc)
                ? utc.ToLocalTime().ToString("dd.MM.yyyy", CultureInfo.CurrentCulture)
                : "";
        }
    }
}
