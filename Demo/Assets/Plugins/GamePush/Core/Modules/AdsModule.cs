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
        }

        #region FullScreen Ads

        public event Action OnFullscreenStart;
        public event Action<bool> OnFullscreenClose;

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            Action combinedStart = () =>
            {
                OnFullscreenStart?.Invoke();
                onFullscreenStart?.Invoke();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnFullscreenClose?.Invoke(success); 
                onFullscreenClose?.Invoke(success); 
            };


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
                OnRewardedStart?.Invoke();
                onRewardedStart?.Invoke();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnRewardedClose?.Invoke(success);
                onRewardedClose?.Invoke(success);
            };

            Action<string> combinedReward = (string tag) =>
            {
                OnRewardedReward?.Invoke(tag);
                onRewardedReward?.Invoke(tag);
            };


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
        public event Action OnStickyClose;
        public event Action OnStickyRefresh;
        public event Action OnStickyRender;

        public void ShowSticky()
        {
#if UNITY_ANDROID
            adsMobile.ShowSticky();
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
                OnPreloaderStart?.Invoke();
                onPreloaderStart?.Invoke();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnPreloaderClose?.Invoke(success);
                onPreloaderClose?.Invoke(success);
            };

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
#if UNITY_ANDROID
            return adsMobile.IsStickyAvailable();
#else
       
            Logger.Log("IS STICKY BANNER AD AVAILABLE: ", "TRUE");
            return false;
#endif
        }

        public bool IsFullscreenAvailable()
        {
#if UNITY_ANDROID
            return adsMobile.IsFullscreenAvailable();
#else
            Logger.Log("IS FULL SCREEN AD AVAILABLE: ", "TRUE");
            return false;
#endif
        }

        public bool IsRewardedAvailable()
        {
#if UNITY_ANDROID
            return adsMobile.IsRewardedAvailable();
#else
            Logger.Log("IS REWARD AD AVAILABLE: ", "TRUE");
            return false;
#endif
        }

        public bool IsPreloaderAvailable()
        {
#if UNITY_ANDROID
            return adsMobile.IsPreloaderAvailable();
#else
            Logger.Log("IS PRELOADER AD AVAILABLE: ", "TRUE");
            return false;
#endif
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

        public bool IsRewardPlaying()
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

        #endregion
    }
}
