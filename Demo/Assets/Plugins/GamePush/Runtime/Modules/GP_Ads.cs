using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Ads : MonoBehaviour
    {
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
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("FULL SCREEN AD ", "SHOW");
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
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("SHOW REWARDED AD -> TAG: ", idOrTag);
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
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("PRELOADER AD: ", "SHOW");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowSticky();
        public static void ShowSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowSticky();
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("STICKY BANNER AD: ", "SHOW");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_CloseSticky();
        public static void CloseSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_CloseSticky();
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("STICKY BANNER AD: ", "CLOSE");
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Ads_RefreshSticky();
        public static void RefreshSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_RefreshSticky();
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("STICKY BANNER AD: ", "REFRESH");
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsAdblockEnabled();
        public static bool IsAdblockEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsAdblockEnabled() == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS ADBLOCK ENABLED: ", "FALSE");
            return false;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyAvailable();
        public static bool IsStickyAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyAvailable() == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS STICKY BANNER AD AVAILABLE: ", "TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsFullscreenAvailable();
        public static bool IsFullscreenAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenAvailable() == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS FULL SCREEN AD AVAILABLE: ", "TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedAvailable();
        public static bool IsRewardedAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedAvailable() == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS REWARD AD AVAILABLE: ", "TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsPreloaderAvailable();
        public static bool IsPreloaderAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderAvailable() == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS PRELOADER AD AVAILABLE: ", "TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyPlaying();
        public static bool IsStickyPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyPlaying() == "true";
#else
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS STICKY PLAYING: ", "FALSE");
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
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS FULLSCREEN AD PLAYING: ", "FALSE");
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
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS REWARDED AD PLAYING: ", "FALSE");
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
            if (GP_ConsoleController.Instance.AdsConsoleLogs)
                Console.Log("IS PRELOADER AD PLAYING: ", "FALSE");
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