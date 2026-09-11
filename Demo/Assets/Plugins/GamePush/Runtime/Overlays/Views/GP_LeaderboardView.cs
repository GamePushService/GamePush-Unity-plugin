using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush.Native;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    public sealed class GP_LeaderboardView : GP_OverlayView
    {
        public GP_OverlayList list;
        public GP_OverlayLayoutMode layoutMode;

        [Header("Pinned player row")]
        public GameObject selfRowHolder;

        public GP_LeaderboardRow selfRow;
        public TMP_Text subtitleLabel;
        public GP_LeaderboardRow headerRow;

        NativeLeaderboardResult _result;
        string _displayFields = "";

        public override void Bind(object args)
        {
            var data = args as GP_LeaderboardArgs ?? new GP_LeaderboardArgs();
            _displayFields = data.displayFields ?? "";
            SetTitle(GP_OverlayStrings.Leaderboard);
            SetStatus(GP_OverlayStrings.Loading);
            list?.Clear();
            if (selfRowHolder != null)
                selfRowHolder.SetActive(false);
            EnsureHeader();
            if (headerRow != null)
                headerRow.gameObject.SetActive(false);

            if (layoutMode != null)
                layoutMode.ModeChanged += OnModeChanged;

            if (data.scoped)
            {
                NativeLeaderboardScoped.FetchForOverlay(data.idOrTag, data.variant, data.order, data.limit,
                    data.showNearest, data.includeFields, data.withMe, OnLoaded, ShowError);
            }
            else
            {
                NativeLeaderboard.FetchForOverlay(data.idOrTag, data.orderBy, data.order, data.limit,
                    data.showNearest, data.withMe, data.includeFields, OnLoaded, ShowError);
            }
        }

        protected override void OnClosing()
        {
            if (layoutMode != null)
                layoutMode.ModeChanged -= OnModeChanged;
        }

        void OnModeChanged(GP_LayoutMode mode) => Render();

        void OnLoaded(NativeLeaderboardResult result)
        {
            _result = result;
            if (!string.IsNullOrEmpty(result?.name))
                SetTitle(result.name);
            Render();
        }

        void Render()
        {
            if (_result == null)
                return;

            var mode = layoutMode != null ? layoutMode.Mode : GP_LayoutMode.Compact;
            var selfId = NativePlayer.Id;
            var columns = NativeLeaderboard.VisibleFields(_result.fields, _displayFields);

            if (subtitleLabel != null)
            {
                subtitleLabel.text = _result.player != null && _result.player.position > 0
                    ? GP_OverlayStrings.Position + ": " + _result.player.position
                    : "";
                GP_OverlayTone.Paint(subtitleLabel, Skin, GP_OverlayColorRole.TextMuted);
                subtitleLabel.gameObject.SetActive(!string.IsNullOrEmpty(subtitleLabel.text));
            }

            EnsureHeader();
            if (headerRow != null)
            {
                headerRow.gameObject.SetActive(true);
                headerRow.BindHeader(columns);
            }

            if (_result.players.Count == 0)
            {
                ShowEmpty();
                list?.Clear();
                return;
            }

            SetStatus(null);
            list?.Bind(_result.players.Count, (row, index) =>
            {
                var component = row.GetComponent<GP_LeaderboardRow>();
                if (component == null)
                    return;
                var entry = _result.players[index];
                component.Bind(entry, columns, index, entry.id == selfId, mode);
            }, Skin.leaderboardRow);

            var pinned = _result.player;
            var alreadyInList = pinned != null && NativeLeaderboard.Contains(_result.players, pinned.id);
            if (selfRowHolder == null || selfRow == null || pinned == null || alreadyInList)
            {
                if (selfRowHolder != null)
                    selfRowHolder.SetActive(false);
                return;
            }
            selfRowHolder.SetActive(true);
            selfRow.Bind(pinned, columns, 0, true, mode);
        }

        void EnsureHeader()
        {
            if (headerRow != null || Skin == null || Skin.leaderboardRow == null)
                return;
            var parent = list != null ? list.transform.parent : transform;
            if (parent == null)
                return;
            var instance = Instantiate(Skin.leaderboardRow, parent);
            instance.name = "LeaderboardHeader";
            if (list != null)
                instance.transform.SetSiblingIndex(list.transform.GetSiblingIndex());
            var layout = instance.GetComponent<LayoutElement>() ?? instance.AddComponent<LayoutElement>();
            layout.minHeight = 48f;
            layout.preferredHeight = 48f;
            layout.flexibleHeight = 0f;
            headerRow = instance.GetComponent<GP_LeaderboardRow>();
        }
    }
}
