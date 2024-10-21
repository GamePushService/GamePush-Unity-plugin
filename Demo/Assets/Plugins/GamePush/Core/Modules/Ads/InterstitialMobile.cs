using System;
using System.Timers;
using YandexMobileAds;
using YandexMobileAds.Base;
using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class InterstitialMobile
    {
#if YANDEX_SIMPLE_MONETIZATION
        private InterstitialAdLoader interstitialAdLoader;
        private Interstitial interstitial;
#endif
        private AdBanner _bannerData;

        private bool _isPlaying;
        private void SetPlaying(bool isPlay) => _isPlaying = isPlay;
        public bool IsPlaying() => _isPlaying;
        
        private event Action OnFullscreenStart;
        private event Action<bool> OnFullscreenClose;

        public InterstitialMobile(AdBanner banner)
        {
            _bannerData = banner;

            SetUpLoader();
            RequestInterstitial();
        }

        public void SetUpLoader()
        {
#if YANDEX_SIMPLE_MONETIZATION
            interstitialAdLoader = new InterstitialAdLoader();
            interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
            interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
#endif
        }

        private void RequestInterstitial()
        {
#if YANDEX_SIMPLE_MONETIZATION
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            string adUnitId = _bannerData.bannerId;

            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            interstitialAdLoader.LoadAd(CreateAdRequest(adUnitId));
#else
        OnFullscreenClose?.Invoke(false);
#endif
            Logger.Log("Interstitial is requested");
        }

        public void ShowInterstitial(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
#if YANDEX_SIMPLE_MONETIZATION
            if (interstitial == null)
            {
                Logger.Log("Interstitial is not ready yet");
                onFullscreenClose?.Invoke(false);
                return;
            }

            OnFullscreenStart = onFullscreenStart;
            OnFullscreenClose = onFullscreenClose;

            interstitial.Show();
            SetPlaying(true);
#endif
        }

#if YANDEX_SIMPLE_MONETIZATION
        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }
#endif

        private void DestroyBanner(bool success)
        {
#if YANDEX_SIMPLE_MONETIZATION
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

            OnFullscreenClose?.Invoke(success);
#endif
            SetPlaying(false);
        }

#region Interstitial callback handlers

#if YANDEX_SIMPLE_MONETIZATION
        public void HandleAdLoaded(object sender, InterstitialAdLoadedEventArgs args)
        {
            Logger.Log("HandleAdLoaded event received");

            interstitial = args.Interstitial;

            interstitial.OnAdClicked += HandleAdClicked;
            interstitial.OnAdShown += HandleAdShown;
            interstitial.OnAdFailedToShow += HandleAdFailedToShow;
            interstitial.OnAdImpression += HandleImpression;
            interstitial.OnAdDismissed += HandleAdDismissed;
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Logger.Log($"HandleAdFailedToLoad event received with message: {args.Message}");

        }
        public void HandleAdClicked(object sender, EventArgs args)
        {
            Logger.Log("HandleAdClicked event received");
        }

        public void HandleAdShown(object sender, EventArgs args)
        {
            Logger.Log("HandleAdShown event received");
            OnFullscreenStart?.Invoke();
        }

        public void HandleAdDismissed(object sender, EventArgs args)
        {
            Logger.Log("HandleAdDismissed event received");

            
            RequestInterstitial();
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            //Logger.Log($"HandleImpression event received with data: {data}");

            DestroyBanner(true);
            RequestInterstitial();
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Logger.Log($"HandleAdFailedToShow event received with message: {args.Message}");

            DestroyBanner(false);
            RequestInterstitial();
        }
#endif
#endregion
    }
}
