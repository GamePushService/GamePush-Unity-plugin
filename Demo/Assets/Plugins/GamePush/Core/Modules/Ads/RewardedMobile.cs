using System;
using YandexMobileAds;
using YandexMobileAds.Base;
using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class RewardedMobile
    {

#if YANDEX_SIMPLE_MONETIZATION
        private RewardedAdLoader rewardedAdLoader;
        private RewardedAd rewardedAd;
#endif
        private AdBanner _bannerData;

        private string _rewardTag;
        private bool _isPlaying;
        private void SetPlaying(bool isPlay) => _isPlaying = isPlay;

        public bool IsPlaying() => _isPlaying;

        public event Action<string> OnRewardedReward;
        public event Action OnRewardedStart;
        public event Action<bool> OnRewardedClose;

        public RewardedMobile(AdBanner banner)
        {
            _bannerData = banner;

            SetUpLoader();
            RequestRewardedAd();
        }

        public void SetUpLoader()
        {
#if YANDEX_SIMPLE_MONETIZATION
            rewardedAdLoader = new RewardedAdLoader();
            rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
#endif
        }

        private void RequestRewardedAd()
        {
            Logger.Log("RewardedAd is not ready yet");

#if YANDEX_SIMPLE_MONETIZATION
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }

            string adUnitId = _bannerData.bannerId;

            rewardedAdLoader.LoadAd(CreateAdRequest(adUnitId));
            Logger.Log("Rewarded Ad is requested");
#endif
        }


        public void ShowRewardedAd(
            string idOrTag = "COINS",
            Action<string> onRewardedReward = null,
            Action onRewardedStart = null,
            Action<bool> onRewardedClose = null
            )
        {
            Logger.Log("Reward in rewarded mobile");

#if YANDEX_SIMPLE_MONETIZATION
            if (rewardedAd == null)
            {
                Logger.Log("RewardedAd is not ready yet");
                onRewardedClose?.Invoke(false);
                return;
            }

            _rewardTag = idOrTag;

            OnRewardedReward = onRewardedReward;
            OnRewardedStart = onRewardedStart;
            OnRewardedClose = onRewardedClose;

            rewardedAd.Show();
#else
            onRewardedClose?.Invoke(false);
#endif
            SetPlaying(true);
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        private void DestroyBanner(bool success)
        {
#if YANDEX_SIMPLE_MONETIZATION
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }
#endif

            SetPlaying(false);
        }

        #region Rewarded Ad callback handlers

#if YANDEX_SIMPLE_MONETIZATION
        public void HandleAdLoaded(object sender, RewardedAdLoadedEventArgs args)
        {
            Logger.Log("HandleAdLoaded event received");
            rewardedAd = args.RewardedAd;

            rewardedAd.OnAdClicked += HandleAdClicked;
            rewardedAd.OnAdShown += HandleAdShown;
            rewardedAd.OnAdFailedToShow += HandleAdFailedToShow;
            rewardedAd.OnAdImpression += HandleImpression;
            rewardedAd.OnAdDismissed += HandleAdDismissed;
            rewardedAd.OnRewarded += HandleRewarded;
        }

        public void HandleAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Logger.Log($"HandleAdFailedToLoad event received with message: {args.Message}");
        }

        public void HandleAdClicked(object sender, EventArgs args)
        {
            Logger.Log("HandleAdClicked event received");
        }

        public void HandleAdShown(object sender, EventArgs args)
        {
            Logger.Log("HandleAdShown event received");
            OnRewardedStart?.Invoke();
        }

        public void HandleAdDismissed(object sender, EventArgs args)
        {
            Logger.Log("HandleAdDismissed event received");

            
            RequestRewardedAd();
        }

        public void HandleImpression(object sender, ImpressionData impressionData)
        {
            var data = impressionData == null ? "null" : impressionData.rawData;
            //Logger.Log($"HandleImpression event received with data: {data}");

            DestroyBanner(true);
            RequestRewardedAd();
        }

        public void HandleRewarded(object sender, Reward args)
        {
            Logger.Log($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");
            OnRewardedReward?.Invoke(_rewardTag);
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Logger.Log(
                $"HandleAdFailedToShow event received with message: {args.Message}");

            DestroyBanner(false);
            RequestRewardedAd();
        }
#endif
        #endregion

    }
}
