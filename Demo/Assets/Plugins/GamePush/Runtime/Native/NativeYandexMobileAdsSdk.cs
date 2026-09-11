#if GP_YANDEX_MOBILE_ADS
using System;
using UnityEngine;
using YandexMobileAds;
using YandexMobileAds.Base;

namespace GamePush.Native
{
    sealed class NativeYandexMobileAdsSdk : INativeAdsSdk
    {
        InterstitialAdLoader _interstitialLoader;
        RewardedAdLoader _rewardedLoader;
        AppOpenAdLoader _appOpenLoader;
        object _shownAd;
        Banner _banner;
        bool _initialized;

        public bool IsAvailable => true;

        public void Initialize()
        {
            if (_initialized)
                return;
            _interstitialLoader = new InterstitialAdLoader();
            _rewardedLoader = new RewardedAdLoader();
            _appOpenLoader = new AppOpenAdLoader();
            _initialized = true;
        }

        public void Show(string type, string adUnitId, Action<bool> done)
        {
            Initialize();
            if (string.IsNullOrEmpty(adUnitId))
            {
                done?.Invoke(false);
                return;
            }

            var request = new AdRequest(adUnitId);
            if (type == "REWARDED")
            {
                _rewardedLoader.LoadAd(request,
                    onLoaded: ad => PresentRewarded(ad, done),
                    onFailed: _ => done?.Invoke(false));
                return;
            }

            var useAppOpen = type == "PRELOADER" && Screen.height >= Screen.width;
            if (useAppOpen)
            {
                _appOpenLoader.LoadAd(request,
                    onLoaded: ad => PresentAppOpen(ad, done),
                    onFailed: _ => done?.Invoke(false));
                return;
            }

            _interstitialLoader.LoadAd(request,
                onLoaded: ad => PresentInterstitial(ad, done),
                onFailed: _ => done?.Invoke(false));
        }

        public bool ShowBanner(string adUnitId, string position)
        {
            Initialize();
            if (string.IsNullOrEmpty(adUnitId))
                return false;
            HideBanner();
            var adPosition = string.Equals(position, "top", StringComparison.OrdinalIgnoreCase)
                ? AdPosition.TopCenter
                : AdPosition.BottomCenter;
            var widthDp = ScreenUtils.ConvertPixelsToDp((int)Screen.safeArea.width);
            _banner = new Banner(BannerAdSize.Sticky(widthDp), adPosition);
            _banner.OnAdLoaded += (_, __) =>
            {
                try { _banner?.Show(); }
                catch (Exception exception)
                {
                    Debug.LogWarning("[GamePush Native] Sticky show failed: " + exception.Message);
                }
            };
            _banner.OnAdFailedToLoad += (_, args) =>
            {
                Debug.LogWarning("[GamePush Native] Sticky load failed: " + (args != null ? args.Message : "unknown"));
            };
            _banner.LoadAd(new AdRequest(adUnitId));
            return true;
        }

        public void HideBanner()
        {
            if (_banner == null)
                return;
            try { _banner.Destroy(); }
            catch { /* ignore */ }
            _banner = null;
        }

        void PresentInterstitial(Interstitial ad, Action<bool> done)
        {
            _shownAd = ad;
            var finished = false;
            void Finish(bool success)
            {
                if (finished)
                    return;
                finished = true;
                try { ad.Destroy(); }
                catch { /* ignore */ }
                if (ReferenceEquals(_shownAd, ad))
                    _shownAd = null;
                done?.Invoke(success);
            }

            ad.OnAdDismissed += (_, __) => Finish(true);
            ad.OnAdFailedToShow += (_, __) => Finish(false);
            try { ad.Show(); }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] Interstitial show failed: " + exception.Message);
                Finish(false);
            }
        }

        void PresentAppOpen(AppOpenAd ad, Action<bool> done)
        {
            _shownAd = ad;
            var finished = false;
            void Finish(bool success)
            {
                if (finished)
                    return;
                finished = true;
                try { ad.Destroy(); }
                catch { /* ignore */ }
                if (ReferenceEquals(_shownAd, ad))
                    _shownAd = null;
                done?.Invoke(success);
            }

            ad.OnAdDismissed += (_, __) => Finish(true);
            ad.OnAdFailedToShow += (_, __) => Finish(false);
            try { ad.Show(); }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] App open show failed: " + exception.Message);
                Finish(false);
            }
        }

        void PresentRewarded(RewardedAd ad, Action<bool> done)
        {
            _shownAd = ad;
            var rewarded = false;
            var finished = false;
            void Finish(bool success)
            {
                if (finished)
                    return;
                finished = true;
                try { ad.Destroy(); }
                catch { /* ignore */ }
                if (ReferenceEquals(_shownAd, ad))
                    _shownAd = null;
                done?.Invoke(success);
            }

            ad.OnRewarded += (_, __) => rewarded = true;
            ad.OnAdDismissed += (_, __) => Finish(rewarded);
            ad.OnAdFailedToShow += (_, __) => Finish(false);
            try { ad.Show(); }
            catch (Exception exception)
            {
                Debug.LogWarning("[GamePush Native] Rewarded show failed: " + exception.Message);
                Finish(false);
            }
        }
    }
}
#endif
