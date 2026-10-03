using System;
using UnityEditor.PackageManager;

namespace GamePushEditor.Adapters
{
    enum GP_AdapterKind
    {
        Ads,
        Auth,
        Payments
    }

    sealed class GP_AdapterInfo
    {
        public GP_AdapterKind Kind;
        public string Id;
        public string SdkId;
        public string Title;
        public string Description;
        public string PackageName;
        public string DocsUrl;
        public bool Installable;
        public bool ComingSoon;
        public string[] GradleCoords;
    }

    static class GP_AdapterRegistry
    {
        public const string YandexAdsId = "yandex-simple-monetization";
        public const string YandexPackage = "com.yandex.mobileads";
        public const string YandexAdsSdkId = "YandexSimpleMonetization";
        public const string YandexDocsUrl = "https://yandex.ru/support2/mobile-ads/ru/dev/unity";

        public const string OneStoreId = "pay-onestore";
        public const string OneStoreRoot = "Assets/OneStoreCorpPlugins";
        public const string OneStorePurchaseRoot = "Assets/OneStoreCorpPlugins/Purchase";
        public const string OneStoreVersion = "1.3.4";
        public const string OneStoreDocsUrl = "https://onestore-dev.gitbook.io/dev/eng/tools/billing/v21/unity";

        public static readonly string[] YandexGradleCoords =
        {
            "com.yandex.android:mobileads:8.3.0",
            "androidx.lifecycle:lifecycle-process:2.4.1",
            "io.appmetrica.analytics:analytics:8.0.0"
        };

        public static readonly string[] OneStoreGradleCoords =
        {
            "com.onestorecorp.sdk:sdk-iap:21.04.00"
        };

        public static readonly GP_AdapterInfo[] Ads =
        {
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Ads,
                Id = YandexAdsId,
                SdkId = YandexAdsSdkId,
                Title = "Yandex Mobile Ads",
                Description = "Official Yandex Mobile Ads Unity plugin. Banner IDs come from the GamePush Android ads config. The AAR is packed only if this store uses it.",
                PackageName = YandexPackage,
                DocsUrl = YandexDocsUrl,
                Installable = true,
                GradleCoords = YandexGradleCoords
            },
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Ads,
                Id = "ads-coming-soon",
                Title = "Other networks",
                Description = "More ad providers will appear here later.",
                Installable = false,
                ComingSoon = true
            }
        };

        public static readonly GP_AdapterInfo[] Auth =
        {
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Auth,
                Id = "auth-google",
                SdkId = "GOOGLE",
                Title = "Google OAuth",
                Description = "GamePush hosted OAuth (same flow as the JS SDK). No extra Unity package. Packed as a shared WebView if this store has any Android auth.",
                DocsUrl = "https://docs.gamepush.com/docs/authorization/providers/google",
                Installable = false
            },
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Auth,
                Id = "auth-yandex",
                SdkId = "YANDEX",
                Title = "Yandex ID",
                Description = "GamePush hosted OAuth. Provider is chosen at runtime from the live platform config.",
                DocsUrl = "https://docs.gamepush.com/docs/authorization/providers/yandex",
                Installable = false
            },
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Auth,
                Id = "auth-xsolla",
                SdkId = "XSOLLA",
                Title = "Xsolla Login",
                Description = "Xsolla Login widget in the same auth WebView. Switch Google/Yandex/Xsolla in the dashboard without a rebuild if the WebView is already in the APK.",
                DocsUrl = "https://docs.gamepush.com/docs/authorization/providers/xsolla",
                Installable = false
            }
        };

        public static readonly GP_AdapterInfo[] Payments =
        {
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Payments,
                Id = "pay-google-play",
                SdkId = "GOOGLE_PLAY",
                Title = "Google Play Billing",
                Description = "Native store billing via Unity IAP. Packed when this store uses Google Play.",
                Installable = false
            },
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Payments,
                Id = OneStoreId,
                SdkId = "ONESTORE",
                Title = "One Store",
                Description = "Official ONE store IAP Unity plugin. Packed when this store uses One Store.",
                DocsUrl = OneStoreDocsUrl,
                Installable = true,
                GradleCoords = OneStoreGradleCoords
            },
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Payments,
                Id = "pay-xsolla",
                SdkId = "XSOLLA",
                Title = "Xsolla",
                Description = "Web checkout in the shared Android WebView. Packed when this store uses Xsolla.",
                DocsUrl = "https://docs.gamepush.com/docs/purchases/providers/xsolla",
                Installable = false
            },
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Payments,
                Id = "pay-robokassa",
                SdkId = "ROBOKASSA",
                Title = "Robokassa",
                Description = "Web checkout in the shared Android WebView. Packed when this store uses Robokassa.",
                DocsUrl = "https://docs.gamepush.com/docs/purchases/providers/robokassa",
                Installable = false
            },
            new GP_AdapterInfo
            {
                Kind = GP_AdapterKind.Payments,
                Id = "pay-stripe",
                SdkId = "STRIPE",
                Title = "Stripe",
                Description = "Web checkout in the shared Android WebView. Packed when this store uses Stripe.",
                DocsUrl = "https://docs.gamepush.com/docs/purchases/providers/stripe",
                Installable = false
            }
        };

        public static bool TryGetInstalledVersion(string packageName, out string version)
        {
            version = null;
            if (string.IsNullOrEmpty(packageName))
                return false;
            foreach (var package in PackageInfo.GetAllRegisteredPackages())
            {
                if (package != null && package.name == packageName)
                {
                    version = package.version;
                    return true;
                }
            }
            return false;
        }

        public static bool IsYandexAdsInstalled => TryGetInstalledVersion(YandexPackage, out _);
        public static bool IsUnityPurchasingInstalled => TryGetInstalledVersion("com.unity.purchasing", out _);
        public static bool IsOneStoreInstalled =>
            System.IO.Directory.Exists(System.IO.Path.Combine(UnityEngine.Application.dataPath, "OneStoreCorpPlugins", "Purchase"));
    }
}
