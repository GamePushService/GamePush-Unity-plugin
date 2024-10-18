using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

using GamePush.Data;
using GamePush.Mobile;

namespace GamePush.Core
{
    public class AdsModule
    {
        public event Action OnAdsStart;
        public event Action<bool> OnAdsClose;
        
        private AdsMobile adsMobile;

        private AdsConfig adsConfig;
        private List<AdBanner> banners;
        private CustomAdsConfig customAds;

        private PlayerAdsInfo adsInfo;


        //public event Action<Action, Action<bool>> OnShowFullscreen;
        //public event Action<string, Action<string>, Action, Action<bool>> OnShowRewarded;

        public void Init(AdsConfig config, PlatformConfig platformConfig)
        {
            adsConfig = config;
            banners = platformConfig.banners;
            customAds = platformConfig.customAdsConfig;
            adsInfo = LoadAdsInfo();
            CheckLimitsExpired(true);
        }

        public void LateInit()
        {

#if UNITY_ANDROID && CUSTOM_ADS_MOBILE
            adsMobile = new AdsMobile();
            InjectStickyParams();
            adsMobile.Init(customAds.configs.android);
            SetUpTimer();
#endif
        }

        private void InjectStickyParams()
        {
            AdBanner stickyBanner = GetBanner(BannerType.STICKY);
            AdBanner androidSticky = GetBannerFromBanners(BannerType.STICKY, customAds.configs.android.banners);

            //androidSticky.refreshInterval = stickyBanner.refreshInterval;
            androidSticky.maxHeight = stickyBanner.maxHeight;
            androidSticky.maxHeightDimension = stickyBanner.maxHeightDimension;
            androidSticky.maxWidth = stickyBanner.maxWidth;
            androidSticky.maxWidthDimension = stickyBanner.maxWidthDimension;
            androidSticky.position = stickyBanner.position;
            androidSticky.fitCanvas = stickyBanner.fitCanvas;

            _refreshIntervalSeconds = stickyBanner.refreshInterval;
        }

        private AdBanner GetBanner(BannerType type) => GetBannerFromBanners(type, banners);

        private AdBanner GetBannerFromBanners(BannerType type, List<AdBanner> banners)
        {
            foreach (AdBanner banner in banners)
            {
                if (banner.type == type.ToString())
                    return banner;
            }
            return null;
        }

        #region FullScreen Ads

        public event Action OnFullscreenStart;
        public event Action<bool> OnFullscreenClose;

        private void TrackFullscreen() => TrackBannerDisplay(BannerType.FULLSCREEN);

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnFullscreenStart?.Invoke();
                onFullscreenStart?.Invoke();
                TrackFullscreen();

                StopFrequencyTimer();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnFullscreenClose?.Invoke(success); 
                onFullscreenClose?.Invoke(success);

                StartFrequencyTimer();
            };

            if (!IsFullscreenAvailable())
            {
                Logger.Log("Not available");
                OnFullscreenClose?.Invoke(false);
                onFullscreenClose?.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            //OnShowFullscreen?.Invoke(combinedStart, combinedClose);
            adsMobile?.ShowFullscreen(combinedStart, combinedClose);
#else
            Logger.Log("FULL SCREEN AD ", "SHOW");
            combinedStart?.Invoke();
            combinedClose?.Invoke(true);
#endif
        }

        #endregion

        #region Rewarded Ads

        public event Action OnRewardedStart;
        public event Action<bool> OnRewardedClose;
        public event Action<string> OnRewardedReward;

        private void TrackRewarded() => TrackBannerDisplay(BannerType.REWARDED);

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnRewardedStart?.Invoke();
                onRewardedStart?.Invoke();
                TrackRewarded();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnRewardedClose?.Invoke(success);
                onRewardedClose?.Invoke(success);
            };

            Action<string> combinedReward = (string tag) =>
            {
                OnRewardedReward?.Invoke(tag);
                onRewardedReward?.Invoke(tag);
            };

            if (!IsRewardedAvailable())
            {
                Logger.Log("Not available");
                OnRewardedClose?.Invoke(false);
                onRewardedClose?.Invoke(false);
                return;
            }

            Logger.Log("Reward in ads module");
#if UNITY_ANDROID

            //OnShowRewarded?.Invoke(idOrTag, combinedReward, combinedStart, combinedClose);
            adsMobile?.ShowRewarded(idOrTag, combinedReward, combinedStart, combinedClose);
#else
            Logger.Log("SHOW REWARDED AD -> TAG: ", idOrTag);

            combinedStart.Invoke()
            combinedReward.Invoke(idOrTag);
            combinedClose.Invoke(close);
#endif
        }

        #endregion

        #region Sticky Ads

        public event Action OnStickyStart;
        public event Action<bool> OnStickyClose;
        public event Action OnStickyRefresh;
        public event Action OnStickyRender;

        private void TrackSticky() => TrackBannerDisplay(BannerType.STICKY);

        public void ShowSticky()
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnStickyStart?.Invoke();
                StartStickyRefresh();
            };

            Action combinedRefresh = () =>
            {
                OnStickyRefresh?.Invoke();
            };

            Action combinedRender = () =>
            {
                OnStickyRender?.Invoke();
                TrackSticky();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnStickyClose?.Invoke(success);
                StopStickyRefresh();
            };

            if (!IsStickyAvailable())
            {
                Logger.Log("Not available");
                OnStickyClose?.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            adsMobile?.ShowSticky(combinedStart, combinedClose, combinedRefresh, combinedRender);
#else
            Logger.Log("STICKY BANNER AD: ", "SHOW");
            combinedStart?.Invoke();
#endif

        }

        public void CloseSticky()
        {
#if UNITY_ANDROID
            adsMobile?.CloseSticky();
#else
            Logger.Log("STICKY BANNER AD: ", "CLOSE");
#endif
        }

        public void RefreshSticky()
        {
            if (!IsStickyAvailable())
            {
                Logger.Log("Not available");
                OnStickyClose?.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            StopStickyRefresh();
            adsMobile?.RefreshSticky();
            StartStickyRefresh();
#else
            Logger.Log("STICKY BANNER AD: ", "REFRESH");
#endif
        }

        private Task _stickyRefreshTimer;
        private int _refreshIntervalSeconds;
        private bool _isRefresh;

        public async void StartStickyRefresh()
        {
            if (_refreshIntervalSeconds > 0)
            //if (_stickyRefreshTimer == null && _refreshIntervalSeconds > 0)
            {
                _isRefresh = true;

                Logger.Log("Start Sticky Refresh Timer: " + _refreshIntervalSeconds);

                int interval = _refreshIntervalSeconds * 1000;

                _stickyRefreshTimer = Task.Delay(interval);
                await _stickyRefreshTimer;

                if (_isRefresh)
                {
                    RefreshSticky();
                    StartStickyRefresh();
                }
            }
        }

        public async void StopStickyRefresh()
        {
            if (_stickyRefreshTimer != null)
            {
                _isRefresh = false;

                Logger.Log("Stop Sticky Refresh Timer");
                await _stickyRefreshTimer;

                _stickyRefreshTimer.Dispose();
                _stickyRefreshTimer = null;
            }
        }



        #endregion

        #region Preload Ads

        public event Action OnPreloaderStart;
        public event Action<bool> OnPreloaderClose;

        private void TrackPreload() => TrackBannerDisplay(BannerType.PRELOADER);

        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnPreloaderStart?.Invoke();
                onPreloaderStart?.Invoke();
                TrackPreload();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnPreloaderClose?.Invoke(success);
                onPreloaderClose?.Invoke(success);
            };

            if (!IsPreloaderAvailable())
            {
                Logger.Log("Not available");
                OnPreloaderClose?.Invoke(false);
                onPreloaderClose?.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            adsMobile?.ShowPreloader(combinedStart, combinedClose);
#else
            Logger.Log("PRELOADER AD: ", "SHOW");
            combinedStart?.Invoke();
            combinedClose?.Invoke(true);
#endif
        }

        #endregion

        #region Is Methods

        public bool IsAdblockEnabled()
        {

#if UNITY_ANDROID
            return adsMobile == null ? false : adsMobile.IsAdblockEnabled();
#else
            Logger.Log("IS ADBLOCK ENABLED: ", "FALSE");
            return false;
#endif
        }

        public bool IsStickyAvailable()
        {
            ShowLimits(BannerType.STICKY);

            return
            //this.adapter.isStickyAvailable &&
            GetBanner(BannerType.STICKY).enabled &&
            !IsBannerLimitReached(BannerType.STICKY);

//#if UNITY_ANDROID
//            return adsMobile.IsStickyAvailable();
//#else
       
//            Logger.Log("IS STICKY BANNER AD AVAILABLE: ", "TRUE");
//            return false;
//#endif
        }

        public bool IsFullscreenAvailable()
        {
            ShowLimits(BannerType.FULLSCREEN);

            return
            !IsFullscreenPlaying() &&
            !IsRewardedPlaying() &&
            !IsPreloaderPlaying() &&
            //this.adapter.isFullscreenAvailable &&
            GetBanner(BannerType.FULLSCREEN).enabled &&
            !IsFullsceenTimer() &&
            !IsBannerLimitReached(BannerType.FULLSCREEN);

//#if UNITY_ANDROID
//            return adsMobile.IsFullscreenAvailable();
//#else
//            Logger.Log("IS FULL SCREEN AD AVAILABLE: ", "TRUE");
//            return false;
//#endif
        }

        public bool IsRewardedAvailable()
        {
            ShowLimits(BannerType.REWARDED);

            return
            !IsFullscreenPlaying() &&
            !IsRewardedPlaying() &&
            !IsPreloaderPlaying() &&
            //this.adapter.isRewardedAvailable &&
            GetBanner(BannerType.REWARDED).enabled &&
            !IsBannerLimitReached(BannerType.REWARDED);

//#if UNITY_ANDROID
//            return adsMobile.IsRewardedAvailable();
//#else
//            Logger.Log("IS REWARD AD AVAILABLE: ", "TRUE");
//            return false;
//#endif
        }

        public bool IsPreloaderAvailable()
        {
            ShowLimits(BannerType.PRELOADER);

            return
            !IsFullscreenPlaying() &&
            !IsRewardedPlaying() &&
            !IsPreloaderPlaying() &&
            //this.adapter.isPreloaderAvailable &&
            GetBanner(BannerType.PRELOADER).enabled &&
            !IsBannerLimitReached(BannerType.PRELOADER);
        
            //#if UNITY_ANDROID
            //            return adsMobile.IsPreloaderAvailable();
            //#else
            //            Logger.Log("IS PRELOADER AD AVAILABLE: ", "TRUE");
            //            return false;
            //#endif
        }

        public bool IsStickyPlaying()
        {
#if UNITY_ANDROID

            return adsMobile == null ? false : adsMobile.IsStickyPlaying();
#else
            Logger.Log("IS STICKY PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsFullscreenPlaying()
        {
#if UNITY_ANDROID
            return adsMobile == null ? false : adsMobile.IsFullscreenPlaying();
#else
            Logger.Log("IS FULLSCREEN AD PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsRewardedPlaying()
        {
#if UNITY_ANDROID
            return adsMobile == null ? false : adsMobile.IsRewardPlaying();
#else
            Logger.Log("IS REWARDED AD PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsPreloaderPlaying()
        {
#if UNITY_ANDROID
            return adsMobile == null ? false : adsMobile.IsPreloaderPlaying();
#else
            Logger.Log("IS PRELOADER AD PLAYING: ", "FALSE");
            return false;
#endif
        }

        public bool IsCountdownOverlayEnabled()
        {
            return adsConfig.showCountdownOverlay;
        }

        public bool IsRewardedFailedOverlayEnabled()
        {
            return adsConfig.showRewardedFailedOverlay;
        }

        public bool CanShowFullscreenBeforeGamePlay()
        {
            return false;
        }

        private bool IsBannerLimitReached(BannerType bannerType)
        {
            AdBanner banner = GetBanner(bannerType);
            ShowCounts(bannerType);

            bool hourLimits = false;
            bool dayLimits = false;
            bool sessionLimits = false;

            if (adsInfo == null)
                adsInfo = LoadAdsInfo();

            if (banner.limits.hour > 0)
                hourLimits = adsInfo.GetBanner(bannerType).hour.count >= banner.limits.hour;
            if (banner.limits.day > 0)
                dayLimits = adsInfo.GetBanner(bannerType).day.count >= banner.limits.day;
            if (banner.limits.session > 0)
                sessionLimits = adsInfo.GetBanner(bannerType).session.count >= banner.limits.session;

            return hourLimits || dayLimits || sessionLimits;
        }

        #endregion

        #region Fullscreen frequency

        private Task _frequencyTimer;
        private int _frequencyInterval;

        public bool IsFullsceenTimer()
        {
            bool isTimer = _frequencyTimer != null;
            Logger.Log("Fullscreen timer: " + isTimer);
            return isTimer;
        }

        private void SetUpTimer()
        {
            AdBanner adBanner = GetBanner(BannerType.FULLSCREEN);
            _frequencyInterval = adBanner.frequency;
        }

        public async void StartFrequencyTimer()
        {
            if (_frequencyTimer == null && _frequencyInterval > 0)
            {
                Logger.Log("Start Frequency Timer: " + _frequencyInterval);

                int interval = _frequencyInterval * 1000;
                _frequencyTimer = Task.Delay(interval);

                await _frequencyTimer;
                StopFrequencyTimer();
            }
        }

        public void StopFrequencyTimer()
        {
            if (_frequencyTimer != null)
            {
                Logger.Log("Stop Frequency Timer");
                _frequencyTimer.Dispose();
                _frequencyTimer = null;
            }
        }

        #endregion

        #region Limits Info

        public void ShowLimits(BannerType bannerType)
        {
            AdBanner adBanner = GetBanner(bannerType);

            Logger.Log(
                "\n" + bannerType.ToString() + " limits:" +
                "\n" + " Hour: " + adBanner.limits.hour +
                "\n" + " Day: " + adBanner.limits.day +
                "\n" + " Session: " + adBanner.limits.session
            );
        }

        public void ShowCounts(BannerType bannerType)
        {
            BannerLimitInfo limits = adsInfo.GetBanner(bannerType);

            Logger.Log(
                "\n" + bannerType.ToString() + " counts:" +
                "\n" + " Hour: " + limits.hour.count +
                "\n" + " Day: " + limits.day.count +
                "\n" + " Session: " + limits.session.count
            );
        }

        private void TrackBannerDisplay(BannerType bannerType)
        {
            BannerLimitInfo limits = adsInfo.GetBanner(bannerType);
            limits.hour.count += 1;
            limits.day.count += 1;
            limits.session.count += 1;

            if (limits.day.timestamp == 0)
            {
                limits.day.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
            if (limits.hour.timestamp == 0)
            {
                limits.hour.timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }

            adsInfo.SetBanner(bannerType, limits);
            ShowCounts(bannerType);
            SaveAdsInfo();
        }

        public PlayerAdsInfo LoadAdsInfo()
        {
            try
            {
                var json = PlayerPrefs.GetString("adsInfo", null);
                Logger.Log(json);
                var info = string.IsNullOrEmpty(json) ? new PlayerAdsInfo() : JsonUtility.FromJson<PlayerAdsInfo>(json);
                
                return info;
            }
            catch (System.Exception ex)
            {
                Logger.Warn($"Warning: {ex.Message}");
                //_tempAdsInfo = _adsInfoStub;
                return new PlayerAdsInfo();
            }
        }

        public void SaveAdsInfo()
        {
            Logger.Log("Save Ad Info");
            var json = JsonUtility.ToJson(adsInfo);
            //Logger.Log(json);
            PlayerPrefs.SetString("adsInfo", json);
            PlayerPrefs.Save();
        }

        private bool IsTimestampExpired(long timestamp, long range)
        {
            var msDiff = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestamp;
            //Logger.Log(msDiff);
            return msDiff > range;
        }

        private const long HOUR = 3600000; // 1 час в миллисекундах
        private const long DAY = 86400000; // 1 день в миллисекундах

        public void CheckLimitsExpired() => CheckLimitsExpired(false);
        private void CheckLimitsExpired(bool isInit)
        {
            if (CheckBannerLimitsExpired(isInit, adsInfo.FULLSCREEN) ||
                CheckBannerLimitsExpired(isInit, adsInfo.PRELOADER) ||
                CheckBannerLimitsExpired(isInit, adsInfo.REWARDED) ||
                CheckBannerLimitsExpired(isInit, adsInfo.STICKY) )
            {
                SaveAdsInfo();
            }
        }

        private bool CheckBannerLimitsExpired(bool isInit, BannerLimitInfo info)
        {
            bool hasChanges = false;

            if (isInit)
            {
                info.session.count = 0;
            }

            if (info.hour.timestamp != 0 && IsTimestampExpired(info.hour.timestamp, HOUR))
            {
                info.hour.timestamp = 0;
                info.hour.count = 0;
                hasChanges = true;
            }

            if (info.day.timestamp != 0 && IsTimestampExpired(info.day.timestamp, DAY))
            {
                info.day.timestamp = 0;
                info.day.count = 0;
                hasChanges = true;
            }

            return hasChanges;
        }

            #endregion
    }
}
