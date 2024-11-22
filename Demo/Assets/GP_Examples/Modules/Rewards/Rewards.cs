using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Rewards
{
    public class Rewards : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _rewardTag;
        [Space]
        [SerializeField] private Button _buttonGive;
        [SerializeField] private Button _buttonAccept;
        [SerializeField] private Button _buttonList;
        [SerializeField] private Button _buttonGivenList;
        [SerializeField] private Button _buttonGetReward;
        [SerializeField] private Button _buttonHas;
        [SerializeField] private Button _buttonHasAccepted;
        [SerializeField] private Button _buttonHasUnaccepted;

        private void OnEnable()
        {
            _buttonGive.onClick.AddListener(Give);
            _buttonAccept.onClick.AddListener(Accept);
            _buttonList.onClick.AddListener(List);
            _buttonGivenList.onClick.AddListener(GivenList);
            _buttonGetReward.onClick.AddListener(GetReward);
            _buttonHas.onClick.AddListener(Has);
            _buttonHasAccepted.onClick.AddListener(HasAccepted);
            _buttonHasUnaccepted.onClick.AddListener(HasUnaccepted);

            GP_Rewards.OnRewardsGive += OnGive;
            GP_Rewards.OnRewardsGiveError += OnGiveError;
            GP_Rewards.OnRewardsAccept += OnAccept;
            GP_Rewards.OnRewardsAcceptError += OnAcceptError;

        }

        private void OnDisable()
        {
            _buttonGive.onClick.RemoveListener(Give);
            _buttonAccept.onClick.RemoveListener(Accept);
            _buttonList.onClick.RemoveListener(List);
            _buttonGivenList.onClick.RemoveListener(GivenList);
            _buttonGetReward.onClick.RemoveListener(GetReward);
            _buttonHas.onClick.RemoveListener(Has);
            _buttonHasAccepted.onClick.RemoveListener(HasAccepted);
            _buttonHasUnaccepted.onClick.RemoveListener(HasUnaccepted);

            GP_Rewards.OnRewardsGive -= OnGive;
            GP_Rewards.OnRewardsGiveError -= OnGiveError;
            GP_Rewards.OnRewardsAccept -= OnAccept;
            GP_Rewards.OnRewardsAcceptError -= OnAcceptError;

        }

        public void Give()
        {
            ConsoleUI.Instance.Log("Try Give: " + _rewardTag.text);
            GP_Rewards.Give(_rewardTag.text);
        }

        public void Accept()
        {
            ConsoleUI.Instance.Log("Try Accept: " + _rewardTag.text);
            GP_Rewards.Accept(_rewardTag.text);
        }

        public void List()
        {
            RewardData[] rewardsData = GP_Rewards.List();
            if (rewardsData.Length == 0)
                ConsoleUI.Instance.Log("No rewards");

            foreach (RewardData data in rewardsData)
            {
                ConsoleUI.Instance.Log("ID: " + data.id);
                ConsoleUI.Instance.Log("Tag: " + data.tag);
                ConsoleUI.Instance.Log("Name: " + data.name);

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void GivenList()
        {
            PlayerReward[] playerRewards = GP_Rewards.GivenList();
            if(playerRewards.Length == 0)
                ConsoleUI.Instance.Log("No player rewards");

            foreach (PlayerReward data in playerRewards)
            {
                ConsoleUI.Instance.Log("Reward ID: " + data.rewardId);
                ConsoleUI.Instance.Log("Count Total: " + data.countTotal);
                ConsoleUI.Instance.Log("Count Accepted: " + data.countAccepted);

                ConsoleUI.Instance.Log(" ");
            }
        }

        public void GetReward()
        {
            RewardData data = GP_Rewards.GetReward(_rewardTag.text).reward;
            if(data.tag == null)
            {
                ConsoleUI.Instance.Log("No such reward");
                return;
            }

            
            ConsoleUI.Instance.Log("Tag: " + data.tag);
            ConsoleUI.Instance.Log("Name: " + data.name);
            ConsoleUI.Instance.Log("Description: " + data.description);
            ConsoleUI.Instance.Log("Icon: " + data.icon);
            ConsoleUI.Instance.Log("Icon small: " + data.iconSmall);
            ConsoleUI.Instance.Log("is Auto Accept: " + data.isAutoAccept);
            ConsoleUI.Instance.Log("Mutations: ");
            foreach (DataMutation mutation in data.mutations)
            {
                ConsoleUI.Instance.Log(" Type: " + mutation.type);
                ConsoleUI.Instance.Log(" Key: " + mutation.key);
                ConsoleUI.Instance.Log(" Action: " + mutation.action);
                ConsoleUI.Instance.Log(" Value: " + mutation.value);
                ConsoleUI.Instance.Log(" ");
            }

            ConsoleUI.Instance.Log(" ");
        }

        public void Has()
        {
            bool has = GP_Rewards.Has(_rewardTag.text);
            ConsoleUI.Instance.Log("Has reward: " + has);
            ConsoleUI.Instance.Log(" ");
        }

        public void HasAccepted()
        {
            bool has = GP_Rewards.HasAccepted(_rewardTag.text);
            ConsoleUI.Instance.Log("Has accepted reward: " + has);
            ConsoleUI.Instance.Log(" ");
        }

        public void HasUnaccepted()
        {
            bool has = GP_Rewards.HasUnaccepted(_rewardTag.text);
            ConsoleUI.Instance.Log("Has unaccepted reward: " + has);
            ConsoleUI.Instance.Log(" ");
        }


        public void OnGive(AllRewardData result)
        {
            ConsoleUI.Instance.Log("Give reward: " + result.reward.name);
            ConsoleUI.Instance.Log(" ");
        }

        public void OnGiveError(string error)
        {
            ConsoleUI.Instance.Log("Give error: " + error);
            ConsoleUI.Instance.Log(" ");
        }

        public void OnAccept(AllRewardData result)
        {
            ConsoleUI.Instance.Log("Accept reward: " + result.reward.name);
            ConsoleUI.Instance.Log(" ");
        }

        public void OnAcceptError(string error)
        {
            ConsoleUI.Instance.Log("Accept error: " + error);
            ConsoleUI.Instance.Log(" ");
        }
    }
}

