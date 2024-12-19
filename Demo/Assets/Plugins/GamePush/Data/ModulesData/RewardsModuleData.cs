using System.Collections;
using System.Collections.Generic;

namespace GamePush
{
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

