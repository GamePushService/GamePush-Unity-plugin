using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Utilities;

namespace GamePush
{
    public class GP_Schedulers : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Schedulers);

        public static event UnityAction<SchedulerInfo> OnSchedulerRegister;
        public static event UnityAction<string> OnSchedulerRegisterError;

        public static event UnityAction<SchedulerDayInfo> OnSchedulerClaimDay;
        public static event UnityAction<string> OnSchedulerClaimDayError;

        public static event UnityAction<SchedulerDayInfo> OnSchedulerClaimDayAdditional;
        public static event UnityAction<string> OnSchedulerClaimDayAdditionalError;

        public static event UnityAction<SchedulerDayInfo> OnSchedulerClaimAllDay;
        public static event UnityAction<string> OnSchedulerClaimAllDayError;

        public static event UnityAction<SchedulerInfo> OnSchedulerClaimAllDays;
        public static event UnityAction<string> OnSchedulerClaimAllDaysError;

        public static event UnityAction<PlayerScheduler> OnSchedulerJoin;
        public static event UnityAction<string> OnSchedulerJoinError;


        private void CallOnSchedulerRegister(string data) { OnSchedulerRegister?.Invoke(JsonUtility.FromJson<SchedulerInfo>(data)); }
        private void CallOnSchedulerRegisterError(string error) { OnSchedulerRegisterError?.Invoke(error); }

        private void CallOnSchedulerClaimDay(string data) { OnSchedulerClaimDay?.Invoke(JsonUtility.FromJson<SchedulerDayInfo>(data)); }
        private void CallOnSchedulerClaimDayError(string error) { OnSchedulerClaimDayError?.Invoke(error); }

        private void CallOnSchedulerClaimDayAdditional(string data) { OnSchedulerClaimDayAdditional?.Invoke(JsonUtility.FromJson<SchedulerDayInfo>(data)); }
        private void CallOnSchedulerClaimDayAdditionalError(string error) { OnSchedulerClaimDayAdditionalError?.Invoke(error); }

        private void CallOnSchedulerClaimAllDay(string data) { OnSchedulerClaimAllDay?.Invoke(JsonUtility.FromJson<SchedulerDayInfo>(data)); }
        private void CallOnSchedulerClaimAllDayError(string error) { OnSchedulerClaimAllDayError?.Invoke(error); }

        private void CallOnSchedulerClaimAllDays(string data) { OnSchedulerClaimAllDays?.Invoke(JsonUtility.FromJson<SchedulerInfo>(data)); }
        private void CallOnSchedulerClaimAllDaysError(string error) { OnSchedulerClaimAllDaysError?.Invoke(error); }

        private void CallOnSchedulerJoin(string data) { OnSchedulerJoin?.Invoke(JsonUtility.FromJson<PlayerScheduler>(data)); }
        private void CallOnSchedulerJoinError(string error) { OnSchedulerJoinError?.Invoke(error); }


        [DllImport("__Internal")]
        private static extern void GP_Schedulers_Register(string idOrTag);
        public static void Register(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Schedulers_Register(idOrTag);
#else

            ConsoleLog("Register");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Schedulers_ClaimDay(string idOrTag, int day);
        public static void ClaimDay(string idOrTag, int day)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Schedulers_ClaimDay(idOrTag, day);
#else

            ConsoleLog("ClaimDay");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Schedulers_ClaimDayAdditional(string idOrTag, int day, string triggerIdOrTag);
        public static void ClaimDayAdditional(string idOrTag, int day, string triggerIdOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Schedulers_ClaimDayAdditional(idOrTag, day, triggerIdOrTag);
#else

            ConsoleLog("ClaimDayAdditional");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Schedulers_ClaimAllDay(string idOrTag, int day);
        public static void ClaimAllDay(string idOrTag, int day)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Schedulers_ClaimAllDay(idOrTag, day);
#else

            ConsoleLog("ClaimDay");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Schedulers_ClaimAllDays(string idOrTag);
        public static void ClaimAllDays(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Schedulers_ClaimAllDays(idOrTag);
#else

            ConsoleLog("ClaimDay");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Schedulers_List();
        public static SchedulerData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Schedulers_List();
            return UtilityJSON.GetArray<SchedulerData>(data);
#else

            Console.Log("Schedulers: ", "List");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Schedulers_ActiveList();
        public static PlayerScheduler[] ActiveList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Schedulers_ActiveList();
            return UtilityJSON.GetArray<PlayerScheduler>(data);
#else

            Console.Log("Schedulers: ", "List");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Schedulers_GetScheduler(string idOrTag);
        public static SchedulerInfo GetScheduler(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Schedulers_GetScheduler(idOrTag);
            return UtilityJSON.Get<SchedulerInfo>(data);
#else

            Console.Log("Schedulers: ", "Get Scheduler");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Schedulers_GetSchedulerDay(string idOrTag, int day);
        public static SchedulerDayInfo GetSchedulerDay(string idOrTag, int day)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Schedulers_GetSchedulerDay(idOrTag, day);
            return UtilityJSON.Get<SchedulerDayInfo>(data);
#else

            Console.Log("Schedulers: ", "Get Scheduler Day");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Schedulers_GetSchedulerCurrentDay(string idOrTag);
        public static SchedulerDayInfo GetSchedulerCurrentDay(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Schedulers_GetSchedulerCurrentDay(idOrTag);
            return UtilityJSON.Get<SchedulerDayInfo>(data);
#else

            Console.Log("Schedulers: ", "Get Scheduler");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Schedulers_IsRegistered(string idOrTag);
        public static bool IsRegistered(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Schedulers_IsRegistered(idOrTag) == "true";
#else

            Console.Log("REWARDS: ", "Has");

            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Schedulers_IsTodayRewardClaimed(string idOrTag);
        public static bool IsTodayRewardClaimed(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Schedulers_IsTodayRewardClaimed(idOrTag) == "true";
#else

            Console.Log("REWARDS: ", "Has");

            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Schedulers_CanClaimDay(string idOrTag, int day);
        public static bool CanClaimDay(string idOrTag, int day)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Schedulers_CanClaimDay(idOrTag, day) == "true";
#else

            Console.Log("REWARDS: ", "Has");

            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Schedulers_CanClaimDayAdditional(string idOrTag, int day, string triggerIdOrTag);
        public static bool CanClaimDayAdditional(string idOrTag, int day, string triggerIdOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Schedulers_CanClaimDayAdditional(idOrTag, day, triggerIdOrTag) == "true";
#else

            Console.Log("REWARDS: ", "Has");

            return true;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Schedulers_CanClaimAllDay(string idOrTag, int day);
        public static bool CanClaimAllDay(string idOrTag, int day)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Schedulers_CanClaimAllDay(idOrTag, day) == "true";
#else

            Console.Log("REWARDS: ", "Has");

            return true;
#endif
        }

    }

    public enum SchedulerType
    {
        ACTIVE_DAYS,
        ACTIVE_DAYS_CONSECUTIVE
    }

    [System.Serializable]
    public class SchedulerData
    {
        public int id;
        public string tag;
        public SchedulerType type;
        public int days;
        public bool isRepeat;
        public bool isAutoRegister;
        public TriggerData[] triggers;
    }

    [System.Serializable]
    public class PlayerScheduler
    {
        public int schedulerId;
        public int daysClaimed;
        public PlayerSchedulerStats stats;
    }

    [System.Serializable]
    public class PlayerSchedulerStats
    {
        public int activeDays;
        public int activeDaysConsecutive;
    }

    [System.Serializable]
    public class SchedulerInfo
    {
        public SchedulerData scheduler;
        public PlayerSchedulerStats stats;
        public int[] daysClaimed;
        public bool isRegistered;
        public int currentDay;
    }

    [System.Serializable]
    public class SchedulerDayInfo
    {
        public SchedulerData scheduler;
        public int day;
        public bool isDayReached;
        public bool isDayComplete;
        public bool isDayClaimed;
        public bool isAllDayClaimed;
        public TriggerData[] triggers;
    }
}
