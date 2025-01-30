using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using GamePush.Data;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;

namespace GamePush
{
    public class GP_Players : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Players);

        private static event Action<GP_Data> _onFetchSuccess;
        private static event Action _onFetchError;

        public static event UnityAction<GP_Data> OnFetchSuccess;
        public static event UnityAction OnFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Players_Fetch(string value);
#endif

        private void OnEnable()
        {
            CoreSDK.Players.OnFetchSuccess += CallPlayersFetchSuccessData;
            CoreSDK.Players.OnFetchError += CallPlayersFetchError;
        }

        public static void Fetch(int playerId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null)
        {
            Fetch(playerId.ToString(), onFetchSuccess, onFetchError);
        }

        public static void Fetch(List<int> playersId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null)
        {
            if (playersId.Count == 0)
            {
                Debug.Log("List is Empty");
                return;
            }

            PlayersIdList ids = new PlayersIdList();
            ids.idsList = playersId;

            string json = JsonUtility.ToJson(ids);
            Fetch(json, onFetchSuccess, onFetchError);
        }

        public static void Fetch(int[] playersId, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null)
        {
            if (playersId.Length == 0)
            {
                Debug.Log("Array is Empty");
                return;
            }

            PlayersIdArray ids = new PlayersIdArray();
            ids.idsArray = playersId;

            string json = JsonUtility.ToJson(ids);
            Fetch(json, onFetchSuccess, onFetchError);
        }
        
        public static void Fetch(string playersIds, Action<GP_Data> onFetchSuccess = null, Action onFetchError = null)
        {
            _onFetchSuccess = onFetchSuccess;
            _onFetchError = onFetchError;

            FetchJson(playersIds);
        }
        

        private static void FetchJson(string json)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Players_Fetch(json);
#else
            CoreSDK.Players.Fetch(json);
#endif
        }

        private void CallPlayersFetchSuccessData(GP_Data data)
        {
            OnFetchSuccess?.Invoke(data);
            _onFetchSuccess?.Invoke(data);
        }
        
        private void CallPlayersFetchSuccess(string data)
        {
            GP_Data dataArray = new GP_Data(data);
            CallPlayersFetchSuccessData(dataArray);
        }

        private void CallPlayersFetchError()
        {
            _onFetchError?.Invoke(); 
            OnFetchError?.Invoke();
        }
    }

}