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

#if UNITY_ANDROID && CUSTOM_ADS_MOBILE

        private StickyMobile sticky;
        private InterstitialMobile fullscreen;
        private RewardedMobile rewarded;
        private PreloaderMobile preloader;

        public void Init(BannersList bannersList)
        {
            foreach (AdBanner banner in bannersList.banners)
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
           Action onStickyRefresh = null,
           Action onStickyRender = null)
           => sticky.ShowBanner(onStickyStart, onStickyClose, onStickyRefresh, onStickyRender);

        public void RefreshSticky() => sticky.RefreshBanner();
        public void CloseSticky() => sticky.CloseBanner();

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            fullscreen.ShowInterstitial(onFullscreenStart, onFullscreenClose);
        }

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            rewarded.ShowRewardedAd(idOrTag, onRewardedReward, onRewardedStart, onRewardedClose);
        }

        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            preloader.ShowAppOpenAd(onPreloaderStart, onPreloaderClose);
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


#else
        public void ShowSticky(
           Action onStickyStart = null,
           Action<bool> onStickyClose = null,
           Action onStickyRefresh = null,
           Action onStickyRender = null)
        {
            onStickyStart?.Invoke();
            onStickyClose?.Invoke(false);
        }

        public void RefreshSticky(){}
        public void CloseSticky(){}

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            onFullscreenStart?.Invoke();
            onFullscreenClose?.Invoke(false);
        }

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            onRewardedStart?.Invoke();
            onRewardedClose?.Invoke(false);
        }

        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            onPreloaderStart?.Invoke();
            onPreloaderClose?.Invoke(false);
        }

        public bool IsFullScreenAvailable() => false;
        public bool IsStickyPlaying() => false;
        public bool IsFullscreenPlaying() => false;
        public bool IsRewardPlaying() => false;
        public bool IsPreloaderPlaying() => false;
        
#endif
        public bool IsAdblockEnabled()
        {
            return false;
        }

    }

}
