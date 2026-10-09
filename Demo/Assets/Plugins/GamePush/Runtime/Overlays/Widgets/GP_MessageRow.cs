using System;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Native;

namespace GamePush.Overlays.Widgets
{
    public sealed class GP_MessageRow : MonoBehaviour
    {
        public Image bubble;
        public GP_RemoteImage avatar;
        public TMP_Text authorLabel;
        public TMP_Text textLabel;
        public TMP_Text timeLabel;
        public HorizontalLayoutGroup layout;
        public Button deleteButton;

        void OnEnable()
        {
            if (layout == null)
                layout = GetComponent<HorizontalLayoutGroup>();
            if (layout != null)
                layout.childForceExpandHeight = false;
        }

        /// <summary>Feedback threads carry no player object, only an author role and a timestamp.</summary>
        public void Bind(FeedbackMessageData message, bool isOwn)
        {
            Bind(new NativeChatMessage
            {
                id = message.id,
                text = message.text ?? "",
                createdAt = message.createdAt ?? "",
                authorName = message.author ?? ""
            }, isOwn, null);
        }

        public void Bind(NativeChatMessage message, bool isOwn, Action onDelete)
        {
            var skin = GP_OverlaySkin.Instance;

            if (bubble != null)
                GP_OverlayTone.Paint(bubble, skin, isOwn ? GP_OverlayColorRole.RowAlt : GP_OverlayColorRole.Row);

            if (authorLabel != null)
            {
                authorLabel.text = isOwn
                    ? GP_OverlayStrings.You
                    : string.IsNullOrEmpty(message.authorName) ? "#" + message.authorId : message.authorName;
                GP_OverlayTone.Paint(authorLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            if (textLabel != null)
            {
                textLabel.text = message.text;
                GP_OverlayTone.Paint(textLabel, skin, GP_OverlayColorRole.Text);
            }

            if (timeLabel != null)
            {
                timeLabel.text = FormatTime(message.createdAt);
                GP_OverlayTone.Paint(timeLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            if (avatar != null)
                avatar.LoadPlayer(message.authorAvatar, message.authorId, skin.avatarPlaceholder);

            // Own messages hug the right edge, everyone else's the left.
            if (layout != null)
                layout.reverseArrangement = isOwn;

            if (deleteButton == null)
                return;
            deleteButton.gameObject.SetActive(isOwn && onDelete != null);
            deleteButton.onClick.RemoveAllListeners();
            if (isOwn && onDelete != null)
                deleteButton.onClick.AddListener(() => onDelete());
        }

        static string FormatTime(string raw)
        {
            if (string.IsNullOrEmpty(raw))
                return "";
            if (DateTime.TryParse(raw, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out var utc))
                return utc.ToLocalTime().ToString("HH:mm", CultureInfo.CurrentCulture);
            return "";
        }
    }
}
