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

        private void CallOnTriggerDataActive(TriggerData trigger) { OnTriggerActivate?.Invoke(trigger); }
        private void CallOnTriggerDataClaim(TriggerData trigger) { OnTriggerClaim?.Invoke(trigger); }
        
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

        private void OnEnable()
        {
            CoreSDK.Triggers.OnTriggerActivate += CallOnTriggerDataActive;
            CoreSDK.Triggers.OnTriggerClaim += CallOnTriggerDataClaim;
            CoreSDK.Triggers.OnTriggerClaimError += CallOnTriggerClaimError;
        }

        private void OnDisable()
        {
            CoreSDK.Triggers.OnTriggerActivate -= CallOnTriggerDataActive;
            CoreSDK.Triggers.OnTriggerClaim -= CallOnTriggerDataClaim;
            CoreSDK.Triggers.OnTriggerClaimError -= CallOnTriggerClaimError;
        }

        public static void Claim(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Triggers_Claim(idOrTag);
#else
            CoreSDK.Triggers.ClaimAsync(idOrTag);
#endif
        }

        public static TriggerData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string triggers = GP_Triggers_List();
            return UtilityJSON.GetArray<TriggerData>(triggers);
#else

            return CoreSDK.Triggers.List.ToArray();
#endif
        }

        public static PlayerTrigger[] ActivatedList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string triggers = GP_Triggers_ActivatedList();
            return UtilityJSON.GetArray<PlayerTrigger>(triggers);
#else
            return CoreSDK.Triggers.ActivatedList.ToArray();
#endif
        }

        public static PlayerTriggerInfo GetTrigger(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Triggers_GetTrigger(idOrTag);
            return UtilityJSON.Get<PlayerTriggerInfo>(data);
#else
            return CoreSDK.Triggers.GetTrigger(idOrTag);
#endif
        }

        public static bool IsActivated(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Triggers_IsActivated(idOrTag) == "true";
#else
            return CoreSDK.Triggers.IsActivated(idOrTag);
#endif
        }

        public static bool IsClaimed(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Triggers_IsClaimed(idOrTag) == "true";
#else
            return CoreSDK.Triggers.IsClaimed(idOrTag);
#endif
        }

    }



    
}
