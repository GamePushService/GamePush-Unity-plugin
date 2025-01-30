﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;
using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Events : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Events);

        public static event UnityAction<PlayerEvents> OnEventJoin;
        public static event UnityAction<string> OnEventJoinError;

        private void CallOnEventJoin(string eventData) { OnEventJoin?.Invoke(JsonUtility.FromJson<PlayerEvents>(eventData)); }
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

        public static void Join(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Events_Join(idOrTag);
#else

            ConsoleLog("Join");
#endif
        }

        public static EventData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string eventsData = GP_Events_List();
            return UtilityJSON.GetArray<EventData>(eventsData );
#else

            ConsoleLog("LIST");

            return null;
#endif
        }

        public static PlayerEvents[] ActiveList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string activeEvents = GP_Events_ActiveList();
            return UtilityJSON.GetArray<PlayerEvents>(activeEvents);
#else

            ConsoleLog("Active List");

            return null;
#endif
        }

        public static EventData GetEvent(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Events_GetEvent(idOrTag);
            return UtilityJSON.Get<EventData>(data);
#else

            ConsoleLog("Get Event");

            return null;
#endif
        }

        public static bool IsActive(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Events_IsActive(idOrTag) == "true";
#else
            ConsoleLog("IsActive");
            return false;
#endif
        }

        public static bool IsJoined(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Events_IsJoined(idOrTag) == "true";
#else
            ConsoleLog("IsJoined");
            return false;
#endif
        }


    }

}