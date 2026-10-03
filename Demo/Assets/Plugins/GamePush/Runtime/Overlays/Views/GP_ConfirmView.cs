using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    public sealed class GP_ConfirmView : GP_OverlayView
    {
        public TMP_Text messageLabel;
        public Button confirmButton;
        public Button cancelButton;
        public TMP_Text confirmLabel;
        public TMP_Text cancelLabel;
        public Image confirmBackground;
        public Image cancelBackground;

        Action<bool> _onResult;
        bool _answered;

        public override void Bind(object args)
        {
            var data = args as GP_ConfirmArgs ?? new GP_ConfirmArgs();
            _onResult = data.onResult;
            _answered = false;

            SetTitle(string.IsNullOrEmpty(data.title) ? GP_OverlayStrings.Confirm : data.title);
            if (messageLabel != null)
            {
                messageLabel.text = data.text ?? "";
                messageLabel.gameObject.SetActive(!string.IsNullOrEmpty(messageLabel.text));
            }

            var skin = Skin;
            if (confirmLabel != null)
                confirmLabel.text = string.IsNullOrEmpty(data.confirmLabel)
                    ? GP_OverlayStrings.Ok
                    : data.confirmLabel;
            if (cancelLabel != null)
                cancelLabel.text = string.IsNullOrEmpty(data.cancelLabel)
                    ? GP_OverlayStrings.Cancel
                    : data.cancelLabel;

            if (confirmBackground != null)
                GP_OverlayTone.Paint(confirmBackground, skin,
                    data.invertButtonColors ? GP_OverlayColorRole.Button : GP_OverlayColorRole.Accent);
            if (cancelBackground != null)
                GP_OverlayTone.Paint(cancelBackground, skin,
                    data.invertButtonColors ? GP_OverlayColorRole.Accent : GP_OverlayColorRole.Button);

            if (cancelButton != null)
                cancelButton.gameObject.SetActive(!data.hideCancelButton);

            // A dismissal that is not an explicit confirm must read as "cancel".
            closeOnBackdrop = !data.hideCancelButton;
            if (confirmButton != null)
            {
                confirmButton.onClick.RemoveAllListeners();
                confirmButton.onClick.AddListener(() => Answer(true));
            }
            if (cancelButton != null)
            {
                cancelButton.onClick.RemoveAllListeners();
                cancelButton.onClick.AddListener(() => Answer(false));
            }
        }

        void Answer(bool value)
        {
            if (_answered)
                return;
            _answered = true;
            var callback = _onResult;
            _onResult = null;
            Close();
            callback?.Invoke(value);
        }

        protected override void OnClosing()
        {
            if (_answered)
                return;
            _answered = true;
            var callback = _onResult;
            _onResult = null;
            callback?.Invoke(false);
        }
    }
}
