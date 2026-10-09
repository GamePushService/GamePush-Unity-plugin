using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public static class NativeAds
    {
        static bool _fullscreenPlaying;
        static bool _rewardedPlaying;
        static bool _preloaderPlaying;
        static bool _stickyPlaying;
        static readonly Dictionary<string, int> HourCounts = new Dictionary<string, int>();
        static readonly Dictionary<string, int> DayCounts = new Dictionary<string, int>();
        static string _hourKey;
        static string _dayKey;

        public static bool IsFullscreenPlaying => _fullscreenPlaying;
        public static bool IsRewardedPlaying => _rewardedPlaying;
        public static bool IsPreloaderPlaying => _preloaderPlaying;
        public static bool IsStickyPlaying => _stickyPlaying;

        public static bool IsAvailable(string type)
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var banner = NativeCore.Ads.Get(type);
            return CanShow(banner);
#else
            return false;
#endif
        }

        public static void ShowFullscreen(bool forceCountdown = false)
        {
            WithCountdown(forceCountdown, () => Show("FULLSCREEN", false, null));
        }

        public static void ShowRewarded(string tag)
        {
            WithCountdown(false, () => Show("REWARDED", true, tag));
        }

        /// <summary>
        /// Runs the ad behind the countdown curtain when the project asks for it; otherwise
        /// starts it right away.
        /// </summary>
        static void WithCountdown(bool force, Action start)
        {
            if (!force && !NativeCore.ShowAdCountdownOverlay)
            {
                start();
                return;
            }
            var opened = Overlays.GP_Overlays.Open(Overlays.GP_OverlayKind.AdCountdown,
                new Overlays.GP_AdCountdownArgs { seconds = 3f, onDone = start });
            if (!opened)
                start();
        }

        public static void ShowPreloader()
        {
            Show("PRELOADER", false, null);
        }

        public static void ShowSticky()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            var banner = NativeCore.Ads.Get("STICKY");
            if (!CanShow(banner))
                return;
            _stickyPlaying = NativeAdsSdks.Current.ShowBanner(banner.BannerId, banner.Position);
            if (_stickyPlaying)
            {
                NativeMainThread.Run(() => GP_Ads.FireStickyStart());
                NoteShown(banner);
            }
#else
            GP_Logger.Info("ADS", "Sticky unavailable on this platform");
#endif
        }

        public static void CloseSticky()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            NativeAdsSdks.Current.HideBanner();
#endif
            _stickyPlaying = false;
            NativeMainThread.Run(() => GP_Ads.FireStickyClose());
        }

        public static void RefreshSticky()
        {
            CloseSticky();
            ShowSticky();
            NativeMainThread.Run(() => GP_Ads.FireStickyRefresh());
        }

        static void Show(string type, bool rewarded, string tag)
        {
#if !UNITY_ANDROID || UNITY_EDITOR
            NativeMainThread.Run(() =>
            {
                GP_Ads.FireAdsStart();
                if (type == "FULLSCREEN") GP_Ads.FireFullscreenStart();
                if (type == "REWARDED") GP_Ads.FireRewardedStart();
                if (type == "PRELOADER") GP_Ads.FirePreloaderStart();
                Complete(type, rewarded, tag, false);
            });
            return;
#else
            var banner = NativeCore.Ads.Get(type);
            if (!CanShow(banner) || LimitReached(banner))
            {
                if (banner != null && !banner.AdServerSupported)
                    GP_Logger.Warn("ADS", "Ad Server [" + banner.AdServer + "] is not supported for " + type);
                Complete(type, rewarded, tag, false);
                return;
            }

            SetPlaying(type, true);
            NativeMainThread.Run(() =>
            {
                GP_Ads.FireAdsStart();
                if (type == "FULLSCREEN") GP_Ads.FireFullscreenStart();
                if (type == "REWARDED") GP_Ads.FireRewardedStart();
                if (type == "PRELOADER") GP_Ads.FirePreloaderStart();
            });

            NativeAdsSdks.Current.Show(type, banner.BannerId, success =>
            {
                NoteShown(banner);
                Complete(type, rewarded, tag, success);
            });
#endif
        }

        static bool CanShow(NativeBanner banner)
        {
            if (banner == null || !banner.Enabled || string.IsNullOrEmpty(banner.BannerId) || !banner.AdServerSupported)
                return false;
            if (!NativeAdsSdks.Current.IsAvailable)
            {
                NativeAdsSdks.LogMissingOnce();
                return false;
            }
            return !LimitReached(banner);
        }

        static void Complete(string type, bool rewarded, string tag, bool success)
        {
            SetPlaying(type, false);
            NativeMainThread.Run(() =>
            {
                if (rewarded && success)
                    GP_Ads.FireRewardedReward(tag ?? "");
                if (rewarded && !success && NativeCore.ShowRewardedFailedOverlay)
                    Overlays.GP_Overlays.Open(Overlays.GP_OverlayKind.AdFailed, new Overlays.GP_AdFailedArgs());
                if (type == "FULLSCREEN") GP_Ads.FireFullscreenClose(success);
                if (type == "REWARDED") GP_Ads.FireRewardedClose(success);
                if (type == "PRELOADER") GP_Ads.FirePreloaderClose(success);
                GP_Ads.FireAdsClose(success);
            });
        }

        static void SetPlaying(string type, bool playing)
        {
            if (type == "FULLSCREEN") _fullscreenPlaying = playing;
            if (type == "REWARDED") _rewardedPlaying = playing;
            if (type == "PRELOADER") _preloaderPlaying = playing;
        }

        static bool LimitReached(NativeBanner banner)
        {
            RotateWindows();
            if (banner.SessionLimit > 0 && banner.SessionShown >= banner.SessionLimit)
                return true;
            if (banner.Frequency > 0 && Time.unscaledTime - banner.LastShownAt < banner.Frequency)
                return true;
            if (banner.HourLimit > 0 && HourCounts.TryGetValue(banner.Type, out var hour) && hour >= banner.HourLimit)
                return true;
            if (banner.DayLimit > 0 && DayCounts.TryGetValue(banner.Type, out var day) && day >= banner.DayLimit)
                return true;
            return false;
        }

        static void NoteShown(NativeBanner banner)
        {
            RotateWindows();
            banner.SessionShown++;
            banner.LastShownAt = Time.unscaledTime;
            HourCounts.TryGetValue(banner.Type, out var hour);
            HourCounts[banner.Type] = hour + 1;
            DayCounts.TryGetValue(banner.Type, out var day);
            DayCounts[banner.Type] = day + 1;
        }

        static void RotateWindows()
        {
            var hour = DateTime.UtcNow.ToString("yyyyMMddHH");
            var day = DateTime.UtcNow.ToString("yyyyMMdd");
            if (_hourKey != hour)
            {
                HourCounts.Clear();
                _hourKey = hour;
            }
            if (_dayKey != day)
            {
                DayCounts.Clear();
                _dayKey = day;
            }
        }
    }
}
