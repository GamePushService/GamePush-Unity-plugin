using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Triggers : MonoBehaviour
    {
        public static event UnityAction<TriggerData> OnTriggerActive;
        public static event UnityAction<TriggerData> OnTriggerClaim;
        public static event UnityAction<string> OnTriggerClaimError;

        private void CallOnTriggerActive(string trigger) { OnTriggerActive?.Invoke(JsonUtility.FromJson<TriggerData>(trigger)); }
        private void CallOnTriggerClaim(string trigger) { OnTriggerClaim?.Invoke(JsonUtility.FromJson<TriggerData>(trigger)); }
        private void CallOnTriggerClaimError(string error) { OnTriggerClaimError?.Invoke(error); }

        [DllImport("__Internal")]
        private static extern string GP_Triggers_Claim(string idOrTag);
        public static TriggerData Claim(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string trigger = GP_Triggers_Claim(idOrTag);
            return JsonUtility.FromJson<TriggerData>(trigger);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("TRIGGERS: ", "CLAIM");

            return new TriggerData();
#endif
        }

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

    }

    [System.Serializable]
    public class TriggerData
    {
        public int id;
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
}
