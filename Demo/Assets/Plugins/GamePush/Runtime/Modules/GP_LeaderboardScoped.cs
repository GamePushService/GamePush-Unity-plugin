using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_LeaderboardScoped : MonoBehaviour
    {
        public static event UnityAction<string, GP_Data> OnFetchSuccess;
        public static event UnityAction<string, string, GP_Data> OnFetchTagVariant;
        public static event UnityAction OnFetchError;

        public static event UnityAction<string, int> OnFetchPlayerRating;
        public static event UnityAction<string, string, int> OnFetchPlayerRatingTagVariant;
        public static event UnityAction OnFetchPlayerRatingError;

        public static event UnityAction OnOpen;
        public static event UnityAction OnClose;

        public static event UnityAction OnPublishRecordComplete;
        public static event UnityAction OnPublishRecordError;


        private string _leaderboardFetchTag;
        private string _leaderboardFetchVariant;

        private string _leaderboardPlayerRatingFetchTag;
        private string _leaderboardPlayerRatingFetchVariant;



        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_Open(string idOrTag = "", string variant = "", string order = "DESC", int limit = 10, string includeFields = "", string displayFields = "", string withMe = "none");
        public static void Open(string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, string includeFields = "", string displayFields = "", WithMe withMe = WithMe.first)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_Open(idOrTag, variant, order.ToString(), limit, includeFields, displayFields, withMe.ToString());
#else
            if (GP_ConsoleController.Instance.LeaderboardScopedConsoleLogs)
                Console.Log("LEADERBOARD SCOPED: ", "OPEN");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_Fetch(string idOrTag = "", string variant = "", string order = "DESC", int limit = 10, string includeFields = "", string withMe = "none");
        public static void Fetch(string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, string includeFields = "", WithMe withMe = WithMe.none)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_Fetch(idOrTag, variant, order.ToString(), limit, includeFields, withMe.ToString());
#else
            if (GP_ConsoleController.Instance.LeaderboardScopedConsoleLogs)
                Console.Log("LEADERBOARD SCOPED: ", "FETCH");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_PublishRecord(string idOrTag = "", string variant = "", bool Override = true, string key1 = "", float value1 = 0, string key2 = "", float value2 = 0, string key3 = "", float value3 = 0);
        public static void PublishRecord(string idOrTag = "", string variant = "some_variant", bool Override = true, string key1 = "", int record_value1 = 0, string key2 = "", int record_value2 = 0, string key3 = "", int record_value3 = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_PublishRecord(idOrTag, variant, Override, key1, record_value1, key2, record_value2, key3, record_value3);
#else
            if (GP_ConsoleController.Instance.LeaderboardScopedConsoleLogs)
                Console.Log("LEADERBOARD SCOPED: ", "PUBLICH RECORD");
#endif
        }

        public static void PublishRecord(string idOrTag = "", string variant = "some_variant", bool Override = true, string key1 = "", float record_value1 = 0, string key2 = "", float record_value2 = 0, string key3 = "", float record_value3 = 0)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_PublishRecord(idOrTag, variant, Override, key1, record_value1, key2, record_value2, key3, record_value3);
#else
            if (GP_ConsoleController.Instance.LeaderboardScopedConsoleLogs)
                Console.Log("LEADERBOARD SCOPED: ", "PUBLICH RECORD");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Leaderboard_Scoped_FetchPlayerRating(string idOrTag = "", string variant = "", string includeFields = "");
        public static void FetchPlayerRating(string idOrTag = "", string variant = "some_variant", string includeFields = "")
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Leaderboard_Scoped_FetchPlayerRating(idOrTag, variant, includeFields);
#else
            if (GP_ConsoleController.Instance.LeaderboardScopedConsoleLogs)
                Console.Log("LEADERBOARD SCOPED: ", "FETCH PLAYER RATING");
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


        private void CallLeaderboardScopedPublishRecordComplete() => OnPublishRecordComplete?.Invoke();
        private void CallLeaderboardScopedPublishRecordError() => OnPublishRecordError?.Invoke();


        private void CallLeaderboardScopedFetchPlayerTag(string lastTag) => _leaderboardPlayerRatingFetchTag = lastTag;
        private void CallLeaderboardScopedFetchPlayerVariant(string lastVariant) => _leaderboardPlayerRatingFetchVariant = lastVariant;


        private void CallLeaderboardScopedFetchPlayer(int playerPosition)
        {
            OnFetchPlayerRating?.Invoke(_leaderboardPlayerRatingFetchTag, playerPosition);
            OnFetchPlayerRatingTagVariant?.Invoke(_leaderboardPlayerRatingFetchTag, _leaderboardPlayerRatingFetchVariant, playerPosition);
        }

        private void CallLeaderboardScopedFetchPlayerRatingError() => OnFetchPlayerRatingError?.Invoke();
    }
}