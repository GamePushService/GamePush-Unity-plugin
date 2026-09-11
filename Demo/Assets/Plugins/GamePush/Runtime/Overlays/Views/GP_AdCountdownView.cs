using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    /// <summary>
    /// Short "ad in N…" curtain shown before a fullscreen or rewarded ad so the break does not
    /// interrupt the player mid-action. Driven by the project's showCountdownOverlay flag.
    /// </summary>
    public sealed class GP_AdCountdownView : GP_OverlayView
    {
        public TMP_Text countdownLabel;
        public TMP_Text captionLabel;
        public Button skipButton;

        GP_AdCountdownArgs _args = new GP_AdCountdownArgs();
        float _left;
        bool _fired;

        public override void Bind(object args)
        {
            _args = args as GP_AdCountdownArgs ?? new GP_AdCountdownArgs();
            _left = Mathf.Max(0f, _args.seconds);
            _fired = false;

            // The curtain must not be dismissible: the ad is going to play either way.
            closeOnBack = false;
            closeOnBackdrop = false;

            SetTitle("");
            if (captionLabel != null)
            {
                captionLabel.text = GP_OverlayStrings.AdSoon;
                GP_OverlayTone.Paint(captionLabel, Skin, GP_OverlayColorRole.Text);
            }

            if (skipButton != null)
            {
                skipButton.onClick.RemoveAllListeners();
                skipButton.onClick.AddListener(Finish);
                var label = skipButton.GetComponentInChildren<TMP_Text>();
                if (label != null)
                    label.text = GP_OverlayStrings.Skip;
            }

            Redraw();
        }

        protected override void Update()
        {
            base.Update();
            if (_fired)
                return;
            // unscaled: the overlay may have paused the game already
            _left -= Time.unscaledDeltaTime;
            Redraw();
            if (_left <= 0f)
                Finish();
        }

        void Redraw()
        {
            if (countdownLabel == null)
                return;
            countdownLabel.text = GP_OverlayStrings.Seconds(_left);
            GP_OverlayTone.Paint(countdownLabel, Skin, GP_OverlayColorRole.Text);
        }

        void Finish()
        {
            if (_fired)
                return;
            _fired = true;
            var done = _args.onDone;
            Close();
            done?.Invoke();
        }
    }
}
