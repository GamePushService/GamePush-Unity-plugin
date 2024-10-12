using System;
using YandexMobileAds;
using YandexMobileAds.Base;
using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class InterstitialMobile
    {
        private InterstitialAdLoader interstitialAdLoader;
        private Interstitial interstitial;

        private AdBanner bannerData;

        private bool isPlaying;
        private void SetPlaying(bool isPlay) => isPlaying = isPlay;
        public bool IsPlaying() => isPlaying;

        private event Action OnFullscreenStart;
        private event Action<bool> OnFullscreenClose;

        public InterstitialMobile(AdBanner banner)
        {
            bannerData = banner;

            SetUp();
            RequestInterstitial();
        }

        public void SetUp()
        {
            interstitialAdLoader = new InterstitialAdLoader();
            interstitialAdLoader.OnAdLoaded += HandleAdLoaded;
            interstitialAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        }

        private void RequestInterstitial()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            string adUnitId = bannerData.bannerId;

            if (interstitial != null)
            {
                interstitial.Destroy();
            }

            interstitialAdLoader.LoadAd(CreateAdRequest(adUnitId));
            Logger.Log("Interstitial is requested");
        }

        public void ShowInterstitial(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {

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
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        private void DestroyBanner(bool success)
        {
            if (interstitial != null)
            {
                interstitial.Destroy();
                interstitial = null;
            }

            OnFullscreenClose(success);

            SetPlaying(false);
        }

        #region Interstitial callback handlers

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
            Logger.Log($"HandleImpression event received with data: {data}");

            DestroyBanner(true);
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Logger.Log($"HandleAdFailedToShow event received with message: {args.Message}");

            DestroyBanner(false);
            RequestInterstitial();
        }

        #endregion
    }
}
