using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush.Data;

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

        public event Action OnShowAcievementsList;

        private AchievementsSettings _settings;
        //private SyncManager _syncManager;
        private List<Achievement> _achievements = new();
        private List<AchievementsGroup> _achievementsGroups = new();
        private List<PlayerAchievement> _playerAchievements = new();
        //private Dictionary<string, Transaction<UnlockPlayerAchievementOutput>> _unlockTransactions = new();
        private Dictionary<string, int> _setProgressTimeouts = new();
        //private Dictionary<string, Transaction<UnlockPlayerAchievementOutput>> _setProgressTransactions = new();

        private Dictionary<int, Achievement> _achievementsMapID = new();
        private Dictionary<string, Achievement> _achievementsMapTag = new();
        private Dictionary<int, PlayerAchievement> _playerAchievementsMap = new();
        private Dictionary<int, AchievementsGroup> _achievementsGroupsMap = new();
        private HashSet<int> _alreadyUnlocked = new();


        public void Init(AllConfigData config)
        {
            _settings = config.project.achievements;
            _achievements = config.achievements;
            _achievementsGroups = config.achievementsGroups;
            string langKey = CoreSDK.currentLang.ToLower();

            foreach (var a in config.achievements)
            {
                AchievementData achievement = new AchievementData();
                achievement.name = LanguageTypes.GetTranslation(langKey, a.names) != "" ? LanguageTypes.GetTranslation(langKey, a.names) : LanguageTypes.GetTranslation(LanguageTypes.English, a.names);
                achievement.description = LanguageTypes.GetTranslation(langKey, a.descriptions) != "" ? LanguageTypes.GetTranslation(langKey, a.descriptions) : LanguageTypes.GetTranslation(LanguageTypes.English, a.descriptions);
                //a.LockedIcon = ResizeImage(a.LockedIcon, 256, 256, false);
                //a.LockedIconSmall = ResizeImage(a.LockedIcon, 48, 48, false);
                //a.Icon = ResizeImage(a.Icon, 256, 256, false);
                //a.IconSmall = ResizeImage(a.Icon, 48, 48, false);
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

        private void SetAchievementsList(List<PlayerAchievement> achievements)
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

        private static async Task PreloadImage(string url)
        {
            try
            {
                // ????????? ???????? ???????????
                await Task.Delay(100);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
            }
        }

        //private static string GetAchievementIcon(Achievement achievement)
        //{
        //    if (DevicePixelRatio > 1)
        //    {
        //        return achievement.unlocked ? achievement.Icon ?? achievement.LockedIcon : achievement.LockedIcon ?? achievement.Icon;
        //    }
        //    else
        //    {
        //        return achievement.unlocked ? achievement.IconSmall ?? achievement.LockedIconSmall : achievement.LockedIconSmall ?? achievement.IconSmall;
        //    }
        //}

        public void Open(Action onOpen = null, Action onClose = null)
        {

        }

        public void Fetch()
        {

        }

        public void Unlock(string idOrTag)
        {

        }

        public bool Has(string idOrTag)
        {
            return false;
        }

        public void SetProgress(string idOrTag, int progress)
        {
            
        }

        public int GetProgress(string idOrTag)
        {
            return 0;
        }
    }
}
