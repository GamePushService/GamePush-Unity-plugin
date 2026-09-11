using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Overlays;

namespace GamePush.Overlays.Widgets
{
    /// <summary>Selectable group/tab chip: title, optional count, accent outline when selected.</summary>
    public sealed class GP_OverlayChip : MonoBehaviour
    {
        public Image background;
        public Outline selectedOutline;
        public TMP_Text titleLabel;
        public TMP_Text countLabel;

        public void Bind(string title, string count, bool selected)
        {
            var skin = GP_OverlaySkin.Instance;

            if (titleLabel != null)
            {
                titleLabel.text = title ?? "";
                GP_OverlayTone.Paint(titleLabel, skin,
                    selected ? GP_OverlayColorRole.Accent : GP_OverlayColorRole.TextMuted);
            }

            if (countLabel != null)
            {
                var show = !string.IsNullOrEmpty(count);
                countLabel.gameObject.SetActive(show);
                countLabel.text = count ?? "";
                GP_OverlayTone.Paint(countLabel, skin,
                    selected ? GP_OverlayColorRole.Text : GP_OverlayColorRole.TextMuted);
            }

            if (background != null)
                GP_OverlayTone.Paint(background, skin,
                    selected ? GP_OverlayColorRole.ButtonSelected : GP_OverlayColorRole.Button);

            if (selectedOutline == null)
                return;
            selectedOutline.effectColor = skin.accent;
            selectedOutline.enabled = selected;
        }
    }
}
