﻿using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;
using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Events : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Events);

        public static event UnityAction<PlayerEvent> OnEventJoin;
        public static event UnityAction<string> OnEventJoinError;

        private void CallOnEventJoinData(PlayerEvent eventData)
        {
            OnEventJoin?.Invoke(eventData);
        }

        private void CallOnEventJoin(string eventData)
        {
            OnEventJoin?.Invoke(JsonUtility.FromJson<PlayerEvent>(eventData));
        }
        private void CallOnEventJoinError(string error) { OnEventJoinError?.Invoke(error); }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Events_Join(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Events_List();
        [DllImport("__Internal")]
        private static extern string GP_Events_ActiveList();
        [DllImport("__Internal")]
        private static extern string GP_Events_GetEvent(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Events_IsActive(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Events_IsJoined(string idOrTag);
#endif

        private void OnEnable()
        {
            CoreSDK.Events.OnEventJoin += CallOnEventJoinData;
            CoreSDK.Events.OnEventJoinError += CallOnEventJoinError;
        }

        public static void Join(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Events_Join(idOrTag);
#else

            CoreSDK.Events.Join(idOrTag);
#endif
        }
        
        public static async Task<PlayerEventInfo> JoinAsync(string idOrTag)
        {
            PlayerEventInfo info = await CoreSDK.Events.Join(idOrTag);
            return info;
        }
        
//         public static async Task<PlayerEventInfo> Join(string idOrTag)
//         {
// #if !UNITY_EDITOR && UNITY_WEBGL
//             return null;
// #else
//
//             PlayerEventInfo info = await CoreSDK.Events.Join(idOrTag);
//             return info;
// #endif
//         }

        public static EventData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string eventsData = GP_Events_List();
            return UtilityJSON.GetArray<EventData>(eventsData);
#else
            return CoreSDK.Events.List();
#endif
        }

        public static PlayerEvent[] ActiveList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string activeEvents = GP_Events_ActiveList();
            return UtilityJSON.GetArray<PlayerEvents>(activeEvents);
#else
            return CoreSDK.Events.ActiveList();
#endif
        }

        public static EventData GetEvent(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Events_GetEvent(idOrTag);
            return UtilityJSON.Get<EventData>(data);
#else
            return CoreSDK.Events.GetEvent(idOrTag).Event;
#endif
        }

        public static bool IsActive(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Events_IsActive(idOrTag) == "true";
#else
            return CoreSDK.Events.IsActive(idOrTag);
#endif
        }

        public static bool IsJoined(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Events_IsJoined(idOrTag) == "true";
#else
            return CoreSDK.Events.IsJoined(idOrTag);
#endif
        }


    }

}