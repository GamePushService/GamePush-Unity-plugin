using System.Collections.Generic;
using System.IO;
using GamePush.Data;
using UnityEditor;
using UnityEngine;

namespace GamePushEditor
{
    static class GP_InitSceneUtility
    {
        public const string Path = "Assets/Plugins/GamePush/InitScene/AwaitInit.unity";

        public static bool Exists => File.Exists(Path);

        public static int BuildIndex
        {
            get
            {
                var scenes = EditorBuildSettings.scenes;
                for (var i = 0; i < scenes.Length; i++)
                {
                    if (scenes[i].path == Path)
                        return scenes[i].enabled ? i : -1;
                }
                return -1;
            }
        }

        public static bool IsReady => Exists && BuildIndex == 0;

        public static bool EnsureReady(out string error)
        {
            error = null;
            if (!Exists)
            {
                error = "Init scene not found: " + Path;
                return false;
            }

            EnableAwaitPluginReady();
            PlaceAtBuildIndexZero();
            return true;
        }

        public static void PlaceAtBuildIndexZero()
        {
            if (!Exists)
                return;

            var scenes = EditorBuildSettings.scenes;
            var list = new List<EditorBuildSettingsScene>(scenes);
            var existing = list.FindIndex(scene => scene.path == Path);
            if (existing == 0 && list[0].enabled)
                return;

            EditorBuildSettingsScene init;
            if (existing >= 0)
            {
                init = list[existing];
                list.RemoveAt(existing);
                init.enabled = true;
            }
            else
            {
                init = new EditorBuildSettingsScene(Path, true);
            }

            list.Insert(0, init);
            EditorBuildSettings.scenes = list.ToArray();
            GamePushEditor.Play2Web.GP_Play2WebWindow.PushLog("Init scene set as build index 0");
        }

        public static void RemoveFromBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes;
            var list = new List<EditorBuildSettingsScene>(scenes);
            if (list.RemoveAll(scene => scene.path == Path) > 0)
                EditorBuildSettings.scenes = list.ToArray();
        }

        static void EnableAwaitPluginReady()
        {
            var linker = Resources.Load<SavedDataSO>("GP_DataLinker");
            if (linker == null || linker.saveFile == null)
                return;

            var jsonPath = AssetDatabase.GetAssetPath(linker.saveFile);
            if (string.IsNullOrEmpty(jsonPath) || !File.Exists(jsonPath))
                return;

            var json = File.ReadAllText(jsonPath);
            var data = JsonUtility.FromJson<SavedProjectData>(json);
            if (data == null)
                return;

            var wrote = false;
            if (!data.waitPluginReady)
            {
                data.waitPluginReady = true;
                File.WriteAllText(jsonPath, JsonUtility.ToJson(data));
                wrote = true;
            }

            GP_Window.SyncWaitPluginReady(true);

            if (linker.projectData != null)
            {
                var sharpPath = AssetDatabase.GetAssetPath(linker.projectData);
                if (!string.IsNullOrEmpty(sharpPath) && File.Exists(sharpPath))
                {
                    var sharp = File.ReadAllText(sharpPath);
                    var updated = sharp.Replace("WAIT_PLAGIN_READY = false", "WAIT_PLAGIN_READY = true");
                    if (updated != sharp)
                    {
                        File.WriteAllText(sharpPath, updated);
                        wrote = true;
                    }
                }
            }

            if (wrote)
            {
                AssetDatabase.Refresh();
                GamePushEditor.Play2Web.GP_Play2WebWindow.PushLog("Enabled Await plugin ready");
            }
        }
    }
}
