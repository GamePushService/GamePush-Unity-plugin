using UnityEngine.Events;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamePush
{
    public class GP_LeaderboardScoped : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.LeaderboardScoped);

        #region Actions
        public static event UnityAction<string, GP_Data> OnFetchSuccess;
        public static event UnityAction<string, GP_Data> OnFetchTopPlayers;
        public static event UnityAction<string, GP_Data> OnFetchAbovePlayers;
        public static event UnityAction<string, GP_Data> OnFetchBelowPlayers;
        public static event UnityAction<string, GP_Data> OnFetchPlayer;
        public static event UnityAction<string, string, GP_Data> OnFetchTagVariant;
        public static event UnityAction OnFetchError;

        public static event UnityAction<string, int> OnFetchPlayerRating;
        public static event UnityAction<string, string, int> OnFetchPlayerRatingTagVariant;
        public static event UnityAction OnFetchPlayerRatingError;

        public static event UnityAction OnOpen;
        public static event UnityAction OnClose;

        public static event UnityAction OnPublishRecordComplete;
        public static event UnityAction OnPublishRecordError;
        #endregion

        private string _leaderboardFetchTag;
        private string _leaderboardFetchVariant;

        private string _leaderboardPlayerRatingFetchTag;
        private string _leaderboardPlayerRatingFetchVariant;

#if !UNITY_EDITOR && UNITY_WEBGL
        #region DllImport

        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_Open(
            string idOrTag = "",
            string variant = "",
            string order = "DESC",
            int limit = 10,
            int showNearest = 5,
            string includeFields = "",
            string displayFields = "",
            string withMe = "none"
        );
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_Fetch(
            string idOrTag = "",
            string variant = "",
            string order = "DESC",
            int limit = 10,
            int showNearest = 5,
            string includeFields = "",
            string withMe = "none"
        );
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_PublishRecord(
            string idOrTag = "",
            string variant = "",
            bool Override = true,
            string key1 = "",
            float value1 = 0,
            string key2 = "",
            float value2 = 0,
            string key3 = "",
            float value3 = 0
        );
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_FetchPlayerRating(
            string idOrTag = "",
            string variant = "",
            string includeFields = ""
        );
        #endregion
#endif
        private void OnEnable()
        {
            CoreSDK.leaderboard.OnScopedFetchSuccess += (string _leaderboardFetchTag, GP_Data data) => OnFetchSuccess?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnScopedFetchTopPlayers += (string _leaderboardFetchTag, GP_Data data) => OnFetchTopPlayers?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnScopedFetchAbovePlayers += (string _leaderboardFetchTag, GP_Data data) => OnFetchAbovePlayers?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnScopedFetchBelowPlayers += (string _leaderboardFetchTag, GP_Data data) => OnFetchBelowPlayers?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnScopedFetchPlayer += (string _leaderboardFetchTag, GP_Data data) => OnFetchPlayer?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnScopedFetchPlayer += (string _leaderboardFetchTag, GP_Data data) => OnFetchPlayer?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnScopedFetchError += () => OnFetchError?.Invoke();

            CoreSDK.leaderboard.OnScopedFetchPlayerRating += (string _leaderboardFetchTag, int position) => OnFetchPlayerRating?.Invoke(_leaderboardFetchTag, position);
            CoreSDK.leaderboard.OnScopedFetchPlayerRatingTagVariant += (string _leaderboardFetchTag, string variant, int position) => OnFetchPlayerRatingTagVariant?.Invoke(_leaderboardFetchTag, variant, position);
            CoreSDK.leaderboard.OnScopedFetchPlayerRatingError += () => OnFetchError?.Invoke();

            CoreSDK.leaderboard.OnScopedOpen += () => OnOpen?.Invoke();
            CoreSDK.leaderboard.OnScopedClose += () => OnClose?.Invoke();

            CoreSDK.leaderboard.OnScopedPublishRecordComplete += () => OnPublishRecordComplete?.Invoke();
            CoreSDK.leaderboard.OnScopedPublishRecordError += () => OnPublishRecordError?.Invoke();
        }


        public static void Open(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "",
            string displayFields = "",
            WithMe withMe = WithMe.first)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_Open(idOrTag, variant, order.ToString(), limit, showNearest, includeFields, displayFields, withMe.ToString());
#else
            CoreSDK.leaderboard.OpenScoped(idOrTag, variant, order, limit, showNearest, includeFields, displayFields, withMe);
#endif
        }

        public static void Fetch(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "",
            WithMe withMe = WithMe.none)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_Fetch(idOrTag, variant, order.ToString(), limit, showNearest, includeFields, withMe.ToString());
#else
            CoreSDK.leaderboard.SimpleFetchScoped(idOrTag, variant, order, limit, showNearest, includeFields, withMe);
#endif
        }

        public static async Task<RatingData> AsyncFetch(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "",
            WithMe withMe = WithMe.none)
        {
            return await CoreSDK.leaderboard.FetchScoped(idOrTag, variant, order, limit, showNearest, includeFields, withMe);
        }

        public static void FetchPlayerRating(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_FetchPlayerRating(idOrTag, variant, order, limit, showNearest, includeFields);
#else
            CoreSDK.leaderboard.SimpleFetchScopedPlayerRating(idOrTag, variant, order, limit, showNearest, includeFields);
#endif
        }

        public static async Task<PlayerRatingData> AsyncFetchPlayerRating(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "")
        {
            return await CoreSDK.leaderboard.FetchScopedPlayerRating(idOrTag, variant, order, limit, showNearest, includeFields);
        }


        public static void PublishRecord(string idOrTag = "", string variant = "some_variant", bool Override = true, string key1 = "", int record_value1 = 0, string key2 = "", int record_value2 = 0, string key3 = "", int record_value3 = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_PublishRecord(idOrTag, variant, Override, key1, record_value1, key2, record_value2, key3, record_value3);
#else
            Dictionary<string, object> record = new Dictionary<string, object>();
            record.Add(key1, record_value1);
            if (key2 != "") record.Add(key2, record_value2);
            if (key3 != "") record.Add(key3, record_value3);
            CoreSDK.leaderboard.PublishRecord(idOrTag, variant, Override, record);
#endif
        }

        public static void PublishRecord(string idOrTag = "", string variant = "some_variant", bool Override = true, string key1 = "", float record_value1 = 0, string key2 = "", float record_value2 = 0, string key3 = "", float record_value3 = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_PublishRecord(idOrTag, variant, Override, key1, record_value1, key2, record_value2, key3, record_value3);
#else
            Dictionary<string, object> record = new Dictionary<string, object>();
            record.Add(key1, record_value1);
            if (key2 != "") record.Add(key2, record_value2);
            if (key3 != "") record.Add(key3, record_value3);
            CoreSDK.leaderboard.PublishRecord(idOrTag, variant, Override, record);
#endif
        }

        public static void PublishRecord(string idOrTag = "", string variant = "some_variant", bool Override = true, Dictionary<string, object> record = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_PublishRecord(idOrTag, variant, Override, key1, record_value1, key2, record_value2, key3, record_value3);
#else
            CoreSDK.leaderboard.PublishRecord(idOrTag, variant, Override, record);
#endif
        }


        private void CallLeaderboardOpen() => OnOpen?.Invoke();
        private void CallLeaderboardClose() => OnClose?.Invoke();

        private void CallLeaderboardScopedFetchTag(string lastTag) => _leaderboardFetchTag = lastTag;
        private void CallLeaderboardScopedFetchVariant(string lastVariant) => _leaderboardFetchVariant = lastVariant;

        private void CallLeaderboardScopedFetchError() => OnFetchError?.Invoke();


        private void CallLeaderboardScopedFetch(string data)
        {
            GP_Data dataArray = new GP_Data(data);
            OnFetchSuccess?.Invoke(_leaderboardFetchTag, dataArray);
            OnFetchTagVariant?.Invoke(_leaderboardFetchTag, _leaderboardFetchVariant, dataArray);
        }

        private void CallLeaderboardScopedFetchTop(string data) => OnFetchTopPlayers?.Invoke(_leaderboardFetchTag, new GP_Data(data));
        private void CallLeaderboardScopedFetchAbove(string data) => OnFetchAbovePlayers?.Invoke(_leaderboardFetchTag, new GP_Data(data));
        private void CallLeaderboardScopedFetchBelow(string data) => OnFetchBelowPlayers?.Invoke(_leaderboardFetchTag, new GP_Data(data));
        private void CallLeaderboardScopedFetchOnlyPlayer(string data) => OnFetchPlayer?.Invoke(_leaderboardFetchTag, new GP_Data(data));

        private void CallLeaderboardScopedPublishRecordComplete() => OnPublishRecordComplete?.Invoke();
        private void CallLeaderboardScopedPublishRecordError() => OnPublishRecordError?.Invoke();


        private void CallLeaderboardScopedFetchPlayerTag(string lastTag) => _leaderboardPlayerRatingFetchTag = lastTag;
        private void CallLeaderboardScopedFetchPlayerVariant(string lastVariant) => _leaderboardPlayerRatingFetchVariant = lastVariant;


        private void CallLeaderboardScopedFetchPlayerRating(int playerPosition)
        {
            OnFetchPlayerRating?.Invoke(_leaderboardPlayerRatingFetchTag, playerPosition);
            OnFetchPlayerRatingTagVariant?.Invoke(_leaderboardPlayerRatingFetchTag, _leaderboardPlayerRatingFetchVariant, playerPosition);
        }

        private void CallLeaderboardScopedFetchPlayerRatingError() => OnFetchPlayerRatingError?.Invoke();
    }
}