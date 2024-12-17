using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Events
{
    public class Events : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _eventTag;
        [Space]
        [SerializeField] private Button _buttonJoin;
        [SerializeField] private Button _buttonGetEvent;
        [SerializeField] private Button _buttonList;
        [SerializeField] private Button _buttonActiveList;
        [SerializeField] private Button _buttonIsActive;
        [SerializeField] private Button _buttonIsJoined;

        private void OnEnable()
        {
            _buttonJoin.onClick.AddListener(Join);

            _buttonGetEvent.onClick.AddListener(GetEvent);
            _buttonIsActive.onClick.AddListener(IsActive);
            _buttonIsJoined.onClick.AddListener(IsJoined);

            _buttonList.onClick.AddListener(List);
            _buttonActiveList.onClick.AddListener(ActiveList);

            GP_Events.OnEventJoin += OnJoin;
            GP_Events.OnEventJoinError += OnJoinError;
        }

        private void OnDisable()
        {
            _buttonJoin.onClick.RemoveListener(Join);

            _buttonGetEvent.onClick.RemoveListener(GetEvent);
            _buttonIsActive.onClick.RemoveListener(IsActive);
            _buttonIsJoined.onClick.RemoveListener(IsJoined);

            _buttonList.onClick.RemoveListener(List);
            _buttonActiveList.onClick.RemoveListener(ActiveList);

            GP_Events.OnEventJoin -= OnJoin;
            GP_Events.OnEventJoinError -= OnJoinError;
        }

        public void Join()
        {
            ConsoleUI.Instance.Log("Try Join: " + _eventTag.text);
            GP_Events.Join(_eventTag.text);
            ConsoleUI.Instance.Log(" ");
        }

        public void List()
        {
            EventData[] eventsData = GP_Events.List();
            foreach (EventData data in eventsData)
            {
                ConsoleUI.Instance.Log("ID: " + data.id);
                ConsoleUI.Instance.Log("Tag: " + data.tag);
                ConsoleUI.Instance.Log("Name: " + data.name);
                ConsoleUI.Instance.Log("Description: " + data.description);
                ConsoleUI.Instance.Log("Icon: " + data.icon);
                ConsoleUI.Instance.Log("Icon small: " + data.iconSmall);
                ConsoleUI.Instance.Log("Date Start: " + data.dateStart);
                ConsoleUI.Instance.Log("Date End: " + data.dateEnd);
                ConsoleUI.Instance.Log("Is Active: " + data.isActive);
                ConsoleUI.Instance.Log("Time left: " + data.timeLeft);
                ConsoleUI.Instance.Log("Is Auto join: " + data.isAutoJoin);

                foreach (TriggerData trigger in data.triggers)
                {
                    ConsoleUI.Instance.Log("Trigger: " + JsonUtility.ToJson(trigger));
                }

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void ActiveList()
        {
            PlayerEvents[] playerEvents = GP_Events.ActiveList();
            foreach (PlayerEvents playerEvent in playerEvents)
            {
                ConsoleUI.Instance.Log("ID: " + playerEvent.eventId);
                ConsoleUI.Instance.Log("Active Days: " + playerEvent.stats.activeDays);
                ConsoleUI.Instance.Log("Active Days Consecutive: " + playerEvent.stats.activeDaysConsecutive);

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void GetEvent()
        {
            EventData data= GP_Events.GetEvent(_eventTag.text);

            ConsoleUI.Instance.Log("ID: " + data.id);
            ConsoleUI.Instance.Log("Tag: " + data.tag);
            ConsoleUI.Instance.Log("Name: " + data.name);
            ConsoleUI.Instance.Log("Description: " + data.description);
            ConsoleUI.Instance.Log("Icon: " + data.icon);
            ConsoleUI.Instance.Log("Icon small: " + data.iconSmall);
            ConsoleUI.Instance.Log("Date Start: " + data.dateStart);
            ConsoleUI.Instance.Log("Date End: " + data.dateEnd);
            ConsoleUI.Instance.Log("Is Active: " + data.isActive);
            ConsoleUI.Instance.Log("Time left: " + data.timeLeft);
            ConsoleUI.Instance.Log("Is Auto join: " + data.isAutoJoin);

            foreach (TriggerData trigger in data.triggers)
            {
                ConsoleUI.Instance.Log("Trigger: " + JsonUtility.ToJson(trigger));
            }

            ConsoleUI.Instance.Log(" ");
        }

        public void IsActive()
        {
            bool isActive = GP_Events.IsActive(_eventTag.text);
            ConsoleUI.Instance.Log(isActive);
            ConsoleUI.Instance.Log(" ");
        }

        public void IsJoined()
        {
            bool isJoined = GP_Events.IsJoined(_eventTag.text);
            ConsoleUI.Instance.Log(isJoined);
            ConsoleUI.Instance.Log(" ");
        }

        public void OnJoin(PlayerEvents Event)
        {
            ConsoleUI.Instance.Log("Join event: " + JsonUtility.ToJson(Event));
            ConsoleUI.Instance.Log(" ");
        }

        public void OnJoinError(string error)
        {
            ConsoleUI.Instance.Log("Join error: " + error);
            ConsoleUI.Instance.Log(" ");
        }
    }
}

