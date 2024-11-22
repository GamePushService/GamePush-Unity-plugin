using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;


namespace Examples.Triggers
{
    public class Triggers : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _triggerTag;
        [Space]
        [SerializeField] private Button _buttonClaim;
        [SerializeField] private Button _buttonGetTrigger;
        [SerializeField] private Button _buttonList;
        [SerializeField] private Button _buttonActivatedList;
        [SerializeField] private Button _buttonIsActivated;
        [SerializeField] private Button _buttonIsClaimed;

        private void OnEnable()
        {
            _buttonClaim.onClick.AddListener(Claim);

            _buttonGetTrigger.onClick.AddListener(GetTrigger);
            _buttonIsActivated.onClick.AddListener(IsActivated);
            _buttonIsClaimed.onClick.AddListener(IsClaimed);

            _buttonList.onClick.AddListener(List);
            _buttonActivatedList.onClick.AddListener(ActivatedList);

            GP_Triggers.OnTriggerActivate += OnActivate;
            GP_Triggers.OnTriggerClaim += OnClaim;
            GP_Triggers.OnTriggerClaimError += OnClaimError;
        }

        private void OnDisable()
        {
            _buttonClaim.onClick.RemoveListener(Claim);

            _buttonGetTrigger.onClick.RemoveListener(GetTrigger);
            _buttonIsActivated.onClick.RemoveListener(IsActivated);
            _buttonIsClaimed.onClick.RemoveListener(IsClaimed);

            _buttonList.onClick.RemoveListener(List);
            _buttonActivatedList.onClick.RemoveListener(ActivatedList);

            GP_Triggers.OnTriggerActivate -= OnActivate;
            GP_Triggers.OnTriggerClaim -= OnClaim;
            GP_Triggers.OnTriggerClaimError -= OnClaimError;
        }

        public void Claim()
        {
            ConsoleUI.Instance.Log("Claim: " + _triggerTag.text);
            GP_Triggers.Claim(_triggerTag.text);
        }

        public void List()
        {
            TriggerData[] triggers = GP_Triggers.List();
            foreach(TriggerData trigger in triggers)
            {
                ConsoleUI.Instance.Log("ID: " + trigger.id);
                ConsoleUI.Instance.Log("Tag: " + trigger.tag);
                ConsoleUI.Instance.Log("AutoClaim: " + trigger.isAutoClaim);
                ConsoleUI.Instance.Log("Description: " + trigger.description);

                foreach (TriggerCondition condition in trigger.conditions)
                {
                    ConsoleUI.Instance.Log("Condition: " + JsonUtility.ToJson(condition));
                }

                foreach (TriggerBonus bonus in trigger.bonuses)
                {
                    ConsoleUI.Instance.Log("Bonus: " + JsonUtility.ToJson(bonus));
                }

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void ActivatedList()
        {
            TriggerActive[] triggers = GP_Triggers.ActivatedList();
            foreach (TriggerActive trigger in triggers)
            {
                ConsoleUI.Instance.Log("ID: " + trigger.triggerId);
                ConsoleUI.Instance.Log("Claimed: " + trigger.claimed);

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void GetTrigger()
        {
            TriggerData trigger = GP_Triggers.GetTrigger(_triggerTag.text).trigger;

            ConsoleUI.Instance.Log("ID: " + trigger.id);
            ConsoleUI.Instance.Log("Tag: " + trigger.tag);
            ConsoleUI.Instance.Log("AutoClaim: " + trigger.isAutoClaim);
            ConsoleUI.Instance.Log("Description: " + trigger.description);

            foreach(TriggerCondition condition in trigger.conditions)
            {
                ConsoleUI.Instance.Log("Condition: " + JsonUtility.ToJson(condition));
            }

            foreach (TriggerBonus bonus in trigger.bonuses)
            {
                ConsoleUI.Instance.Log("Bonus: " + JsonUtility.ToJson(bonus));
            }
            ConsoleUI.Instance.Log(" ");
        }

        public void IsActivated()
        {
            bool isActivated = GP_Triggers.IsActivated(_triggerTag.text);
            ConsoleUI.Instance.Log(isActivated);
        }

        public void IsClaimed()
        {
            bool isClaimed = GP_Triggers.IsClaimed(_triggerTag.text);
            ConsoleUI.Instance.Log(isClaimed);
        }


        public void OnActivate(TriggerData trigger)
        {
            ConsoleUI.Instance.Log("Activate: " + JsonUtility.ToJson(trigger));
        }

        public void OnClaim(TriggerData trigger)
        {
            ConsoleUI.Instance.Log("Activate: " + JsonUtility.ToJson(trigger));
        }

        public void OnClaimError(string error)
        {
            ConsoleUI.Instance.Log(error);
        }
    }
}
