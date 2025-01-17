using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush.Data;
using GamePush.Tools;

namespace GamePush.Core
{
    public class AchievementsModule
    {
        public event Action OnAchievementsOpen;
        public event Action OnAchievementsClose;

        public event Action<List<AchievementData>> OnAchievementsFetch;
        public event Action OnAchievementsFetchError;

        public event Action<List<AchievementsGroupData>> OnAchievementsFetchGroups;
        public event Action<List<PlayerAchievement>> OnAchievementsFetchPlayer;

        public event Action<string> OnAchievementsUnlock;
        public event Action<string> OnAchievementsUnlockError;

        public event Action<string> OnAchievementsSetProgress;
        public event Action<string> OnAchievementsSetProgressError;

        public event Action<Achievement> OnShowAcievementUnlock;
        public event Action<Achievement> OnShowAcievementProgress;

        public event Action<FetchPlayerAchievementsOutput, AchievementsSettings, Action, Action> OnShowAcievementsList;

        private AchievementsSettings _settings;
        //private SyncManager _syncManager;
        private List<Achievement> _achievements = new();
        private List<AchievementsGroup> _achievementsGroups = new();
        private List<PlayerAchievement> _playerAchievements = new();
        private Dictionary<string, int> _setProgressTimeouts = new();

        private Dictionary<int, Achievement> _achievementsMapID = new();
        private Dictionary<string, Achievement> _achievementsMapTag = new();
        private Dictionary<int, PlayerAchievement> _playerAchievementsMap = new();
        private Dictionary<int, AchievementsGroup> _achievementsGroupsMap = new();
        private HashSet<int> _alreadyUnlocked = new();

        private const string AlreadyUnlockedError = "already_unlocked";
        private const string ProgressTheSameError = "progress_the_same";
        private const string AchievementNotFoundError = "achievement_not_found";

        public void Init(AllConfigData config)
        {
            _settings = config.project.achievements;

            string langKey = CoreSDK.currentLang.ToLower();

            foreach (var a in config.achievements)
            {
                a.name = LanguageTypes.GetTranslation(langKey, a.names) != "" ? LanguageTypes.GetTranslation(langKey, a.names) : LanguageTypes.GetTranslation(LanguageTypes.English, a.names);
                a.description = LanguageTypes.GetTranslation(langKey, a.descriptions) != "" ? LanguageTypes.GetTranslation(langKey, a.descriptions) : LanguageTypes.GetTranslation(LanguageTypes.English, a.descriptions);

                a.icon = UtilityImage.ResizeImage(a.icon, 256, 256, false);
                a.iconSmall = UtilityImage.ResizeImage(a.icon, 48, 48, false);

                a.lockedIcon = UtilityImage.ResizeImage(a.lockedIcon, 256, 256, false);
                a.lockedIconSmall = UtilityImage.ResizeImage(a.lockedIcon, 48, 48, false);
            }

            _achievements = new List<Achievement>(config.achievements);
            _achievementsGroups = new List<AchievementsGroup>(config.achievementsGroups);
            RefreshAchievementsMap();
            RefreshAchievementsGroupsMap();

            CoreSDK.Language.OnChangeLanguage += RenameOnLanguageChange;
        }

        public void UnlockAchievements(IEnumerable<int> ids)
        {
            foreach (var id in ids)
            {
                var achievementInfo = GetAchievementInfo(id);
                if (achievementInfo.Achievement == null)
                {
                    Logger.Error($"Achievement not found, ID: {id}");
                    continue;
                }

                if (achievementInfo.PlayerAchievement != null)
                {
                    Logger.Error($"Player achievement already unlocked, ID: {id}, Tag: {achievementInfo.Achievement.tag}");
                    continue;
                }

                var newUnlockedPlayerAchievement = new PlayerAchievement
                {
                    achievementId = achievementInfo.Achievement.id,
                    unlocked = true,
                    progress = achievementInfo.Achievement.maxProgress,
                    createdAt = DateTime.UtcNow.ToString("o") // ISO 8601 формат
                };

                var mergedAchievement = MergeAchievement(achievementInfo.Achievement, newUnlockedPlayerAchievement);

                UpsertInPlayersList(mergedAchievement);

                if (_settings.enableUnlockToast)
                {
                    
                }
            }
        }

        private void RefreshAchievementsMap()
        {
            _achievementsMapID.Clear();
            _achievementsMapTag.Clear();
            foreach (var achievement in _achievements)
            {
                _achievementsMapID[achievement.id] = achievement;
                _achievementsMapTag[achievement.tag] = achievement;
            }
        }

        private void RefreshAchievementsGroupsMap()
        {
            _achievementsGroupsMap.Clear();
            foreach (var ag in _achievementsGroups)
            {
                _achievementsGroupsMap[ag.id] = ag;
            }
        }

        private void RenameOnLanguageChange(Language language)
        {
            foreach (var achievement in _achievements)
            {
                string name = LanguageTypes.GetTranslation(language, achievement.names);
                achievement.name = name != "" ? name : achievement.names.en;
                string description = LanguageTypes.GetTranslation(language, achievement.descriptions);
                achievement.description = description != "" ? description : achievement.descriptions.en;
            }
            foreach (var group in _achievementsGroups)
            {
                string name = LanguageTypes.GetTranslation(language, group.names);
                group.name = name != "" ? name : group.names.en;
                string description = LanguageTypes.GetTranslation(language, group.descriptions);
                group.description = description != "" ? description : group.descriptions.en;
            }
            RefreshAchievementsMap();
        }

        public void SetAchievementsList(List<PlayerAchievement> achievements)
        {
            Logger.Log("Set players achievements " + achievements.Count);
            _playerAchievements = new List<PlayerAchievement>(achievements);
            RefreshPlayerAchievementsMap();
        }

        private void UpsertInPlayersList(PlayerAchievement achievement)
        {
            var index = _playerAchievements.FindIndex(a => a.achievementId == achievement.achievementId);
            if (index >= 0)
            {
                _playerAchievements[index] = achievement;
            }
            else
            {
                _playerAchievements.Add(achievement);
            }

            RefreshPlayerAchievementsMap();
        }

        private void RefreshPlayerAchievementsMap()
        {
            _playerAchievementsMap.Clear();
            foreach (var pa in _playerAchievements)
            {
                _playerAchievementsMap[pa.achievementId] = pa;
            }
        }

        private PlayerAchievementInfo GetAchievementInfo(object idOrTag)
        {
            var info = new PlayerAchievementInfo();

            if (int.TryParse(idOrTag.ToString(), out int id) && _achievementsMapID.TryGetValue(id, out var achievement))
            {
                info.Achievement = achievement;
            }
            else if (idOrTag is string tag && _achievementsMapTag.TryGetValue(tag, out achievement))
            {
                info.Achievement = achievement;
            }

            if (info.Achievement != null && _playerAchievementsMap.TryGetValue(info.Achievement.id, out var playerAchievement))
            {
                info.PlayerAchievement = playerAchievement;
            }

            return info;
        }

        private static PlayerAchievement MergeAchievement(Achievement achievement, PlayerAchievement playerAchievement)
        {
            return new PlayerAchievement
            {
                achievementId = achievement.id,
                unlocked = playerAchievement.unlocked,
                progress = playerAchievement.progress,
                createdAt = playerAchievement.createdAt
            };
        }

        public void Open()
        {
            FetchPlayerAchievementsOutput info = new FetchPlayerAchievementsOutput();

            info.Achievements = _achievements;
            info.AchievementsGroups = _achievementsGroups;
            info.PlayerAchievements = _playerAchievements;

            OnShowAcievementsList?.Invoke(info, _settings, OnAchievementsOpen, OnAchievementsClose);
        }

        public void Fetch()
        {
            List<AchievementData> achievementDatas = new List<AchievementData>();
            List<AchievementsGroupData> achievementsGroupDatas = new List<AchievementsGroupData>();

            foreach(Achievement achievement in _achievements)
            {
                AchievementData data = achievement.ToAchievementData();
                achievementDatas.Add(data);
            }

            foreach (AchievementsGroup group in _achievementsGroups)
            {
                AchievementsGroupData data = group.ToAchievementsGruopData();
                achievementsGroupDatas.Add(data);
            }

            OnAchievementsFetch?.Invoke(achievementDatas);
            OnAchievementsFetchGroups?.Invoke(achievementsGroupDatas);
            OnAchievementsFetchPlayer?.Invoke(_playerAchievements);
        }

        public bool Has(string idOrTag)
        {
            PlayerAchievementInfo info = GetAchievementInfo(idOrTag);
            if (info.PlayerAchievement != null)
                return info.PlayerAchievement.unlocked;
            else if (info.Achievement != null)
            {
                return false;
            }
            else
            {
                Logger.Error(AchievementNotFoundError);
                return false;
            }
        }

        public async void Unlock(string idOrTag)
        {
            var info = GetAchievementInfo(idOrTag);

            if (info.Achievement == null)
            {
                OnAchievementsUnlockError?.Invoke(AchievementNotFoundError);
                return;
            }

            if (info.PlayerAchievement?.unlocked == true)
            {
                OnAchievementsUnlockError?.Invoke(AlreadyUnlockedError);
                return;
            }

            if (_alreadyUnlocked.Contains(info.Achievement.id))
            {
                OnAchievementsUnlockError?.Invoke(AlreadyUnlockedError);
                return;
            }

            UnlockPlayerAchievementInput input = new UnlockPlayerAchievementInput();
            input.id = info.Achievement.id;
            input.tag = info.Achievement.tag;


            var unlockResult = await DataFetcher.UnlockAchievement(input);

            var mergedAchievement = new Achievement
            {
                id = unlockResult.id,
                unlocked = true,
                progress = unlockResult.maxProgress,
                icon = info.Achievement.icon,
                iconSmall = info.Achievement.iconSmall,
                name = info.Achievement.name,
                description = info.Achievement.description,
                rare = info.Achievement.rare,
            };

            var playerAchievement = new PlayerAchievement
            {
                achievementId = unlockResult.id,
                unlocked = true,
                progress = unlockResult.maxProgress,
            };

            UpsertInPlayersList(playerAchievement);

            if (_settings.enableUnlockToast)
            {
                OnShowAcievementUnlock?.Invoke(mergedAchievement);
            }

            OnAchievementsUnlock?.Invoke(mergedAchievement.id.ToString());
        }

        public async void SetProgress(string idOrTag, int progress)
        {
            var info = GetAchievementInfo(idOrTag);

            if (info.Achievement == null)
            {
                OnAchievementsSetProgressError?.Invoke(AchievementNotFoundError);
                return;
            }

            if (info.PlayerAchievement?.unlocked == true)
            {
                OnAchievementsSetProgressError?.Invoke(AlreadyUnlockedError);
                return;
            }

            if (_alreadyUnlocked.Contains(info.Achievement.id))
            {
                OnAchievementsSetProgressError?.Invoke(AlreadyUnlockedError);
                return;
            }

            var prevProgress = info.PlayerAchievement?.progress ?? 0;
            if (prevProgress == progress)
            {
                OnAchievementsSetProgressError?.Invoke(ProgressTheSameError);
                return;
            }

            PlayerSetAchievementProgressInput input = new PlayerSetAchievementProgressInput();
            input.id = info.Achievement.id;
            input.tag = info.Achievement.tag;
            input.progress = progress;


            var result = await DataFetcher.SetAchievemntProgress(input);
            var mergedAchievement = info.Achievement;

            mergedAchievement.id = result.achievementId;
            mergedAchievement.tag = info.Achievement.tag;
            mergedAchievement.unlocked = result.unlocked;
            mergedAchievement.progress = result.progress;
            mergedAchievement.progressStep = info.Achievement.progressStep;
            mergedAchievement.name = info.Achievement.name;
            mergedAchievement.description = info.Achievement.description;

            var prevStep = prevProgress / mergedAchievement.progressStep;
            var currentStep = mergedAchievement.progress / mergedAchievement.progressStep;

            UpsertInPlayersList(result);

            if (_settings.enableUnlockToast && (mergedAchievement.unlocked || prevStep < currentStep))
            {
                OnShowAcievementProgress?.Invoke(mergedAchievement);
            }

            //string jsonAchievement = UtilityJSON.ToJson(mergedAchievement.ToAchievementData());

            if (mergedAchievement.unlocked)
            {
                OnAchievementsUnlock?.Invoke(mergedAchievement.id.ToString());
            }

            OnAchievementsSetProgress?.Invoke(mergedAchievement.id.ToString());
        }

        public int GetProgress(string idOrTag)
        {
            PlayerAchievementInfo info = GetAchievementInfo(idOrTag);

            if (info.Achievement == null)
                return 0;
            if (info.PlayerAchievement != null)
                return info.PlayerAchievement.progress;

            return 0;
        }
    }
}
