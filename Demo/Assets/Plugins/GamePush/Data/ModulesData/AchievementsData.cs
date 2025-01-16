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

    public enum Rare
    {
        COMMON,
        UNCOMMON,
        RARE,
        EPIC,
        LEGENDARY,
        MYTHIC
    }

    public static class RareColors
    {
        public const string COMMON = "FFFFFF";
        public const string UNCOMMON = "31C33B";
        public const string RARE = "3152C3";
        public const string EPIC = "BF32C8";
        public const string LEGENDARY = "CC992D";
        public const string MYTHIC = "C5CC2D";
    }

    public static class RareTypes
    {
        public const string COMMON = "COMMON";
        public const string UNCOMMON = "UNCOMMON";
        public const string RARE = "RARE";
        public const string EPIC = "EPIC";
        public const string LEGENDARY = "LEGENDARY";
        public const string MYTHIC = "MYTHIC";

        public static string GetColor(Rare rare)
        {
            return rare switch
            {
                Rare.COMMON => RareColors.COMMON,
                Rare.UNCOMMON => RareColors.UNCOMMON,
                Rare.RARE => RareColors.RARE,
                Rare.EPIC => RareColors.EPIC,
                Rare.LEGENDARY => RareColors.LEGENDARY,
                Rare.MYTHIC => RareColors.MYTHIC,
                _ => RareColors.COMMON
            };
        }

        public static string GetColor(string rare)
        {
            return rare switch
            {
                COMMON => RareColors.COMMON,
                UNCOMMON => RareColors.UNCOMMON,
                RARE => RareColors.RARE,
                EPIC => RareColors.EPIC,
                LEGENDARY => RareColors.LEGENDARY,
                MYTHIC => RareColors.MYTHIC,
                _ => RareColors.COMMON
            };
        }

    }
}

namespace GamePush.Data
{
    [System.Serializable]
    public class Achievement
    {
        public int id;
        public string name;
        public string description;
        public string icon;
        public string iconSmall;
        public string tag;
        public string rare;
        public int progress;
        public int maxProgress;
        public int progressStep;
        public bool unlocked;
        public string lockedIcon;
        public string lockedIconSmall;
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
        public int? id;
        public string tag;
    }

    public class PlayerSetAchievementProgressInput
    {
        public int? id;
        public string tag;
        public int progress;
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