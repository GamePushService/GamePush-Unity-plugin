using System.Collections.Generic;
using Newtonsoft.Json;

namespace GamePush.Data
{

    [System.Serializable]
    public enum SyncStorageType
    {
        preffered,
        local,
        platform,
        cloud
    }

    [System.Serializable]
    public class AutoSyncData
    {
        public bool isEnable;
        public SyncStorageType storage;
        public int interval;
        public bool @override;

        public string lastSync;

        public AutoSyncData(
            bool isEnableSync,
            SyncStorageType storageType,
            int syncInterval = 0,
            bool isOverride = false,
            string lastSyncTime = ""
            )
        {
            isEnable = isEnableSync;
            storage = storageType;
            interval = syncInterval;
            @override = isOverride;
            lastSync = lastSyncTime;
        }
    }

    
    [System.Serializable]
    public class PlayerState
    {
        int id;
        bool active;
        bool removed;
        bool test;
        string name;
        string avatar;
        int score;
        string secretCode;
        StateValue[] stateValues;
    }

    [System.Serializable]
    public class StateValue
    {
        string key;
        object value;
    }

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

        //[JsonProperty("override")]
        public bool @override;

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
        public PlayerFieldIncrement intervalIncrement;
        public PlayerFieldLimits limits;
    }


    [System.Serializable]
    public class PlayerField
    {
        public string name;
        public string key;
        public string type;
        public string @default;
        public bool important;
        //[JsonProperty("public")]
        public bool @public;
        public List<PlayerFieldVariant> variants;
        public PlayerFieldLimits limits;
        public PlayerFieldIncrement intervalIncrement;
    }

    [System.Serializable]
    public class PlayerFieldIncrement
    {
        public float interval;
        public float increment;
    }

    [System.Serializable]
    public class PlayerFieldLimits
    {
        public float min;
        public float max;
        public bool couldGoOverLimit;
    }

    [System.Serializable]
    public class PlayerFieldVariant
    {
        public string value; // string | number
        public string name;
    }

    
}
