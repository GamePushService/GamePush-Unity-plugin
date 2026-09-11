using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Native;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    public sealed class GP_AchievementsView : GP_OverlayView
    {
        public GP_OverlayList list;
        public GP_OverlayLayoutMode layoutMode;
        public GP_FlexibleGrid grid;

        [Header("Group filter (left rail in Wide, chips in Compact)")]
        public RectTransform groupRail;

        public Button groupButtonTemplate;
        public RectTransform compactGroupRail;
        public Button compactGroupButtonTemplate;
        public TMP_Text counterLabel;

        protected override Transform StatusHost =>
            list != null ? list.transform : null;

        readonly List<AchievementsFetch> _all = new List<AchievementsFetch>();
        readonly List<AchievementsFetch> _visible = new List<AchievementsFetch>();
        readonly List<AchievementsFetchGroups> _groups = new List<AchievementsFetchGroups>();
        readonly List<Button> _groupButtons = new List<Button>();
        readonly List<Button> _compactGroupButtons = new List<Button>();

        int _selectedGroup = -1;

        public override void Bind(object args)
        {
            SetTitle(GP_OverlayStrings.Achievements);
            FitCounter();
            ShowLoading();
            list?.Clear();

            GP_Achievements.OnAchievementsFetch += OnFetched;
            GP_Achievements.OnAchievementsFetchGroups += OnGroups;
            GP_Achievements.OnAchievementsFetchError += OnError;
            GP_Achievements.Fetch();
        }

        protected override void OnClosing()
        {
            GP_Achievements.OnAchievementsFetch -= OnFetched;
            GP_Achievements.OnAchievementsFetchGroups -= OnGroups;
            GP_Achievements.OnAchievementsFetchError -= OnError;
        }

        void OnError() => ShowError(null);

        void OnGroups(List<AchievementsFetchGroups> groups)
        {
            _groups.Clear();
            if (groups != null)
                _groups.AddRange(groups);
            BuildGroupRail();
        }

        void OnFetched(List<AchievementsFetch> achievements)
        {
            _all.Clear();
            if (achievements != null)
                _all.AddRange(achievements);
            BuildGroupRail();
            Refresh();
        }

        void BuildGroupRail()
        {
            BuildGroupRail(groupRail, groupButtonTemplate, _groupButtons);
            BuildGroupRail(compactGroupRail, compactGroupButtonTemplate, _compactGroupButtons);
        }

        void BuildGroupRail(RectTransform rail, Button template, List<Button> buttons)
        {
            if (rail == null || template == null)
                return;
            var needed = _groups.Count + 1;
            for (var i = buttons.Count; i < needed; i++)
            {
                var button = Instantiate(template, rail);
                button.gameObject.SetActive(true);
                buttons.Add(button);
            }

            for (var i = 0; i < buttons.Count; i++)
            {
                var button = buttons[i];
                var show = i < needed;
                button.gameObject.SetActive(show);
                if (!show)
                    continue;

                var groupIndex = i - 1;
                var selected = groupIndex == _selectedGroup;
                var title = groupIndex < 0 ? GP_OverlayStrings.All : _groups[groupIndex].name;
                var count = GroupCount(groupIndex);
                var chip = button.GetComponent<GP_OverlayChip>();
                if (chip != null)
                {
                    chip.Bind(title, count, selected);
                }
                else
                {
                    var label = button.GetComponentInChildren<TMP_Text>();
                    if (label != null)
                    {
                        label.text = title;
                        GP_OverlayTone.Paint(label, Skin,
                            selected ? GP_OverlayColorRole.Accent : GP_OverlayColorRole.TextMuted);
                    }
                }

                button.onClick.RemoveAllListeners();
                var captured = groupIndex;
                button.onClick.AddListener(() =>
                {
                    _selectedGroup = captured;
                    BuildGroupRail();
                    Refresh();
                });
            }

            rail.gameObject.SetActive(_groups.Count > 0);
        }

        string GroupCount(int groupIndex)
        {
            var unlocked = 0;
            var total = 0;
            if (groupIndex < 0)
            {
                total = _all.Count;
                unlocked = CountUnlocked(_all);
            }
            else if (groupIndex < _groups.Count)
            {
                var ids = _groups[groupIndex].achievements;
                if (ids == null)
                    return "0 / 0";
                foreach (var achievement in _all)
                {
                    if (System.Array.IndexOf(ids, achievement.id) < 0)
                        continue;
                    total++;
                    var entry = NativeAchievements.PlayerEntry(achievement.id);
                    if (entry != null && entry.unlocked)
                        unlocked++;
                }
            }

            return unlocked + " / " + total;
        }

        static int CountUnlocked(List<AchievementsFetch> achievements)
        {
            var unlocked = 0;
            foreach (var achievement in achievements)
            {
                var entry = NativeAchievements.PlayerEntry(achievement.id);
                if (entry != null && entry.unlocked)
                    unlocked++;
            }
            return unlocked;
        }

        void Refresh()
        {
            _visible.Clear();
            if (_selectedGroup < 0 || _selectedGroup >= _groups.Count)
            {
                _visible.AddRange(_all);
            }
            else
            {
                var ids = _groups[_selectedGroup].achievements;
                foreach (var achievement in _all)
                {
                    if (System.Array.IndexOf(ids, achievement.id) >= 0)
                        _visible.Add(achievement);
                }
            }

            if (_visible.Count == 0)
            {
                ShowEmpty();
                list?.Clear();
                UpdateCounter();
                return;
            }

            SetStatus(null);
            list?.Bind(_visible.Count, (row, index) =>
            {
                var component = row.GetComponent<GP_AchievementRow>();
                if (component == null)
                    return;
                var achievement = _visible[index];
                component.Bind(achievement, NativeAchievements.PlayerEntry(achievement.id), index);
            }, Skin.achievementRow);
            grid?.Rebuild();
            UpdateCounter();
        }

        void UpdateCounter()
        {
            if (counterLabel == null)
                return;
            FitCounter();
            counterLabel.richText = true;
            counterLabel.text = GP_OverlayStrings.UnlockedProgress(CountUnlocked(_all), _all.Count, Skin.AccentHex);
            GP_OverlayTone.Paint(counterLabel, Skin, GP_OverlayColorRole.TextMuted);
        }

        void FitCounter()
        {
            if (counterLabel == null)
                return;
            EllipsisSingleLine(counterLabel);
            var layout = counterLabel.GetComponent<LayoutElement>() ??
                         counterLabel.gameObject.AddComponent<LayoutElement>();
            layout.minWidth = 0f;
            if (layout.preferredWidth > 200f)
                layout.preferredWidth = 200f;
            if (layout.flexibleWidth < 0f)
                layout.flexibleWidth = 0.4f;
        }
    }
}
