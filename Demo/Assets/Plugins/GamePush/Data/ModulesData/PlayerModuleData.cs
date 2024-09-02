using System.Collections.Generic;
using Newtonsoft.Json;

namespace GamePush.Data
{
    [System.Serializable]
    public class PlayerStats
    {
        public int playtimeAll;
        public int playtimeToday;
        public int activeDays;
        public int activeDaysConsecutive;
    }

    [System.Serializable]
    public class GetPlayerInput
    {
        public bool isFirstRequest;
    }

    [System.Serializable]
    public class RewardData
    {
        public object[] activatedTriggersNow = { };
        public object[] claimedTriggersNow = { };
        public object[] claimedSchedulersDaysNow = { };
        public object[] givenAchievements = { };
        public object[] givenRewards = { };
        public object[] givenProducts = { };
    }

    [System.Serializable]
    public class SyncPlayerInput
    {
        public object playerState;

        [JsonProperty("override")]
        public bool Override;

        public bool isFirstRequest;

        public RewardToIncrement[] acceptedRewards = { };
        public RewardToIncrement[] givenRewards = { };
        public string[] claimedTriggers = { };
        public ClaimSchedulerDayInput[] claimedSchedulersDays = { };
    }

    [System.Serializable]
    public class ClaimSchedulerDayInput
    {
        public int schedulerId;
        public int day;
    }

    [System.Serializable]
    public class RewardToIncrement
    {
        public int id;
        public int count;
    }

    [System.Serializable]
    public class PlayerFetchFieldsData
    {
        public string name;
        public string key;
        public string type;
        public string defaultValue; // string | bool | number
        public bool important;
        public bool isPublic;
        public PlayerFieldVariant[] variants;
    }


    [System.Serializable]
    public class PlayerField
    {
        public string name;
        public string key;
        public string type;
        public string @default;
        public bool important;
        public bool @public;
        public List<PlayerFieldVariant> variants;
    }

    [System.Serializable]
    public class PlayerFieldVariant
    {
        public string value;
        public string name;
    }
}