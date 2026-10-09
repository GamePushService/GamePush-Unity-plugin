using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Native;
using GamePush.Overlays;

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


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Open(
                string orderBy = "score",
                string order = "DESC",
                int limit = 10,
                int showNearest = 5,
                string withMe = "none",
                string includeFields = "",
                string displayFields = ""
              );
        #endif
        public static void Open(string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 5, WithMe withMe = WithMe.none, string includeFields = "", string displayFields = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Open(orderBy, order.ToString(), limit, showNearest, withMe.ToString(), includeFields, displayFields);
#else
            if (GamePushHost.UseNativeCore && GP_Overlays.Open(GP_OverlayKind.Leaderboard, new GP_LeaderboardArgs
                {
                    scoped = false,
                    orderBy = orderBy,
                    order = order.ToString(),
                    limit = limit,
                    showNearest = showNearest,
                    withMe = withMe.ToString(),
                    includeFields = includeFields,
                    displayFields = displayFields
                }))
                return;
            if (GP_Play2Web.Call("LeaderboardOpen", orderBy, order.ToString(), limit, showNearest, withMe.ToString(), includeFields, displayFields))
                return;
            ConsoleLog("OPEN");
#endif
        }



        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Fetch(
            string tag = "",
            string orderBy = "score",
            string order = "DESC",
            int limit = 10,
            int showNearest = 5,
            string withMe = "none",
            string includeFields = ""
        );
        #endif
        public static void Fetch(string tag = "", string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 0, WithMe withMe = WithMe.none, string includeFields = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Fetch(tag, orderBy, order.ToString(), limit, showNearest, withMe.ToString(), includeFields);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeLeaderboard.Fetch(tag, orderBy, order.ToString(), limit, showNearest, withMe.ToString(), includeFields);
                return;
            }
            ConsoleLog("FETCH");
#endif
        }



        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_FetchPlayerRating(
            string tag = "",
            string orderBy = "score",
            string order = "DESC"
        );
        #endif
        public static void FetchPlayerRating(string tag = "", string orderBy = "score", Order order = Order.DESC)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_FetchPlayerRating(tag, orderBy, order.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeLeaderboard.FetchPlayerRating(tag, orderBy, order.ToString());
                return;
            }
            ConsoleLog("FETCH PLAYER RATING");
#endif
        }

        internal static void NativeFireFetch(NativeLeaderboardResult result)
        {
            OnFetchSuccess?.Invoke(result.tag, new GP_Data(result.playersJson));
            OnFetchTopPlayers?.Invoke(result.tag, new GP_Data(result.topPlayersJson));
            OnFetchAbovePlayers?.Invoke(result.tag, new GP_Data(result.abovePlayersJson));
            OnFetchBelowPlayers?.Invoke(result.tag, new GP_Data(result.belowPlayersJson));
            OnFetchPlayer?.Invoke(result.tag, new GP_Data(result.playerJson));
        }

        internal static void NativeFireFetchError() => OnFetchError?.Invoke();
        internal static void NativeFirePlayerRating(string tag, int position) => OnFetchPlayerRatingSuccess?.Invoke(tag, position);
        internal static void NativeFirePlayerRatingError() => OnFetchPlayerRatingError?.Invoke();
        internal static void NativeFireOpen() => OnLeaderboardOpen?.Invoke();
        internal static void NativeFireClose() => OnLeaderboardClose?.Invoke();


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

    public enum Order : byte
    {
        DESC,
        ASC
    }

    public enum WithMe : byte
    {
        none,
        first,
        last
    }
}
