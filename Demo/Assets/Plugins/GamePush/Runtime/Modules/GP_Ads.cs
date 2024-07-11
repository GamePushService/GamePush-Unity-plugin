using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;


namespace GamePush
{
    public class GP_Ads : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Ads);

        public static event UnityAction OnAdsStart;
        public static event UnityAction<bool> OnAdsClose;
        public static event UnityAction OnFullscreenStart;
        public static event UnityAction<bool> OnFullscreenClose;
        public static event UnityAction OnPreloaderStart;
        public static event UnityAction<bool> OnPreloaderClose;
        public static event UnityAction OnRewardedStart;
        public static event UnityAction<bool> OnRewardedClose;
        public static event UnityAction<string> OnRewardedReward;
        public static event UnityAction OnStickyStart;
        public static event UnityAction OnStickyClose;
        public static event UnityAction OnStickyRefresh;
        public static event UnityAction OnStickyRender;

        private static event Action _onFullscreenStart;
        private static event Action<bool> _onFullscreenClose;

        private static event Action _onPreloaderStart;
        private static event Action<bool> _onPreloaderClose;

        private static event Action<string> _onRewardedReward;
        private static event Action _onRewardedStart;
        private static event Action<bool> _onRewardedClose;


        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowFullscreen();
        public static void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            _onFullscreenStart = onFullscreenStart;
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
             GP_Ads_ShowFullscreen();
#else
            ConsoleLog("FULL SCREEN AD: SHOW");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowRewarded(string idOrTag);
        public static void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            _onRewardedReward = onRewardedReward;
            _onRewardedStart = onRewardedStart;
            _onRewardedClose = onRewardedClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowRewarded(idOrTag);
#else
            ConsoleLog("SHOW REWARDED AD -> TAG: " + idOrTag);
            OnRewardedReward?.Invoke(idOrTag);
            _onRewardedReward?.Invoke(idOrTag);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowPreloader();
        public static void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            _onPreloaderStart = onPreloaderStart;
            _onPreloaderClose = onPreloaderClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowPreloader();
#else
            ConsoleLog("PRELOADER AD: SHOW");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowSticky();
        public static void ShowSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowSticky();
#else

            ConsoleLog("STICKY BANNER AD: SHOW");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_CloseSticky();
        public static void CloseSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_CloseSticky();
#else

            ConsoleLog("STICKY BANNER AD: CLOSE");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_RefreshSticky();
        public static void RefreshSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_RefreshSticky();
#else

            ConsoleLog("STICKY BANNER AD: REFRESH");
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsAdblockEnabled();
        public static bool IsAdblockEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsAdblockEnabled() == "true";
#else
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsAdblockEnabled;
            ConsoleLog("IS ADBLOCK ENABLED: " + isVal);
            return isVal;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyAvailable();
        public static bool IsStickyAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyAvailable() == "true";
#else
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsStickyAvailable;
            ConsoleLog("IS STICKY BANNER AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsFullscreenAvailable();
        public static bool IsFullscreenAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenAvailable() == "true";
#else
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsFullscreenAvailable;
            ConsoleLog("IS FULL SCREEN AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedAvailable();
        public static bool IsRewardedAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedAvailable() == "true";
#else
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsRewardedAvailable;
            ConsoleLog("IS REWARD AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsPreloaderAvailable();
        public static bool IsPreloaderAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderAvailable() == "true";
#else
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsPreloaderAvailable;
            ConsoleLog("IS PRELOADER AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyPlaying();
        public static bool IsStickyPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyPlaying() == "true";
#else

            ConsoleLog("IS STICKY PLAYING: FALSE");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsFullscreenPlaying();
        public static bool IsFullscreenPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenPlaying() == "true";
#else

            ConsoleLog("IS FULLSCREEN AD PLAYING: FALSE");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedPlaying();
        public static bool IsRewardPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedPlaying() == "true";
#else

            ConsoleLog("IS REWARDED AD PLAYING: FALSE");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsPreloaderPlaying();
        public static bool IsPreloaderPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderPlaying() == "true";
#else

            ConsoleLog("IS PRELOADER AD PLAYING: FALSE");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsCountdownOverlayEnabled();
        public static bool IsCountdownOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsCountdownOverlayEnabled() == "true";
#else

            ConsoleLog("Is Countdown Overlay Enabled: FALSE");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedFailedOverlayEnabled();
        public static bool IsRewardedFailedOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedFailedOverlayEnabled() == "true";
#else

            ConsoleLog("Is Rewarded Failed Overlay Enabled: FALSE");
            return false;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Ads_CanShowFullscreenBeforeGamePlay();
        public static bool CanShowFullscreenBeforeGamePlay()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_CanShowFullscreenBeforeGamePlay() == "true";
#else

            ConsoleLog("Can Show Fullscreen Before Gameplay: FALSE");
            return false;
#endif
        }


        private void CallAdsStart() => OnAdsStart?.Invoke();
        private void CallAdsClose(string success) => OnAdsClose?.Invoke(success == "true");

        private void CallAdsFullscreenStart()
        {
            _onFullscreenStart?.Invoke();
            OnFullscreenStart?.Invoke();
        }
        private void CallAdsFullscreenClose(string success)
        {
            _onFullscreenClose?.Invoke(success == "true");
            OnFullscreenClose?.Invoke(success == "true");
        }

        private void CallAdsPreloaderStart()
        {
            _onPreloaderStart?.Invoke();
            OnPreloaderStart?.Invoke();
        }
        private void CallAdsPreloaderClose(string success)
        {
            _onPreloaderClose?.Invoke(success == "true");
            OnPreloaderClose?.Invoke(success == "true");
        }

        private void CallAdsRewardedStart()
        {
            _onRewardedStart?.Invoke();
            OnRewardedStart?.Invoke();
        }
        private void CallAdsRewardedClose(string success)
        {
            _onRewardedClose?.Invoke(success == "true");
            OnRewardedClose?.Invoke(success == "true");
        }
        private void CallAdsRewardedReward(string Tag)
        {
            _onRewardedReward?.Invoke(Tag);
            OnRewardedReward?.Invoke(Tag);
        }

        private void CallAdsStickyStart() => OnStickyStart?.Invoke();
        private void CallAdsStickyClose() => OnStickyClose?.Invoke();
        private void CallAdsStickyRefresh() => OnStickyRefresh?.Invoke();
        private void CallAdsStickyRender() => OnStickyRender?.Invoke();

    }
}