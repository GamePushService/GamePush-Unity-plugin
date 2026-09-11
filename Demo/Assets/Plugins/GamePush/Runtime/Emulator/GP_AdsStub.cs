#if UNITY_EDITOR
using GamePush.Data;
using UnityEngine;

namespace GamePush
{
    public enum GP_AdsStubKind
    {
        None,
        Preloader,
        Fullscreen,
        Rewarded
    }

    public static class GP_AdsStub
    {
        public static bool Enabled => ProjectData.ADS_STUBS && !GP_Play2Web.Enabled;

        public static GP_AdsStubKind Kind { get; private set; }
        public static string RewardedTag { get; private set; }

        public static bool IsPlaying => Kind != GP_AdsStubKind.None;
        public static bool IsPaused => IsPlaying;
        public static bool IsPreloaderPlaying => Kind == GP_AdsStubKind.Preloader;
        public static bool IsFullscreenPlaying => Kind == GP_AdsStubKind.Fullscreen;
        public static bool IsRewardedPlaying => Kind == GP_AdsStubKind.Rewarded;

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void ResetStatics()
        {
            Kind = GP_AdsStubKind.None;
            RewardedTag = null;
        }

        public static void ShowPreloader() => Begin(GP_AdsStubKind.Preloader, null);

        public static void ShowFullscreen() => Begin(GP_AdsStubKind.Fullscreen, null);

        public static void ShowRewarded(string idOrTag) => Begin(GP_AdsStubKind.Rewarded, idOrTag);

        public static void Complete(bool success)
        {
            if (!IsPlaying)
                return;

            GP_AdsStubKind kind = Kind;
            string tag = RewardedTag;
            Kind = GP_AdsStubKind.None;
            RewardedTag = null;

            if (kind == GP_AdsStubKind.Rewarded && success)
                GP_Ads.FireRewardedReward(tag);

            switch (kind)
            {
                case GP_AdsStubKind.Preloader:
                    GP_Ads.FirePreloaderClose(success);
                    break;
                case GP_AdsStubKind.Fullscreen:
                    GP_Ads.FireFullscreenClose(success);
                    break;
                case GP_AdsStubKind.Rewarded:
                    GP_Ads.FireRewardedClose(success);
                    break;
            }

            GP_Ads.FireAdsClose(success);
            GP_Game.FireResume();
        }

        private static void Begin(GP_AdsStubKind kind, string idOrTag)
        {
            if (IsPlaying)
            {
                GP_Logger.ModuleLog("AD STUB ALREADY PLAYING", ModuleName.Ads);
                return;
            }

            if (!IsAvailable(kind))
            {
                GP_Logger.ModuleLog($"{kind.ToString().ToUpper()} AD STUB: NOT AVAILABLE", ModuleName.Ads);
                FireCloseOnly(kind, false);
                return;
            }

            Kind = kind;
            RewardedTag = idOrTag;

            GP_Ads.FireAdsStart();
            switch (kind)
            {
                case GP_AdsStubKind.Preloader:
                    GP_Ads.FirePreloaderStart();
                    break;
                case GP_AdsStubKind.Fullscreen:
                    GP_Ads.FireFullscreenStart();
                    break;
                case GP_AdsStubKind.Rewarded:
                    GP_Ads.FireRewardedStart();
                    break;
            }

            GP_Game.FirePause();
        }

        private static void FireCloseOnly(GP_AdsStubKind kind, bool success)
        {
            switch (kind)
            {
                case GP_AdsStubKind.Preloader:
                    GP_Ads.FirePreloaderClose(success);
                    break;
                case GP_AdsStubKind.Fullscreen:
                    GP_Ads.FireFullscreenClose(success);
                    break;
                case GP_AdsStubKind.Rewarded:
                    GP_Ads.FireRewardedClose(success);
                    break;
            }

            GP_Ads.FireAdsClose(success);
        }

        private static bool IsAvailable(GP_AdsStubKind kind)
        {
            if (GP_Settings.instance.platformSettings == null)
                return true;

            PlatformSettings settings = GP_Settings.instance.GetPlatformSettings();
            switch (kind)
            {
                case GP_AdsStubKind.Preloader:
                    return settings.IsPreloaderAvailable;
                case GP_AdsStubKind.Fullscreen:
                    return settings.IsFullscreenAvailable;
                case GP_AdsStubKind.Rewarded:
                    return settings.IsRewardedAvailable;
                default:
                    return false;
            }
        }
    }
}
#endif
