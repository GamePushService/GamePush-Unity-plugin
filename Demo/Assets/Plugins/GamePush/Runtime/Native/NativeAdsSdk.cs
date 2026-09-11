using System;
using GamePush;

namespace GamePush.Native
{
    public interface INativeAdsSdk
    {
        bool IsAvailable { get; }
        void Initialize();
        void Show(string type, string adUnitId, Action<bool> done);
        bool ShowBanner(string adUnitId, string position);
        void HideBanner();
    }

    public static class NativeAdsSdks
    {
        static bool _loggedMissing;

        public static INativeAdsSdk Current { get; private set; } = NativeUnavailableAdsSdk.Instance;

        public static void Initialize()
        {
#if GP_BUILD_ADS_YANDEX && GP_YANDEX_MOBILE_ADS && UNITY_ANDROID && !UNITY_EDITOR
            if (NativeCore.Ads.UsesYandex)
            {
                Current = new NativeYandexMobileAdsSdk();
                Current.Initialize();
                GP_Logger.Info("ADS", "Yandex Mobile Ads adapter ready");
                return;
            }
            Current = NativeUnavailableAdsSdk.Instance;
#else
            Current = NativeUnavailableAdsSdk.Instance;
            if (NativeCore.Ads.UsesYandex)
                LogMissingOnce();
#endif
        }

        public static void LogMissingOnce()
        {
            if (_loggedMissing)
                return;
            _loggedMissing = true;
            GP_Logger.Warn("ADS",
                "This APK was built without Yandex Mobile Ads, or the plugin is not installed. Open Tools/GamePush → Android, install Yandex Mobile Ads, Save, then rebuild.");
        }
    }

    sealed class NativeUnavailableAdsSdk : INativeAdsSdk
    {
        public static readonly NativeUnavailableAdsSdk Instance = new NativeUnavailableAdsSdk();

        public bool IsAvailable => false;

        public void Initialize() { }

        public void Show(string type, string adUnitId, Action<bool> done)
        {
            NativeAdsSdks.LogMissingOnce();
            done?.Invoke(false);
        }

        public bool ShowBanner(string adUnitId, string position)
        {
            NativeAdsSdks.LogMissingOnce();
            return false;
        }

        public void HideBanner()
        {
        }
    }
}
