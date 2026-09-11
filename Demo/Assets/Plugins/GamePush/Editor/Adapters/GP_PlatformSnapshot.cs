using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using GamePush.Data;
using GamePush.Native;
using UnityEditor;
using UnityEngine;

namespace GamePushEditor.Adapters
{
    [Serializable]
    class GP_AndroidAdapterSnapshot
    {
        public string platform = "";
        public string platformTag = "";
        public string adsImplementation = "";
        public string adsService = "";
        public string authImplementation = "";
        public string authService = "";
        public string paymentsImplementation = "";
        public string paymentsService = "";
        public bool needYandexAds;
        public bool needAuthWebView;
        public bool needPaymentsWebView;
        public bool needGooglePlayIap;
        public bool needOneStoreIap;
        public string fetchedAt = "";
        public string error = "";

        public bool SameBake(GP_AndroidAdapterSnapshot other)
        {
            if (other == null)
                return false;
            return platform == other.platform
                   && platformTag == other.platformTag
                   && adsImplementation == other.adsImplementation
                   && adsService == other.adsService
                   && authImplementation == other.authImplementation
                   && authService == other.authService
                   && paymentsImplementation == other.paymentsImplementation
                   && paymentsService == other.paymentsService
                   && needYandexAds == other.needYandexAds
                   && needAuthWebView == other.needAuthWebView
                   && needPaymentsWebView == other.needPaymentsWebView
                   && needGooglePlayIap == other.needGooglePlayIap
                   && needOneStoreIap == other.needOneStoreIap;
        }
    }

    static class GP_PlatformSnapshot
    {
        public const string FileName = "AndroidAdapterSnapshot.json";
        public static string Status { get; private set; } = "";
        public static bool Busy { get; private set; }

        public static string FilePath
        {
            get
            {
                var linker = Resources.Load<SavedDataSO>("GP_DataLinker");
                if (linker == null || linker.saveFile == null)
                    return Path.Combine(Application.dataPath, "Plugins/GamePush/Data", FileName);
                var save = AssetDatabase.GetAssetPath(linker.saveFile);
                return Path.Combine(Path.GetDirectoryName(save) ?? "", FileName).Replace('\\', '/');
            }
        }

        public static GP_AndroidAdapterSnapshot Load()
        {
            var path = FilePath;
            if (!File.Exists(path))
                return null;
            try
            {
                return JsonUtility.FromJson<GP_AndroidAdapterSnapshot>(File.ReadAllText(path));
            }
            catch
            {
                return null;
            }
        }

        public static void Save(GP_AndroidAdapterSnapshot snapshot)
        {
            var path = FilePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path) ?? "");
            GP_EditorFiles.WriteIfChanged(path, JsonUtility.ToJson(snapshot, true));
        }

        public static async void FetchAndApply(int projectId, string token, string platform, string platformTag)
        {
            if (Busy)
                return;
            Busy = true;
            Status = "Fetching GamePush config for " + platform + "…";
            var dirty = false;
            try
            {
                var previous = Load();
                var snapshot = await Fetch(projectId, token, platform, platformTag);
                if (!snapshot.SameBake(previous))
                {
                    Save(snapshot);
                    dirty = true;
                }

                GP_AndroidBuildAdapters.Apply(snapshot);
                Status = Describe(snapshot);
            }
            catch (Exception exception)
            {
                Status = "Config fetch failed: " + exception.Message;
                Debug.LogWarning("[GamePush] " + Status);
            }
            finally
            {
                Busy = false;
                if (dirty)
                    AssetDatabase.Refresh();
                var windows = Resources.FindObjectsOfTypeAll<GamePushEditor.GP_Window>();
                foreach (var window in windows)
                {
                    if (window != null)
                        window.Repaint();
                }
            }
        }

        public static async Task<GP_AndroidAdapterSnapshot> Fetch(int projectId, string token, string platform,
            string platformTag)
        {
            var client = new GraphQLClient(projectId, token);
            client.SetPlatform(string.IsNullOrEmpty(platform) ? "ANDROID" : platform, platformTag ?? "");
            client.SetLang("EN");
            var json = await client.Fetch(NativeQueries.FetchConfig);
            var result = GpJson.GetObject(json, "result") ?? json;
            if (GpJson.TryGetString(result, "__typename", out var typeName) && typeName == "Problem")
                throw new Exception(GpJson.TryGetString(result, "message", out var msg) ? msg : "config_problem");

            var platformJson = GpJson.GetObject(result, "platformConfig") ?? "{}";
            var ads = new NativeAdsConfig();
            var auth = new NativeAuthConfig();
            var payments = new NativePaymentsConfig();
            ads.LoadFromPlatformJson(platformJson);
            auth.LoadFromPlatformJson(platformJson);
            payments.LoadFromPlatformJson(platformJson);

            var snapshot = new GP_AndroidAdapterSnapshot
            {
                platform = string.IsNullOrEmpty(platform) ? "ANDROID" : platform,
                platformTag = platformTag ?? "",
                adsImplementation = ads.Implementation ?? "",
                adsService = ads.UsesYandex ? NativeAdsConfig.YandexSimpleMonetization : "",
                authImplementation = auth.Implementation ?? "",
                authService = auth.ActiveService ?? "",
                paymentsImplementation = payments.Implementation ?? "",
                paymentsService = payments.ActiveService ?? "",
                needYandexAds = ads.UsesYandex,
                needAuthWebView = auth.HasIntegratedAuth,
                needPaymentsWebView = payments.NeedsWebCheckout,
                needGooglePlayIap = payments.NeedsGooglePlay,
                needOneStoreIap = payments.NeedsOneStore,
                fetchedAt = DateTime.UtcNow.ToString("o")
            };
            return snapshot;
        }

        public static string Describe(GP_AndroidAdapterSnapshot snapshot)
        {
            if (snapshot == null)
                return "No snapshot. Save or Sync to fetch the store config.";
            var ads = snapshot.needYandexAds ? "Yandex Mobile Ads" : "none";
            var auth = string.IsNullOrEmpty(snapshot.authService) ? "none" : snapshot.authService;
            var pay = string.IsNullOrEmpty(snapshot.paymentsService) ? "none" : snapshot.paymentsService;
            var packed = new List<string>();
            if (snapshot.needYandexAds)
                packed.Add("Yandex Mobile Ads");
            if (snapshot.needAuthWebView || snapshot.needPaymentsWebView)
                packed.Add("WebView");
            if (snapshot.needGooglePlayIap)
                packed.Add("Unity IAP");
            if (snapshot.needOneStoreIap)
                packed.Add("One Store IAP");
            if (packed.Count == 0)
                packed.Add("none");
            return "This store: Ads=" + ads + ", Auth=" + auth + ", Payments=" + pay +
                   ". Packed: " + string.Join(", ", packed) + ".";
        }
    }
}
