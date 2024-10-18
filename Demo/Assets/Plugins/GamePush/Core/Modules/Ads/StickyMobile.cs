using System;
using System.Threading.Tasks;

using GamePush.Core;
using GamePush.Data;

#if YANDEX_SIMPLE_MONETIZATION
using YandexMobileAds;
using YandexMobileAds.Base;
#endif

namespace GamePush.Mobile
{
    public class StickyMobile
    {

#if YANDEX_SIMPLE_MONETIZATION
        private Banner _banner;
        private BannerAdSize _stickyAdSize;
        private AdPosition _stickyPosition;
#endif

        private AdBanner _bannerData;

        public enum Position { top, bottom }
        public enum Dimention { PX, PERCENT }

        private bool _isPlaying;
        private void SetPlaying(bool isPlay)
            => _isPlaying = isPlay;
        public bool IsPlaying()
            => _isPlaying;

        private event Action OnStickyStart;
        private event Action<bool> OnStickyClose;
        private event Action OnStickyRefresh;
        private event Action OnStickyRender;

        //private Task _stickyRefreshTimer;
        //private int _refreshIntervalSeconds;
        //private bool _isRefresh;

        public StickyMobile(AdBanner banner)
        {
            _bannerData = banner;
            //_refreshIntervalSeconds = _bannerData.refreshInterval;
            SetUpBannerSize();
        }

        private void SetUpBannerSize()
        {
            if (_bannerData.position == Position.top.ToString())
                _stickyPosition = AdPosition.TopCenter;
            else if (_bannerData.position == Position.bottom.ToString())
                _stickyPosition = AdPosition.BottomCenter;
            else
                _stickyPosition = AdPosition.BottomCenter;

            int screenWidth = (int)UnityEngine.Screen.safeArea.width;
            int screenHeight = (int)UnityEngine.Screen.safeArea.height;

            int screenWidthDp = ScreenUtils.ConvertPixelsToDp(screenWidth);
            //int screenHeightDp = ScreenUtils.ConvertPixelsToDp(screenHeight);

            if (_bannerData.maxWidth == 0 && _bannerData.maxHeight == 0)
            {
                _stickyAdSize = BannerAdSize.StickySize(screenWidthDp);
                return;
            }

            if (_bannerData.maxWidthDimension == Dimention.PERCENT.ToString())
            {
                float bannerWidth = _bannerData.maxWidth * screenWidth * 0.01f;
                _bannerData.maxWidth = (int)Math.Clamp(bannerWidth, 1, screenWidth);
            }
            else if (_bannerData.maxWidth > screenWidth)
                _bannerData.maxWidth = screenWidth;


            if (_bannerData.maxHeightDimension == Dimention.PERCENT.ToString())
            {
                float bannerHeight = _bannerData.maxHeight * screenHeight * 0.01f;
                _bannerData.maxHeight = (int)Math.Clamp(bannerHeight, 1, screenHeight);
            }
            if (_bannerData.maxHeightDimension == Dimention.PX.ToString() && _bannerData.maxHeight > screenHeight)
                _bannerData.maxHeight = screenHeight;

            
            int bannerWidthDp = ScreenUtils.ConvertPixelsToDp(_bannerData.maxWidth);
            int bannerHeightDp = ScreenUtils.ConvertPixelsToDp(_bannerData.maxHeight);


            if (_bannerData.maxHeight == 0)
            {
                _stickyAdSize = BannerAdSize.StickySize(bannerWidthDp);
                return;
            }
            if(_bannerData.maxWidth == 0)
            {
                _stickyAdSize = BannerAdSize.InlineSize(screenWidthDp, bannerHeightDp);
                return;
            }

            _stickyAdSize = BannerAdSize.InlineSize(bannerWidthDp, bannerHeightDp);
        }

        

        public void ShowBanner(
            Action onStickyStart = null,
            Action<bool> onStickyClose = null,
            Action onStickyRefresh = null,
            Action onStickyRender = null)
        {
            OnStickyStart = onStickyStart;
            OnStickyClose = onStickyClose;
            OnStickyRefresh = onStickyRefresh;
            OnStickyRender = onStickyRender;

            RequestBanner();
            //StartStickyRefresh();

            OnStickyStart?.Invoke();
        }

        public void CloseBanner()
        {
            DestroyBanner();
            //StopStickyRefresh();
            OnStickyClose?.Invoke(true);
        }

        public void RefreshBanner()
        {
            //StopStickyRefresh();
            RefreshSticky();
            //StartStickyRefresh();
        }

        private void RefreshSticky()
        {
            Logger.Log("Refresh Sticky");
            OnStickyRefresh?.Invoke();
            RequestBanner();
        }

        //public async void StartStickyRefresh()
        //{
        //    if (_refreshIntervalSeconds > 0)
        //        //if (_stickyRefreshTimer == null && _refreshIntervalSeconds > 0)
        //    {
        //        _isRefresh = true;

        //        Logger.Log("Start Sticky Refresh Timer: " + _refreshIntervalSeconds);
                
        //        int interval = _refreshIntervalSeconds * 1000;

        //        _stickyRefreshTimer = Task.Delay(interval);
        //        await _stickyRefreshTimer;
                
        //        if (_isRefresh)
        //        {
        //            RefreshSticky();
        //            StartStickyRefresh();
        //        }
                   
        //        //_stickyRefreshTimer = new Timer(
        //        //    _ => RefreshSticky(),
        //        //    null,
        //        //    interval,
        //        //    interval);
        //    }
        //}

        //public async void StopStickyRefresh()
        //{
        //    if (_stickyRefreshTimer != null)
        //    {
        //        _isRefresh = false;

        //        Logger.Log("Stop Sticky Refresh Timer");
        //        await _stickyRefreshTimer;

        //        _stickyRefreshTimer.Dispose();
        //        _stickyRefreshTimer = null;
        //    }
        //}



        #region Banner methods

        private void RequestBanner()
        {
#if YANDEX_SIMPLE_MONETIZATION
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            string adUnitId = _bannerData.bannerId;

            if (_banner != null)
            {
                DestroyBanner();
            }

            _banner = new Banner(adUnitId, _stickyAdSize, _stickyPosition);

            _banner.OnAdLoaded += HandleAdLoaded;
            _banner.OnAdFailedToLoad += HandleAdFailedToLoad;
            _banner.OnReturnedToApplication += HandleReturnedToApplication;
            _banner.OnLeftApplication += HandleLeftApplication;
            _banner.OnAdClicked += HandleAdClicked;
            _banner.OnImpression += HandleImpression;

            _banner.LoadAd(CreateAdRequest());
#endif
            Logger.Log("Banner is requested");
        }

        private AdRequest CreateAdRequest()
        {
            return new AdRequest.Builder().Build();
        }

        private void DestroyBanner()
        {
            SetPlaying(false);
            _banner.Destroy();
        }

        #endregion

        #region Banner callback handlers

#if YANDEX_SIMPLE_MONETIZATION
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
            //Logger.Log("HandleImpression event received with data: " + data);
            OnStickyRender?.Invoke();
        }
#endif
#endregion
    }
}
