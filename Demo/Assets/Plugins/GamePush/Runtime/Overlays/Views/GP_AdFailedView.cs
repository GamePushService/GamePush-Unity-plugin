using UnityEngine.UI;
using TMPro;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    /// <summary>
    /// Told the player the rewarded ad could not be shown, so a missing reward does not look
    /// like a bug. Driven by the project's showRewardedFailedOverlay flag.
    /// </summary>
    public sealed class GP_AdFailedView : GP_OverlayView
    {
        public TMP_Text textLabel;
        public Button okButton;

        public override void Bind(object args)
        {
            var data = args as GP_AdFailedArgs ?? new GP_AdFailedArgs();

            SetTitle("");
            if (textLabel != null)
            {
                textLabel.text = string.IsNullOrEmpty(data.text) ? GP_OverlayStrings.AdUnavailable : data.text;
                GP_OverlayTone.Paint(textLabel, Skin, GP_OverlayColorRole.Text);
            }

            if (okButton == null)
                return;
            okButton.onClick.RemoveAllListeners();
            okButton.onClick.AddListener(Close);
            var label = okButton.GetComponentInChildren<TMP_Text>();
            if (label != null)
                label.text = GP_OverlayStrings.Ok;
        }
    }
}
