using System;
using System.Collections.Generic;

namespace GamePush.Data
{
    [Serializable]
    public enum BannerType
    {
        PRELOADER,
        FULLSCREEN,
        REWARDED,
        STICKY
    }

    [Serializable]
    public enum AdServerType
    {
        YandexSimpleMonetization
    }

    [Serializable]
    public class AdTimestamp
    {
        public long timestamp;
        public int count;

        public AdTimestamp()
        {
            timestamp = 0;
            count = 0;
        }
    }

    [Serializable]
    public class BannerLimitInfo
    {
        public AdTimestamp hour;
        public AdTimestamp day;
        public AdTimestamp session;

        public BannerLimitInfo()
        {
            hour = new AdTimestamp();
            day = new AdTimestamp();
            session = new AdTimestamp();
        }
    }

    [Serializable]
    public class PlayerAdsInfo
    {
        public BannerLimitInfo PRELOADER;
        public BannerLimitInfo FULLSCREEN;
        public BannerLimitInfo REWARDED;
        public BannerLimitInfo STICKY;

        public PlayerAdsInfo()
        {
            PRELOADER = new BannerLimitInfo();
            FULLSCREEN = new BannerLimitInfo();
            REWARDED = new BannerLimitInfo();
            STICKY = new BannerLimitInfo();
        }

        public BannerLimitInfo GetBanner(BannerType type)
        {
            return type switch
            {
                BannerType.PRELOADER => PRELOADER,
                BannerType.FULLSCREEN => FULLSCREEN,
                BannerType.REWARDED => REWARDED,
                BannerType.STICKY => STICKY,
                _ => new BannerLimitInfo()
            };
        }

        public void SetBanner(BannerType type, BannerLimitInfo info)
        {
            _ = type switch
            {
                BannerType.PRELOADER => PRELOADER = info,
                BannerType.FULLSCREEN => FULLSCREEN = info,
                BannerType.REWARDED => REWARDED = info,
                BannerType.STICKY => STICKY = info,
                _ => throw new NotImplementedException(),
            };
        }
    }

    [Serializable]
    public class AdsConfig
    {
        public bool showCountdownOverlay;
        public bool showRewardedFailedOverlay;
    }

    [System.Serializable]
    public class AdBanner
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
        public BannerLimits limits;
        public bool useNative;
    }

    [System.Serializable]
    public class BannerLimits
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
        public AdsConfigs configs;
    }

    [System.Serializable]
    public class AdsConfigs
    {
        public BannersList web;
        public BannersList android;
    }

    [System.Serializable]
    public class BannersList
    {
        public List<AdBanner> banners;
    }

}