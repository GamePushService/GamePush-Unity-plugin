using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GamePush
{
    public class GP_Leaderboard : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Leaderboard);

        public static event UnityAction<string, GP_Data> OnFetchSuccess;
        public static event UnityAction<string, GP_Data> OnFetchTopPlayers;
        public static event UnityAction<string, GP_Data> OnFetchAbovePlayers;
        public static event UnityAction<string, GP_Data> OnFetchBelowPlayers;
        public static event UnityAction<string, GP_Data> OnFetchPlayer;
        public static event UnityAction OnFetchError;

        public static event UnityAction<string, int> OnFetchPlayerRatingSuccess;
        public static event UnityAction OnFetchPlayerRatingError;

        public static event UnityAction OnLeaderboardOpen;
        public static event UnityAction OnLeaderboardClose;

        private string _leaderboardFetchTag;
        private string _leaderboardPlayerFetchTag;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Open(
                string orderBy = "score",
                // DESC | ASC
                string order = "DESC",
                int limit = 10,
                int showNearest = 5,
                // none | first | last
                string withMe = "none",
                // level,exp,rank
                string includeFields = "",
                // level,rank
                string displayFields = ""
              );
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Fetch(
            string tag = "",
            string orderBy = "score",
            // DESC | ASC
            string order = "DESC",
            int limit = 10,
            int showNearest = 5,
            // none | first | last
            string withMe = "none",
            // level,exp,rank
            string includeFields = ""
        );
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_FetchPlayerRating(
            string tag = "",
            string orderBy = "score",
            // DESC | ASC
            string order = "DESC"
        );
#endif

        private void OnEnable()
        {
            CoreSDK.leaderboard.OnFetchSuccess += (string _leaderboardFetchTag, GP_Data data) => OnFetchSuccess?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnFetchTopPlayers += (string _leaderboardFetchTag, GP_Data data) => OnFetchTopPlayers?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnFetchAbovePlayers += (string _leaderboardFetchTag, GP_Data data) => OnFetchAbovePlayers?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnFetchBelowPlayers += (string _leaderboardFetchTag, GP_Data data) => OnFetchBelowPlayers?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnFetchPlayer += (string _leaderboardFetchTag, GP_Data data) => OnFetchPlayer?.Invoke(_leaderboardFetchTag, data);
            CoreSDK.leaderboard.OnFetchError += () => OnFetchError?.Invoke();

            CoreSDK.leaderboard.OnFetchPlayerRatingSuccess += (string _leaderboardFetchTag, int position) => OnFetchPlayerRatingSuccess?.Invoke(_leaderboardFetchTag, position);
            CoreSDK.leaderboard.OnFetchPlayerRatingError += () => OnFetchError?.Invoke();

            CoreSDK.leaderboard.OnLeaderboardOpen += () => OnLeaderboardOpen?.Invoke();
            CoreSDK.leaderboard.OnLeaderboardClose += () => OnLeaderboardClose?.Invoke();
        }


        public static void Open(string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 5, WithMe withMe = WithMe.none, string includeFields = "", string displayFields = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Open(orderBy, order.ToString(), limit, showNearest, withMe.ToString(), includeFields, displayFields);
#else
            CoreSDK.leaderboard.Open(orderBy, order, limit, showNearest, withMe, includeFields, displayFields);
#endif
        }

        public static void Fetch(string tag = "", string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 0, WithMe withMe = WithMe.none, string includeFields = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Fetch(tag, orderBy, order.ToString(), limit, showNearest, withMe.ToString(), includeFields);
#else
            CoreSDK.leaderboard.SimpleFetch(tag, orderBy, order, limit, showNearest, withMe, includeFields);
#endif
        }

        public static async Task<RatingData> AsyncFetch(string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 0, WithMe withMe = WithMe.none, string includeFields = "")
        {
            return await CoreSDK.leaderboard.Fetch(orderBy, order, limit, showNearest, withMe, includeFields);
        }

        public static void FetchPlayerRating(string tag = "", string orderBy = "score", Order order = Order.DESC)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_FetchPlayerRating(tag, orderBy, order.ToString());
#else
            CoreSDK.leaderboard.SimpleFetchPlayerRating(tag, orderBy, order);
#endif
        }

        public static async Task<PlayerRatingData> AsyncFetchPlayerRating(string orderBy = "score", Order order = Order.DESC)
        {
            return await CoreSDK.leaderboard.FetchPlayerRating(orderBy, order);
        }


        private void CallLeaderboardOpen() => OnLeaderboardOpen?.Invoke();
        private void CallLeaderboardClose() => OnLeaderboardClose?.Invoke();

        private void CallLeaderboardFetch(string data) => OnFetchSuccess?.Invoke(_leaderboardFetchTag, new GP_Data(data));
        private void CallLeaderboardFetchTop(string data) => OnFetchTopPlayers?.Invoke(_leaderboardFetchTag, new GP_Data(data));
        private void CallLeaderboardFetchAbove(string data) => OnFetchAbovePlayers?.Invoke(_leaderboardFetchTag, new GP_Data(data));
        private void CallLeaderboardFetchBelow(string data) => OnFetchBelowPlayers?.Invoke(_leaderboardFetchTag, new GP_Data(data));
        private void CallLeaderboardFetchOnlyPlayer(string data) => OnFetchPlayer?.Invoke(_leaderboardFetchTag, new GP_Data(data));

        private void CallLeaderboardFetchTag(string lastTag) => _leaderboardFetchTag = lastTag;
        private void CallLeaderboardFetchError() => OnFetchError?.Invoke();

        private void CallLeaderboardFetchPlayerTag(string lastTag) => _leaderboardPlayerFetchTag = lastTag;

        private void CallLeaderboardFetchPlayerRating(int playerPosition) => OnFetchPlayerRatingSuccess?.Invoke(_leaderboardPlayerFetchTag, playerPosition);

        private void CallLeaderboardFetchPlayerError() => OnFetchPlayerRatingError?.Invoke();
    }

    
}
