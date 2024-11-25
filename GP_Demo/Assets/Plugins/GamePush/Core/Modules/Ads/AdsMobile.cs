using System;
using System.Collections.Generic;

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
        {
            if (sticky != null)
                sticky.ShowBanner(onStickyStart, onStickyClose, onStickyRefresh, onStickyRender);
            else
                onStickyClose?.Invoke(false);

        }

        public void RefreshSticky() => sticky?.RefreshBanner();
        public void CloseSticky() => sticky?.CloseBanner();

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            if (fullscreen != null)
                fullscreen.ShowInterstitial(onFullscreenStart, onFullscreenClose);
            else
                onFullscreenClose?.Invoke(false);
        }

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            if (rewarded != null)
                rewarded.ShowRewardedAd(idOrTag, onRewardedReward, onRewardedStart, onRewardedClose);
            else
                onRewardedClose?.Invoke(false);
        }

        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            if (preloader != null)
                preloader.ShowAppOpenAd(onPreloaderStart, onPreloaderClose);
            else
                onPreloaderClose?.Invoke(false);
        }

        public bool IsStickyPlaying()
        {
            if (sticky != null)
                return sticky.IsPlaying();
            else
                return false;
        }

        public bool IsFullscreenPlaying()
        {
            if (fullscreen != null)
                return fullscreen.IsPlaying();
            else
                return false;
        }

        public bool IsRewardPlaying()
        {
            if (fullscreen != null)
                return rewarded.IsPlaying();
            else
                return false;
        }

        public bool IsPreloaderPlaying()
        {
            if (fullscreen != null)
                return preloader.IsPlaying();
            else
                return false;
        }


#else
        public void ShowSticky(
           Action onStickyStart = null,
           Action<bool> onStickyClose = null,
           Action onStickyRefresh = null,
           Action onStickyRender = null)
        {
            //onStickyStart?.Invoke();
            onStickyClose?.Invoke(false);
        }

        public void RefreshSticky(){}
        public void CloseSticky(){}

        public void ShowFullscreen(Action onFullscreenStart = null, Action<bool> onFullscreenClose = null)
        {
            //onFullscreenStart?.Invoke();
            onFullscreenClose?.Invoke(false);
        }

        public void ShowRewarded(string idOrTag = "COINS", Action<string> onRewardedReward = null, Action onRewardedStart = null, Action<bool> onRewardedClose = null)
        {
            //onRewardedStart?.Invoke();
            onRewardedClose?.Invoke(false);
        }

        public void ShowPreloader(Action onPreloaderStart = null, Action<bool> onPreloaderClose = null)
        {
            //onPreloaderStart?.Invoke();
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
