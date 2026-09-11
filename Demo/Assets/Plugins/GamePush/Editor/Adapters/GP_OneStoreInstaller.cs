using System;
using System.IO;
using System.Net;
using UnityEditor;
using UnityEngine;

namespace GamePushEditor.Adapters
{
    static class GP_OneStoreInstaller
    {
        const string PackageUrl =
            "https://github.com/ONE-store/unity_plugins/releases/download/v1.3.4/integrated-inapp-v1.3.4.unitypackage";
        const string CommonAsmdefGuid = "8f4a2c1e6b9d47e0a3c5d8f1b2e4a670";
        const string PurchasingAsmdefGuid = "1c8e5d3a7b924f6e9a0c4d8b2f1e6a35";
        const string WrapperAsmdefPath =
            "Assets/Plugins/GamePush/Runtime/Native/OneStore/GamePush.Native.OneStore.asmdef";

        public static bool Busy { get; private set; }
        public static string Status { get; private set; } = "";

        public static void Install()
        {
            if (Busy)
                return;
            try
            {
                Busy = true;
                Status = "Downloading ONE store IAP " + GP_AdapterRegistry.OneStoreVersion + "…";
                var packagePath = Path.Combine(Path.GetTempPath(), "integrated-inapp-v1.3.4.unitypackage");
                using (var client = new WebClient())
                    client.DownloadFile(PackageUrl, packagePath);
                Status = "Importing ONE store IAP plugin…";
                AssetDatabase.ImportPackage(packagePath, false);
                try { File.Delete(packagePath); }
                catch { }

                RemoveBundledEdm4u();
                RemoveUnusedModules();
                EnsurePluginWiring();
                Status = "Installed ONE store IAP " + GP_AdapterRegistry.OneStoreVersion;
                var snapshot = GP_PlatformSnapshot.Load();
                if (snapshot != null)
                    GP_AndroidBuildAdapters.Apply(snapshot);
                EditorUtility.DisplayDialog("GamePush",
                    "ONE store IAP " + GP_AdapterRegistry.OneStoreVersion +
                    " imported. Save in Tools/GamePush if this store uses One Store, then rebuild.",
                    "OK");
            }
            catch (Exception exception)
            {
                Status = exception.Message;
                EditorUtility.DisplayDialog("GamePush",
                    "ONE store IAP install failed:\n" + exception.Message, "OK");
            }
            finally
            {
                Busy = false;
            }
        }

        public static bool EnsurePluginWiring()
        {
            if (!GP_AdapterRegistry.IsOneStoreInstalled)
                return false;
            var changed = WriteAsmdef("Assets/OneStoreCorpPlugins/Common/Runtime/OneStore.Common.asmdef", CommonAsmdefGuid,
                "{\n  \"name\": \"OneStore.Common\",\n  \"rootNamespace\": \"OneStore\",\n  \"references\": [],\n  \"includePlatforms\": [],\n  \"excludePlatforms\": [],\n  \"allowUnsafeCode\": false,\n  \"overrideReferences\": false,\n  \"precompiledReferences\": [],\n  \"autoReferenced\": false,\n  \"defineConstraints\": [ \"GP_BUILD_PAYMENTS_ONESTORE\" ],\n  \"noEngineReferences\": false\n}\n");
            changed = WriteAsmdef("Assets/OneStoreCorpPlugins/Purchase/Runtime/OneStore.Purchasing.asmdef", PurchasingAsmdefGuid,
                "{\n  \"name\": \"OneStore.Purchasing\",\n  \"rootNamespace\": \"OneStore.Purchasing\",\n  \"references\": [\n    \"GUID:" + CommonAsmdefGuid + "\"\n  ],\n  \"includePlatforms\": [],\n  \"excludePlatforms\": [],\n  \"allowUnsafeCode\": false,\n  \"overrideReferences\": false,\n  \"precompiledReferences\": [],\n  \"autoReferenced\": false,\n  \"defineConstraints\": [ \"GP_BUILD_PAYMENTS_ONESTORE\" ],\n  \"noEngineReferences\": false\n}\n") || changed;
            return PatchWrapperReference() || changed;
        }

        static bool WriteAsmdef(string assetPath, string guid, string contents)
        {
            var full = Path.Combine(Directory.GetParent(Application.dataPath).FullName, assetPath.Replace('/', Path.DirectorySeparatorChar));
            var dir = Path.GetDirectoryName(full);
            if (!Directory.Exists(dir))
                return false;
            var changed = GP_EditorFiles.WriteIfChanged(full, contents);
            var meta = full + ".meta";
            if (!File.Exists(meta))
            {
                File.WriteAllText(meta,
                    "fileFormatVersion: 2\nguid: " + guid +
                    "\nAssemblyDefinitionImporter:\n  externalObjects: {}\n  userData: \n  assetBundleName: \n  assetBundleVariant: \n");
                changed = true;
            }
            return changed;
        }

        static bool PatchWrapperReference()
        {
            var full = Path.Combine(Directory.GetParent(Application.dataPath).FullName,
                WrapperAsmdefPath.Replace('/', Path.DirectorySeparatorChar));
            if (!File.Exists(full))
                return false;
            var text = File.ReadAllText(full);
            if (text.IndexOf(PurchasingAsmdefGuid, StringComparison.Ordinal) >= 0)
                return false;
            const string needle = "\"GUID:e2c3782dc6445491e87d280768bacab5\"";
            var at = text.IndexOf(needle, StringComparison.Ordinal);
            if (at < 0)
                return false;
            text = text.Insert(at + needle.Length, ",\n    \"GUID:" + PurchasingAsmdefGuid + "\"");
            return GP_EditorFiles.WriteIfChanged(full, text);
        }

        static void RemoveBundledEdm4u()
        {
            DeleteAssetFolder("Assets/ExternalDependencyManager");
            DeleteAssetFolder("Assets/PlayServicesResolver");
        }

        static void RemoveUnusedModules()
        {
            DeleteAssetFolder("Assets/OneStoreCorpPlugins/AppLicenseChecker");
            DeleteAssetFolder("Assets/OneStoreCorpPlugins/Authentication");
        }

        static void DeleteAssetFolder(string assetPath)
        {
            if (AssetDatabase.IsValidFolder(assetPath))
                AssetDatabase.DeleteAsset(assetPath);
        }
    }
}
