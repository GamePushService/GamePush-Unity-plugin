using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_GamesCollections : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.GamesCollections);

        #region Actions
        public static event UnityAction OnGamesCollectionsOpen;
        public static event UnityAction OnGamesCollectionsClose;

        public static event UnityAction<string, GamesCollectionsData> OnGamesCollectionsFetch;
        public static event UnityAction OnGamesCollectionsFetchError;

        private static event Action _onGamesCollectionsOpen;
        private static event Action _onGamesCollectionsClose;

        private static event Action<string, GamesCollectionsData> _onGamesCollectionsFetch;
        private static event Action _onGamesCollectionsFetchError;
        #endregion

        private string _gamesCollectionsFetchTag;

#if !UNITY_EDITOR && UNITY_WEBGL
        #region DllImport
        [DllImport("__Internal")]
        private static extern void GP_GamesCollections_Open(string idOrTag);
        [DllImport("__Internal")]
        private static extern void GP_GamesCollections_Fetch(string idOrTag);
        #endregion
#endif

        private void OnEnable()
        {
            CoreSDK.GameCollections.OnGamesCollectionsOpen += () => CallGamesCollectionsOpen();
            CoreSDK.GameCollections.OnGamesCollectionsClose += () => CallGamesCollectionsClose();

            CoreSDK.GameCollections.OnGamesCollectionsFetch += (string idOrTag, GamesCollectionsData data) => CallGamesCollectionsDataFetch(idOrTag, data);
            CoreSDK.GameCollections.OnGamesCollectionsFetchError += () => CallGamesCollectionsClose();
        }

        public static void Open(string idOrTag, Action onGamesCollectionsOpen = null, Action onGamesCollectionsClose = null)
        {
            _onGamesCollectionsOpen = onGamesCollectionsOpen;
            _onGamesCollectionsClose = onGamesCollectionsClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GamesCollections_Open(idOrTag);
#else
            CoreSDK.GameCollections.Open(idOrTag);
#endif
        }

        public async static void Fetch(string idOrTag, Action<string, GamesCollectionsData> onFetchSuccess = null, Action onFetchError = null)
        {
            _onGamesCollectionsFetch = onFetchSuccess;
            _onGamesCollectionsFetchError = onFetchError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GamesCollections_Fetch(idOrTag);
#else
            await CoreSDK.GameCollections.Fetch(idOrTag);
#endif
        }

        private void CallGamesCollectionsOpen() { _onGamesCollectionsOpen?.Invoke(); OnGamesCollectionsOpen?.Invoke(); }
        private void CallGamesCollectionsClose() { _onGamesCollectionsClose?.Invoke(); OnGamesCollectionsClose?.Invoke(); }

        private void CallGamesCollectionsDataFetch(string idOrTag, GamesCollectionsData data) {
            _gamesCollectionsFetchTag = idOrTag;
            _onGamesCollectionsFetch?.Invoke(_gamesCollectionsFetchTag, data);
            OnGamesCollectionsFetch?.Invoke(_gamesCollectionsFetchTag, data); }
        private void CallGamesCollectionsFetch(string data) {
            _onGamesCollectionsFetch?.Invoke(_gamesCollectionsFetchTag, JsonUtility.FromJson<GamesCollectionsData>(data));
            OnGamesCollectionsFetch?.Invoke(_gamesCollectionsFetchTag, JsonUtility.FromJson<GamesCollectionsData>(data)); }

        private void CallGamesCollectionsFetchTag(string idOrTag) => _gamesCollectionsFetchTag = idOrTag;
        private void CallGamesCollectionsFetchError() { _onGamesCollectionsFetchError?.Invoke(); OnGamesCollectionsFetchError?.Invoke(); }
    }

}