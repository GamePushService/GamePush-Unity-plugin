using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;

namespace GamePush
{
    public class GP_Players : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Players);

        private static event Action<GP_Data> _onFetchSuccess;
        private static event Action _onFetchError;

        public static event UnityAction<GP_Data> OnFetchSuccess;
        public static event UnityAction OnFetchError;

        [DllImport("__Internal")]
        private static extern void GP_Players_Fetch(string value);

        public static void Fetch(int playerId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null)
        {
            _onFetchSuccess = onFetchSuccess;
            _onFetchError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Players_Fetch(playerId.ToString());
#else

            ConsoleLog("FETCH");
#endif
        }

        public static void Fetch(List<int> playersId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null)
        {
            _onFetchSuccess = onFetchSuccess;
            _onFetchError = onFetchError;

            if (playersId.Count == 0)
            {
                Debug.Log("List is Empty");
                return;
            }
            PlayersIdList ids = new PlayersIdList();

            ids.idsList = playersId;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Players_Fetch(JsonUtility.ToJson(ids));
#else

            ConsoleLog("FETCH");
#endif
        }

        public static void Fetch(int[] playersId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null)
        {
            _onFetchSuccess = onFetchSuccess;
            _onFetchError = onFetchError;

            if (playersId.Length == 0)
            {
                Debug.Log("Array is Empty");
                return;
            }
            PlayersIdArray ids = new PlayersIdArray();
            ids.idsArray = playersId;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Players_Fetch(JsonUtility.ToJson(ids));
#else

            ConsoleLog("FETCH");
#endif
        }



        private void CallPlayersFetchSuccess(string data)
        {
            GP_Data dataArray = new GP_Data(data);
            OnFetchSuccess?.Invoke(dataArray);
            _onFetchSuccess?.Invoke(dataArray);
        }
        private void CallPlayersFetchError() { _onFetchError?.Invoke(); OnFetchError?.Invoke(); }
    }

}