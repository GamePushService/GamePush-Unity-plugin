using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Tools;

namespace GamePush
{
    public class GP_Triggers : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Triggers);

        public static event UnityAction<TriggerData> OnTriggerActivate;
        public static event UnityAction<TriggerData> OnTriggerClaim;
        public static event UnityAction<string> OnTriggerClaimError;

        private void CallOnTriggerActive(string trigger) { OnTriggerActivate?.Invoke(JsonUtility.FromJson<TriggerData>(trigger)); }
        private void CallOnTriggerClaim(string trigger) { OnTriggerClaim?.Invoke(JsonUtility.FromJson<TriggerData>(trigger)); }
        private void CallOnTriggerClaimError(string error) { OnTriggerClaimError?.Invoke(error); }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Triggers_Claim(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Triggers_List();
        [DllImport("__Internal")]
        private static extern string GP_Triggers_ActivatedList();
        [DllImport("__Internal")]
        private static extern string GP_Triggers_GetTrigger(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Triggers_IsActivated(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Triggers_IsClaimed(string idOrTag);
#endif

        public static void Claim(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Triggers_Claim(idOrTag);
#else

            ConsoleLog("CLAIM");
#endif
        }

        public static TriggerData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string triggers = GP_Triggers_List();
            return UtilityJSON.GetArray<TriggerData>(triggers);
#else

            ConsoleLog("LIST");

            return null;
#endif
        }

        public static TriggerActive[] ActivatedList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string triggers = GP_Triggers_ActivatedList();
            return UtilityJSON.GetArray<TriggerActive>(triggers);
#else

            ConsoleLog("Activated List");

            return null;
#endif
        }

        public static TriggerAllData GetTrigger(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Triggers_GetTrigger(idOrTag);
            return UtilityJSON.Get<TriggerAllData>(data);
#else

            ConsoleLog("Get Trigger");

            return null;
#endif
        }

        public static bool IsActivated(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Triggers_IsActivated(idOrTag) == "true";
#else
            ConsoleLog("IsActivated");
            return false;
#endif
        }

        public static bool IsClaimed(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Triggers_IsClaimed(idOrTag) == "true";
#else

            ConsoleLog("IsClaimed");
            return false;
#endif
        }

    }



    
}
