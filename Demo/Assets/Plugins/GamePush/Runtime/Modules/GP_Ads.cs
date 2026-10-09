using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Native;


namespace GamePush
{
    public class GP_Ads : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Ads);

        #region Events

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

        internal static void FireAdsStart() => OnAdsStart?.Invoke();
        internal static void FireAdsClose(bool success) => OnAdsClose?.Invoke(success);
        internal static void FireFullscreenStart()
        {
            OnFullscreenStart?.Invoke();
            _onFullscreenStart?.Invoke();
        }
        internal static void FireFullscreenClose(bool success)
        {
            OnFullscreenClose?.Invoke(success);
            _onFullscreenClose?.Invoke(success);
        }
        internal static void FirePreloaderStart()
        {
            OnPreloaderStart?.Invoke();
            _onPreloaderStart?.Invoke();
        }
        internal static void FirePreloaderClose(bool success)
        {
            OnPreloaderClose?.Invoke(success);
            _onPreloaderClose?.Invoke(success);
        }
        internal static void FireRewardedStart()
        {
            OnRewardedStart?.Invoke();
            _onRewardedStart?.Invoke();
        }
        internal static void FireRewardedClose(bool success)
        {
            OnRewardedClose?.Invoke(success);
            _onRewardedClose?.Invoke(success);
        }
        internal static void FireRewardedReward(string tag)
        {
            OnRewardedReward?.Invoke(tag);
            _onRewardedReward?.Invoke(tag);
        }
        internal static void FireStickyStart() => OnStickyStart?.Invoke();
        internal static void FireStickyClose() => OnStickyClose?.Invoke();
        internal static void FireStickyRefresh() => OnStickyRefresh?.Invoke();

        #endregion
       

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowFullscreen(string showCountdownOverlay);
        #endif
        public static void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            ShowFullscreen(false, onFullscreenStart, onFullscreenClose);
        }

        public static void ShowFullscreen(bool showCountdownOverlay, Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            _onFullscreenStart = onFullscreenStart;
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
             GP_Ads_ShowFullscreen(showCountdownOverlay.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAds.ShowFullscreen(showCountdownOverlay);
                return;
            }
            if (GP_Play2Web.Call("AdsShowFullscreen", showCountdownOverlay.ToString()))
                return;
            ConsoleLog("FULL SCREEN AD: SHOW");
#if UNITY_EDITOR
            if (GP_AdsStub.Enabled)
                GP_AdsStub.ShowFullscreen();
#endif
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowRewarded(string idOrTag);
        #endif
        public static void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            _onRewardedReward = onRewardedReward;
            _onRewardedStart = onRewardedStart;
            _onRewardedClose = onRewardedClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowRewarded(idOrTag);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAds.ShowRewarded(idOrTag);
                return;
            }
            if (GP_Play2Web.Call("AdsShowRewarded", idOrTag))
                return;
            ConsoleLog("SHOW REWARDED AD -> TAG: " + idOrTag);
#if UNITY_EDITOR
            if (GP_AdsStub.Enabled)
            {
                GP_AdsStub.ShowRewarded(idOrTag);
                return;
            }
#endif
            OnRewardedReward?.Invoke(idOrTag);
            _onRewardedReward?.Invoke(idOrTag);
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowPreloader();
        #endif
        public static void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            _onPreloaderStart = onPreloaderStart;
            _onPreloaderClose = onPreloaderClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowPreloader();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAds.ShowPreloader();
                return;
            }
            if (GP_Play2Web.Call("AdsShowPreloader"))
                return;
            ConsoleLog("PRELOADER AD: SHOW");
#if UNITY_EDITOR
            if (GP_AdsStub.Enabled)
                GP_AdsStub.ShowPreloader();
#endif
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowSticky();
        #endif
        public static void ShowSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowSticky();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAds.ShowSticky();
                return;
            }
            if (GP_Play2Web.Call("AdsShowSticky"))
                return;
            ConsoleLog("STICKY BANNER AD: SHOW");
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Ads_CloseSticky();
        #endif
        public static void CloseSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_CloseSticky();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAds.CloseSticky();
                return;
            }
            if (GP_Play2Web.Call("AdsCloseSticky"))
                return;
            ConsoleLog("STICKY BANNER AD: CLOSE");
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Ads_RefreshSticky();
        #endif
        public static void RefreshSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_RefreshSticky();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAds.RefreshSticky();
                return;
            }
            if (GP_Play2Web.Call("AdsRefreshSticky"))
                return;
            ConsoleLog("STICKY BANNER AD: REFRESH");
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsAdblockEnabled();
        #endif
        public static bool IsAdblockEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsAdblockEnabled() == "true";
#else
            if (GP_Play2Web.TryGetBool("AdsIsAdblockEnabled", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsAdblockEnabled;
            ConsoleLog("IS ADBLOCK ENABLED: " + isVal);
            return isVal;
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyAvailable();
        #endif
        public static bool IsStickyAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyAvailable() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsAvailable("STICKY");
            if (GP_Play2Web.TryGetBool("AdsIsStickyAvailable", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsStickyAvailable;
            ConsoleLog("IS STICKY BANNER AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsFullscreenAvailable();
        #endif
        public static bool IsFullscreenAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenAvailable() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsAvailable("FULLSCREEN");
            if (GP_Play2Web.TryGetBool("AdsIsFullscreenAvailable", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsFullscreenAvailable;
            ConsoleLog("IS FULL SCREEN AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedAvailable();
        #endif
        public static bool IsRewardedAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedAvailable() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsAvailable("REWARDED");
            if (GP_Play2Web.TryGetBool("AdsIsRewardedAvailable", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsRewardedAvailable;
            ConsoleLog("IS REWARD AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsPreloaderAvailable();
        #endif
        public static bool IsPreloaderAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderAvailable() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsAvailable("PRELOADER");
            if (GP_Play2Web.TryGetBool("AdsIsPreloaderAvailable", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetPlatformSettings().IsPreloaderAvailable;
            ConsoleLog("IS PRELOADER AD AVAILABLE: " + isVal);
            return isVal;
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyPlaying();
        #endif
        public static bool IsStickyPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyPlaying() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsStickyPlaying;
            if (GP_Play2Web.TryGetBool("AdsIsStickyPlaying", out var live))
                return live;
            ConsoleLog("IS STICKY PLAYING: FALSE");
            return false;
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsFullscreenPlaying();
        #endif
        public static bool IsFullscreenPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenPlaying() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsFullscreenPlaying;
            if (GP_Play2Web.TryGetBool("AdsIsFullscreenPlaying", out var live))
                return live;
#if UNITY_EDITOR
            bool playing = GP_AdsStub.Enabled && GP_AdsStub.IsFullscreenPlaying;
            ConsoleLog("IS FULLSCREEN AD PLAYING: " + playing);
            return playing;
#else
            ConsoleLog("IS FULLSCREEN AD PLAYING: FALSE");
            return false;
#endif
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedPlaying();
        #endif
        public static bool IsRewardPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedPlaying() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsRewardedPlaying;
            if (GP_Play2Web.TryGetBool("AdsIsRewardedPlaying", out var live))
                return live;
#if UNITY_EDITOR
            bool playing = GP_AdsStub.Enabled && GP_AdsStub.IsRewardedPlaying;
            ConsoleLog("IS REWARDED AD PLAYING: " + playing);
            return playing;
#else
            ConsoleLog("IS REWARDED AD PLAYING: FALSE");
            return false;
#endif
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsPreloaderPlaying();
        #endif
        public static bool IsPreloaderPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderPlaying() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeAds.IsPreloaderPlaying;
            if (GP_Play2Web.TryGetBool("AdsIsPreloaderPlaying", out var live))
                return live;
#if UNITY_EDITOR
            bool playing = GP_AdsStub.Enabled && GP_AdsStub.IsPreloaderPlaying;
            ConsoleLog("IS PRELOADER AD PLAYING: " + playing);
            return playing;
#else
            ConsoleLog("IS PRELOADER AD PLAYING: FALSE");
            return false;
#endif
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsCountdownOverlayEnabled();
        #endif
        public static bool IsCountdownOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsCountdownOverlayEnabled() == "true";
#else
            if (GP_Play2Web.TryGetBool("AdsIsCountdownOverlayEnabled", out var live))
                return live;
            ConsoleLog("Is Countdown Overlay Enabled: FALSE");
            return false;
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedFailedOverlayEnabled();
        #endif
        public static bool IsRewardedFailedOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedFailedOverlayEnabled() == "true";
#else
            if (GP_Play2Web.TryGetBool("AdsIsRewardedFailedOverlayEnabled", out var live))
                return live;
            ConsoleLog("Is Rewarded Failed Overlay Enabled: FALSE");
            return false;
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Ads_CanShowFullscreenBeforeGamePlay();
        #endif
        public static bool CanShowFullscreenBeforeGamePlay()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_CanShowFullscreenBeforeGamePlay() == "true";
#else
            if (GP_Play2Web.TryGetBool("AdsCanShowFullscreenBeforeGamePlay", out var live))
                return live;
            ConsoleLog("Can Show Fullscreen Before Gameplay: FALSE");
            return false;
#endif
        }


        private void CallAdsStart() => FireAdsStart();
        private void CallAdsClose(string success) => FireAdsClose(success == "true");
        private void CallAdsFullscreenStart() => FireFullscreenStart();
        private void CallAdsFullscreenClose(string success) => FireFullscreenClose(success == "true");
        private void CallAdsPreloaderStart() => FirePreloaderStart();
        private void CallAdsPreloaderClose(string success) => FirePreloaderClose(success == "true");
        private void CallAdsRewardedStart() => FireRewardedStart();
        private void CallAdsRewardedClose(string success) => FireRewardedClose(success == "true");
        private void CallAdsRewardedReward(string Tag) => FireRewardedReward(Tag);

        private void CallAdsStickyStart() => OnStickyStart?.Invoke();
        private void CallAdsStickyClose() => OnStickyClose?.Invoke();
        private void CallAdsStickyRefresh() => OnStickyRefresh?.Invoke();
        private void CallAdsStickyRender() => OnStickyRender?.Invoke();

    }
}