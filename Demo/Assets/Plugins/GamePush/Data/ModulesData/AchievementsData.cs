using UnityEngine;

namespace GamePush
{
    [System.Serializable]
    public class AchievementsFetch
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
    public class AchievementsFetchGroups
    {
        public int id;
        public string tag;
        public string name;
        public string description;
        public int[] achievements;
    }

    [System.Serializable]
    public class AchievementsFetchPlayer
    {
        public int achievementId;
        public string createdAt;
        public int progress;
        public bool unlocked;
    }
}