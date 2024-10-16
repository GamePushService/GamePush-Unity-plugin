using System;
using System.Timers;
using YandexMobileAds;
using YandexMobileAds.Base;
using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class StickyMobile
    {
        private Banner _banner;

        private AdBanner _bannerData;

        private bool _isPlaying;
        private void SetPlaying(bool isPlay) => _isPlaying = isPlay;

        public bool IsPlaying() => _isPlaying;

        private event Action OnStickyStart;
        private event Action<bool> OnStickyClose;
        private event Action OnStickyRefresh;

        private Timer _stickyRefreshTimer;
        private int _refreshIntervalSeconds;

        public StickyMobile(AdBanner banner)
        {
            _bannerData = banner;
            _refreshIntervalSeconds = _bannerData.refreshInterval;
        }


        public void ShowBanner(
            Action onStickyStart = null,
            Action<bool> onStickyClose = null,
            Action onStickyRefresh = null)
        {
            OnStickyStart = onStickyStart;
            OnStickyClose = onStickyClose;
            OnStickyRefresh = onStickyRefresh;
            RequestBanner();
            StartStickyRefresh();
        }

        public void CloseBanner()
        {
            DestroyBanner();
            StopStickyRefresh();
        }

        public void RefreshBanner()
        {
            StopStickyRefresh();
            RefreshSticky();
            StartStickyRefresh();
        }

        private void RefreshSticky()
        {
            RequestBanner();
            OnStickyRefresh?.Invoke();
        }

        public void StartStickyRefresh()
        {
            if (_stickyRefreshTimer == null && _refreshIntervalSeconds > 0)
            {
                Logger.Log("Start Sticky Refresh Timer: " + _refreshIntervalSeconds);
                _stickyRefreshTimer = new Timer(_refreshIntervalSeconds * 1000);
                _stickyRefreshTimer.Elapsed += (sender, e) => RefreshSticky();

                _stickyRefreshTimer.AutoReset = true; // Повторное выполнение
                _stickyRefreshTimer.Enabled = true;   // Запуск таймера
            }
        }

        public void StopStickyRefresh()
        {
            if (_stickyRefreshTimer != null)
            {
                Logger.Log("Stop Sticky Refresh Timer");
                _stickyRefreshTimer.Stop();
                _stickyRefreshTimer.Dispose();
                _stickyRefreshTimer = null;
            }
        }


        #region Banner methods

        private void RequestBanner()
        {
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            string adUnitId = _bannerData.bannerId;

            if (_banner != null)
            {
                DestroyBanner();
            }
            // Set sticky banner width
            BannerAdSize bannerSize = BannerAdSize.StickySize(GetScreenWidthDp());
            // Or set inline banner maximum width and height
            // BannerAdSize bannerSize = BannerAdSize.InlineSize(GetScreenWidthDp(), 300);
            _banner = new Banner(adUnitId, bannerSize, AdPosition.BottomCenter);

            _banner.OnAdLoaded += HandleAdLoaded;
            _banner.OnAdFailedToLoad += HandleAdFailedToLoad;
            _banner.OnReturnedToApplication += HandleReturnedToApplication;
            _banner.OnLeftApplication += HandleLeftApplication;
            _banner.OnAdClicked += HandleAdClicked;
            _banner.OnImpression += HandleImpression;

            _banner.LoadAd(CreateAdRequest());
            
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
            _banner.Destroy();
            SetPlaying(false);
            OnStickyClose?.Invoke(true);
        }

        #endregion

        #region Banner callback handlers

        public void HandleAdLoaded(object sender, EventArgs args)
        {
            Logger.Log("HandleAdLoaded event received");
            SetPlaying(true);
            _banner.Show();
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
