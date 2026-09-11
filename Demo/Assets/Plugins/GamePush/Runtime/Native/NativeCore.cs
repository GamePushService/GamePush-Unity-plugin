using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush.Data;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public static class NativeCore
    {
        public const string CentrifugeWs = "wss://ws.eponesh.com/connection/websocket?format=protobuf";
        public const string CentrifugeHttp = "https://ws.eponesh.com/connection/http_stream?format=protobuf";
        public const string CustomPlatformTag = "pc";

        public static bool Ready { get; private set; }
        public static bool IsDev { get; private set; }
        public static bool IsAllowedOrigin { get; private set; } = true;
        public static string LastError { get; private set; }
        public static GraphQLClient Client { get; private set; }
        public static string ConfigJson { get; private set; }
        public static string PlatformType { get; private set; } = "NONE";
        public static string PlatformTag { get; private set; } = "";
        public static string ServerTime { get; private set; }
        public static int MainChatId { get; private set; }
        public static bool MainChatEnabled { get; private set; }
        public static bool ShowAdCountdownOverlay { get; private set; }
        public static bool ShowRewardedFailedOverlay { get; private set; }
        public static string AvatarGenerator { get; private set; } = "dicebear_retro";
        public static string AvatarGeneratorTemplate { get; private set; } = "";
        public static NativeAdsConfig Ads { get; } = new NativeAdsConfig();
        public static NativeAuthConfig Auth { get; } = new NativeAuthConfig();
        public static NativePaymentsConfig Payments { get; } = new NativePaymentsConfig();
        public static List<FetchProducts> Catalog { get; private set; } = new List<FetchProducts>();

        public static async Task<bool> Initialize(NativeJsSession jsSession = null)
        {
            try
            {
                NativeMainThread.Instance.enabled = true;
                Application.runInBackground = true;
                GP_Logger.Info("Native", "Initialize");
                if (!int.TryParse(ProjectData.ID, out var projectId) || projectId <= 0)
                    throw new Exception("ProjectData.ID is missing");

                if (jsSession != null && !string.IsNullOrEmpty(jsSession.PlatformType))
                {
                    PlatformType = jsSession.PlatformType;
                    PlatformTag = jsSession.PlatformTag ?? "";
                }
                else
                {
                    PlatformType = DetectPlatformType();
                    PlatformTag = ResolvePlatformTag(PlatformType);
                }
                Client = new GraphQLClient(projectId, ProjectData.TOKEN);
                Client.SetPlatform(PlatformType, PlatformTag);
                Client.SetLang(Application.systemLanguage == SystemLanguage.Russian ? "RU" : "EN");
                if (jsSession != null && jsSession.PlayerId > 0)
                    NativePlayer.AdoptSession(jsSession, Client);
                else
                    NativePlayer.PrepareCredentials(Client);

                ConfigJson = await Client.Fetch(NativeQueries.FetchConfig);
                var result = GpJson.GetObject(ConfigJson, "result") ?? ConfigJson;
                if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                    throw new Exception(GpJson.TryGetString(result, "message", out var msg) ? msg : "config_problem");

                ServerTime = GpJson.TryGetString(result, "serverTime", out var st) ? st : "";
                IsDev = GpJson.GetBool(result, "isDev");
                IsAllowedOrigin = GpJson.GetBool(result, "isAllowedOrigin", true);

                var config = GpJson.GetObject(result, "config");
                if (config != null)
                {
                    AvatarGenerator = GpJson.TryGetString(config, "avatarGenerator", out var generator) &&
                                      !string.IsNullOrEmpty(generator)
                        ? generator
                        : "dicebear_retro";
                    AvatarGeneratorTemplate = GpJson.TryGetString(config, "avatarGeneratorTemplate", out var template)
                        ? template ?? ""
                        : "";
                }

                var project = GpJson.GetObject(result, "project");
                if (project != null)
                {
                    MainChatId = GpJson.GetInt(project, "mainChatId");
                    MainChatEnabled = GpJson.GetBool(project, "enableMainChat");
                    var adsConfig = GpJson.GetObject(project, "ads");
                    if (adsConfig != null)
                    {
                        ShowAdCountdownOverlay = GpJson.GetBool(adsConfig, "showCountdownOverlay");
                        ShowRewardedFailedOverlay = GpJson.GetBool(adsConfig, "showRewardedFailedOverlay");
                    }
                }

                var platform = GpJson.GetObject(result, "platformConfig");
                if (platform != null)
                {
                    if (GpJson.TryGetString(platform, "type", out var pType) && !string.IsNullOrEmpty(pType))
                        PlatformType = pType;
                    PlatformTag = GpJson.TryGetString(platform, "tag", out var tag) ? tag : PlatformTag;
                    Client.SetPlatform(PlatformType, PlatformTag);
                    Ads.LoadFromPlatformJson(platform);
                    Auth.LoadFromPlatformJson(platform);
                    Payments.LoadFromPlatformJson(platform);
                }

                Catalog = NativePaymentsService.ParseProductList(GpJson.GetObjectArray(result, "products"));
                GP_Logger.Info("PAYMENTS", "Config catalog n=" + Catalog.Count +
                                           (Catalog.Count == 0
                                               ? ""
                                               : " " + NativePayments.DescribeProduct(Catalog[0])));

                if (jsSession != null && jsSession.PlayerId > 0)
                    await NativePlayer.FetchExisting(Client);
                else
                    await NativePlayer.Bootstrap(Client);
                GP_Logger.Info("Native", "id=" + NativePlayer.Id + " " + PlatformType + "/" + PlatformTag + " isDev=" + IsDev);
#if UNITY_ANDROID && !UNITY_EDITOR
                NativeAdsSdks.Initialize();
                NativeAuthSdks.Initialize();
                NativePaymentsSdks.Initialize();
#endif
                Ready = true;
                LastError = null;
                return true;
            }
            catch (Exception exception)
            {
                LastError = exception.Message;
                Ready = false;
                GP_Logger.Error("Native", exception.Message);
                return false;
            }
        }

        static string DetectPlatformType()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            return string.IsNullOrEmpty(ProjectData.ANDROID_PLATFORM) ? "ANDROID" : ProjectData.ANDROID_PLATFORM;
#elif UNITY_STANDALONE && !UNITY_EDITOR
            return "CUSTOM";
#elif UNITY_WEBGL && !UNITY_EDITOR
            return "CUSTOM";
#else
            return "NONE";
#endif
        }

        static string ResolvePlatformTag(string platformType)
        {
            if (platformType == "CUSTOM")
                return CustomPlatformTag;
            if (platformType == "CUSTOM_ANDROID")
                return ProjectData.ANDROID_PLATFORM_TAG ?? "";
            return "";
        }
    }

    public sealed class NativeJsSession
    {
        public int PlayerId;
        public string Name = "";
        public string Avatar = "";
        public string Credentials = "";
        public string SecretCode = "";
        public string PlatformType = "";
        public string PlatformTag = "";

        public static NativeJsSession Parse(string json)
        {
            var session = new NativeJsSession();
            if (string.IsNullOrEmpty(json) || json == "{}")
                return session;
            session.PlayerId = GpJson.GetInt(json, "id");
            session.Name = GpJson.TryGetString(json, "name", out var name) ? name ?? "" : "";
            session.Avatar = GpJson.TryGetString(json, "avatar", out var avatar) ? avatar ?? "" : "";
            session.Credentials = GpJson.TryGetString(json, "credentials", out var cred) ? cred ?? "" : "";
            session.SecretCode = GpJson.TryGetString(json, "secretCode", out var secret) ? secret ?? "" : "";
            session.PlatformType = GpJson.TryGetString(json, "platform", out var platform) ? platform ?? "" : "";
            session.PlatformTag = GpJson.TryGetString(json, "tag", out var tag) ? tag ?? "" : "";
            return session;
        }
    }

    public sealed class NativeAdsConfig
    {
        public const string YandexSimpleMonetization = "YandexSimpleMonetization";
        public const string ImplementationInternal = "INTERNAL";
        public readonly Dictionary<string, NativeBanner> Banners = new Dictionary<string, NativeBanner>();
        public string Implementation = "";

        public bool UsesYandex
        {
            get
            {
                foreach (var banner in Banners.Values)
                {
                    if (banner != null && banner.Enabled && banner.AdServerSupported &&
                        banner.AdServer == YandexSimpleMonetization)
                        return true;
                }
                return false;
            }
        }

        public void LoadFromPlatformJson(string platformJson)
        {
            Banners.Clear();
            Implementation = "";
            var platformBanners = IndexBanners(GpJson.GetObjectArray(platformJson, "banners"));
            var custom = GpJson.GetObject(platformJson, "customAdsConfig");
            var configs = custom != null ? GpJson.GetObject(custom, "configs") : null;
            var android = configs != null ? GpJson.GetObject(configs, "android") : null;
            if (android != null && GpJson.TryGetString(android, "implementation", out var impl) && !string.IsNullOrEmpty(impl))
                Implementation = impl;
            var androidBanners = IndexBanners(android != null ? GpJson.GetObjectArray(android, "banners") : null);

            var types = new HashSet<string>(platformBanners.Keys);
            foreach (var key in androidBanners.Keys)
                types.Add(key);

            foreach (var type in types)
            {
                platformBanners.TryGetValue(type, out var platformItem);
                androidBanners.TryGetValue(type, out var androidItem);
                var merged = MergeBanner(type, platformItem, androidItem);
                if (merged != null)
                    Banners[type] = merged;
            }
        }

        public NativeBanner Get(string type) => Banners.TryGetValue(type, out var banner) ? banner : null;

        static Dictionary<string, string> IndexBanners(List<string> items)
        {
            var map = new Dictionary<string, string>();
            if (items == null)
                return map;
            foreach (var item in items)
            {
                var type = GpJson.TryGetString(item, "type", out var t) ? t : "";
                if (string.IsNullOrEmpty(type))
                    continue;
                map[type] = item;
            }
            return map;
        }

        static NativeBanner MergeBanner(string type, string platformItem, string androidItem)
        {
            var limitsSource = platformItem ?? androidItem;
            var idSource = androidItem ?? platformItem;
            if (limitsSource == null && idSource == null)
                return null;

            var enabledPlatform = platformItem == null || GpJson.GetBool(platformItem, "enabled", true);
            var enabledAndroid = androidItem == null || GpJson.GetBool(androidItem, "enabled", true);
            var adServer = GpJson.TryGetString(idSource, "adServer", out var server) ? server ?? "" : "";
            var supported = string.IsNullOrEmpty(adServer) || adServer == YandexSimpleMonetization;
            if (!supported)
                GP_Logger.Warn("ADS", "Ad Server [" + adServer + "] is not supported for " + type);

            var position = "bottom";
            if (platformItem != null && GpJson.TryGetString(platformItem, "position", out var platformPos) && !string.IsNullOrEmpty(platformPos))
                position = platformPos;
            else if (androidItem != null && GpJson.TryGetString(androidItem, "position", out var androidPos) && !string.IsNullOrEmpty(androidPos))
                position = androidPos;

            return new NativeBanner
            {
                Type = type,
                Enabled = enabledPlatform && enabledAndroid,
                BannerId = GpJson.TryGetString(idSource, "bannerId", out var id) ? id ?? "" : "",
                AdServer = adServer,
                AdServerSupported = supported,
                Position = position,
                Frequency = GpJson.GetInt(limitsSource, "frequency"),
                RefreshInterval = GpJson.GetInt(limitsSource, "refreshInterval"),
                HourLimit = GpJson.GetInt(GpJson.GetObject(limitsSource, "limits"), "hour"),
                DayLimit = GpJson.GetInt(GpJson.GetObject(limitsSource, "limits"), "day"),
                SessionLimit = GpJson.GetInt(GpJson.GetObject(limitsSource, "limits"), "session")
            };
        }
    }

    public sealed class NativeBanner
    {
        public string Type;
        public bool Enabled;
        public string BannerId;
        public string AdServer;
        public bool AdServerSupported = true;
        public string Position;
        public int Frequency;
        public int RefreshInterval;
        public int HourLimit;
        public int DayLimit;
        public int SessionLimit;
        public int SessionShown;
        public float LastShownAt;
    }

    public sealed class NativeAuthConfig
    {
        public const string ImplementationInternal = "INTERNAL";
        public const string Yandex = "YANDEX";
        public const string Google = "GOOGLE";
        public const string Xsolla = "XSOLLA";
        public const string AuthHost = "https://gamepush.com/sdk";

        public string Implementation = "";
        public string ActiveService = "";
        public string GoogleClientId = "";
        public string YandexClientId = "";
        public string XsollaLoginProjectId = "";

        public bool HasIntegratedAuth =>
            Implementation == ImplementationInternal && !string.IsNullOrEmpty(ActiveService);

        public bool IsLogoutAvailable => HasIntegratedAuth;

        public string GoogleRedirectUri => AuthHost + "/static/pages/google/auth.html";
        public string YandexRedirectUri => AuthHost + "/static/pages/yandex/auth.html";
        public string XsollaRedirectUri => AuthHost + "/static/pages/xsolla/auth.html";

        public void LoadFromPlatformJson(string platformJson)
        {
            Implementation = "";
            ActiveService = "";
            GoogleClientId = "";
            YandexClientId = "";
            XsollaLoginProjectId = "";
            var auth = GpJson.GetObject(platformJson, "authConfig");
            if (auth == null)
                return;
            var google = GpJson.GetObject(auth, "googleConfig");
            var yandex = GpJson.GetObject(auth, "yandexConfig");
            var xsolla = GpJson.GetObject(auth, "xsollaConfig");
            if (google != null && GpJson.TryGetString(google, "clientID", out var googleId))
                GoogleClientId = googleId ?? "";
            if (yandex != null && GpJson.TryGetString(yandex, "clientID", out var yandexId))
                YandexClientId = yandexId ?? "";
            if (xsolla != null && GpJson.TryGetString(xsolla, "loginProjectId", out var loginId))
                XsollaLoginProjectId = loginId ?? "";
            var configs = GpJson.GetObject(auth, "configs");
            var android = configs != null ? GpJson.GetObject(configs, "android") : null;
            if (android == null)
                return;
            if (GpJson.TryGetString(android, "implementation", out var impl) && !string.IsNullOrEmpty(impl))
                Implementation = impl;
            if (GpJson.TryGetString(android, "activeService", out var service) && !string.IsNullOrEmpty(service))
                ActiveService = service;
        }
    }

    public sealed class NativePaymentsConfig
    {
        public const string ImplementationInternal = "INTERNAL";
        public const string GooglePlay = "GOOGLE_PLAY";
        public const string OneStore = "ONESTORE";
        public const string Xsolla = "XSOLLA";
        public const string Robokassa = "ROBOKASSA";
        public const string Stripe = "STRIPE";

        public string Implementation = "";
        public string ActiveService = "";
        public bool Sandbox;
        public string PublicKey = "";

        public bool HasPayments =>
            Implementation == ImplementationInternal && !string.IsNullOrEmpty(ActiveService);

        public bool NeedsWebCheckout =>
            HasPayments && (ActiveService == Xsolla || ActiveService == Robokassa || ActiveService == Stripe);

        public bool NeedsGooglePlay =>
            HasPayments && ActiveService == GooglePlay;

        public bool NeedsOneStore =>
            HasPayments && ActiveService == OneStore;

        public bool SupportsSubscriptions =>
            ActiveService == Xsolla || ActiveService == GooglePlay;

        public void LoadFromPlatformJson(string platformJson)
        {
            Implementation = "";
            ActiveService = "";
            Sandbox = false;
            PublicKey = "";
            var payments = GpJson.GetObject(platformJson, "paymentsConfig");
            if (payments == null)
                return;
            Sandbox = GpJson.GetBool(payments, "sandbox");
            var oneStore = GpJson.GetObject(payments, "oneStoreConfig");
            if (oneStore != null && GpJson.TryGetString(oneStore, "publicKey", out var key) && !string.IsNullOrEmpty(key))
                PublicKey = key;
            var configs = GpJson.GetObject(payments, "configs");
            var android = configs != null ? GpJson.GetObject(configs, "android") : null;
            if (android == null)
                return;
            if (GpJson.TryGetString(android, "implementation", out var impl) && !string.IsNullOrEmpty(impl))
                Implementation = impl;
            if (GpJson.TryGetString(android, "activeService", out var service) && !string.IsNullOrEmpty(service))
                ActiveService = service;
        }
    }
}
