using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;

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
        public static event UnityAction<bool> OnStickyClose;
        public static event UnityAction OnStickyRefresh;
        public static event UnityAction OnStickyRender;

        private static event Action _onFullscreenStart;
        private static event Action<bool> _onFullscreenClose;

        private static event Action _onPreloaderStart;
        private static event Action<bool> _onPreloaderClose;

        private static event Action<string> _onRewardedReward;
        private static event Action _onRewardedStart;
        private static event Action<bool> _onRewardedClose;

        private void OnEnable()
        {
            CoreSDK.ads.OnAdsStart += CallAdsStart;
            CoreSDK.ads.OnAdsClose += CallAdsCloseBool;

            CoreSDK.ads.OnFullscreenStart += CallAdsFullscreenStart;
            CoreSDK.ads.OnFullscreenClose += CallAdsFullscreenCloseBool;

            CoreSDK.ads.OnRewardedReward += CallAdsRewardedReward;
            CoreSDK.ads.OnRewardedStart += CallAdsRewardedStart;
            CoreSDK.ads.OnRewardedClose += CallAdsRewardedCloseBool;

            CoreSDK.ads.OnStickyStart += CallAdsStickyStart;
            CoreSDK.ads.OnStickyClose += CallAdsStickyCloseBool;
            CoreSDK.ads.OnStickyRefresh += CallAdsStickyRefresh;
            CoreSDK.ads.OnStickyRender += CallAdsStickyRender;

            CoreSDK.ads.OnPreloaderStart += CallAdsPreloaderStart;
            CoreSDK.ads.OnPreloaderClose += CallAdsPreloaderCloseBool;
        }

        private void OnDisable()
        {
            CoreSDK.ads.OnAdsStart -= CallAdsStart;
            CoreSDK.ads.OnAdsClose -= CallAdsCloseBool;

            CoreSDK.ads.OnFullscreenStart -= CallAdsFullscreenStart;
            CoreSDK.ads.OnFullscreenClose -= CallAdsFullscreenCloseBool;

            CoreSDK.ads.OnRewardedReward -= CallAdsRewardedReward;
            CoreSDK.ads.OnRewardedStart -= CallAdsRewardedStart;
            CoreSDK.ads.OnRewardedClose -= CallAdsRewardedCloseBool;

            CoreSDK.ads.OnStickyStart -= CallAdsStickyStart;
            CoreSDK.ads.OnStickyClose -= CallAdsStickyCloseBool;
            CoreSDK.ads.OnStickyRefresh -= CallAdsStickyRefresh;
            CoreSDK.ads.OnStickyRender -= CallAdsStickyRender;

            CoreSDK.ads.OnPreloaderStart -= CallAdsPreloaderStart;
            CoreSDK.ads.OnPreloaderClose -= CallAdsPreloaderCloseBool;
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("libARWrapper.so")]
        private static extern void GP_Ads_ShowFullscreen();

        [DllImport("libARWrapper.so")]
        private static extern void GP_Ads_ShowRewarded(string idOrTag);

        [DllImport("libARWrapper.so")]
        private static extern void GP_Ads_ShowPreloader();

        [DllImport("libARWrapper.so")]
        private static extern void GP_Ads_ShowSticky();

        [DllImport("libARWrapper.so")]
        private static extern void GP_Ads_CloseSticky();

        [DllImport("libARWrapper.so")]
        private static extern void GP_Ads_RefreshSticky();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsStickyAvailable();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsFullscreenAvailable();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsAdblockEnabled();
        
        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsRewardedAvailable();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsPreloaderAvailable();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsStickyPlaying();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsFullscreenPlaying();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsRewardedPlaying();
        
        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsPreloaderPlaying();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsCountdownOverlayEnabled();

        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_IsRewardedFailedOverlayEnabled();
        
        [DllImport("libARWrapper.so")]
        private static extern string GP_Ads_CanShowFullscreenBeforeGamePlay();

#endif

        public static void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            _onFullscreenStart = onFullscreenStart;
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowFullscreen();
#else
            CoreSDK.ads.ShowFullscreen(onFullscreenStart, onFullscreenClose);
#endif
        }
        
        public static void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            _onRewardedReward = onRewardedReward;
            _onRewardedStart = onRewardedStart;
            _onRewardedClose = onRewardedClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowRewarded(idOrTag);
#else
            CoreSDK.ads.ShowRewarded(idOrTag, onRewardedReward, onRewardedStart, onRewardedClose);
#endif
        }

        public static void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            _onPreloaderStart = onPreloaderStart;
            _onPreloaderClose = onPreloaderClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowPreloader();
#else
            CoreSDK.ads.ShowPreloader(onPreloaderStart, onPreloaderClose);
#endif
        }

        public static void ShowSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowSticky();
#else
            CoreSDK.ads.ShowSticky();
#endif
        }


        public static void CloseSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_CloseSticky();
#else
            CoreSDK.ads.CloseSticky();
#endif
        }

        public static void RefreshSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_RefreshSticky();
#else
            CoreSDK.ads.RefreshSticky();
#endif
        }

        public static bool IsAdblockEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsAdblockEnabled() == "true";
#else
            return CoreSDK.ads.IsAdblockEnabled();
#endif
        }

        public static bool IsStickyAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyAvailable() == "true";
#else
            return CoreSDK.ads.IsStickyAvailable();
#endif
        }

        public static bool IsFullscreenAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenAvailable() == "true";
#else
            return CoreSDK.ads.IsFullscreenAvailable();
#endif
        }


        public static bool IsRewardedAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedAvailable() == "true";
#else
            return CoreSDK.ads.IsRewardedAvailable();
#endif
        }

        public static bool IsPreloaderAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderAvailable() == "true";
#else
            return CoreSDK.ads.IsPreloaderAvailable();
#endif
        }

        public static bool IsStickyPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyPlaying() == "true";
#else
            return CoreSDK.ads.IsStickyPlaying();
#endif
        }

        public static bool IsFullscreenPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenPlaying() == "true";
#else
            return CoreSDK.ads.IsFullscreenPlaying();
#endif
        }

        public static bool IsRewardedPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedPlaying() == "true";
#else
            return CoreSDK.ads.IsRewardedPlaying();
#endif
        }

        public static bool IsPreloaderPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderPlaying() == "true";
#else
            return CoreSDK.ads.IsPreloaderPlaying();
#endif
        }

        public static bool IsCountdownOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsCountdownOverlayEnabled() == "true";
#else
            return CoreSDK.ads.IsCountdownOverlayEnabled();
#endif
        }

        public static bool IsRewardedFailedOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedFailedOverlayEnabled() == "true";
#else
            return CoreSDK.ads.IsRewardedFailedOverlayEnabled();
#endif
        }

        public static bool CanShowFullscreenBeforeGamePlay()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_CanShowFullscreenBeforeGamePlay() == "true";
#else
            return CoreSDK.ads.CanShowFullscreenBeforeGamePlay();
#endif
        }


        private void CallAdsStart() => OnAdsStart?.Invoke();
        private void CallAdsClose(string success) => OnAdsClose?.Invoke(success == "true");
        private void CallAdsCloseBool(bool success) => OnAdsClose?.Invoke(success);

        private void CallAdsFullscreenStart()
        {
            _onFullscreenStart?.Invoke();
            OnFullscreenStart?.Invoke();
        }

        private void CallAdsFullscreenCloseBool(bool success) => CallAdsFullscreenClose(success.ToString());
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

        private void CallAdsPreloaderCloseBool(bool success) => CallAdsPreloaderClose(success.ToString());
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

        private void CallAdsRewardedCloseBool(bool success) => CallAdsRewardedClose(success.ToString());
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
        private void CallAdsStickyClose() => OnStickyClose?.Invoke(true);
        private void CallAdsStickyCloseBool(bool success) => OnStickyClose?.Invoke(success);
        private void CallAdsStickyRefresh() => OnStickyRefresh?.Invoke();
        private void CallAdsStickyRender() => OnStickyRender?.Invoke();

    }
}