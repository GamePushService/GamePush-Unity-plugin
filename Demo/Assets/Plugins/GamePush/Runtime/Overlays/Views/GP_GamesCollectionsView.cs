using UnityEngine;
using GamePush.Native;
using GamePush.Overlays.Widgets;

namespace GamePush.Overlays.Views
{
    public sealed class GP_GamesCollectionsView : GP_OverlayView
    {
        public GP_OverlayList list;
        public GP_FlexibleGrid grid;

        GamesCollectionsFetchData _data;

        public override void Bind(object args)
        {
            var request = args as GP_GamesCollectionsArgs ?? new GP_GamesCollectionsArgs();
            SetTitle(GP_OverlayStrings.Games);
            ShowLoading();
            list?.Clear();

            NativeGamesCollections.FetchForOverlay(request.idOrTag, OnLoaded, ShowError);
        }

        void OnLoaded(GamesCollectionsFetchData data)
        {
            _data = data;
            if (!string.IsNullOrEmpty(data?.name))
                SetTitle(data.name);

            var games = data?.games;
            if (games == null || games.Length == 0)
            {
                ShowEmpty();
                return;
            }

            SetStatus(null);
            list?.Bind(games.Length, (row, index) =>
            {
                var card = row.GetComponent<GP_GameCard>();
                card?.Bind(games[index], index);
            }, Skin.gameCard);
            grid?.Rebuild();
        }
    }
}
