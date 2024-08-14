using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;
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

        [DllImport("__Internal")]
        private static extern void GP_Events_Join(string idOrTag);
        public static void Join(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Events_Join(idOrTag);
#else

            ConsoleLog("Join");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Events_List();
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

        [DllImport("__Internal")]
        private static extern string GP_Events_ActiveList();
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

        [DllImport("__Internal")]
        private static extern string GP_Events_GetEvent(string idOrTag);
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

        [DllImport("__Internal")]
        private static extern string GP_Events_IsActive(string idOrTag);
        public static bool IsActive(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Events_IsActive(idOrTag) == "true";
#else
            ConsoleLog("IsActive");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Events_IsJoined(string idOrTag);
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

    [System.Serializable]
    public class EventData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public string dateStart;
        public string dateEnd;
        public bool isActive;
        public int timeLeft;
        public bool isAutoJoin;
        public TriggerData[] triggers;
    }

    [System.Serializable]
    public class PlayerEvents
    {
        public int eventId;
        public EventStats stats;
    }

    [System.Serializable]
    public class EventStats
    {
        public int activeDays;
        public int activeDaysConsecutive;
    }

}