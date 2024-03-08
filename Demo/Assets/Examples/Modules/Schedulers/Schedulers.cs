using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

using GamePush;
using Examples.Console;

namespace Examples.Schedulers
{
    public class Schedulers : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _schedulerTag;
        [SerializeField] private TMP_InputField _dayNumber;
        [SerializeField] private TMP_InputField _triggerTag;
        [Space]
        [SerializeField] private Button _buttonRegister;
        [SerializeField] private Button _buttonList;
        [SerializeField] private Button _buttonActiveList;
        [SerializeField] private Button _buttonClaimDay;
        [SerializeField] private Button _buttonClaimDayAdditional;
        [SerializeField] private Button _buttonClaimAllDay;
        [SerializeField] private Button _buttonClaimAllDays;
        [SerializeField] private Button _buttonGetScheduler;
        [SerializeField] private Button _buttonGetSchedulerDay;
        [SerializeField] private Button _buttonGetSchedulerCurrentDay;
        [SerializeField] private Button _buttonCanClaimDay;
        [SerializeField] private Button _buttonCanClaimDayAdditional;
        [SerializeField] private Button _buttonCanClaimAllDay;
        [SerializeField] private Button _buttonIsRegistered;
        [SerializeField] private Button _buttonIsTodayRewardClaimed;

        private void OnEnable()
        {
            _buttonRegister.onClick.AddListener(Register);
            _buttonList.onClick.AddListener(List);
            _buttonActiveList.onClick.AddListener(ActiveList);
            _buttonClaimDay.onClick.AddListener(ClaimDay);
            _buttonClaimDayAdditional.onClick.AddListener(ClaimDayAdditional);
            _buttonClaimAllDay.onClick.AddListener(ClaimAllDay);
            _buttonClaimAllDays.onClick.AddListener(ClaimAllDays);
            _buttonGetScheduler.onClick.AddListener(GetScheduler);
            _buttonGetSchedulerDay.onClick.AddListener(GetSchedulerDay);
            _buttonGetSchedulerCurrentDay.onClick.AddListener(GetSchedulerCurrentDay);
            _buttonCanClaimDay.onClick.AddListener(CanClaimDay);
            _buttonCanClaimDayAdditional.onClick.AddListener(CanClaimDayAdditional);
            _buttonCanClaimAllDay.onClick.AddListener(CanClaimAllDay);
            _buttonIsRegistered.onClick.AddListener(IsRegistered);
            _buttonIsTodayRewardClaimed.onClick.AddListener(IsTodayRewardClaimed);

            GP_Schedulers.OnSchedulerRegister += OnSchedulerRegister;
            GP_Schedulers.OnSchedulerRegisterError += OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimAllDay += OnSchedulerClaimAllDay;
            GP_Schedulers.OnSchedulerClaimAllDayError += OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimAllDays += OnSchedulerClaimAllDays;
            GP_Schedulers.OnSchedulerClaimAllDaysError += OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimDay += OnSchedulerClaimDay;
            GP_Schedulers.OnSchedulerClaimDayError += OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimDayAdditional += OnSchedulerClaimDayAdditional;
            GP_Schedulers.OnSchedulerClaimDayAdditionalError += OnSchedulerError;
            GP_Schedulers.OnSchedulerJoin += OnSchedulerJoin;
            GP_Schedulers.OnSchedulerJoinError += OnSchedulerError;
        }

        private void OnDisable()
        {
            _buttonRegister.onClick.RemoveListener(Register);
            _buttonList.onClick.RemoveListener(List);
            _buttonActiveList.onClick.RemoveListener(ActiveList);
            _buttonClaimDay.onClick.RemoveListener(ClaimDay);
            _buttonClaimDayAdditional.onClick.RemoveListener(ClaimDayAdditional);
            _buttonClaimAllDay.onClick.RemoveListener(ClaimAllDay);
            _buttonClaimAllDays.onClick.RemoveListener(ClaimAllDays);
            _buttonGetScheduler.onClick.RemoveListener(GetScheduler);
            _buttonGetSchedulerDay.onClick.RemoveListener(GetSchedulerDay);
            _buttonGetSchedulerCurrentDay.onClick.RemoveListener(GetSchedulerCurrentDay);
            _buttonCanClaimDay.onClick.RemoveListener(CanClaimDay);
            _buttonCanClaimDayAdditional.onClick.RemoveListener(CanClaimDayAdditional);
            _buttonCanClaimAllDay.onClick.RemoveListener(CanClaimAllDay);
            _buttonIsRegistered.onClick.RemoveListener(IsRegistered);
            _buttonIsTodayRewardClaimed.onClick.RemoveListener(IsTodayRewardClaimed);

            GP_Schedulers.OnSchedulerRegister -= OnSchedulerRegister;
            GP_Schedulers.OnSchedulerRegisterError -= OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimAllDay -= OnSchedulerClaimAllDay;
            GP_Schedulers.OnSchedulerClaimAllDayError -= OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimAllDays -= OnSchedulerClaimAllDays;
            GP_Schedulers.OnSchedulerClaimAllDaysError -= OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimDay -= OnSchedulerClaimDay;
            GP_Schedulers.OnSchedulerClaimDayError -= OnSchedulerError;
            GP_Schedulers.OnSchedulerClaimDayAdditional -= OnSchedulerClaimDayAdditional;
            GP_Schedulers.OnSchedulerClaimDayAdditionalError -= OnSchedulerError;
            GP_Schedulers.OnSchedulerJoin -= OnSchedulerJoin;
            GP_Schedulers.OnSchedulerJoinError -= OnSchedulerError;
        }

        #region Get Values

        string SchedulerTag() => _schedulerTag.text;

        int DayNumber()
        {
            int result;
            int.TryParse(_dayNumber.text, out result);
            Debug.Log("Get day " + result.ToString());
            return result;
        }

        string TriggerTag() => _triggerTag.text;

        #endregion

        #region Module methods

        public void Register()
        {
            ConsoleUI.Instance.Log("Try Register: " + _schedulerTag.text);
            GP_Schedulers.Register(_schedulerTag.text);
        }

        public void List()
        {
            SchedulerData[] schedulerData = GP_Schedulers.List();
            if (schedulerData.Length == 0)
                ConsoleUI.Instance.Log("No schedulers");

            foreach (SchedulerData data in schedulerData)
            {
                ConsoleUI.Instance.Log("ID: " + data.id);
                ConsoleUI.Instance.Log("Tag: " + data.tag);
                ConsoleUI.Instance.Log("Days: " + data.days);

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void ActiveList()
        {
            PlayerScheduler[] schedulerData = GP_Schedulers.ActiveList();
            if (schedulerData.Length == 0)
                ConsoleUI.Instance.Log("No schedulers");

            foreach (PlayerScheduler data in schedulerData)
            {
                ConsoleUI.Instance.Log("ID: " + data.schedulerId);
                ConsoleUI.Instance.Log("Active Days: " + data.stats.activeDays);
                ConsoleUI.Instance.Log("Days claimed: " + string.Join(",", data.daysClaimed));

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void ClaimDay() => GP_Schedulers.ClaimDay(SchedulerTag(), DayNumber());

        public void ClaimDayAdditional() => GP_Schedulers.ClaimDayAdditional(SchedulerTag(), DayNumber(), TriggerTag());

        public void ClaimAllDay() => GP_Schedulers.ClaimAllDay(SchedulerTag(), DayNumber());

        public void ClaimAllDays() => GP_Schedulers.ClaimAllDays(SchedulerTag());

        public void GetScheduler()
        {
            SchedulerInfo data = GP_Schedulers.GetScheduler(SchedulerTag());

            ConsoleUI.Instance.Log("ID: " + data.scheduler.id);
            ConsoleUI.Instance.Log("Type: " + data.scheduler.type);
            ConsoleUI.Instance.Log("Current Day: " + data.currentDay);
            ConsoleUI.Instance.Log("Active Days: " + data.stats.activeDays);
            ConsoleUI.Instance.Log("Days claimed: " + string.Join(",", data.daysClaimed));
            ConsoleUI.Instance.Log("Is Registered: " + data.isRegistered);

            ConsoleUI.Instance.Log(" ");
        }

        public void GetSchedulerDay()
        {
            SchedulerDayInfo data = GP_Schedulers.GetSchedulerDay(SchedulerTag(), DayNumber());

            ConsoleUI.Instance.Log("ID: " + data.scheduler.id);
            ConsoleUI.Instance.Log("Day: " + data.day);
            ConsoleUI.Instance.Log("Day Reached: " + data.isDayReached);
            ConsoleUI.Instance.Log("Day Claimed: " + data.isDayClaimed);
            ConsoleUI.Instance.Log("Day Complete: " + data.isDayComplete);

            ConsoleUI.Instance.Log(" ");
        }

        public void GetSchedulerCurrentDay()
        {
            SchedulerDayInfo data = GP_Schedulers.GetSchedulerCurrentDay(SchedulerTag());

            ConsoleUI.Instance.Log("ID: " + data.scheduler.id);
            ConsoleUI.Instance.Log("Day: " + data.day);
            ConsoleUI.Instance.Log("Day Reached: " + data.isDayReached);
            ConsoleUI.Instance.Log("Day Claimed: " + data.isDayClaimed);
            ConsoleUI.Instance.Log("Day Complete: " + data.isDayComplete);

            ConsoleUI.Instance.Log(" ");
        }

        public void CanClaimDay()
        {
            bool canClaim = GP_Schedulers.CanClaimDay(SchedulerTag(), DayNumber());
            ConsoleUI.Instance.Log("Can Claim Day: " + canClaim);
            ConsoleUI.Instance.Log(" ");
        }

        public void CanClaimDayAdditional()
        {
            bool canClaim = GP_Schedulers.CanClaimDayAdditional(SchedulerTag(), DayNumber(), TriggerTag());
            ConsoleUI.Instance.Log("Can Claim Day Additional: " + canClaim);
            ConsoleUI.Instance.Log(" ");
        }

        public void CanClaimAllDay()
        {
            bool canClaim = GP_Schedulers.CanClaimAllDay(SchedulerTag(), DayNumber());
            ConsoleUI.Instance.Log("Can Claim All Day: " + canClaim);
            ConsoleUI.Instance.Log(" ");
        }

        public void IsRegistered()
        {
            bool isRegistered = GP_Schedulers.IsRegistered(SchedulerTag());
            ConsoleUI.Instance.Log("Is Registered: " + isRegistered);
            ConsoleUI.Instance.Log(" ");
        }

        public void IsTodayRewardClaimed()
        {
            bool isTodayRewardClaimed = GP_Schedulers.IsTodayRewardClaimed(SchedulerTag());
            ConsoleUI.Instance.Log("Is Today Reward Claimed: " + isTodayRewardClaimed);
            ConsoleUI.Instance.Log(" ");
        }
        #endregion

        #region Module callbacks

        private void OnSchedulerError(string error)
        {
            ConsoleUI.Instance.Log("Error: " + error);
            ConsoleUI.Instance.Log(" ");
        }

        private void OnSchedulerRegister(SchedulerInfo data)
        {
            ConsoleUI.Instance.Log("Scheduler Register");
            ConsoleUI.Instance.Log("Day: " + data.currentDay);
            ConsoleUI.Instance.Log("Is Registered: " + data.isRegistered);
            ConsoleUI.Instance.Log(" ");
        }

        private void OnSchedulerClaimDay(SchedulerDayInfo data)
        {
            ConsoleUI.Instance.Log("Scheduler Claim");
            ConsoleUI.Instance.Log("ID: " + data.scheduler.id);
            ConsoleUI.Instance.Log("Day: " + data.day);
            ConsoleUI.Instance.Log("Day Reached: " + data.isDayReached);
            ConsoleUI.Instance.Log("Day Claimed: " + data.isDayClaimed);
            ConsoleUI.Instance.Log("Day Complete: " + data.isDayComplete);
            ConsoleUI.Instance.Log(" ");
        }

        private void OnSchedulerClaimDayAdditional(SchedulerDayInfo data)
        {
            ConsoleUI.Instance.Log("Scheduler Claim Additional");
            ConsoleUI.Instance.Log(JsonUtility.ToJson(data));
            ConsoleUI.Instance.Log(" ");
        }

        private void OnSchedulerClaimAllDay(SchedulerDayInfo data)
        {
            ConsoleUI.Instance.Log("Scheduler Claim All Day");
            ConsoleUI.Instance.Log(JsonUtility.ToJson(data));
            ConsoleUI.Instance.Log(" ");
        }

        private void OnSchedulerClaimAllDays(SchedulerInfo data)
        {
            ConsoleUI.Instance.Log("Scheduler Claim All Days");
            ConsoleUI.Instance.Log(JsonUtility.ToJson(data));
            ConsoleUI.Instance.Log(" ");
        }

        private void OnSchedulerJoin(PlayerScheduler data)
        {
            ConsoleUI.Instance.Log("Scheduler Join");
            ConsoleUI.Instance.Log(JsonUtility.ToJson(data));
            ConsoleUI.Instance.Log(" ");
        }
        #endregion
    }
}
