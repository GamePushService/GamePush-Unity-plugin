using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamePush;
using GamePush.Data;

namespace GamePush.Core
{
    public class GameCollectionsModule
    {
        public event Action OnGamesCollectionsOpen;
        public event Action OnGamesCollectionsClose;

        public event Action<string, GamesCollectionsData> OnGamesCollectionsFetch;
        public event Action OnGamesCollectionsFetchError;

        private List<string> availablePlatforms = new List<string>{
            PlatformTypes.YANDEX,
            PlatformTypes.VK,
            PlatformTypes.OK,
            PlatformTypes.GAMEPIX,
            PlatformTypes.Y8,
        };

        public bool IsAvailable()
        {
            #if UNITY_EDITOR
            return true;
            #else
            return CoreSDK.Platform.isExternalLinksAllowed || availablePlatforms.Contains(CoreSDK.Platform.Type);
            #endif
        }

        public void Open(string idOrTag)
        {
           
        }

        
        private GamePreview MapProjectToGamePreview(GameProject project)
        {
            return new GamePreview
            {
                Id = project.Id,
                Name = project.Name,
                Description = project.Description,
                Url = project.Url,
                Icon = project.Assets.Icon
                // Icon4x3 = GetAssetImage(project.Assets.Icon4x3),
                // Cover = GetAssetImage(project.Assets.Cover),
                // AlbumScreenshots = GetAssetImages(project.Assets.AlbumScreenshots),
                // PortraitScreenshots = GetAssetImages(project.Assets.PortraitScreenshots),
            };
        }
        public async Task<GamesCollection> Fetch(string idOrTag = "ALL")
        {
            if (!IsAvailable())
            {
                Logger.Warn($"Not available on {CoreSDK.Platform.Type}");
                OnGamesCollectionsFetchError?.Invoke();
                return null;
            }

            FetchPlayerGamesCollectionInput input = new FetchPlayerGamesCollectionInput();
            if (int.TryParse(idOrTag, out var id))
            {
                input.id = id;
            }
            else
            {
                input.tag = idOrTag;
            }

            input.urlFrom = CoreSDK.App.AppLink() == null ? "" : CoreSDK.App.AppLink();
            
            var result = await DataFetcher.FetchGameCollections(input);
            
            if (result == null)
            {
                Logger.Warn("Can't fetch GamesCollections list");
                OnGamesCollectionsFetchError?.Invoke();
                return null;
            }

            var mappedGames = result.Games;
            
            var collection = new GamesCollection
            {
                Id = result.Id,
                Tag = result.Tag,
                Name = result.Name,
                Description = result.Description,
                Games = mappedGames
                    .Where(p => !string.IsNullOrEmpty(p.Url))
                    .Select(MapProjectToGamePreview)
                    .ToList()
            };

            if (collection.Games == null || collection.Games.Count == 0)
            {
                Logger.Warn("Empty games collection");
            }

            // OnGamesCollectionsFetch?.Invoke(idOrTag, collection);
            // Генерируем событие завершения

            return collection;

        }
    }
}
