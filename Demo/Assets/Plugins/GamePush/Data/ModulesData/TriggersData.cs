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
        public TriggerCondition[] conditions;
        public TriggerBonus[] bonuses;
    }

    [System.Serializable]
    public class TriggerCondition
    {
        public string conditionType;
        public string key;
        public string operatorType;
        public string[] value;
    }

    [System.Serializable]
    public class TriggerBonus
    {
        public string type;
        public int id;
    }

    [System.Serializable]
    public class PlayerTrigger
    {
        public string triggerId;
        public bool claimed;
    }
}