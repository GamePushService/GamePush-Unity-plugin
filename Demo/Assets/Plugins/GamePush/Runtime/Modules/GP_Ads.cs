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

        public static event UnityAction OnRewardedStart;
        public static event UnityAction<bool> OnRewardedClose;
        public static event UnityAction<string> OnRewardedReward;

        public static event UnityAction OnStickyStart;
        public static event UnityAction<bool> OnStickyClose;
        public static event UnityAction OnStickyRefresh;
        public static event UnityAction OnStickyRender;

        public static event UnityAction OnPreloaderStart;
        public static event UnityAction<bool> OnPreloaderClose;

        private static event Action _onFullscreenStart;
        private static event Action<bool> _onFullscreenClose;

        private static event Action _onPreloaderStart;
        private static event Action<bool> _onPreloaderClose;

        private static event Action<string> _onRewardedReward;
        private static event Action _onRewardedStart;
        private static event Action<bool> _onRewardedClose;

        private void OnEnable()
        {
            CoreSDK.Ads.OnAdsStart += () => OnAdsStart?.Invoke();
            CoreSDK.Ads.OnAdsClose += (bool success) => OnAdsClose?.Invoke(success);

            CoreSDK.Ads.OnFullscreenStart += () => OnFullscreenStart?.Invoke();
            CoreSDK.Ads.OnFullscreenClose += (bool success) => OnFullscreenClose?.Invoke(success);

            CoreSDK.Ads.OnRewardedReward += (string tag) => OnRewardedReward?.Invoke(tag);
            CoreSDK.Ads.OnRewardedStart += () => OnRewardedStart?.Invoke();
            CoreSDK.Ads.OnRewardedClose += (bool success) => OnRewardedClose?.Invoke(success);

            CoreSDK.Ads.OnStickyStart += () => OnStickyStart?.Invoke();
            CoreSDK.Ads.OnStickyClose += (bool success) => OnStickyClose?.Invoke(success);
            CoreSDK.Ads.OnStickyRefresh += () => OnStickyRefresh?.Invoke();
            CoreSDK.Ads.OnStickyRender += () => OnStickyRender?.Invoke();

            CoreSDK.Ads.OnPreloaderStart += () => OnPreloaderStart?.Invoke();
            CoreSDK.Ads.OnPreloaderClose += (bool success) => OnPreloaderClose?.Invoke(success);
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowFullscreen();

        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowRewarded(string idOrTag);

        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowPreloader();

        [DllImport("__Internal")]
        private static extern void GP_Ads_ShowSticky();

        [DllImport("__Internal")]
        private static extern void GP_Ads_CloseSticky();

        [DllImport("__Internal")]
        private static extern void GP_Ads_RefreshSticky();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyAvailable();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsFullscreenAvailable();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsAdblockEnabled();
        
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedAvailable();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsPreloaderAvailable();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsStickyPlaying();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsFullscreenPlaying();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedPlaying();
        
        [DllImport("__Internal")]
        private static extern string GP_Ads_IsPreloaderPlaying();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsCountdownOverlayEnabled();

        [DllImport("__Internal")]
        private static extern string GP_Ads_IsRewardedFailedOverlayEnabled();
        
        [DllImport("__Internal")]
        private static extern string GP_Ads_CanShowFullscreenBeforeGamePlay();

#endif

        public static void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            _onFullscreenStart = onFullscreenStart;
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowFullscreen();
#else
            CoreSDK.Ads.ShowFullscreen(onFullscreenStart, onFullscreenClose);
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
            CoreSDK.Ads.ShowRewarded(idOrTag, onRewardedReward, onRewardedStart, onRewardedClose);
#endif
        }

        public static void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            _onPreloaderStart = onPreloaderStart;
            _onPreloaderClose = onPreloaderClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowPreloader();
#else
            CoreSDK.Ads.ShowPreloader(onPreloaderStart, onPreloaderClose);
#endif
        }

        public static void ShowSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_ShowSticky();
#else
            CoreSDK.Ads.ShowSticky();
#endif
        }


        public static void CloseSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_CloseSticky();
#else
            CoreSDK.Ads.CloseSticky();
#endif
        }

        public static void RefreshSticky()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Ads_RefreshSticky();
#else
            CoreSDK.Ads.RefreshSticky();
#endif
        }

        public static bool IsAdblockEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsAdblockEnabled() == "true";
#else
            return CoreSDK.Ads.IsAdblockEnabled();
#endif
        }

        public static bool IsStickyAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyAvailable() == "true";
#else
            return CoreSDK.Ads.IsStickyAvailable();
#endif
        }

        public static bool IsFullscreenAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenAvailable() == "true";
#else
            return CoreSDK.Ads.IsFullscreenAvailable();
#endif
        }


        public static bool IsRewardedAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedAvailable() == "true";
#else
            return CoreSDK.Ads.IsRewardedAvailable();
#endif
        }

        public static bool IsPreloaderAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderAvailable() == "true";
#else
            return CoreSDK.Ads.IsPreloaderAvailable();
#endif
        }

        public static bool IsStickyPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsStickyPlaying() == "true";
#else
            return CoreSDK.Ads.IsStickyPlaying();
#endif
        }

        public static bool IsFullscreenPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsFullscreenPlaying() == "true";
#else
            return CoreSDK.Ads.IsFullscreenPlaying();
#endif
        }

        public static bool IsRewardedPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedPlaying() == "true";
#else
            return CoreSDK.Ads.IsRewardedPlaying();
#endif
        }

        public static bool IsPreloaderPlaying()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsPreloaderPlaying() == "true";
#else
            return CoreSDK.Ads.IsPreloaderPlaying();
#endif
        }

        public static bool IsCountdownOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsCountdownOverlayEnabled() == "true";
#else
            return CoreSDK.Ads.IsCountdownOverlayEnabled();
#endif
        }

        public static bool IsRewardedFailedOverlayEnabled()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_IsRewardedFailedOverlayEnabled() == "true";
#else
            return CoreSDK.Ads.IsRewardedFailedOverlayEnabled();
#endif
        }

        public static bool CanShowFullscreenBeforeGamePlay()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Ads_CanShowFullscreenBeforeGamePlay() == "true";
#else
            return CoreSDK.Ads.CanShowFullscreenBeforeGamePlay();
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