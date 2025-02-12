using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GamePush.Data;

namespace GamePush.Core
{
    public class RewardsModule
    {

        private List<object> _notFoundList = new List<object>();
        private List<RewardData> _rewardsList = new List<RewardData>();
        private List<PlayerReward> _playerRewardsList = new List<PlayerReward>();
        private Dictionary<int, RewardData> _rewardsMapID = new Dictionary<int, RewardData>();
        private Dictionary<string, RewardData> _rewardsMapTag = new Dictionary<string, RewardData>();
        private Dictionary<int, PlayerReward> _playerRewardsMap = new Dictionary<int, PlayerReward>();
        
        public void Init()
        {
            
        }
        
        public List<RewardData> List => _rewardsList;
        public List<PlayerReward> GivenList => new List<PlayerReward>(_playerRewardsList);

        public bool Has(int id) => GetRewardInfo(id).playerReward?.countTotal > 0;
        public bool HasAccepted(int id) => GetRewardInfo(id).playerReward?.countAccepted > 0;
        public bool HasUnaccepted(int id)
        {
            var info = GetRewardInfo(id);
            return info.playerReward != null && info.playerReward.countTotal > info.playerReward.countAccepted;
        }

    // public AllRewardData GetReward(int id) => GetRewardInfo(id);

    public RewardData GetReward(int id)
    {
        _rewardsMapID.TryGetValue(id, out var reward);
        return reward;
    }

    private PlayerReward _getPlayerRewardById(int id)
    {
        _playerRewardsMap.TryGetValue(id, out var reward);
        return reward;
    }

    private AllRewardData GetRewardInfo(int id)
    {
        var info = new AllRewardData { reward = null, playerReward = null };
        var reward = GetReward(id);
        if (reward == null) return info;

        info.reward = reward;
        var playerReward = _getPlayerRewardById(reward.id);
        info.playerReward = playerReward ?? new PlayerReward { rewardId = reward.id, countAccepted = 0, countTotal = 0 };

        return info;
    }

    private void SetRewardsList(List<PlayerReward> playerRewards, List<RewardToIncrement> notSentGivenRewards, List<RewardToIncrement> notSentAcceptedRewards)
    {
        var notSentAcceptedLeft = new List<RewardToIncrement>(notSentAcceptedRewards);

        var notSentRewards = notSentGivenRewards
            .Select(r =>
            {
                var playerReward = playerRewards.FirstOrDefault(pr => pr.rewardId == r.id);
                if (playerReward != null)
                {
                    playerReward.countTotal += r.count;
                    return null;
                }

                var reward = _rewardsList.FirstOrDefault(rew => rew.id == r.id);
                return reward != null ? new PlayerReward { rewardId = r.id, countTotal = r.count, countAccepted = 0 } : null;
            })
            .Where(r => r != null)
            .ToList();

        _playerRewardsList = playerRewards.Concat(notSentRewards).ToList();
        RefreshPlayerRewardsMap();
    }

    private void _addReward(int id)
    {
        var reward = GetReward(id);
        if (reward == null) return;

        if (_playerRewardsMap.TryGetValue(id, out var existingReward))
        {
            existingReward.countTotal += 1;
        }
        else
        {
            _playerRewardsList.Insert(0, new PlayerReward { rewardId = id, countTotal = 1, countAccepted = 0 });
            RefreshPlayerRewardsMap();
        }
    }

        private void AcceptReward(PlayerReward playerReward)
        {
            var reward = GetReward(playerReward.rewardId);
            if (reward == null)
            {
                Logger.Error($"Reward {playerReward.rewardId} not found");
                return;
            }

            playerReward.countAccepted++;
            ApplyRewardMutations(reward);
        }

        private void ApplyRewardMutations(RewardData reward)
        {
            try
            {
                MutationHandler.ApplyMutations(reward.mutations.ToList());
            }
            catch (Exception ex)
            {
                Logger.Error("Failed to apply reward", ex.Message);
            }
        }

        private void RefreshRewardsMap()
        {
            _rewardsMapID.Clear();
            _rewardsMapTag.Clear();

            foreach (var reward in _rewardsList)
            {
                _rewardsMapID[reward.id] = reward;
                _rewardsMapTag[reward.tag] = reward;
            }
        }

        private void RefreshPlayerRewardsMap()
        {
            _playerRewardsMap.Clear();
            foreach (var pr in _playerRewardsList)
            {
                _playerRewardsMap[pr.rewardId] = pr;
            }
        }
    }
}
