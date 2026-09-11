using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamePush.Overlays.Widgets
{
    public sealed class GP_MemberRow : MonoBehaviour
    {
        public Image background;
        public GP_RemoteImage avatar;
        public TMP_Text nameLabel;
        public TMP_Text stateLabel;
        public Image onlineDot;
        public Button muteButton;
        public Button kickButton;

        void OnEnable()
        {
            var horizontal = GetComponent<HorizontalLayoutGroup>();
            if (horizontal != null)
                horizontal.childForceExpandHeight = false;
            LockOnlineDot();
        }

        void LockOnlineDot()
        {
            if (onlineDot == null)
                return;
            var layout = onlineDot.GetComponent<LayoutElement>();
            var side = 14f;
            if (layout != null)
                side = Mathf.Max(layout.minWidth, layout.minHeight, layout.preferredWidth, layout.preferredHeight, 14f);
            GP_LayoutSquare.Lock(onlineDot, side);
        }

        public void Bind(string json, int index, bool canMute, bool canKick, Action onMute, Action onKick)
        {
            var skin = GP_OverlaySkin.Instance;

            var id = Native.GpJson.GetInt(json, "id");
            var isOnline = Native.GpJson.GetBool(json, "isOnline");
            var state = Native.GpJson.GetObject(json, "state");
            var mute = Native.GpJson.GetObject(json, "mute");
            var isMuted = mute != null && Native.GpJson.GetBool(mute, "isMuted");

            var displayName = "";
            var avatarUrl = "";
            if (!string.IsNullOrEmpty(state))
            {
                if (Native.GpJson.TryGetString(state, "name", out var stateName))
                    displayName = stateName ?? "";
                if (Native.GpJson.TryGetString(state, "avatar", out var stateAvatar))
                    avatarUrl = stateAvatar ?? "";
            }

            if (background != null)
                GP_OverlayTone.Paint(background, skin, GP_OverlayTone.RowRole(index));

            if (nameLabel != null)
            {
                nameLabel.text = string.IsNullOrEmpty(displayName) ? "#" + id : displayName;
                GP_OverlayTone.Paint(nameLabel, skin, GP_OverlayColorRole.Text);
            }

            if (stateLabel != null)
            {
                stateLabel.text = isMuted ? GP_OverlayStrings.Muted : "";
                GP_OverlayTone.Paint(stateLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            if (onlineDot != null)
                GP_OverlayTone.Paint(onlineDot, skin,
                    isOnline ? GP_OverlayColorRole.Accent : GP_OverlayColorRole.TextMuted);

            if (avatar != null)
                avatar.LoadPlayer(avatarUrl, id, skin.avatarPlaceholder);

            Wire(muteButton, canMute, onMute);
            Wire(kickButton, canKick, onKick);
        }

        static void Wire(Button button, bool visible, Action action)
        {
            if (button == null)
                return;
            button.gameObject.SetActive(visible && action != null);
            button.onClick.RemoveAllListeners();
            if (visible && action != null)
                button.onClick.AddListener(() => action());
        }
    }
}
