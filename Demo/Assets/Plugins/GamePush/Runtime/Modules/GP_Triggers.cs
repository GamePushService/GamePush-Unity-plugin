using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Triggers : MonoBehaviour
    {
        public static event UnityAction<TriggerData> OnTriggerActivate;
        public static event UnityAction<TriggerData> OnTriggerClaim;
        public static event UnityAction<string> OnTriggerClaimError;

        private void CallOnTriggerActive(string trigger) { OnTriggerActivate?.Invoke(JsonUtility.FromJson<TriggerData>(trigger)); }
        private void CallOnTriggerClaim(string trigger) { OnTriggerClaim?.Invoke(JsonUtility.FromJson<TriggerData>(trigger)); }
        private void CallOnTriggerClaimError(string error) { OnTriggerClaimError?.Invoke(error); }

        [DllImport("__Internal")]
        private static extern void GP_Triggers_Claim(string idOrTag);
        public static void Claim(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Triggers_Claim(idOrTag);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("TRIGGERS: ", "CLAIM");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Triggers_List();
        public static TriggerData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string triggers = GP_Triggers_List();
            return GP_JSON.GetArray<TriggerData>(triggers);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("TRIGGERS: ", "LIST");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Triggers_ActivatedList();
        public static TriggerActive[] ActivatedList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string triggers = GP_Triggers_ActivatedList();
            return GP_JSON.GetArray<TriggerActive>(triggers);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("TRIGGERS: ", "Activated List");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Triggers_GetTrigger(string idOrTag);
        public static TriggerAllData GetTrigger(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Triggers_GetTrigger(idOrTag);
            return GP_JSON.Get<TriggerAllData>(data);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("TRIGGERS: ", "Get Trigger");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Triggers_IsActivated(string idOrTag);
        public static bool IsActivated(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Triggers_IsActivated(idOrTag) == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("TRIGGERS: ", "IsActivated");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Triggers_IsClaimed(string idOrTag);
        public static bool IsClaimed(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Triggers_IsClaimed(idOrTag) == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("TRIGGERS: ", "IsClaimed");
            return false;
#endif
        }

    }



    [System.Serializable]
    public class TriggerAllData
    {
        public TriggerData trigger;
        public bool isActivated;
        public bool isClaimed;
    }

    [System.Serializable]
    public class TriggerData
    {
        public string id;
        public string tag;
        public bool isAutoClaim;
        public string description;
        public TriggerCondition[] conditions;
        public TriggerBonus[] bonuses;
    }

    [System.Serializable]
    public class TriggerCondition
    {
        public string conditionType;
        public string key;
        public string operatorType;
        public string[] value;
    }

    [System.Serializable]
    public class TriggerBonus
    {
        public string type;
        public string id;
    }

    [System.Serializable]
    public class TriggerActive
    {
        public string triggerId;
        public bool claimed;
    }
}
