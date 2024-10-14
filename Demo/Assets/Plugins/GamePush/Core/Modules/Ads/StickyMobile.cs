using System;
using YandexMobileAds;
using YandexMobileAds.Base;
using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class StickyMobile
    {
        private Banner banner;

        private AdBanner bannerData;

        public StickyMobile(AdBanner banner)
        {
            bannerData = banner;
            //if (bannerData.enabled)
            //{
            //    ShowBanner();
            //}
        }

        private bool isPlaying;
        private void SetPlaying(bool isPlay) => isPlaying = isPlay;

        public bool IsPlaying() => isPlaying;

        private event Action OnStickyStart;
        private event Action<bool> OnStickyClose;
        private event Action OnStickyRefresh;

        public void ShowBanner(
            Action onStickyStart = null,
            Action<bool> onStickyClose = null,
            Action onStickyRefresh = null)
        {
            OnStickyStart = onStickyStart;
            OnStickyClose = onStickyClose;
            OnStickyRefresh = onStickyRefresh;
            RequestBanner();
        }
        
        public void CloseBanner() => DestroyBanner();
        public void RefreshBanner()
        {
            banner.LoadAd(CreateAdRequest());
            OnStickyRefresh?.Invoke();
        }


        #region Banner methods

        private void RequestBanner()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            string adUnitId = bannerData.bannerId;

            if (banner != null)
            {
                DestroyBanner();
            }
            // Set sticky banner width
            BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
            // Or set inline banner maximum width and height
            // BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
            banner = new Banner(adUnitId, bannerSize, AdPosition.BottomCenter);

            banner.OnAdLoaded += HandleAdLoaded;
            banner.OnAdFailedToLoad += HandleAdFailedToLoad;
            banner.OnReturnedToApplication += HandleReturnedToApplication;
            banner.OnLeftApplication += HandleLeftApplication;
            banner.OnAdClicked += HandleAdClicked;
            banner.OnImpression += HandleImpression;

            banner.LoadAd(CreateAdRequest());
            
            Logger.Log("Banner is requested");
        }

        // Example how to get screen width for request
        private int GetScreenWidthDp()
        {
            int screenWidth = (int)UnityEngine.Screen.safeArea.width;
            return ScreenUtils.ConvertPixelsToDp(screenWidth);
        }

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }

        private void DestroyBanner()
        {
            banner.Destroy();
            SetPlaying(false);
            OnStickyClose?.Invoke(true);
        }

        #endregion

        #region Banner callback handlers

        public void HandleAdLoaded(object sender, EventArgs args)
        {
            Logger.Log("HandleAdLoaded event received");
            SetPlaying(true);
            banner.Show();
        }

        public void HandleAdFailedToLoad(object sender, AdFailureEventArgs args)
        {
            Logger.Log("HandleAdFailedToLoad event received with message: " + args.Message);
            SetPlaying(false);
        }

        public void HandleLeftApplication(object sender, EventArgs args)
        {
            //SetPlaying(false);
            Logger.Log("HandleLeftApplication event received");
        }

        public void HandleReturnedToApplication(object sender, EventArgs args)
        {
            Logger.Log("HandleReturnedToApplication event received");
        }

        public void HandleAdLeftApplication(object sender, EventArgs args)
        {
            //SetPlaying(false);
            Logger.Log("HandleAdLeftApplication event received");
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            Logger.Log("HandleAdClicked event received");
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            Logger.Log("HandleImpression event received with data: " + data);
            OnStickyStart?.Invoke();
        }

        #endregion
    }
}
