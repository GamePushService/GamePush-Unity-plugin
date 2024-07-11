using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Utilities;

namespace GamePush
{
    public class GP_Rewards : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Rewards);

        public static event UnityAction<AllRewardData> OnRewardsGive;
        public static event UnityAction<string> OnRewardsGiveError;
        public static event UnityAction<AllRewardData> OnRewardsAccept;
        public static event UnityAction<string> OnRewardsAcceptError;

        private void CallOnRewardsGive(string data) { OnRewardsGive?.Invoke(JsonUtility.FromJson<AllRewardData>(data)); }
        private void CallOnRewardsGiveError(string error) { OnRewardsGiveError?.Invoke(error); }
        private void CallOnRewardsAccept(string data) { OnRewardsAccept?.Invoke(JsonUtility.FromJson<AllRewardData>(data)); }
        private void CallOnRewardsAcceptError(string error) { OnRewardsAcceptError?.Invoke(error); }


        [DllImport("__Internal")]
        private static extern void GP_Rewards_Give(string idOrTag, bool lazy);
        public static void Give(string idOrTag, bool lazy = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Rewards_Give(idOrTag, lazy);
#else

            ConsoleLog("Give");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Rewards_Accept(string idOrTag);
        public static void Accept(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Rewards_Accept(idOrTag);
#else

            ConsoleLog("Accept");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_List();
        public static RewardData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_List();
            return UtilityJSON.GetArray<RewardData>(data);
#else

            ConsoleLog("LIST");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_GivenList();
        public static PlayerReward[] GivenList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_GivenList();
            return UtilityJSON.GetArray<PlayerReward>(data);
#else

            ConsoleLog("Given List");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_GetReward(string idOrTag);
        public static AllRewardData GetReward(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_GetReward(idOrTag);
            return UtilityJSON.Get<AllRewardData>(data);
#else

            ConsoleLog("Get Reward");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_Has(string idOrTag);
        public static bool Has(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Rewards_Has(idOrTag) == "true";
#else

            ConsoleLog("Has");

            return true;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_HasAccepted(string idOrTag);
        public static bool HasAccepted(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Rewards_HasAccepted(idOrTag) == "true";
#else

            ConsoleLog("Has Accepted");

            return true;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_HasUnaccepted(string idOrTag);
        public static bool HasUnaccepted(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Rewards_HasUnaccepted(idOrTag) == "true";
#else

            ConsoleLog("Has Unaccepted");

            return true;
#endif
        }

    }

    [System.Serializable]
    public class AllRewardData
    {
        public RewardData reward;
        public PlayerReward playerReward;
    }

    [System.Serializable]
    public class RewardData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public DataMutation[] mutations;
        public bool isAutoAccept;
    }

    [System.Serializable]
    public class PlayerReward
    {
        public int rewardId;
        public int countTotal;
        public int countAccepted;
    }

    [System.Serializable]
    public class DataMutation
    {
        public string type;
        public string key;
        public MutationAction action;
        public string value;
    }

    public enum MutationAction { ADD, REMOVE, SET };

}
