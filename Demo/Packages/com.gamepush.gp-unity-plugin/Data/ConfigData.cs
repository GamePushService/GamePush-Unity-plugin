using System.Collections;
using System.Collections.Generic;

namespace GamePush.Data
{
    [System.Serializable]
    public class ConfigData
    {
        public DataWrap data;
    }

    [System.Serializable]
    public class DataWrap
    {
        public AllData result;
    }

    [System.Serializable]
    public class AllData
    {
        public bool isDev;
        public bool isAllowedOrigin;
        public Config config;
        public Project project;
        public PlatformConfig platformConfig;
        public List<PlayerField> playerFields;
        public List<GameVariable> gameVariables;
        public string serverTime;
    }

    [System.Serializable]
    public class Config
    {
        public string lang;
        public string avatarGenerator;
        public string avatarGeneratorTemplate;
        public int ymCounterId;
        public string gtagCounterId;
        public bool showLoader;
        public bool showReqCounter;
        public string orientation;
        public bool showOrientationOverlay;
        public List<string> targetOS;
        public bool showUnsupportedOSOverlay;
        public Translations communityLinks;
    }

    [System.Serializable]
    public class Project
    {
        public string name;
        public string description;
        public string icon;
        public int mainChatId;
        public bool enableMainChat;
        public Achievements achievements;
        public Ads ads;
    }

    [System.Serializable]
    public class Ads
    {
        public bool showCountdownOverlay;
        public bool showRewardedFailedOverlay;
        public List<Banner> banners;
        public string customAdsConfigId;
        public CustomAdsConfig customAdsConfig;
    }

    [System.Serializable]
    public class Banner
    {
        public string type;
        public bool enabled;
        public string bannerId;
        public string desktopBannerId;
        public object adServer;
        public object adServerDesktop;
        public int frequency;
        public int refreshInterval;
        public int maxWidth;
        public int maxHeight;
        public string maxWidthDimension;
        public string maxHeightDimension;
        public int desktopMaxWidth;
        public int desktopMaxHeight;
        public string desktopMaxWidthDimension;
        public string desktopMaxHeightDimension;
        public bool fitCanvas;
        public string position;
        public Limits limits;
        public bool useNative;
    }

    [System.Serializable]
    public class Limits
    {
        public int hour;
        public int day;
        public int session;
    }

    [System.Serializable]
    public class CustomAdsConfig
    {
        public string id;
        public string name;
        public string description;
        public string adFoxOwnerId;
        public Configs configs;
    }

    [System.Serializable]
    public class Configs
    {
        public Web web;
        public Android android;
    }

    [System.Serializable]
    public class Web
    {
        public List<Banner> banners;
    }

    [System.Serializable]
    public class Android
    {
        public List<Banner> banners;
    }

    [System.Serializable]
    public class Achievements
    {
        public bool isLockedVisible;
        public bool isLockedDescriptionVisible;
        public bool enableUnlockToast;
    }

    [System.Serializable]
    public class RootObject
    {
        public Data data;
        public List<PlayerField> playerFields;
        public string serverTime;
        public List<GameVariable> gameVariables;
        public List<Reward> rewards;
        public List<Trigger> triggers;
        public List<Achievement> achievements;
        public List<AchievementsGroup> achievementsGroups;
    }

    [System.Serializable]
    public class Data
    {
        public Result result;
        public Project project;
        public PlatformConfig platformConfig;
    }

    [System.Serializable]
    public class Result
    {
        public string __typename;
        public bool isDev;
        public bool isAllowedOrigin;
        public Config config;
    }

    [System.Serializable]
    public class PlatformConfig
    {
        public string type;
        public string tag;
        public string appId;
        public string gameLink;
        public Translations communityLinks;
        public List<Banner> banners;
    }

    [System.Serializable]
    public class PlayerField
    {
        public string name;
        public string key;
        public string type;
        public string @default;
        public bool important;
        public List<Variant> variants;
    }

    [System.Serializable]
    public class Variant
    {
        public string value;
        public string name;
    }

    [System.Serializable]
    public class GameVariable
    {
        public string key;
        public string value;
        public string type;
    }

    [System.Serializable]
    public class Reward
    {
        public int id;
        public string icon;
        public string tag;
        public bool isAutoAccept;
        public List<Mutation> mutations;
        public Translations names;
        public Translations descriptions;
    }

    [System.Serializable]
    public class Mutation
    {
        public string type;
        public string key;
        public string action;
        public string value;
    }

    [System.Serializable]
    public class Trigger
    {
        public string id;
        public string tag;
        public bool isAutoClaim;
        public string description;
        public Translations descriptions;
        public List<List<Condition>> conditions;
        public List<Bonus> bonuses;
    }

    [System.Serializable]
    public class Condition
    {
        public string type;
        public string key;
        public string @operator;
        public List<string> value;
    }

    [System.Serializable]
    public class Bonus
    {
        public int id;
        public string type;
    }

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
    public class Translations
    {
        public string en;
        public string fr;
        public string it;
        public string de;
        public string es;
        public string zh;
        public string pt;
        public string ko;
        public string ja;
        public string ru;
        public string tr;
        public string ar;
        public string id;
        public string hi;
    }
}
