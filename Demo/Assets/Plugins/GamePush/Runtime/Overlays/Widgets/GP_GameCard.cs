using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamePush.Overlays.Widgets
{
    public sealed class GP_GameCard : MonoBehaviour
    {
        public Image background;
        public GP_RemoteImage icon;
        public TMP_Text nameLabel;
        public TMP_Text playLabel;
        public Button button;

        public void Bind(Games game, int index)
        {
            var skin = GP_OverlaySkin.Instance;

            if (background != null)
                GP_OverlayTone.Paint(background, skin, GP_OverlayTone.RowRole(index));

            if (nameLabel != null)
            {
                nameLabel.text = game.name ?? "";
                GP_OverlayTone.Paint(nameLabel, skin, GP_OverlayColorRole.Text);
            }

            if (playLabel != null)
            {
                playLabel.text = GP_OverlayStrings.Play;
                GP_OverlayTone.Paint(playLabel, skin, GP_OverlayColorRole.Accent);
            }

            if (icon != null)
                icon.Load(game.icon, skin.iconPlaceholder);

            if (button == null)
                return;
            button.onClick.RemoveAllListeners();
            var url = game.url;
            button.interactable = !string.IsNullOrEmpty(url);
            if (!string.IsNullOrEmpty(url))
                button.onClick.AddListener(() => Application.OpenURL(url));
        }
    }
}
