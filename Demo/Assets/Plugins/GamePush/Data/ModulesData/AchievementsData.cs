using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
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
}