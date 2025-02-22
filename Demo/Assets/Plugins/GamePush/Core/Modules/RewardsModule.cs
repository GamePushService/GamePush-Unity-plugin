using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GamePush.Data;
using GamePush.Tools;

namespace GamePush.Core
{
    public class RewardsModule
    {
        private readonly List<object> _notFoundList = new List<object>();
        private List<Reward> _rewardsList = new List<Reward>();
        private List<RewardData> _rewardsDataList = new List<RewardData>();
        private List<PlayerReward> _playerRewardsList = new List<PlayerReward>();
        private Dictionary<int, RewardData> _rewardsMapID = new Dictionary<int, RewardData>();
        private Dictionary<string, RewardData> _rewardsMapTag = new Dictionary<string, RewardData>();
        private Dictionary<int, PlayerReward> _playerRewardsMap = new Dictionary<int, PlayerReward>();

        #region Actions
        public event Action<AllRewardData> OnRewardsGive;
        public event Action<string> OnRewardsGiveError;
        public event Action<AllRewardData> OnRewardsAccept;
        public event Action<string> OnRewardsAcceptError;

        #endregion
        
        private const string RewardNotFoundError = "reward_not_found";
        private const string PlayerRewardNotFoundError = "player_reward_not_found";
        private const string RewardAlreadyAcceptedError = "reward_already_accepted";
        public void Init(List<Reward> rewards)
        {
            _rewardsList = rewards;
            SetRewardDataList(rewards);
            RefreshRewardsMap();

            CoreSDK.Language.OnChangeLanguage += ChangeTranslations;
            CoreSDK.OnInit += AcceptOnStart;
        }

        public async void MarkRewardsGiven(List<int> ids)
        {
            try
            {
                foreach (var id in ids)
                {
                    Logger.Log("Set given: " + id);
                    var reward = GetRewardData(id);
                    if (reward == null)
                    {
                        Logger.Error($"Reward not found, ID {id}");
                        continue;
                    }
                    AddReward(reward.id);

                    await Give(reward.id.ToString());
                    if (reward.isAutoAccept)
                    {
                        await Accept(reward.id.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                Logger.Error(e.Message);
            }
        }

        private void SetRewardDataList(List<Reward> rewards)
        {
            List<RewardData> rewardsDataList = new List<RewardData>();
            
            foreach (var reward in rewards)
            {
                RewardData data = new RewardData
                {
                    id = reward.id,
                    tag = reward.tag,
                    name = CoreSDK.Language.GetTranslation(reward.names),
                    description = CoreSDK.Language.GetTranslation(reward.descriptions),
                    mutations = reward.mutations.ToArray(),
                    icon = UtilityImage.ResizeImage(reward.icon, 256, 256, false),
                    iconSmall = UtilityImage.ResizeImage(reward.icon, 48, 48, false),
                    isAutoAccept = reward.isAutoAccept,
                };

                rewardsDataList.Add(data);
            }
            
            _rewardsDataList = rewardsDataList;
        }

        private void ChangeTranslations(Language lang)
        {
            SetRewardDataList(_rewardsList);
            RefreshRewardsMap();
        }

        public void SetRewardsList(
            List<PlayerReward> playerRewards,
            List<RewardToIncrement> notSentGivenRewards,
            List<RewardToIncrement> notSentAcceptedRewards)
        {
            
            var notSentAcceptedLeft = new List<RewardToIncrement>(notSentAcceptedRewards);

            var notSentRewards = notSentGivenRewards
                .Aggregate(new List<PlayerReward>(), (list, rewardIncrement) =>
                {
                    var playerReward = playerRewards.FirstOrDefault(r => r.rewardId == rewardIncrement.id);
                    if (playerReward != null)
                    {
                        playerReward.countTotal += rewardIncrement.count;
                        return list;
                    }

                    var reward = _rewardsList.FirstOrDefault(r => r.id == rewardIncrement.id);
                    if (reward != null)
                    {
                        list.Add(new PlayerReward
                        {
                            rewardId = rewardIncrement.id,
                            countTotal = rewardIncrement.count,
                            countAccepted = 0
                        });
                    }

                    return list;
                });

            var allPlayerRewards = playerRewards.Concat(notSentRewards).ToList();

            _playerRewardsList = allPlayerRewards
                .Aggregate(new List<PlayerReward>(), (list, pr) =>
                {
                    var reward = _rewardsList.FirstOrDefault(r => r.id == pr.rewardId);
                    if (reward != null)
                    {
                        var notSentAccepted = notSentAcceptedLeft.FirstOrDefault(r => r.id == pr.rewardId);
                        if (notSentAccepted != null)
                        {
                            notSentAcceptedLeft.RemoveAll(r => r.id == pr.rewardId);
                            MakeRewardAccepted(pr, notSentAccepted.count);
                        }

                        list.Add(pr);
                    }

                    return list;
                });

            RefreshPlayerRewardsMap();
        }

        
        
        private async void AcceptOnStart()
        {
            foreach (var playerReward in _playerRewardsList)
            {
                var reward = GetRewardData(playerReward.rewardId);
                if (reward?.isAutoAccept == true)
                {
                    int countLeft = playerReward.countTotal - playerReward.countAccepted;
                    if (countLeft > 0)
                    {
                        for (int i = 0; i < countLeft; i++)
                        {
                            await Accept(reward.id.ToString());
                        }
                    }
                }
            }
        }
        
        public async Task Give(string input, bool lazy = false)
        {
            void HandleError(string reason)
            {
                Logger.Error(reason);
                OnRewardsGiveError?.Invoke(reason);
            }
            
            var reward = GetRewardInfo(input).reward;
            
            if (reward == null)
            {
                HandleError(RewardNotFoundError);
                return;
            }
            
            if (int.TryParse(input, out var id) && _notFoundList.Contains(id))
            {
                HandleError(RewardNotFoundError);
                return;
            }
            
            int rewardID = reward.id;
        
        
            if (lazy)
            {
                var playerReward = new PlayerReward
                {
                    rewardId = reward.id,
                    countTotal = 1,
                    countAccepted = 0
                };
        
                AddReward(reward.id);
                
                CoreSDK.Player.AddGivenReward(new RewardToIncrement { id = reward.id, count = 1 });
                
                var updatedPlayerReward = GetPlayerRewardById(reward.id);
                playerReward.countTotal = updatedPlayerReward.countTotal;
                playerReward.countAccepted = updatedPlayerReward.countAccepted;
        
                AllRewardData rewardInfo = new AllRewardData(reward, playerReward);
                OnRewardsGive?.Invoke(rewardInfo);
        
                if (reward.isAutoAccept)
                {
                    await Accept(input);
                }

                return;
            }
        
            try
            {
                var playerRewardResult =
                    await DataFetcher.Rewards.GiveReward(new GivePlayerRewardInput { id = rewardID });
                var playerReward = playerRewardResult.playerReward;
        
                AddReward(reward.id);
        
                var updatedPlayerReward = GetPlayerRewardById(reward.id);
                playerReward.countTotal = updatedPlayerReward.countTotal;
                playerReward.countAccepted = updatedPlayerReward.countAccepted;
        
                AllRewardData rewardInfo = new AllRewardData(reward, playerReward);
                OnRewardsGive?.Invoke(rewardInfo);
        
                if (reward.isAutoAccept)
                {
                    await Accept(input);
                }

            }
            catch (Exception ex)
            {
                if (ex.Message == RewardNotFoundError)
                {
                    _notFoundList.Add(rewardID);
                }
        
                HandleError(ex.Message);
            }
        }
        
        public async Task Accept(string input)
        {
            var rewardID = int.TryParse(input, out var id) ? id : GetRewardData(input).id;

            void HandleError(string reason)
            {
                Logger.Error(reason);
                OnRewardsAcceptError?.Invoke(reason);
            }

            if (_notFoundList.Contains(rewardID))
            {
                HandleError(RewardNotFoundError);
                return;
            }

            if (!HasUnaccepted(rewardID.ToString()))
            {
                HandleError(RewardAlreadyAcceptedError);
                return;
            }

            var rewardInfo = GetRewardInfo(rewardID.ToString());
            
            if (rewardInfo.playerReward == null)
            {
                HandleError(PlayerRewardNotFoundError);
                return;
            }

            AcceptReward(rewardInfo.playerReward);
            
            CoreSDK.Player.AddAcceptedReward(new RewardToIncrement { id = rewardInfo.playerReward.rewardId, count = 1 });
            OnRewardsAccept?.Invoke(rewardInfo);
        }

        #region Getters

        public List<RewardData> List() => _rewardsDataList;
        public List<PlayerReward> GivenList() => _playerRewardsList;
        public bool Has(string idOrTag) => GetRewardInfo(idOrTag).playerReward?.countTotal > 0;
        
        public bool HasAccepted(string idOrTag) => GetRewardInfo(idOrTag).playerReward?.countAccepted > 0;
        
        public bool HasUnaccepted(string idOrTag)
        {
            var info = GetRewardInfo(idOrTag);
            return info.playerReward != null && info.playerReward.countTotal > info.playerReward.countAccepted;
        }

        public AllRewardData GetReward(int id) => GetRewardInfo(id.ToString());
        public AllRewardData GetReward(string idOrTag) => GetRewardInfo(idOrTag);
        
        private RewardData GetRewardData(int id)
        {
            _rewardsMapID.TryGetValue(id, out var reward);
            return reward;
        }
        
        private RewardData GetRewardData(string tag)
        {
            _rewardsMapTag.TryGetValue(tag, out var reward);
            return reward;
        }

        private PlayerReward GetPlayerRewardById(int id)
        {
            _playerRewardsMap.TryGetValue(id, out var reward);
            return reward;
        }

        private AllRewardData GetRewardInfo(string idOrTag)
        {
            var info = new AllRewardData();
            var reward = new RewardData();
            if (int.TryParse(idOrTag, out var id))
            {
                reward = GetRewardData(id);
            }
            else
            {
                reward = GetRewardData(idOrTag);
            }
            
            if (reward == null) return info;
            
            info.reward = reward;
            var playerReward = GetPlayerRewardById(reward.id);
            info.playerReward = playerReward ?? new PlayerReward { rewardId = reward.id, countAccepted = 0, countTotal = 0 };

            return info;
        }

        #endregion
        

        private void AddReward(int id)
        {
            var reward = GetRewardData(id);
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
            var reward = GetRewardData(playerReward.rewardId);
            if (reward == null)
            {
                Logger.Error($"Reward {playerReward.rewardId} not found");
                return;
            }

            MakeRewardAccepted(playerReward);
            ApplyRewardMutations(reward);
        }
        
        private void MakeRewardAccepted(PlayerReward reward, int count = 1)
        {
            reward.countAccepted += count;
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

        #region Maps Refresh
        private void RefreshRewardsMap()
        {
            _rewardsMapID.Clear();
            _rewardsMapTag.Clear();

            foreach (var reward in _rewardsDataList)
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
        
        #endregion
    }
}
