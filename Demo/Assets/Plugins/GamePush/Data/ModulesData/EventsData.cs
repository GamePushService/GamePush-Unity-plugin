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
        public int timeLeft;
        public bool isAutoJoin;
        public TriggerData[] triggers;
    }

    [System.Serializable]
    public class PlayerEvents
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
}