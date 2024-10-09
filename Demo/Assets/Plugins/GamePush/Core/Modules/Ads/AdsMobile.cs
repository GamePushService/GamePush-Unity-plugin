using System;
using System.Collections.Generic;
using YandexMobileAds;
using YandexMobileAds.Base;

using GamePush.Core;

namespace GamePush.Mobile
{
    public class AdsMobile
    {
        private Banner banner;

        public void ShowBanner() => RequestBanner();
        public void RefreshBanner() => banner.LoadAd(CreateAdRequest());
        public void CloseBanner() => banner.Destroy();
        

        #region Banner methods
        private void RequestBanner()
        {

            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            // Replace demo Unit ID 'demo-banner-yandex' with actual Ad Unit ID
            string adUnitId = "R-M-12294861-1";

            if (this.banner != null)
            {
                this.banner.Destroy();
            }
            // Set sticky banner width
            BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
            // Or set inline banner maximum width and height
            // BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
            this.banner = new Banner(adUnitId, bannerSize, AdPosition.BottomCenter);

            this.banner.OnAdLoaded += this.HandleAdLoaded;
            this.banner.OnAdFailedToLoad += this.HandleAdFailedToLoad;
            this.banner.OnReturnedToApplication += this.HandleReturnedToApplication;
            this.banner.OnLeftApplication += this.HandleLeftApplication;
            this.banner.OnAdClicked += this.HandleAdClicked;
            this.banner.OnImpression += this.HandleImpression;

            this.banner.LoadAd(this.CreateAdRequest());
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

        #endregion

        #region Banner callback handlers

        public void HandleAdLoaded(object sender, EventArgs args)
        {
            Logger.Log("HandleAdLoaded event received");
            this.banner.Show();
        }

        public void HandleAdFailedToLoad(object sender, AdFailureEventArgs args)
        {
            Logger.Log("HandleAdFailedToLoad event received with message: " + args.Message);
        }

        public void HandleLeftApplication(object sender, EventArgs args)
        {
            Logger.Log("HandleLeftApplication event received");
        }

        public void HandleReturnedToApplication(object sender, EventArgs args)
        {
            Logger.Log("HandleReturnedToApplication event received");
        }

        public void HandleAdLeftApplication(object sender, EventArgs args)
        {
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
        }

        #endregion
    }

}
