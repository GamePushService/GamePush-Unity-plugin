using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Core
{
    public class LeaderboardModule
    {
        public event Action<string, GP_Data> OnFetchSuccess;
        public event Action<string, GP_Data> OnFetchTopPlayers;
        public event Action<string, GP_Data> OnFetchAbovePlayers;
        public event Action<string, GP_Data> OnFetchBelowPlayers;
        public event Action<string, GP_Data> OnFetchPlayer;
        public event Action OnFetchError;

        public event Action<string, int> OnFetchPlayerRatingSuccess;
        public event Action OnFetchPlayerRatingError;

        public event Action OnLeaderboardOpen;
        public event Action OnLeaderboardClose;

        public event Action<string, GP_Data> OnScopedFetchSuccess;
        public event Action<string, GP_Data> OnScopedFetchTopPlayers;
        public event Action<string, GP_Data> OnScopedFetchAbovePlayers;
        public event Action<string, GP_Data> OnScopedFetchBelowPlayers;
        public event Action<string, GP_Data> OnScopedFetchPlayer;
        public event Action<string, string, GP_Data> OnScopedFetchTagVariant;
        public event Action OnScopedFetchError;

        public event Action<string, int> OnScopedFetchPlayerRating;
        public event Action<string, string, int> OnScopedFetchPlayerRatingTagVariant;
        public event Action OnScopedFetchPlayerRatingError;

        public event Action OnScopedOpen;
        public event Action OnScopedClose;

        public event Action OnScopedPublishRecordComplete;
        public event Action OnScopedPublishRecordError;


        public LeaderboardModule()
        {

        }

        public void Open(string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 5, WithMe withMe = WithMe.none, string includeFields = "", string displayFields = "")
        {
            Logger.Log("OPEN");
        }

        public async void Fetch(string tag = "", string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 0, WithMe withMe = WithMe.none, string includeFields = "")
        {
            Logger.Log("FETCH");
            GetLeaderboardQuery input = new GetLeaderboardQuery();
            await DataFetcher.FetchTop(input, withMe != WithMe.none);
        }

        public void FetchPlayerRating(string tag = "", string orderBy = "score", Order order = Order.DESC)
        {
            Logger.Log("FETCH PLAYER RATING");
        }

        public void OpenScoped(string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, int showNearest = 5, string includeFields = "", string displayFields = "", WithMe withMe = WithMe.first)
        {
            Logger.Log("OPEN SCOPED");
        }

        public void FetchScoped(string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, int showNearest = 5, string includeFields = "", WithMe withMe = WithMe.none)
        {
            Logger.Log("FETCH");
        }

        public void PublishRecord(string idOrTag = "", string variant = "some_variant", bool Override = true, string key1 = "", float record_value1 = 0, string key2 = "", float record_value2 = 0, string key3 = "", float record_value3 = 0)
        {
            Logger.Log("PUBLICH RECORD");
        }

        public void FetchScopedPlayerRating(string idOrTag = "", string variant = "some_variant", string includeFields = "")
        {
            Logger.Log("FETCH PLAYER RATING");
        }
    }
}
