namespace GamePush
{
    [System.Serializable]
    public class PlayerTriggerInfo
    {
        public TriggerData trigger;
        public bool isActivated;
        public bool isClaimed;
    }

    [System.Serializable]
    public class TriggerData
    {
        public string id;
        public string tag;
        public bool isAutoClaim;
        public string description;
        public Translations descriptions;
        public TriggerCondition[][] conditions;
        public BonusData[] bonuses;
    }

    [System.Serializable]
    public class TriggerCondition
    {
        public string @type;
        public string key;
        public string @operator;
        public string[] value;
    }

    [System.Serializable]
    public class PlayerTrigger
    {
        public string triggerId;
        public bool claimed;
    }
}