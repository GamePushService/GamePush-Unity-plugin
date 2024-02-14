using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Rewards : MonoBehaviour
    {
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
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "Give");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Rewards_Accept(string idOrTag);
        public static void Accept(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Rewards_Accept(idOrTag);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "Accept");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_List();
        public static RewardData[] List()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_List();
            return GP_JSON.GetArray<RewardData>(data);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "LIST");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_GivenList();
        public static PlayerReward[] GivenList()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_GivenList();
            return GP_JSON.GetArray<PlayerReward>(data);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "Given List");

            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Rewards_GetReward(string idOrTag);
        public static AllRewardData GetReward(string idOrTag)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string data = GP_Rewards_GetReward(idOrTag);
            return GP_JSON.Get<AllRewardData>(data);
#else
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "Get Reward");

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
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "Has");

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
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "Has Accepted");

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
            if (GP_ConsoleController.Instance.ChannelConsoleLogs)
                Console.Log("REWARDS: ", "Has Unaccepted");

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
