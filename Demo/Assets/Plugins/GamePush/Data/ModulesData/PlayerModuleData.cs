
using Newtonsoft.Json;

namespace GamePush.Data
{

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
        public Variants[] variants;
    }

    [System.Serializable]
    public class Variants
    {
        public string value; // string | number
        public string name;
    }
}
