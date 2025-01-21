using System;
using System.Collections;
using System.Collections.Generic;
using GamePush;

namespace GamePush.Core
{
    public class GameCollectionsModule
    {
        public event Action OnGamesCollectionsOpen;
        public event Action OnGamesCollectionsClose;

        public event Action<string, GamesCollectionsData> OnGamesCollectionsFetch;
        public event Action OnGamesCollectionsFetchError;

        public void Open(string idOrTag)
        {
           
        }

        public void Fetch(string idOrTag)
        {
           
        }
    }
}
