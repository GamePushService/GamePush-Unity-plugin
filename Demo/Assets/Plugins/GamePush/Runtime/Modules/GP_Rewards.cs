using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Tools;

namespace GamePush
{
    public class GP_Rewards : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Rewards);

        public static event UnityAction<AllRewardData> OnRewardsGive;
        public static event UnityAction<string> OnRewardsGiveError;
        public static event UnityAction<AllRewardData> OnRewardsAccept;
        public static event UnityAction<string> OnRewardsAcceptError;

        private void CallOnRewardsGive(string data) { OnRewardsGive?.Invoke(JsonUtility.FromJson<AllRewardData>(data)); }
        private void CallOnRewardsGiveData(AllRewardData data) { OnRewardsGive?.Invoke(data); }
        private void CallOnRewardsGiveError(string error) { OnRewardsGiveError?.Invoke(error); }
        private void CallOnRewardsAccept(string data) { OnRewardsAccept?.Invoke(JsonUtility.FromJson<AllRewardData>(data)); }
        private void CallOnRewardsAcceptData(AllRewardData data) { OnRewardsAccept?.Invoke(data); }
        private void CallOnRewardsAcceptError(string error) { OnRewardsAcceptError?.Invoke(error); }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Rewards_Give(string idOrTag, bool lazy);
        [DllImport("__Internal")]
        private static extern void GP_Rewards_Accept(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Rewards_List();
        [DllImport("__Internal")]
        private static extern string GP_Rewards_GivenList();
        [DllImport("__Internal")]
        private static extern string GP_Rewards_GetReward(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Rewards_Has(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Rewards_HasAccepted(string idOrTag);
        [DllImport("__Internal")]
        private static extern string GP_Rewards_HasUnaccepted(string idOrTag);
#endif

        private void OnEnable()
        {
            CoreSDK.Rewards.OnRewardsGive += CallOnRewardsGiveData;
            CoreSDK.Rewards.OnRewardsGiveError += CallOnRewardsGiveError;
            CoreSDK.Rewards.OnRewardsAccept += CallOnRewardsAcceptData;
            CoreSDK.Rewards.OnRewardsAcceptError += CallOnRewardsAcceptError;
        }
        
        private void OnDisable()
        {
            CoreSDK.Rewards.OnRewardsGive -= CallOnRewardsGiveData;
            CoreSDK.Rewards.OnRewardsGiveError -= CallOnRewardsGiveError;
            CoreSDK.Rewards.OnRewardsAccept -= CallOnRewardsAcceptData;
            CoreSDK.Rewards.OnRewardsAcceptError -= CallOnRewardsAcceptError;
        }

        
        public static void Give(string idOrTag, bool lazy = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Rewards_Give(idOrTag, lazy);
#else
            CoreSDK.Rewards.Give(idOrTag, lazy);
#endif
        }

        public static void Accept(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Rewards_Accept(idOrTag);
#else
            CoreSDK.Rewards.Accept(idOrTag);
#endif
        }

        public static RewardData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_List();
            return UtilityJSON.GetArray<RewardData>(data);
#else
            return CoreSDK.Rewards.List().ToArray();
#endif
        }

        public static PlayerReward[] GivenList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_GivenList();
            return UtilityJSON.GetArray<PlayerReward>(data);
#else
            return CoreSDK.Rewards.GivenList().ToArray();
#endif
        }

        public static AllRewardData GetReward(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_GetReward(idOrTag);
            return UtilityJSON.Get<AllRewardData>(data);
#else
           return CoreSDK.Rewards.GetReward(idOrTag);
#endif
        }

        public static bool Has(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Rewards_Has(idOrTag) == "true";
#else
            return CoreSDK.Rewards.Has(idOrTag);
#endif
        }

        public static bool HasAccepted(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Rewards_HasAccepted(idOrTag) == "true";
#else
            return CoreSDK.Rewards.HasAccepted(idOrTag);
#endif
        }

        public static bool HasUnaccepted(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Rewards_HasUnaccepted(idOrTag) == "true";
#else
            return CoreSDK.Rewards.HasUnaccepted(idOrTag);
#endif
        }

    }

}
