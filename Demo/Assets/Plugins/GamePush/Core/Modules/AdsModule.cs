using System;
using System.Collections.Generic;
using UnityEngine;

using GamePush.Data;
using GamePush.Mobile;

namespace GamePush.Core
{
    public class AdsModule
    {
        public event Action OnAdsStart;
        public event Action<bool> OnAdsClose;
        
        private AdsMobile adsMobile;

        private AdsConfig adsConfig;
        private List<AdBanner> banners;
        private CustomAdsConfig customAds;

        private PlayerAdsInfo adsInfo;

        public AdsModule()
        {
            adsMobile = new AdsMobile();
        }

        public void Init(AdsConfig config, PlatformConfig platformConfig)
        {
            adsConfig = config;
            banners = platformConfig.banners;
            customAds = platformConfig.customAdsConfig;

            adsMobile = new AdsMobile();
            adsMobile.Init(customAds.configs.android);

            adsInfo = new PlayerAdsInfo();
        }

        private AdBanner GetBanner(BannerType type)
        {
            foreach(AdBanner banner in banners)
            {
                if (banner.type == type.ToString())
                    return banner;
            }
            return null;
        }

        #region FullScreen Ads

        public event Action OnFullscreenStart;
        public event Action<bool> OnFullscreenClose;

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnFullscreenStart?.Invoke();
                onFullscreenStart?.Invoke();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnFullscreenClose?.Invoke(success); 
                onFullscreenClose?.Invoke(success); 
            };

            if (!IsFullscreenAvailable())
            {
                combinedClose.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            adsMobile.ShowFullscreen(combinedStart, combinedClose);
#else
            Logger.Log("FULL SCREEN AD ", "SHOW");
            onFullscreenStart?.Invoke();
            onFullscreenClose?.Invoke(true);
#endif
        }

        #endregion

        #region Rewarded Ads

        public event Action OnRewardedStart;
        public event Action<bool> OnRewardedClose;
        public event Action<string> OnRewardedReward;

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnRewardedStart?.Invoke();
                onRewardedStart?.Invoke();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnRewardedClose?.Invoke(success);
                onRewardedClose?.Invoke(success);
            };

            Action<string> combinedReward = (string tag) =>
            {
                OnRewardedReward?.Invoke(tag);
                onRewardedReward?.Invoke(tag);
            };

            if (!IsRewardedAvailable())
            {
                combinedClose.Invoke(false);
                return;
            }


#if UNITY_ANDROID
            adsMobile.ShowRewarded(idOrTag, combinedReward, combinedStart, combinedClose);
#else
            Logger.Log("SHOW REWARDED AD -> TAG: ", idOrTag);

            OnRewardedReward?.Invoke(idOrTag);
            _onRewardedReward?.Invoke(idOrTag);
#endif
        }

        #endregion

        #region Sticky Ads

        public event Action OnStickyStart;
        public event Action<bool> OnStickyClose;
        public event Action OnStickyRefresh;
        //public event Action OnStickyRender;

        public void ShowSticky()
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnStickyStart?.Invoke();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnStickyClose?.Invoke(success);
            };

            if (!IsStickyAvailable())
            {
                combinedClose.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            adsMobile.ShowSticky(combinedStart, combinedClose, OnStickyRefresh);
#else
            Logger.Log("STICKY BANNER AD: ", "SHOW");
#endif

        }

        public void CloseSticky()
        {
#if UNITY_ANDROID
            adsMobile.CloseSticky();
#else
            Logger.Log("STICKY BANNER AD: ", "CLOSE");
#endif
        }

        public void RefreshSticky()
        {
#if UNITY_ANDROID
            adsMobile.RefreshSticky();
#else
            Logger.Log("STICKY BANNER AD: ", "REFRESH");
#endif
        }

        #endregion

        #region Preload Ads

        public event Action OnPreloaderStart;
        public event Action<bool> OnPreloaderClose;


        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnPreloaderStart?.Invoke();
                onPreloaderStart?.Invoke();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnPreloaderClose?.Invoke(success);
                onPreloaderClose?.Invoke(success);
            };

            if (!IsPreloaderAvailable())
            {
                combinedClose.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            adsMobile.ShowPreloader(combinedStart, combinedClose);
#else
            Logger.Log("PRELOADER AD: ", "SHOW");
#endif
        }

        #endregion

        #region Is Methods

        public bool IsAdblockEnabled()
        {
#if UNITY_ANDROID
            return adsMobile.IsAdblockEnabled();
#else
            Logger.Log("IS ADBLOCK ENABLED: ", "FALSE");
            return false;
#endif
        }

        public bool IsStickyAvailable()
        {
            return
            //this.adapter.isStickyAvailable &&
            GetBanner(BannerType.STICKY).enabled &&
            !IsBannerLimitReached(BannerType.STICKY);

//#if UNITY_ANDROID
//            return adsMobile.IsStickyAvailable();
//#else
       
//            Logger.Log("IS STICKY BANNER AD AVAILABLE: ", "TRUE");
//            return false;
//#endif
        }

        public bool IsFullscreenAvailable()
        {
            return
            !IsFullscreenPlaying() &&
            !IsRewardedPlaying() &&
            !IsPreloaderPlaying() &&
            //this.adapter.isFullscreenAvailable &&
            GetBanner(BannerType.FULLSCREEN).enabled &&
            //!this.fullscreenDisplayIntervalId &&
            !IsBannerLimitReached(BannerType.FULLSCREEN);

//#if UNITY_ANDROID
//            return adsMobile.IsFullscreenAvailable();
//#else
//            Logger.Log("IS FULL SCREEN AD AVAILABLE: ", "TRUE");
//            return false;
//#endif
        }

        public bool IsRewardedAvailable()
        {
            return
            !IsFullscreenPlaying() &&
            !IsRewardedPlaying() &&
            !IsPreloaderPlaying() &&
            //this.adapter.isRewardedAvailable &&
            GetBanner(BannerType.REWARDED).enabled &&
            !IsBannerLimitReached(BannerType.REWARDED);

//#if UNITY_ANDROID
//            return adsMobile.IsRewardedAvailable();
//#else
//            Logger.Log("IS REWARD AD AVAILABLE: ", "TRUE");
//            return false;
//#endif
        }

        public bool IsPreloaderAvailable()
        {
            return
            !IsFullscreenPlaying() &&
            !IsRewardedPlaying() &&
            !IsPreloaderPlaying() &&
            //this.adapter.isPreloaderAvailable &&
            GetBanner(BannerType.PRELOADER).enabled &&
            !IsBannerLimitReached(BannerType.PRELOADER);
        
            //#if UNITY_ANDROID
            //            return adsMobile.IsPreloaderAvailable();
            //#else
            //            Logger.Log("IS PRELOADER AD AVAILABLE: ", "TRUE");
            //            return false;
            //#endif
        }

        public bool IsStickyPlaying()
        {
#if UNITY_ANDROID
            return adsMobile.IsStickyPlaying();
#else
            Logger.Log("IS STICKY PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsFullscreenPlaying()
        {
#if UNITY_ANDROID
            return adsMobile.IsFullscreenPlaying();
#else
            Logger.Log("IS FULLSCREEN AD PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsRewardedPlaying()
        {
#if UNITY_ANDROID
            return adsMobile.IsRewardPlaying();
#else
            Logger.Log("IS REWARDED AD PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsPreloaderPlaying()
        {
#if UNITY_ANDROID
            return adsMobile.IsPreloaderPlaying();
#else
            Logger.Log("IS PRELOADER AD PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsCountdownOverlayEnabled()
        {
            return adsConfig.showCountdownOverlay;
        }

        public bool IsRewardedFailedOverlayEnabled()
        {
            return adsConfig.showRewardedFailedOverlay;
        }

        public bool CanShowFullscreenBeforeGamePlay()
        {
            return false;
        }

        private bool IsBannerLimitReached(BannerType bannerType)
        {
            AdBanner banner = GetBanner(bannerType);
            bool hourLimits = false;
            bool dayLimits = false;
            bool sessionLimits = false;

            if (banner.limits.hour > 0)
                hourLimits = adsInfo.limits[bannerType].hour.count >= banner.limits.hour;
            if (banner.limits.day > 0)
                dayLimits = adsInfo.limits[bannerType].day.count >= banner.limits.day;
            if (banner.limits.session > 0)
                sessionLimits = adsInfo.limits[bannerType].session.count >= banner.limits.session;

            return hourLimits || dayLimits || sessionLimits;
        }

        private void TrackBannerDisplay(BannerType bannerType)
        {
            BannerLimitInfo limits = adsInfo.limits[bannerType];
            limits.hour.count += 1;
            limits.day.count += 1;
            limits.session.count += 1;

            if (limits.day.timestamp == null || limits.day.timestamp == "")
            {
                limits.day.timestamp = DateTime.Now.ToString();
            }
            if (limits.hour.timestamp == null || limits.hour.timestamp == "")
            {
                limits.hour.timestamp = DateTime.Now.ToString();
            }

            SaveAdsInfo(adsInfo);
        }

        public PlayerAdsInfo LoadAdsInfo()
        {
            try
            {
                var json = PlayerPrefs.GetString("adsInfo", null);
                var info = string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<PlayerAdsInfo>(json);
                //var result = MergeAdsInfo(_adsInfoStub, info);
                //_tempAdsInfo = result;
                //return result;
                return info;
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Warning: {ex.Message}");
                //_tempAdsInfo = _adsInfoStub;
                return new PlayerAdsInfo();
            }
        }

        public void SaveAdsInfo(PlayerAdsInfo info)
        {
            try
            {
                var json = JsonUtility.ToJson(info);
                PlayerPrefs.SetString("adsInfo", json);
                PlayerPrefs.Save();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Warning: {ex.Message}");
            }
        }

        

        #endregion
    }
}
