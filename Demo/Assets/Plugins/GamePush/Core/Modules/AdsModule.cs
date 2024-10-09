using System;
using System.Collections.Generic;
using UnityEngine;

using GamePush.Mobile;

namespace GamePush.Core
{
    public class AdsModule
    {
        public event Action OnAdsStart;
        public event Action<bool> OnAdsClose;
        
        public AdsMobile adsMobile;

        public AdsModule()
        {
            adsMobile = new AdsMobile();
        }

        #region FullScreen Ads

        public event Action OnFullscreenStart;
        public event Action<bool> OnFullscreenClose;

        private event Action _onFullscreenStart;
        private event Action<bool> _onFullscreenClose;

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            _onFullscreenStart = onFullscreenStart;
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
             GP_Ads_ShowFullscreen();
#else
            Logger.Log("FULL SCREEN AD ", "SHOW");
#endif
        }

        #endregion

        #region Rewarded Ads

        public event Action OnRewardedStart;
        public event Action<bool> OnRewardedClose;
        public event Action<string> OnRewardedReward;

        private event Action<string> _onRewardedReward;
        private event Action _onRewardedStart;
        private event Action<bool> _onRewardedClose;

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            _onRewardedReward = onRewardedReward;
            _onRewardedStart = onRewardedStart;
            _onRewardedClose = onRewardedClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowRewarded(idOrTag);
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
            adsMobile.ShowBanner();
#else
            Logger.Log("STICKY BANNER AD: ", "SHOW");
#endif

        }

        public void CloseSticky()
        {
#if UNITY_ANDROID
            adsMobile.CloseBanner();
#else
            Logger.Log("STICKY BANNER AD: ", "CLOSE");
#endif
        }

        public void RefreshSticky()
        {
#if UNITY_ANDROID
            adsMobile.RefreshBanner();
#else
            Logger.Log("STICKY BANNER AD: ", "REFRESH");
#endif
        }

        #endregion

        #region Preload Ads

        public event Action OnPreloaderStart;
        public event Action<bool> OnPreloaderClose;

        private event Action _onPreloaderStart;
        private event Action<bool> _onPreloaderClose;

        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            _onPreloaderStart = onPreloaderStart;
            _onPreloaderClose = onPreloaderClose;
#if UNITY_ANDROID

#else
            Logger.Log("PRELOADER AD: ", "SHOW");
#endif
        }

        #endregion

        #region Is Methods

        public bool IsAdblockEnabled()
        {

            Logger.Log("IS ADBLOCK ENABLED: ", "FALSE");
            return false;

        }

        public bool IsStickyAvailable()
        {

            Logger.Log("IS STICKY BANNER AD AVAILABLE: ", "TRUE");
            return false;

        }

        public bool IsFullscreenAvailable()
        {

            Logger.Log("IS FULL SCREEN AD AVAILABLE: ", "TRUE");
            return false;

        }

        public static bool IsRewardedAvailable()
        {

            Logger.Log("IS REWARD AD AVAILABLE: ", "TRUE");
            return false;

        }

        public static bool IsPreloaderAvailable()
        {

            Logger.Log("IS PRELOADER AD AVAILABLE: ", "TRUE");
            return false;

        }

        public static bool IsStickyPlaying()
        {

            Logger.Log("IS STICKY PLAYING: ", "FALSE");
            return false;

        }

        public static bool IsFullscreenPlaying()
        {

            Logger.Log("IS FULLSCREEN AD PLAYING: ", "FALSE");
            return false;

        }

        public static bool IsRewardPlaying()
        {

            Logger.Log("IS REWARDED AD PLAYING: ", "FALSE");
            return false;

        }

        public static bool IsPreloaderPlaying()
        {

            Logger.Log("IS PRELOADER AD PLAYING: ", "FALSE");
            return false;

        }

        public static bool IsCountdownOverlayEnabled()
        {

            Logger.Log("Is Countdown Overlay Enabled: ", "FALSE");
            return false;

        }

        #endregion
    }
}
