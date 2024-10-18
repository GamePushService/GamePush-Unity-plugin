using System;
using YandexMobileAds;
using YandexMobileAds.Base;
using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class PreloaderMobile
    {

#if YANDEX_SIMPLE_MONETIZATION
        private AppOpenAdLoader appOpenAdLoader;
        private AppOpenAd appOpenAd;
#endif

        private AdBanner _bannerData;

        private bool _isPlaying;
        private bool _isAdShowOnColdStart;

        private void SetPlaying(bool isPlay) => _isPlaying = isPlay;
        public bool IsPlaying() => _isPlaying;

        private event Action OnPreloaderStart;
        private event Action<bool> OnPreloaderClose;

        public PreloaderMobile(AdBanner banner)
        {
            _bannerData = banner;

            SetUpLoader();
            RequestAppOpenAd();
        }


        public void SetUpLoader()
        {

#if YANDEX_SIMPLE_MONETIZATION
            appOpenAdLoader = new AppOpenAdLoader();
            appOpenAdLoader.OnAdLoaded += HandleAdLoaded;
            appOpenAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;

            // Use the AppStateObserver to listen to application open/close events.
            AppStateObserver.OnAppStateChanged += HandleAppStateChanged;
#endif
        }

#if YANDEX_SIMPLE_MONETIZATION
        public void OnDestroy()
        {
           
            // Unsubscribe from the event to avoid memory leaks.
            AppStateObserver.OnAppStateChanged -= HandleAppStateChanged;
        }
#endif

        private void RequestAppOpenAd()
        {
#if YANDEX_SIMPLE_MONETIZATION
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            // Replace demo Unit ID 'demo-appOpenAd-yandex' with actual Ad Unit ID
            string adUnitId = _bannerData.bannerId;

            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
            }

            appOpenAdLoader.LoadAd(CreateAdRequestConfiguration(adUnitId));
#endif
            Logger.Log("AppOpenAd is requested");
        }

        public void ShowAppOpenAd(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
#if YANDEX_SIMPLE_MONETIZATION
            if (appOpenAd == null)
            {
                Logger.Log("AppOpenAd is not ready yet");
                return;
            }

            OnPreloaderStart = onPreloaderStart;
            OnPreloaderClose = onPreloaderClose;

            appOpenAd.Show();
#endif
            SetPlaying(true);
        }

#if YANDEX_SIMPLE_MONETIZATION
        private AdRequestConfiguration CreateAdRequestConfiguration(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }
#endif

        private void DestroyBanner()
        {
#if YANDEX_SIMPLE_MONETIZATION
            if (appOpenAd != null)
            {
                appOpenAd.Destroy();
                appOpenAd = null;
            }
#endif
            SetPlaying(false);
        }

#region AppOpenAd callback handlers

#if YANDEX_SIMPLE_MONETIZATION
        public void HandleAppStateChanged(object sender, AppStateChangedEventArgs args)
        {
            if (!args.IsInBackground && _bannerData.enabled)
            {
                //ShowAppOpenAd();
            }
        }

        public void HandleAdLoaded(object sender, AppOpenAdLoadedEventArgs args)
        {
            Logger.Log("HandleAdLoaded event received");

            appOpenAd = args.AppOpenAd;

            appOpenAd.OnAdClicked += HandleAdClicked;
            appOpenAd.OnAdShown += HandleAdShown;
            appOpenAd.OnAdFailedToShow += HandleAdFailedToShow;
            appOpenAd.OnAdImpression += HandleImpression;
            appOpenAd.OnAdDismissed += HandleAdDismissed;

            if (!_isAdShowOnColdStart)
            {
                //ShowAppOpenAd();
                _isAdShowOnColdStart = true;
            }
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Logger.Log("HandleAdFailedToLoad event received with message: " + args.Message);
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            Logger.Log("HandleAdClicked event received");
        }

        public void HandleAdShown(object sender, EventArgs args)
        {
            Logger.Log("HandleAdShown event received");
            OnPreloaderStart?.Invoke();
        }

        public void HandleAdDismissed(object sender, EventArgs args)
        {
            Logger.Log("HandleAdDismissed event received");

            DestroyBanner();
            RequestAppOpenAd();
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            //Logger.Log($"HandleImpression event received with data: {data}");
            OnPreloaderClose?.Invoke(true);
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Logger.Log($"HandleAdFailedToShow event received with message: {args.Message}");

            OnPreloaderClose?.Invoke(false);
            DestroyBanner();
            RequestAppOpenAd();
        }
#endif
#endregion

    }
}
