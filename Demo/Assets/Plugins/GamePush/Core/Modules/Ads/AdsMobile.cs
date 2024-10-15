using System;
using System.Collections.Generic;
using YandexMobileAds;
using YandexMobileAds.Base;

using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class AdsMobile
    {
        private List<AdBanner> banners;

        private StickyMobile sticky;
        private InterstitialMobile fullscreen;
        private RewardedMobile rewarded;
        private PreloaderMobile preloader;

        public void Init(BannersList bannersList)
        {
            banners = bannersList.banners;
            foreach(AdBanner banner in bannersList.banners)
            {
                Enum.TryParse(banner.type, out BannerType type);

                switch (type)
                {
                    case BannerType.PRELOADER:
                        //Logger.Log("Type PRELOADER");
                        preloader = new PreloaderMobile(banner);
                        break;
                    case BannerType.FULLSCREEN:
                        //Logger.Log("Type FULLSCREEN");
                        fullscreen = new InterstitialMobile(banner);
                        break;
                    case BannerType.REWARDED:
                        //Logger.Log("Type REWARDED");
                        rewarded = new RewardedMobile(banner);
                        break;
                    case BannerType.STICKY:
                        //Logger.Log("Type STICKY");
                        sticky = new StickyMobile(banner);
                        break;
                }
            }
        }

        public void ShowSticky(
            Action onStickyStart = null,
            Action<bool> onStickyClose = null,
            Action onStickyRefresh = null)
            => sticky.ShowBanner(onStickyStart, onStickyClose, onStickyRefresh);

        public void RefreshSticky() => sticky.RefreshBanner();
        public void CloseSticky() => sticky.CloseBanner();

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            fullscreen.ShowInterstitial(onFullscreenStart, onFullscreenClose);
        }

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            Logger.Log("Reward in ads mobile");
            rewarded.ShowRewardedAd(idOrTag, onRewardedReward, onRewardedStart, onRewardedClose);
        }

        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            preloader.ShowAppOpenAd(onPreloaderStart, onPreloaderClose);
        }

        public bool IsAdblockEnabled()
        {
            return false;
        }

        public bool IsStickyPlaying()
        {
            return sticky.IsPlaying();
        }

        public bool IsFullscreenPlaying()
        {
            return fullscreen.IsPlaying();
        }

        public bool IsRewardPlaying()
        {
            return rewarded.IsPlaying();
        }

        public bool IsPreloaderPlaying()
        {
            return preloader.IsPlaying();
        }
    }

}
