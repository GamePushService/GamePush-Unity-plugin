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

    [System.Serializable]
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