using System.Collections.Generic;

namespace GamePush.Data
{
    public class ConfigServices
    {
        public const string Xsolla = "XSOLLA";
    }

    [System.Serializable]
    public class AllConfigData
    {
        public bool isDev;
        public bool isAllowedOrigin;
        public Config config;
        public Project project;
        public PlatformConfig platformConfig;
        public List<PlayerField> playerFields;
        public List<GameVariable> gameVariables;
        public List<Product> products;
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
        public AdsConfig ads;
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
        public RootData data;
        public List<PlayerField> playerFields;
        public string serverTime;
        public List<GameVariable> gameVariables;
        public List<PlayerReward> rewards;
        public List<Trigger> triggers;
        public List<Achievement> achievements;
        public List<AchievementsGroup> achievementsGroups;
    }

    [System.Serializable]
    public class RootData
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
        public string progressSaveFormat;
        public bool alwaysSyncPublicFields;
        public Translations communityLinks;
        public List<AdBanner> banners;
        public string customAdsConfigId;
        public CustomAdsConfig customAdsConfig;
        public string paymentsConfigId;
        public PaymentsConfig paymentsConfig;
        public string authConfigId;
        public AuthConfig authConfig;
    }

    [System.Serializable]
    public class PaymentsConfig
    {
        public string id;
        public string name;
        public string description;
        public bool sandbox;
        public PlatfromServiceConfigs configs;
    }

    [System.Serializable]
    public class AuthConfig
    {
        public string id;
        public string name;
        public string description;
        public XsollaAuthConfig xsollaConfig;
        public PlatfromServiceConfigs configs;
    }

    [System.Serializable]
    public class XsollaAuthConfig
    {
        public string loginProjectId;
        public string jwtSecretKey;
    }

    [System.Serializable]
    public class PlatfromServiceConfigs
    {
        public PlatfromServiceConfig web;
        public PlatfromServiceConfig android;
    }

    [System.Serializable]
    public class PlatfromServiceConfig
    {
        public string activeService;
    }


    [System.Serializable]
    public class GameVariable
    {
        public string key;
        public string value;
        public string type;
    }

    [System.Serializable]
    public class PlayerReward
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