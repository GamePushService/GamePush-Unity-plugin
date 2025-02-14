using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization;
using GamePush.Data;

namespace GamePush
{
    [System.Serializable]
    public class AllRewardData
    {
        public RewardData reward;
        public PlayerReward playerReward;

        public AllRewardData(RewardData reward = null, PlayerReward playerReward = null)
        {
            this.reward = reward;
            this.playerReward = playerReward;
        }
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
        public MutationType type;
        public string key;
        public MutationAction action;
        public string value;
    }

    public enum MutationAction
    {
        [EnumMember(Value = "ADD")]
        Add,
    
        [EnumMember(Value = "REMOVE")]
        Remove,
    
        [EnumMember(Value = "SET")]
        Set
    }

    public enum MutationType
    {
        [EnumMember(Value = "PLAYER_FIELD")] PlayerField
    }

    public class GivePlayerRewardInput
    {
        public int? id { get; set; }
        public string? tag { get; set; }
    }

    public class FetchPlayerRewardsOutput
    {
        public List<Reward> Rewards { get; set; } = new();
        public List<PlayerReward> PlayerRewards { get; set; } = new();
    }
}

