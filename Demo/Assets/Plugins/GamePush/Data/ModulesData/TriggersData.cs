namespace GamePush
{
    [System.Serializable]
    public class TriggerAllData
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
        public string id;
    }

    [System.Serializable]
    public class TriggerActive
    {
        public string triggerId;
        public bool claimed;
    }
}