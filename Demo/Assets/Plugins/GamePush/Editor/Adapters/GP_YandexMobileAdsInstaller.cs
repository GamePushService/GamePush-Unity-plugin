using System;
using System.IO;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

namespace GamePushEditor.Adapters
{
    static class GP_YandexMobileAdsInstaller
    {
        const string OpenUpmUrl = "https://package.openupm.com";
        public static bool Busy { get; private set; }
        public static string Status { get; private set; } = "";

        static AddRequest _addRequest;

        public static void Install()
        {
            if (Busy)
                return;
            try
            {
                if (!GP_AndroidTooling.EnsureJavaOrExplain(out var javaError))
                {
                    Status = javaError;
                    EditorUtility.DisplayDialog("GamePush", javaError, "OK");
                    return;
                }

                var registryChanged = EnsureOpenUpmRegistry();
                EnableGradleTemplates();
                Busy = true;
                if (registryChanged)
                {
                    Status = "Adding OpenUPM registry…";
                    EditorApplication.delayCall += StartAdd;
                }
                else
                    StartAdd();
            }
            catch (Exception exception)
            {
                Busy = false;
                Status = exception.Message;
                EditorUtility.DisplayDialog("GamePush", "Failed to start Yandex Mobile Ads install:\n" + exception.Message, "OK");
            }
        }

        static void StartAdd()
        {
            Status = "Installing " + GP_AdapterRegistry.YandexPackage + "…";
            _addRequest = Client.Add(GP_AdapterRegistry.YandexPackage);
            EditorApplication.update += PollAdd;
        }

        static void PollAdd()
        {
            if (_addRequest == null || !_addRequest.IsCompleted)
                return;
            EditorApplication.update -= PollAdd;
            Busy = false;
            if (_addRequest.Status == StatusCode.Success)
            {
                var version = _addRequest.Result != null ? _addRequest.Result.version : "";
                Status = "Installed " + GP_AdapterRegistry.YandexPackage + (string.IsNullOrEmpty(version) ? "" : " " + version);
                var snapshot = GP_PlatformSnapshot.Load();
                if (snapshot != null)
                    GP_AndroidBuildAdapters.Apply(snapshot);
                EditorUtility.DisplayDialog("GamePush",
                    "Yandex Mobile Ads installed" + (string.IsNullOrEmpty(version) ? "." : " (" + version + ").") +
                    "\n\nIf prompted, resolve Android dependencies once. In EDM4U settings leave Auto-Resolution off.",
                    "OK");
            }
            else
            {
                var error = _addRequest.Error != null ? _addRequest.Error.message : "unknown error";
                Status = error;
                EditorUtility.DisplayDialog("GamePush", "Yandex Mobile Ads install failed:\n" + error, "OK");
            }
            _addRequest = null;
        }

        internal static bool EnsureOpenUpmRegistry()
        {
            var path = Path.Combine(Directory.GetParent(Application.dataPath).FullName, "Packages", "manifest.json");
            if (!File.Exists(path))
                throw new FileNotFoundException("Packages/manifest.json not found", path);

            var json = File.ReadAllText(path);
            if (json.IndexOf(OpenUpmUrl, StringComparison.OrdinalIgnoreCase) >= 0)
            {
                if (json.IndexOf("\"com.yandex\"", StringComparison.Ordinal) < 0)
                    throw new Exception("OpenUPM is already registered but without the com.yandex scope. Add com.yandex and com.google in Package Manager → scoped registries.");
                return false;
            }

            const string registryBlock =
                "  \"scopedRegistries\": [\n" +
                "    {\n" +
                "      \"name\": \"OpenUPM\",\n" +
                "      \"url\": \"https://package.openupm.com\",\n" +
                "      \"scopes\": [\n" +
                "        \"com.yandex\",\n" +
                "        \"com.google\"\n" +
                "      ]\n" +
                "    }\n" +
                "  ],\n";

            var insertAt = json.IndexOf('{');
            if (insertAt < 0)
                throw new Exception("Packages/manifest.json is not valid JSON.");

            if (json.IndexOf("\"scopedRegistries\"", StringComparison.Ordinal) >= 0)
            {
                var arrayStart = json.IndexOf('[', json.IndexOf("\"scopedRegistries\"", StringComparison.Ordinal));
                if (arrayStart < 0)
                    throw new Exception("Could not update scopedRegistries in Packages/manifest.json.");
                var entry =
                    "\n    {\n" +
                    "      \"name\": \"OpenUPM\",\n" +
                    "      \"url\": \"https://package.openupm.com\",\n" +
                    "      \"scopes\": [\n" +
                    "        \"com.yandex\",\n" +
                    "        \"com.google\"\n" +
                    "      ]\n" +
                    "    }";
                var afterBracket = arrayStart + 1;
                while (afterBracket < json.Length && char.IsWhiteSpace(json[afterBracket]))
                    afterBracket++;
                if (afterBracket < json.Length && json[afterBracket] == ']')
                    json = json.Insert(arrayStart + 1, entry + "\n  ");
                else
                    json = json.Insert(arrayStart + 1, entry + ",");
            }
            else
            {
                json = json.Insert(insertAt + 1, "\n" + registryBlock);
            }

            File.WriteAllText(path, json);
            AssetDatabase.Refresh();
            return true;
        }

        internal static bool EnableGradleTemplates()
        {
            var changed = false;
            var assets = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/ProjectSettings.asset");
            if (assets != null && assets.Length > 0)
            {
                var so = new SerializedObject(assets[0]);
                changed = SetBoolIfNeeded(so, "useCustomMainGradleTemplate", true);
                changed = SetBoolIfNeeded(so, "useCustomGradlePropertiesTemplate", true) || changed;
                changed = SetBoolIfNeeded(so, "useCustomGradleSettingsTemplate", true) || changed;
                if (changed)
                    so.ApplyModifiedPropertiesWithoutUndo();
            }

            var dstDir = Path.Combine(Application.dataPath, "Plugins", "Android");
            Directory.CreateDirectory(dstDir);
            var srcDir = Path.Combine(EditorApplication.applicationContentsPath, "PlaybackEngines", "AndroidPlayer", "Tools", "GradleTemplates");
            changed = CopyIfMissing(Path.Combine(srcDir, "mainTemplate.gradle"), Path.Combine(dstDir, "mainTemplate.gradle")) || changed;
            changed = CopyIfMissing(Path.Combine(srcDir, "gradleTemplate.properties"), Path.Combine(dstDir, "gradleTemplate.properties")) || changed;
            changed = CopyIfMissing(Path.Combine(srcDir, "settingsTemplate.gradle"), Path.Combine(dstDir, "settingsTemplate.gradle")) || changed;
            return changed;
        }

        static bool SetBoolIfNeeded(SerializedObject so, string name, bool value)
        {
            var property = so.FindProperty(name);
            if (property == null || property.boolValue == value)
                return false;
            property.boolValue = value;
            return true;
        }

        static bool CopyIfMissing(string source, string destination)
        {
            if (!File.Exists(source) || File.Exists(destination))
                return false;
            File.Copy(source, destination);
            return true;
        }
    }
}
