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

        public event Action<string> OnAchievementsProgress;
        public event Action OnAchievementsProgressError;

        public event Action OnShowAcievementUnlock;
        public event Action OnShowAcievementProgress;

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

            _achievements = new List<Achievement>(config.achievements);
            _achievementsGroups = new List<AchievementsGroup>(config.achievementsGroups);
            RefreshAchievementsMap();
            RefreshAchievementsGroupsMap();

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
            if (idOrTag is int id && _achievementsMapID.TryGetValue(id, out var achievement))
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

        }

        public async void Unlock(string idOrTag)
        {
            var info = GetAchievementInfo(idOrTag);

            if (info.Achievement == null)
            {
                OnAchievementsUnlockError?.Invoke(AchievementNotFoundError);
            }

            if (info.PlayerAchievement?.unlocked == true)
            {
                OnAchievementsUnlockError?.Invoke(AlreadyUnlockedError);
            }

            if (_alreadyUnlocked.Contains(info.Achievement.id))
            {
                OnAchievementsUnlockError?.Invoke(AlreadyUnlockedError);
            }
            UnlockPlayerAchievementInput input = new UnlockPlayerAchievementInput();
            input.id = info.Achievement.id;

            var unlockResult = await DataFetcher.UnlockAchievement(input);
            //var mergedAchievement = new Achievement
            //{
            //    id = unlockResult.Achievement.Id,
            //    tag = unlockResult.Achievement.Tag,
            //    unlocked = true,
            //    progress = unlockResult.PlayerAchievement.Progress,
            //};

            UpsertInPlayersList(unlockResult);

            if (_settings.enableUnlockToast)
            {
                //await Task.WhenAll(overlayPromise, PreloadImage(GetAchievementIcon(mergedAchievement)));
                //gp.Overlay.UnlockAchievement(mergedAchievement);
            }
        }

        public bool Has(string idOrTag)
        {
            return GetAchievementInfo(idOrTag).PlayerAchievement.unlocked;
        }

        public void SetProgress(string idOrTag, int progress)
        {
            
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
