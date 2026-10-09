using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Native;

namespace GamePush.Overlays.Widgets
{
    public sealed class GP_LeaderboardRow : MonoBehaviour
    {
        public const float ColumnWidth = 112f;

        public Image background;
        public TMP_Text positionLabel;
        public GP_RemoteImage avatar;
        public TMP_Text nameLabel;
        public TMP_Text scoreLabel;

        [Tooltip("Value columns filled from leaderboard fields.")]
        public RectTransform extraColumns;

        public TMP_Text extraColumnTemplate;

        readonly List<TMP_Text> _extras = new List<TMP_Text>();

        void OnEnable()
        {
            var horizontal = GetComponent<HorizontalLayoutGroup>();
            if (horizontal != null)
                horizontal.childForceExpandHeight = false;
            LockLayout();
        }

        public void Bind(NativeLeaderboardEntry entry, IReadOnlyList<NativeLeaderboardField> fields, int index,
            bool isSelf, GP_LayoutMode mode)
        {
            var skin = GP_OverlaySkin.Instance;
            LockLayout();

            if (background != null)
                GP_OverlayTone.Paint(background, skin, isSelf ? GP_OverlayColorRole.Accent : GP_OverlayTone.RowRole(index));

            if (positionLabel != null)
            {
                positionLabel.text = entry.position > 0 ? entry.position.ToString() : (index + 1).ToString();
                GP_OverlayTone.Paint(positionLabel, skin, GP_OverlayColorRole.Text);
            }

            if (nameLabel != null)
            {
                var displayName = entry.name;
                if (string.IsNullOrEmpty(displayName))
                    displayName = isSelf ? GP_OverlayStrings.You : GP_OverlayStrings.PlayerNumber(entry.id);
                else if (isSelf)
                    displayName += " (" + GP_OverlayStrings.You + ")";
                nameLabel.text = displayName;
                GP_OverlayTone.Paint(nameLabel, skin, GP_OverlayColorRole.Text);
            }

            if (avatar != null)
            {
                avatar.gameObject.SetActive(true);
                var image = avatar.GetComponent<Image>();
                if (image != null)
                    image.enabled = true;
                avatar.LoadPlayer(entry.avatar, entry.id,
                    skin.avatarPlaceholder != null ? skin.avatarPlaceholder : skin.circleSprite);
            }

            BindValues(fields, key => Value(entry, key), skin.text, false);
        }

        public void BindHeader(IReadOnlyList<NativeLeaderboardField> fields)
        {
            var skin = GP_OverlaySkin.Instance;
            LockLayout();

            if (background != null)
                GP_OverlayTone.Paint(background, skin, GP_OverlayColorRole.Header);

            if (positionLabel != null)
            {
                positionLabel.text = "#";
                GP_OverlayTone.Paint(positionLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            if (nameLabel != null)
            {
                nameLabel.text = "";
                GP_OverlayTone.Paint(nameLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            if (avatar != null)
            {
                avatar.gameObject.SetActive(true);
                var image = avatar.GetComponent<Image>();
                if (image != null)
                    image.enabled = false;
            }

            BindValues(fields, field => LabelOf(field), skin.textMuted, true);
        }

        void BindValues(IReadOnlyList<NativeLeaderboardField> fields,
            System.Func<NativeLeaderboardField, string> value, Color color, bool header)
        {
            var columns = fields ?? (IReadOnlyList<NativeLeaderboardField>)System.Array.Empty<NativeLeaderboardField>();
            if (extraColumns != null)
                extraColumns.gameObject.SetActive(columns.Count > 0);

            if (extraColumnTemplate != null)
            {
                for (var i = _extras.Count; i < columns.Count; i++)
                {
                    var label = Instantiate(extraColumnTemplate, extraColumns);
                    label.gameObject.SetActive(true);
                    StyleValue(label);
                    _extras.Add(label);
                }
            }

            for (var i = 0; i < _extras.Count; i++)
            {
                var show = i < columns.Count;
                _extras[i].gameObject.SetActive(show);
                if (!show)
                    continue;
                _extras[i].text = value(columns[i]) ?? "";
                GP_OverlayTone.Paint(_extras[i], GP_OverlaySkin.Instance,
                    header ? GP_OverlayColorRole.TextMuted : GP_OverlayColorRole.Text);
            }

            if (scoreLabel == null)
                return;
            // Score is part of fields when the API returns it; keep the standalone slot only as fallback.
            var scoreInFields = ContainsKey(columns, "score");
            scoreLabel.gameObject.SetActive(!scoreInFields && columns.Count == 0);
            if (scoreLabel.gameObject.activeSelf)
            {
                StyleValue(scoreLabel);
                GP_OverlayTone.Paint(scoreLabel, GP_OverlaySkin.Instance,
                    header ? GP_OverlayColorRole.TextMuted : GP_OverlayColorRole.Text);
                if (!header)
                    scoreLabel.text = value(new NativeLeaderboardField { key = "score", name = "score" });
                else
                    scoreLabel.text = "score";
            }
        }

        void LockLayout()
        {
            StyleName(nameLabel);
            StyleValue(scoreLabel);
            if (extraColumns != null)
            {
                var layout = extraColumns.GetComponent<LayoutElement>() ??
                             extraColumns.gameObject.AddComponent<LayoutElement>();
                layout.flexibleWidth = 0f;
                layout.flexibleHeight = 0f;
                var horizontal = extraColumns.GetComponent<HorizontalLayoutGroup>();
                if (horizontal != null)
                {
                    horizontal.childForceExpandWidth = false;
                    horizontal.childForceExpandHeight = false;
                }
            }

            if (avatar != null)
                GP_LayoutSquare.Lock(avatar, 72f);
        }

        static void StyleName(TMP_Text label)
        {
            if (label == null)
                return;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Ellipsis;
            var layout = label.GetComponent<LayoutElement>() ?? label.gameObject.AddComponent<LayoutElement>();
            layout.flexibleWidth = 1f;
            layout.flexibleHeight = 0f;
            layout.minWidth = 64f;
            layout.preferredWidth = 80f;
        }

        static void StyleValue(TMP_Text label)
        {
            if (label == null)
                return;
            label.alignment = TextAlignmentOptions.MidlineRight;
            label.textWrappingMode = TextWrappingModes.NoWrap;
            label.overflowMode = TextOverflowModes.Ellipsis;
            var layout = label.GetComponent<LayoutElement>() ?? label.gameObject.AddComponent<LayoutElement>();
            layout.minWidth = ColumnWidth;
            layout.preferredWidth = ColumnWidth;
            layout.flexibleWidth = 0f;
            layout.flexibleHeight = 0f;
        }

        static string Value(NativeLeaderboardEntry entry, NativeLeaderboardField field)
        {
            if (entry == null || field == null)
                return "";
            if (field.key == "score")
            {
                var raw = entry.Get("score");
                return string.IsNullOrEmpty(raw) ? Format(entry.score) : raw;
            }
            return entry.Get(field.key);
        }

        static string LabelOf(NativeLeaderboardField field)
        {
            if (field == null)
                return "";
            return string.IsNullOrEmpty(field.name) ? field.key : field.name;
        }

        static bool ContainsKey(IReadOnlyList<NativeLeaderboardField> fields, string key)
        {
            if (fields == null)
                return false;
            for (var i = 0; i < fields.Count; i++)
            {
                if (fields[i] != null && fields[i].key == key)
                    return true;
            }
            return false;
        }

        static string Format(double value)
        {
            if (System.Math.Abs(value - System.Math.Round(value)) < 0.0001)
                return ((long)System.Math.Round(value)).ToString();
            return value.ToString("0.##", System.Globalization.CultureInfo.InvariantCulture);
        }
    }
}
