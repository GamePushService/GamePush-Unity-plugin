using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public enum Rare
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY,
        MYTHIC
    }

    [System.Serializable]
    public class AchievementData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public string lockedIcon;
        public string lockedIconSmall;
        public string rare;
        public int maxProgress;
        public int progressStep;
        public bool lockedVisible;
        public bool lockedDescriptionVisible;
    }

    [System.Serializable]
    public class AchievementsGroupData
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public int[] achievements;
    }

    [System.Serializable]
    public class PlayerAchievement
    {
        public int achievementId;
        public string createdAt;
        public int progress;
        public bool unlocked;
    }
}

namespace GamePush.Data
{
    [System.Serializable]
    public class Achievement
    {
        public int id;
        public string icon;
        public string tag;
        public string rare;
        public int progress;
        public int maxProgress;
        public bool unlocked;
        public string lockedIcon;
        public int progressStep;
        public bool isPublished;
        public bool isLockedVisible;
        public bool isLockedDescriptionVisible;
        public Translations names;
        public Translations descriptions;
    }

    [System.Serializable]
    public class AchievementsGroup
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public List<int> achievements;
        public Translations names;
        public Translations descriptions;
    }

    [System.Serializable]
    public class AchievementsSettings
    {
        public bool isLockedVisible;
        public bool isLockedDescriptionVisible;
        public bool enableUnlockToast;
    }

    public class UnlockPlayerAchievementInput
    {
        public int? Id { get; set; }
        public string Tag { get; set; }
    }

    public class PlayerSetAchievementProgressInput
    {
        public int? Id { get; set; }
        public string Tag { get; set; }
        public int Progress { get; set; }
    }

    public class UnlockPlayerAchievementOutput
    {
        public bool Success { get; set; }
        public Achievement Achievement { get; set; }
        public string Error { get; set; }
    }

    public class FetchPlayerAchievementsOutput
    {
        public List<Achievement> Achievements { get; set; }
        public List<AchievementsGroup> AchievementsGroups { get; set; }
        public List<PlayerAchievement> PlayerAchievements { get; set; }
    }

    public class PlayerAchievementInfo
    {
        public Achievement Achievement { get; set; }
        public PlayerAchievement PlayerAchievement { get; set; }
    }
}