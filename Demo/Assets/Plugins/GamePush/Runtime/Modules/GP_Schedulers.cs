using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public class GP_Schedulers : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }


    }

    public enum ShedulerType { ACTIVE_DAYS, ACTIVE_DAYS_CONSECUTIVE}

    [System.Serializable]
    public class ShedulerData
    {
        public int id;
        public string tag;
        public ShedulerType type;
        public int days;
        public bool isRepeat;
        public bool isAutoRegister;
        public TriggerData[] triggers;
    }

    [System.Serializable]
    public class PlayerSheduler
    {
        public int shedulerId;
        public int daysClaimed;
        public PlayerShedulerStats stats;
    }

    [System.Serializable]
    public class PlayerShedulerStats
    {
        public int activeDays;
        public int activeDaysConsecutive;
    }

    [System.Serializable]
    public class ShedulerInfo
    {
        public ShedulerData sheduler;
        public PlayerShedulerStats stats;
        public int[] daysClaimed;
        public bool isRegistered;
        public int currentDay;
    }

    [System.Serializable]
    public class ShedulerDayInfo
    {
        public ShedulerData sheduler;
        public int day;
        public bool isDayReached;
        public TriggerData[] triggers;
    }
}
