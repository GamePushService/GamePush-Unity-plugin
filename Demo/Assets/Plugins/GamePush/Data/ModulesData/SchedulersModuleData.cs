using System;
using System.Collections.Generic;

namespace GamePush.Data
{
    // public class SaveSchedulerTriggersOnDayInput
    // {
    //     public int SchedulerId { get; set; }
    //     public int Day { get; set; }
    //     public List<BonusInput> Bonuses { get; set; } = new();
    //     public List<EntityTriggerInput> Triggers { get; set; } = new();
    // }
    
    [Serializable]
    public class SchedulerData
    {
        public int id { get; set; }
        public int projectId { get; set; }
        public string tag { get; set; }
        public SchedulerType type { get; set; }
        public int days { get; set; }
        public bool isRepeat { get; set; }
        public bool isPublished { get; set; }
        public bool isAutoRegister { get; set; }
        public List<Trigger> triggers { get; set; } = new();
        public List<SchedulerDaysBonuses> daysBonuses { get; set; } = new();
    }

    [Serializable]
    public class SchedulerDaysBonuses
    {
        public int day { get; set; }
        public List<Bonus> bonuses { get; set; } = new();
    }

    // public class SchedulerTriggersInput
    // {
    //     public int Day { get; set; }
    //     public List<EntityTriggerInput> Triggers { get; set; } = new();
    // }

    public enum SchedulerType
    {
        ActiveDays,
        ActiveDaysConsecutive
    }

    public class SchedulersList
    {
        public List<SchedulerData> Items { get; set; } = new();
        public int Count { get; set; }
    }

    public class SchedulersTriggersOnDayList
    {
        public int SchedulerId { get; set; }
        public int Day { get; set; }
        public List<Bonus> Bonuses { get; set; } = new();
        public List<Trigger> Triggers { get; set; } = new();
    }
}