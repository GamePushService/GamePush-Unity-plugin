using System;
using System.Collections.Generic;
using GamePush.Data;

namespace GamePush
{
    [Serializable]
    public enum SyncStorageType
    {
        preffered,
        local,
        platform,
        cloud
    }

    [Serializable]
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

    [Serializable]
    public class StateValue
    {
        public string key;
        public object value;
    }

    [Serializable]
    public class PlayerStats
    {
        public int playtimeAll;
        public int playtimeToday;
        public int activeDays;
        public int activeDaysConsecutive;
    }

    [Serializable]
    public class GetPlayerInput
    {
        public bool isFirstRequest;
    }

    [Serializable]
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

    [Serializable]
    public class ClaimSchedulerDayInput
    {
        public int schedulerId;
        public int day;
    }

    [Serializable]
    public class RewardToIncrement
    {
        public int id;
        public int count;
    }

    [Serializable]
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


    [Serializable]
    public class PlayerField
    {
        public string name;
        public string key;
        public string type;
        public string @default;
        public bool important;
        public bool @public;
        public List<PlayerFieldVariant> variants;
        public PlayerFieldLimits limits;
        public PlayerFieldIncrement intervalIncrement;
    }

    [Serializable]
    public class PlayerFieldIncrement
    {
        public float interval;
        public float increment;
    }

    [Serializable]
    public class PlayerFieldLimits
    {
        public float min;
        public float max;
        public bool couldGoOverLimit;
    }

    [Serializable]
    public class PlayerFieldVariant
    {
        public string value; // string | number
        public string name;
    }
    
    public class PlayerState
    {
        public int Id { get; set; }
        public bool Active { get; set; }
        public bool Removed { get; set; }
        public bool Test { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int Score { get; set; }
        public string? SecretCode { get; set; }

        // Для хранения дополнительных данных с динамическими ключами
        public Dictionary<string, object> AdditionalProperties { get; set; } = new();
    }

    public class PlayerOutput
    {
        public PlayerState State { get; set; }
        public PlayerStats Stats { get; set; }
        public List<PlayerAchievement> AchievementsList { get; set; } = new();
        public List<PlayerPurchase> PurchasesListV2 { get; set; } = new();
        public List<PlayerReward> Rewards { get; set; } = new();
        // public List<PlayerTrigger> Triggers { get; set; } = new();
        public string Token { get; set; }
        public string FirstReqHash { get; set; }
        public string SessionStart { get; set; }
        // public List<PlayerScheduler> PlayerSchedulers { get; set; } = new();

        public RewardsData RewardsData { get; set; }
        // public List<PlayerEvent> PlayerEvents { get; set; } = new();
        // public List<PlayerExperiment> Experiments { get; set; } = new();
        public List<string> Segments { get; set; } = new();
        public List<string> NewSegments { get; set; } = new();
        public List<string> LeftSegments { get; set; } = new();
         public List<UniquesData> Uniques { get; set; } = new();
        public string ServerTime { get; set; }
    }

    public class RewardsData
    {
        public List<string> ActivatedTriggersNow { get; set; } = new();
        public List<string> ClaimedTriggersNow { get; set; } = new();
        // public List<ClaimSchedulerDayOutput> ClaimedSchedulersDaysNow { get; set; } = new();
        public List<int> GivenAchievements { get; set; } = new();
        public List<int> GivenRewards { get; set; } = new();
        public List<int> GivenProducts { get; set; } = new();
    }
    
    // public class PlayerEvents
    // {
    //     public object? Change { get; set; }
    //     public bool Sync { get; set; }
    //     public bool Load { get; set; }
    //     public bool Login { get; set; }
    //     public bool Logout { get; set; }
    //     public bool FetchFields { get; set; }
    //     public bool CheckUnique { get; set; }
    //     public bool RegisterUnique { get; set; }
    //
    //     // В TypeScript `void` означает отсутствие данных, в C# можно использовать `Action`
    //     public Action? Ready { get; set; }
    //     public Action? SyncBefore { get; set; }
    //
    //     public FieldIncrementEvent? FieldIncrement { get; set; }
    //     public FieldEvent? FieldMaximum { get; set; }
    //     public FieldEvent? FieldMinimum { get; set; }
    // }

    public class FieldIncrementEvent
    {
        public PlayerField Field { get; set; }
        public int OldValue { get; set; }
        public int NewValue { get; set; }
    }

    public class FieldEvent
    {
        public PlayerField Field { get; set; }
    }

}
