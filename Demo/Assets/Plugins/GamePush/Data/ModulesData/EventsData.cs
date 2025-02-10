using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace GamePush
{
    [System.Serializable]
    public class EventData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public string dateStart;
        public string dateEnd;
        public bool isActive;
        public double timeLeft;
        public bool isAutoJoin;
        public TriggerData[] triggers;
        public Translations names;
        public Translations descriptions;
    }

    public class PlayerEventInfo
    {
        public EventData Event { get; set; }
        public PlayerEvent PlayerEvent { get; set; }
    }

    public class EventInfo
    {
        public EventData? Event { get; set; }
        public EventStats Stats { get; set; } = new EventStats();
        public bool IsJoined { get; set; }
        public List<RewardData> Rewards { get; set; } = new();
        public List<AchievementData> Achievements { get; set; } = new();
        public List<ProductData> Products { get; set; } = new();
    }

    [System.Serializable]
    public class PlayerEvent
    {
        public int eventId;
        public EventStats stats;
    }

    [System.Serializable]
    public class EventStats
    {
        public int activeDays;
        public int activeDaysConsecutive;
    }
    
    public static class BonusType {
        public const string Reward = "REWARD";
        public const string Achievement = "ACHIEVEMENT";
        public const string Product = "PRODUCT";
    }

    [System.Serializable]
    public class PlayerJoinEventInput
    {
        public int eventId;

        public PlayerJoinEventInput(int eventId)
        {
            this.eventId = eventId;
        }
    }
}