using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_GamesCollections : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.GamesCollections);

        public static event UnityAction OnGamesCollectionsOpen;
        public static event UnityAction OnGamesCollectionsClose;

        public static event UnityAction<string, GamesCollectionsFetchData> OnGamesCollectionsFetch;
        public static event UnityAction OnGamesCollectionsFetchError;

        private static event Action _onGamesCollectionsOpen;
        private static event Action _onGamesCollectionsClose;

        private static event Action<string, GamesCollectionsFetchData> _onGamesCollectionsFetch;
        private static event Action _onGamesCollectionsFetchError;

        private string _gamesCollectionsFetchTag;


        [DllImport("__Internal")]
        private static extern void GP_GamesCollections_Open(string idOrTag);
        public static void Open(string idOrTag, Action onGamesCollectionsOpen = null, Action onGamesCollectionsClose = null)
        {
            _onGamesCollectionsOpen = onGamesCollectionsOpen;
            _onGamesCollectionsClose = onGamesCollectionsClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GamesCollections_Open(idOrTag);
#else

            ConsoleLog("OPEN: " + idOrTag);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_GamesCollections_Fetch(string idOrTag);
        public static void Fetch(string idOrTag, Action<string, GamesCollectionsFetchData> onFetchSuccess = null, Action onFetchError = null)
        {
            _onGamesCollectionsFetch = onFetchSuccess;
            _onGamesCollectionsFetchError = onFetchError;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GamesCollections_Fetch(idOrTag);
#else

            ConsoleLog("FETCH: " + idOrTag);
#endif
        }


        private void CallGamesCollectionsOpen() { _onGamesCollectionsOpen?.Invoke(); OnGamesCollectionsOpen?.Invoke(); }
        private void CallGamesCollectionsClose() { _onGamesCollectionsClose?.Invoke(); OnGamesCollectionsClose?.Invoke(); }


        private void CallGamesCollectionsFetch(string data) { _onGamesCollectionsFetch?.Invoke(_gamesCollectionsFetchTag, JsonUtility.FromJson<GamesCollectionsFetchData>(data)); OnGamesCollectionsFetch?.Invoke(_gamesCollectionsFetchTag, JsonUtility.FromJson<GamesCollectionsFetchData>(data)); }


        private void CallGamesCollectionsFetchTag(string idOrTag) => _gamesCollectionsFetchTag = idOrTag;
        private void CallGamesCollectionsFetchError() { _onGamesCollectionsFetchError?.Invoke(); OnGamesCollectionsFetchError?.Invoke(); }
    }


    [System.Serializable]
    public class GamesCollectionsFetchData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public Games[] games;
    }

    [System.Serializable]
    public class Games
    {
        public int id;
        public string name;
        public string description;
        public string icon;
        public string url;
    }

}