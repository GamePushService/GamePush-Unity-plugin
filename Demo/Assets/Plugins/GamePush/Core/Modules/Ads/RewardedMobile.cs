using System;
using YandexMobileAds;
using YandexMobileAds.Base;
using GamePush.Core;
using GamePush.Data;

namespace GamePush.Mobile
{
    public class RewardedMobile
    {
        private RewardedAdLoader rewardedAdLoader;
        private RewardedAd rewardedAd;

        private AdBanner bannerData;

        private string rewardTag;
        private bool isPlaying;
        private void SetPlaying(bool isPlay) => isPlaying = isPlay;

        public bool IsPlaying() => isPlaying;

        public event Action<string> OnRewardedReward;
        public event Action OnRewardedStart;
        public event Action<bool> OnRewardedClose;

        public RewardedMobile(AdBanner banner)
        {
            bannerData = banner;

            SetUp();
            RequestRewardedAd();
        }

        public void SetUp()
        {
            rewardedAdLoader = new RewardedAdLoader();
            rewardedAdLoader.OnAdLoaded += HandleAdLoaded;
            rewardedAdLoader.OnAdFailedToLoad += HandleAdFailedToLoad;
        }

        private void RequestRewardedAd()
        {
            Logger.Log("RewardedAd is not ready yet");
            //Sets COPPA restriction for user age under 13
            MobileAds.SetAgeRestrictedUser(true);

            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
            }

            string adUnitId = bannerData.bannerId;


            rewardedAdLoader.LoadAd(CreateAdRequest(adUnitId));
            Logger.Log("Rewarded Ad is requested");
        }


        public void ShowRewardedAd(
            string idOrTag = "COINS",
            Action<string> onRewardedReward = null,
            Action onRewardedStart = null,
            Action<bool> onRewardedClose = null
            )
        {
            if (rewardedAd == null)
            {
                Logger.Log("RewardedAd is not ready yet");
                onRewardedClose(false);
                return;
            }

            rewardTag = idOrTag;

            OnRewardedReward = onRewardedReward;
            OnRewardedStart = onRewardedStart;
            OnRewardedClose = onRewardedClose;

            rewardedAd.Show();
            onRewardedStart?.Invoke();
            SetPlaying(true);
        }

        private AdRequestConfiguration CreateAdRequest(string adUnitId)
        {
            return new AdRequestConfiguration.Builder(adUnitId).Build();
        }

        private void DestroyBanner(bool success)
        {
            if (rewardedAd != null)
            {
                rewardedAd.Destroy();
                rewardedAd = null;
            }

            
            SetPlaying(false);
        }

        #region Rewarded Ad callback handlers

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
            Logger.Log($"HandleImpression event received with data: {data}");

            DestroyBanner(true);
        }

        public void HandleRewarded(object sender, Reward args)
        {
            Logger.Log($"HandleRewarded event received: amout = {args.amount}, type = {args.type}");
            OnRewardedReward?.Invoke(rewardTag);
        }

        public void HandleAdFailedToShow(object sender, AdFailureEventArgs args)
        {
            Logger.Log(
                $"HandleAdFailedToShow event received with message: {args.Message}");

            DestroyBanner(false);
            RequestRewardedAd();
        }

        #endregion

    }
}