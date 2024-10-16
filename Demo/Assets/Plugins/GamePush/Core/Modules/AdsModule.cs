using System;
using System.Timers;
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

        public AdsModule()
        {
            adsInfo = LoadAdsInfo();
        }

        //public event Action<Action, Action<bool>> OnShowFullscreen;
        //public event Action<string, Action<string>, Action, Action<bool>> OnShowRewarded;

        public void Init(AdsConfig config, PlatformConfig platformConfig)
        {
            adsConfig = config;
            banners = platformConfig.banners;
            customAds = platformConfig.customAdsConfig;
            CheckLimitsExpired(true);
        }

        public void LateInit()
        {

#if UNITY_ANDROID && CUSTOM_ADS_MOBILE
            adsMobile = new AdsMobile();
            adsMobile.Init(customAds.configs.android);
            SetUpTimer();
#endif
        }

        private AdBanner GetBanner(BannerType type)
        {
            foreach(AdBanner banner in banners)
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
                combinedClose.Invoke(false);
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
                combinedClose.Invoke(false);
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
        //public event Action OnStickyRender;

        private void TrackSticky() => TrackBannerDisplay(BannerType.STICKY);

        public void ShowSticky()
        {
            Action combinedStart = () =>
            {
                OnAdsStart?.Invoke();
                OnStickyStart?.Invoke();
                TrackSticky();
            };

            Action<bool> combinedClose = (bool success) =>
            {
                OnAdsClose?.Invoke(success);
                OnStickyClose?.Invoke(success);
            };

            if (!IsStickyAvailable())
            {
                combinedClose.Invoke(false);
                return;
            }

#if UNITY_ANDROID
            adsMobile?.ShowSticky(combinedStart, combinedClose, OnStickyRefresh);
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
#if UNITY_ANDROID
            adsMobile?.RefreshSticky();
#else
            Logger.Log("STICKY BANNER AD: ", "REFRESH");
#endif
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
                combinedClose.Invoke(false);
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
            IsFullsceenTimer() &&
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
            bool hourLimits = false;
            bool dayLimits = false;
            bool sessionLimits = false;

            if (banner.limits.hour > 0)
                hourLimits = adsInfo.limits[bannerType].hour.count >= banner.limits.hour;
            if (banner.limits.day > 0)
                dayLimits = adsInfo.limits[bannerType].day.count >= banner.limits.day;
            if (banner.limits.session > 0)
                sessionLimits = adsInfo.limits[bannerType].session.count >= banner.limits.session;

            return hourLimits || dayLimits || sessionLimits;
        }

        #endregion

        #region Fullscreen frequency

        private Timer _frequencyTimer;
        private int _frequencyInterval;

        public bool IsFullsceenTimer() => _frequencyTimer == null;

        private void SetUpTimer()
        {
            AdBanner adBanner = GetBanner(BannerType.FULLSCREEN);
            _frequencyInterval = adBanner.frequency;
        }

        public void StartFrequencyTimer()
        {
            if (_frequencyTimer == null && _frequencyInterval > 0)
            {
                Logger.Log("Start Frequency Timer: " + _frequencyInterval);
                _frequencyTimer = new Timer(_frequencyInterval * 1000);
                _frequencyTimer.Elapsed += (sender, e) => StopFrequencyTimer();
            }
        }

        public void StopFrequencyTimer()
        {
            if (_frequencyTimer != null)
            {
                Logger.Log("Stop Frequency Timer");
                _frequencyTimer.Stop();
                _frequencyTimer.Dispose();
                _frequencyTimer = null;
            }
        }

        #endregion

        #region Limits Info

        public void ShowLimits(BannerType bannerType)
        {
            AdBanner adBanner = GetBanner(bannerType);

            Logger.Log(bannerType.ToString() + " limits:");
            Logger.Log(" Hour: " + adBanner.limits.hour);
            Logger.Log(" Day: " + adBanner.limits.day);
            Logger.Log(" Session: " + adBanner.limits.session);
        }

        private void TrackBannerDisplay(BannerType bannerType)
        {
            BannerLimitInfo limits = adsInfo.limits[bannerType];
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

            Logger.Log(bannerType.ToString() + " counts:");
            Logger.Log(" Hour: " + limits.hour.count);
            Logger.Log(" Day: " + limits.day.count);
            Logger.Log(" Session: " + limits.session.count);

            adsInfo.limits[bannerType] = limits;
            SaveAdsInfo();
        }

        public PlayerAdsInfo LoadAdsInfo()
        {
            try
            {
                var json = PlayerPrefs.GetString("adsInfo", null);
                var info = string.IsNullOrEmpty(json) ? null : JsonUtility.FromJson<PlayerAdsInfo>(json);
                //var result = MergeAdsInfo(_adsInfoStub, info);
                //_tempAdsInfo = result;
                //return result;
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
            try
            {
                var json = JsonUtility.ToJson(adsInfo);
                PlayerPrefs.SetString("adsInfo", json);
                PlayerPrefs.Save();
            }
            catch (System.Exception ex)
            {
                Debug.LogWarning($"Warning: {ex.Message}");
            }
        }

        private bool IsTimestampExpired(long timestamp, long range)
        {
            var msDiff = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() - timestamp;
            return msDiff > range;
        }

        private const long HOUR = 3600000; // 1 час в миллисекундах
        private const long DAY = 86400000; // 1 день в миллисекундах

        public Dictionary<string, BannerLimitInfo> AdsInfoLimits { get; private set; } = new Dictionary<string, BannerLimitInfo>();

        public void CheckLimitsExpired(bool isInit)
        {
            bool hasChanges = false;

            foreach (var info in AdsInfoLimits.Values)
            {
                if (isInit)
                {
                    info.session.count = 0;
                }

                if (IsTimestampExpired(info.hour.timestamp, HOUR))
                {
                    info.hour.timestamp = 0;
                    info.hour.count = 0;
                    hasChanges = true;
                }

                if (IsTimestampExpired(info.day.timestamp, DAY))
                {
                    info.day.timestamp = 0;
                    info.day.count = 0;
                    hasChanges = true;
                }
            }

            if (hasChanges)
            {
                SaveAdsInfo();
            }
        }

            #endregion
        }
}
