using System;
using System.IO;
using System.Text;
using GamePush.Data;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Profile;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace GamePushEditor.Adapters
{
    static class GP_AndroidBuildAdapters
    {
        public const string AdsYandexDefine = "GP_BUILD_ADS_YANDEX";
        public const string AuthDefine = "GP_BUILD_AUTH";
        public const string PaymentsWebDefine = "GP_BUILD_PAYMENTS_WEB";
        public const string PaymentsGoogleDefine = "GP_BUILD_PAYMENTS_GOOGLE";
        public const string PaymentsOneStoreDefine = "GP_BUILD_PAYMENTS_ONESTORE";
        public const string OneStorePluginDefine = "GP_ONESTORE_IAP";
        public const string AuthPluginPath =
            "Assets/Plugins/GamePush/Runtime/Native/Android/GamePushAuth.androidlib";
        public const string OneStoreQueriesPluginPath =
            "Assets/Plugins/GamePush/Runtime/Native/Android/GamePushOneStore.androidlib";

        const string DepsStart = "// GP_ADAPTER_DEPS_START";
        const string DepsEnd = "// GP_ADAPTER_DEPS_END";

        static readonly string[] ManagedDefines =
        {
            AdsYandexDefine,
            AuthDefine,
            PaymentsWebDefine,
            PaymentsGoogleDefine,
            PaymentsOneStoreDefine,
            OneStorePluginDefine,
            "GP_BUILD_AUTH_GOOGLE",
            "GP_BUILD_AUTH_YANDEX",
            "GP_BUILD_AUTH_XSOLLA"
        };

        public static void Apply(GP_AndroidAdapterSnapshot snapshot)
        {
            if (snapshot == null)
                return;
            var packedChanged = ApplyPackedPlugins(snapshot);
            if (packedChanged)
            {
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            ApplyDefines(snapshot);
        }

        public static bool ApplyPackedPlugins(GP_AndroidAdapterSnapshot snapshot)
        {
            if (snapshot == null)
                return false;
            var changed = ApplyPlugin(AuthPluginPath, snapshot.needAuthWebView || snapshot.needPaymentsWebView);
            changed = ApplyPlugin(OneStoreQueriesPluginPath, snapshot.needOneStoreIap) || changed;
            changed = SetYandexPackageAndroidPlugins(snapshot.needYandexAds) || changed;
            changed = SetOneStorePackageAndroidPlugins(snapshot.needOneStoreIap) || changed;
            if (GP_AdapterRegistry.IsOneStoreInstalled)
                changed = GP_OneStoreInstaller.EnsurePluginWiring() || changed;
            changed = ApplyGradle(snapshot) || changed;
            return changed;
        }

        public static bool DefinesMatch(GP_AndroidAdapterSnapshot snapshot)
        {
            if (snapshot == null)
                return true;
            var current = ReadAndroidDefines();
            return HasDefine(current, AdsYandexDefine) == snapshot.needYandexAds
                   && HasDefine(current, AuthDefine) == snapshot.needAuthWebView
                   && HasDefine(current, PaymentsWebDefine) == snapshot.needPaymentsWebView
                   && HasDefine(current, PaymentsGoogleDefine) == snapshot.needGooglePlayIap
                   && HasDefine(current, PaymentsOneStoreDefine) == snapshot.needOneStoreIap
                   && HasDefine(current, OneStorePluginDefine) == GP_AdapterRegistry.IsOneStoreInstalled;
        }

        public static void ApplyDefines(GP_AndroidAdapterSnapshot snapshot)
        {
            if (snapshot == null || DefinesMatch(snapshot))
                return;
            WriteAndroidDefines(MergeManagedDefines(ReadAndroidDefines(), snapshot));
        }

        static string ReadAndroidDefines()
        {
            var profile = BuildProfile.GetActiveBuildProfile();
            if (profile != null && IsAndroidProfile(profile) && ProfileHasDefines(profile))
                return JoinDefines(profile.scriptingDefines);
            return PlayerSettings.GetScriptingDefineSymbols(NamedBuildTarget.Android) ?? "";
        }

        static string MergeManagedDefines(string current, GP_AndroidAdapterSnapshot snapshot)
        {
            var tokens = (current ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            var kept = new StringBuilder();
            foreach (var token in tokens)
            {
                var t = token.Trim();
                if (t.Length == 0 || IsManaged(t))
                    continue;
                if (kept.Length > 0)
                    kept.Append(';');
                kept.Append(t);
            }
            if (snapshot.needYandexAds)
                AppendDefine(kept, AdsYandexDefine);
            if (snapshot.needAuthWebView)
                AppendDefine(kept, AuthDefine);
            if (snapshot.needPaymentsWebView)
                AppendDefine(kept, PaymentsWebDefine);
            if (snapshot.needGooglePlayIap)
                AppendDefine(kept, PaymentsGoogleDefine);
            if (snapshot.needOneStoreIap)
                AppendDefine(kept, PaymentsOneStoreDefine);
            if (GP_AdapterRegistry.IsOneStoreInstalled)
                AppendDefine(kept, OneStorePluginDefine);
            return kept.ToString();
        }

        static void WriteAndroidDefines(string defines)
        {
            var named = NamedBuildTarget.Android;
            if ((PlayerSettings.GetScriptingDefineSymbols(named) ?? "") != defines)
                PlayerSettings.SetScriptingDefineSymbols(named, defines);

            var tokens = (defines ?? "").Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var guid in AssetDatabase.FindAssets("t:BuildProfile"))
            {
                var path = AssetDatabase.GUIDToAssetPath(guid);
                var profile = AssetDatabase.LoadAssetAtPath<BuildProfile>(path);
                if (profile == null || !IsAndroidProfile(profile))
                    continue;
                WriteProfileDefines(profile, tokens);
            }

            SavePlayerSettingsAsset();
        }

        static bool IsAndroidProfile(BuildProfile profile)
        {
            if (profile == null)
                return false;
            var so = new SerializedObject(profile);
            var target = so.FindProperty("m_BuildTarget");
            return target != null && target.intValue == (int)BuildTarget.Android;
        }

        static bool ProfileHasDefines(BuildProfile profile)
        {
            if (profile == null)
                return false;
            var so = new SerializedObject(profile);
            var has = so.FindProperty("m_HasScriptingDefines");
            return has != null && has.boolValue;
        }

        static void WriteProfileDefines(BuildProfile profile, string[] tokens)
        {
            var so = new SerializedObject(profile);
            var has = so.FindProperty("m_HasScriptingDefines");
            var list = so.FindProperty("m_ScriptingDefines");
            if (has == null || list == null)
                return;
            if (has.boolValue && DefinesArrayEquals(list, tokens))
                return;
            has.boolValue = true;
            list.arraySize = tokens.Length;
            for (var i = 0; i < tokens.Length; i++)
                list.GetArrayElementAtIndex(i).stringValue = tokens[i];
            so.ApplyModifiedPropertiesWithoutUndo();
            EditorUtility.SetDirty(profile);
        }

        static bool DefinesArrayEquals(SerializedProperty list, string[] tokens)
        {
            if (list.arraySize != tokens.Length)
                return false;
            for (var i = 0; i < tokens.Length; i++)
            {
                if (list.GetArrayElementAtIndex(i).stringValue != tokens[i])
                    return false;
            }
            return true;
        }

        static string JoinDefines(string[] tokens)
        {
            if (tokens == null || tokens.Length == 0)
                return "";
            return string.Join(";", tokens);
        }

        static void SavePlayerSettingsAsset()
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets != null)
            {
                foreach (var asset in assets)
                {
                    if (asset != null)
                        EditorUtility.SetDirty(asset);
                }
            }
            AssetDatabase.SaveAssets();
        }

        public static void ApplyAuthPlugin(bool enable)
        {
            ApplyPlugin(AuthPluginPath, enable);
        }

        static bool ApplyPlugin(string path, bool enable)
        {
            var importer = AssetImporter.GetAtPath(path) as PluginImporter;
            if (importer == null)
                return false;
            var already = !importer.GetCompatibleWithAnyPlatform()
                          && !importer.GetCompatibleWithEditor()
                          && importer.GetCompatibleWithPlatform(BuildTarget.Android) == enable;
            if (already)
                return false;
            importer.SetCompatibleWithAnyPlatform(false);
            importer.SetCompatibleWithEditor(false);
            importer.SetCompatibleWithPlatform(BuildTarget.Android, enable);
            importer.SaveAndReimport();
            return true;
        }

        public static bool SetYandexPackageAndroidPlugins(bool enable)
        {
            var importers = PluginImporter.GetAllImporters();
            if (importers == null)
                return false;
            var changed = false;
            foreach (var importer in importers)
            {
                if (importer == null || string.IsNullOrEmpty(importer.assetPath))
                    continue;
                var path = importer.assetPath.Replace('\\', '/');
                if (path.IndexOf("com.yandex.mobileads", StringComparison.OrdinalIgnoreCase) < 0 &&
                    path.IndexOf("YandexMobileAds", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                if (importer.GetCompatibleWithPlatform(BuildTarget.Android) == enable)
                    continue;
                importer.SetCompatibleWithPlatform(BuildTarget.Android, enable);
                importer.SaveAndReimport();
                changed = true;
            }
            return changed;
        }

        public static bool SetOneStorePackageAndroidPlugins(bool enable)
        {
            var importers = PluginImporter.GetAllImporters();
            if (importers == null)
                return false;
            var changed = false;
            foreach (var importer in importers)
            {
                if (importer == null || string.IsNullOrEmpty(importer.assetPath))
                    continue;
                var path = importer.assetPath.Replace('\\', '/');
                if (path.IndexOf("OneStoreCorpPlugins", StringComparison.OrdinalIgnoreCase) < 0)
                    continue;
                if (importer.GetCompatibleWithPlatform(BuildTarget.Android) == enable)
                    continue;
                importer.SetCompatibleWithPlatform(BuildTarget.Android, enable);
                importer.SaveAndReimport();
                changed = true;
            }
            return changed;
        }

        public static bool ApplyGradle(GP_AndroidAdapterSnapshot snapshot)
        {
            var changed = GP_YandexMobileAdsInstaller.EnableGradleTemplates();
            var gradlePath = Path.Combine(Application.dataPath, "Plugins", "Android", "mainTemplate.gradle");
            if (!File.Exists(gradlePath))
                return changed;
            var original = File.ReadAllText(gradlePath);
            var text = UpsertAdapterDeps(original, snapshot);
            text = StripEdm4uYandex(text, snapshot != null && snapshot.needYandexAds);
            text = StripLooseYandexDeps(text);
            if (text.Replace("\r\n", "\n") == original.Replace("\r\n", "\n"))
                return changed;
            File.WriteAllText(gradlePath, text);
            return true;
        }

        public static string ValidateOrError(GP_AndroidAdapterSnapshot snapshot)
        {
            if (snapshot == null)
                return "GamePush Android snapshot is missing. Open Tools/GamePush → Android, Save or Sync, wait for compile, then build.";
            var platform = string.IsNullOrEmpty(ProjectData.ANDROID_PLATFORM) ? "ANDROID" : ProjectData.ANDROID_PLATFORM;
            var tag = ProjectData.ANDROID_PLATFORM_TAG ?? "";
            if (!string.Equals(snapshot.platform, platform, StringComparison.Ordinal) ||
                !string.Equals(snapshot.platformTag ?? "", tag, StringComparison.Ordinal))
            {
                return "GamePush snapshot is for " + snapshot.platform +
                       " but ProjectData.ANDROID_PLATFORM is " + platform +
                       ". Change Store, Save or Sync, then build.";
            }
            if (snapshot.needYandexAds && !GP_AdapterRegistry.IsYandexAdsInstalled)
                return "This store uses Yandex Mobile Ads, but com.yandex.mobileads is not installed. Tools/GamePush → Android → Install.";
            if (snapshot.needGooglePlayIap && !GP_AdapterRegistry.IsUnityPurchasingInstalled)
                return "This store uses Google Play Billing, but com.unity.purchasing is not installed.";
            if (snapshot.needOneStoreIap && !GP_AdapterRegistry.IsOneStoreInstalled)
                return "This store uses One Store billing, but the official IAP plugin is not installed. Tools/GamePush → Android → One Store → Install.";
            return null;
        }

        static bool HasDefine(string symbols, string define)
        {
            if (string.IsNullOrEmpty(symbols) || string.IsNullOrEmpty(define))
                return false;
            var tokens = symbols.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries);
            foreach (var token in tokens)
            {
                if (token.Trim() == define)
                    return true;
            }
            return false;
        }

        static bool IsManaged(string token)
        {
            foreach (var define in ManagedDefines)
            {
                if (token == define)
                    return true;
            }
            return false;
        }

        static void AppendDefine(StringBuilder kept, string define)
        {
            if (kept.Length > 0)
                kept.Append(';');
            kept.Append(define);
        }

        static string UpsertAdapterDeps(string text, GP_AndroidAdapterSnapshot snapshot)
        {
            var needYandex = snapshot != null && snapshot.needYandexAds;
            var needOneStore = snapshot != null && snapshot.needOneStoreIap;
            var block = new StringBuilder();
            block.AppendLine(DepsStart);
            if (needYandex)
            {
                foreach (var coord in GP_AdapterRegistry.YandexGradleCoords)
                    block.AppendLine("    implementation '" + coord + "'");
            }
            if (needOneStore)
            {
                foreach (var coord in GP_AdapterRegistry.OneStoreGradleCoords)
                    block.AppendLine("    implementation '" + coord + "'");
            }
            block.Append(DepsEnd);

            var start = text.IndexOf(DepsStart, StringComparison.Ordinal);
            var end = text.IndexOf(DepsEnd, StringComparison.Ordinal);
            if (start >= 0 && end > start)
            {
                end += DepsEnd.Length;
                return text.Substring(0, start) + block + text.Substring(end);
            }

            const string marker = "**DEPS**";
            var at = text.IndexOf(marker, StringComparison.Ordinal);
            if (at < 0)
                return text;
            return text.Insert(at, block + "\n");
        }

        static string StripEdm4uYandex(string text, bool keep)
        {
            if (keep)
                return text;
            var startTag = "// Android Resolver Dependencies Start";
            var endTag = "// Android Resolver Dependencies End";
            var start = text.IndexOf(startTag, StringComparison.Ordinal);
            var end = text.IndexOf(endTag, StringComparison.Ordinal);
            if (start < 0 || end <= start)
                return StripYandexImplementationLines(text);
            var innerStart = start + startTag.Length;
            var inner = text.Substring(innerStart, end - innerStart);
            inner = StripYandexImplementationLines(inner);
            return text.Substring(0, innerStart) + inner + text.Substring(end);
        }

        static string StripLooseYandexDeps(string text)
        {
            var start = text.IndexOf(DepsStart, StringComparison.Ordinal);
            var end = text.IndexOf(DepsEnd, StringComparison.Ordinal);
            string protectedBlock = null;
            if (start >= 0 && end > start)
            {
                end += DepsEnd.Length;
                protectedBlock = text.Substring(start, end - start);
                text = text.Substring(0, start) + "\n" + text.Substring(end);
            }
            text = StripYandexImplementationLines(text);
            if (protectedBlock != null)
            {
                var marker = "**DEPS**";
                var at = text.IndexOf(marker, StringComparison.Ordinal);
                var insertAt = text.IndexOf(DepsStart, StringComparison.Ordinal);
                if (insertAt >= 0)
                    return text;
                if (at >= 0)
                    return text.Insert(at, protectedBlock + "\n");
                return protectedBlock + "\n" + text;
            }
            return text;
        }

        static string StripYandexImplementationLines(string text)
        {
            var lines = text.Replace("\r\n", "\n").Split('\n');
            var sb = new StringBuilder();
            for (var i = 0; i < lines.Length; i++)
            {
                var line = lines[i];
                if (line.IndexOf("com.yandex.android:mobileads", StringComparison.Ordinal) >= 0)
                    continue;
                if (line.IndexOf("io.appmetrica.analytics:analytics", StringComparison.Ordinal) >= 0)
                    continue;
                if (line.IndexOf("com.onestorecorp.sdk:sdk-iap", StringComparison.Ordinal) >= 0)
                    continue;
                if (line.IndexOf("androidx.lifecycle:lifecycle-process", StringComparison.Ordinal) >= 0 &&
                    line.IndexOf("YandexMobileadsDependencies", StringComparison.Ordinal) >= 0)
                    continue;
                if (i > 0 || line.Length > 0)
                {
                    if (sb.Length > 0)
                        sb.Append('\n');
                    sb.Append(line);
                }
            }
            return sb.ToString();
        }
    }

    sealed class GP_AndroidAdapterBuildGuard : IPreprocessBuildWithReport
    {
        public int callbackOrder => 0;

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report == null || report.summary.platform != BuildTarget.Android)
                return;
            var snapshot = GP_PlatformSnapshot.Load();
            var error = GP_AndroidBuildAdapters.ValidateOrError(snapshot);
            if (!string.IsNullOrEmpty(error))
                throw new BuildFailedException(error);
            if (!GP_AndroidBuildAdapters.DefinesMatch(snapshot))
            {
                throw new BuildFailedException(
                    "GamePush provider defines do not match the last Save or Sync. Open Tools/GamePush → Android, press Save or Sync, wait until Unity finishes compiling, then Build.");
            }
            GP_AndroidBuildAdapters.ApplyPackedPlugins(snapshot);
        }
    }
}
