using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace GamePush.Overlays.Widgets
{
    public sealed class GP_AchievementRow : MonoBehaviour
    {
        const float IconSize = 88f;
        const float CompactIconSize = 56f;
        const float CompactRowWidth = 420f;
        const float ChipIconSize = 22f;
        const float ProgressHeight = 8f;

        public GP_RemoteImage icon;
        public Image background;
        public TMP_Text titleLabel;
        public TMP_Text descriptionLabel;
        public TMP_Text progressLabel;
        public Image progressFill;
        public GameObject progressGroup;
        public GameObject lockedBadge;
        public GameObject unlockedBadge;

        void OnEnable()
        {
            foreach (var horizontal in GetComponents<HorizontalLayoutGroup>())
                horizontal.childForceExpandHeight = false;
            ApplyIconSize();
            Ellipsis(titleLabel, wrap: false);
            Ellipsis(descriptionLabel, wrap: true);
            LockChip(unlockedBadge);
            LockChip(lockedBadge);
            LockProgressTrack();
        }

        void OnRectTransformDimensionsChange() => ApplyIconSize();

        public void Bind(AchievementsFetch achievement, AchievementsFetchPlayer progress, int index)
        {
            var skin = GP_OverlaySkin.Instance;
            var unlocked = progress != null && progress.unlocked;
            var current = progress?.progress ?? 0;
            var max = Mathf.Max(0, achievement.maxProgress);

            // A locked achievement can be configured to hide its name and/or description.
            var showName = unlocked || achievement.lockedVisible;
            var showDescription = unlocked || achievement.lockedDescriptionVisible;

            if (background != null)
                GP_OverlayTone.Paint(background, skin, GP_OverlayTone.RowRole(index));

            if (titleLabel != null)
            {
                titleLabel.text = showName ? achievement.name : GP_OverlayStrings.HiddenAchievement;
                Ellipsis(titleLabel, wrap: false);
                GP_OverlayTone.Paint(titleLabel, skin,
                    unlocked ? GP_OverlayColorRole.Text : GP_OverlayColorRole.TextMuted);
            }

            if (descriptionLabel != null)
            {
                descriptionLabel.text = showDescription ? achievement.description : "";
                Ellipsis(descriptionLabel, wrap: true);
                GP_OverlayTone.Paint(descriptionLabel, skin, GP_OverlayColorRole.TextMuted);
                descriptionLabel.gameObject.SetActive(!string.IsNullOrEmpty(descriptionLabel.text));
            }

            var url = IconUrl(achievement, unlocked);
            if (icon != null)
            {
                var hasIcon = !string.IsNullOrEmpty(url);
                icon.gameObject.SetActive(hasIcon);
                if (hasIcon)
                {
                    ApplyIconSize();
                    icon.Load(url, skin.iconPlaceholder);
                }
            }

            var hasProgress = max > 0;
            if (progressGroup != null)
                progressGroup.SetActive(hasProgress);
            SetProgress(progressFill, hasProgress ? Mathf.Clamp01((float)current / max) : 0f,
                unlocked ? GP_OverlayColorRole.Accent : GP_OverlayColorRole.TextMuted, skin);

            if (progressLabel != null)
            {
                progressLabel.gameObject.SetActive(hasProgress);
                progressLabel.text = hasProgress ? current + " / " + max : "";
                GP_OverlayTone.Paint(progressLabel, skin, GP_OverlayColorRole.TextMuted);
            }

            SetChipLabel(unlockedBadge, GP_OverlayStrings.Unlocked, GP_OverlayColorRole.Accent);
            SetChipLabel(lockedBadge, GP_OverlayStrings.Locked, GP_OverlayColorRole.TextMuted);
            if (lockedBadge != null)
                lockedBadge.SetActive(!unlocked);
            if (unlockedBadge != null)
                unlockedBadge.SetActive(unlocked);
        }

        void ApplyIconSize()
        {
            if (icon == null)
                return;
            var width = ((RectTransform)transform).rect.width;
            var size = width > 0f && width < CompactRowWidth ? CompactIconSize : IconSize;
            GP_LayoutSquare.Lock(icon, size);
        }

        static void Ellipsis(TMP_Text text, bool wrap)
        {
            if (text == null)
                return;
            text.textWrappingMode = wrap ? TextWrappingModes.Normal : TextWrappingModes.NoWrap;
            text.overflowMode = TextOverflowModes.Ellipsis;
        }

        static string IconUrl(AchievementsFetch achievement, bool unlocked)
        {
            if (achievement == null)
                return "";
            var url = unlocked ? achievement.icon : achievement.lockedIcon;
            if (string.IsNullOrEmpty(url))
                url = unlocked ? achievement.iconSmall : achievement.lockedIconSmall;
            if (string.IsNullOrEmpty(url))
                url = achievement.icon;
            return url ?? "";
        }

        static void SetProgress(Image fill, float amount, GP_OverlayColorRole role, GP_OverlaySkin skin)
        {
            if (fill == null)
                return;
            GP_OverlayTone.Paint(fill, skin, role);
            if (fill.transform.parent != null)
            {
                var track = fill.transform.parent.GetComponent<Image>();
                if (track != null)
                    GP_OverlayTone.Paint(track, skin, GP_OverlayColorRole.Input);
            }
            fill.type = fill.sprite != null ? Image.Type.Sliced : Image.Type.Simple;
            fill.fillAmount = 1f;
            var rect = fill.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = new Vector2(Mathf.Clamp01(amount), 1f);
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
            fill.enabled = amount > 0.001f;
        }

        static void LockChip(GameObject chip)
        {
            if (chip == null)
                return;
            var horizontal = chip.GetComponent<HorizontalLayoutGroup>();
            if (horizontal != null)
                horizontal.childForceExpandHeight = false;
            var layout = chip.GetComponent<LayoutElement>();
            if (layout != null)
            {
                layout.flexibleHeight = 0f;
                if (layout.preferredHeight < 40f)
                    layout.preferredHeight = 44f;
            }

            var glyph = chip.transform.Find("Icon");
            if (glyph != null)
                GP_LayoutSquare.Lock(glyph, ChipIconSize);
        }

        void LockProgressTrack()
        {
            if (progressFill == null || progressFill.transform.parent == null)
                return;
            var bar = progressFill.transform.parent.gameObject;
            var layout = bar.GetComponent<LayoutElement>() ?? bar.AddComponent<LayoutElement>();
            layout.minHeight = ProgressHeight;
            layout.preferredHeight = ProgressHeight;
            layout.flexibleHeight = 0f;
            if (progressGroup != null)
            {
                var groupLayout = progressGroup.GetComponent<HorizontalLayoutGroup>();
                if (groupLayout != null)
                    groupLayout.childForceExpandHeight = false;
            }
        }

        static void SetChipLabel(GameObject chip, string value, GP_OverlayColorRole role)
        {
            if (chip == null)
                return;
            var image = chip.GetComponent<Image>();
            if (image != null)
                GP_OverlayTone.Paint(image, GP_OverlaySkin.Instance, GP_OverlayColorRole.Button);
            var label = chip.GetComponentInChildren<TMP_Text>(true);
            if (label == null)
                return;
            label.text = value;
            GP_OverlayTone.Paint(label, GP_OverlaySkin.Instance, role);
        }
    }
}
