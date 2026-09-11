using GamePush;
using GamePushEditor;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace GamePushEditor.Play2Web
{
    [InitializeOnLoad]
    static class GP_Play2WebPlayTest
    {
        const string UsedStartScenePref = "GamePush.Play2Web.PlayTestStartScene";
        const string PendingPlayPref = "GamePush.Play2Web.PendingPlayTest";

        static GP_Play2WebPlayTest()
        {
            EditorApplication.playModeStateChanged += OnPlayMode;
            CompilationPipeline.compilationFinished += _ => EditorApplication.delayCall += TryPendingPlay;
            EditorApplication.delayCall += TryPendingPlay;
        }

        public static void Start()
        {
            if (EditorApplication.isPlaying)
            {
                EditorPrefs.SetBool(PendingPlayPref, true);
                EditorApplication.isPlaying = false;
                return;
            }

            PrepareAndPlay();
        }

        public static void Stop()
        {
            EditorPrefs.SetBool(PendingPlayPref, false);
            GP_Play2Web.Requested = false;
            if (EditorApplication.isPlaying)
                EditorApplication.isPlaying = false;
        }

        static void PrepareAndPlay()
        {
            if (!GP_InitSceneUtility.EnsureReady(out var error))
            {
                EditorPrefs.SetBool(PendingPlayPref, false);
                EditorUtility.DisplayDialog("Play2Web", error, "OK");
                return;
            }

            if (!EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
            {
                EditorPrefs.SetBool(PendingPlayPref, false);
                ClearStartScene();
                return;
            }

            if (!AssignStartScene())
            {
                EditorPrefs.SetBool(PendingPlayPref, false);
                EditorUtility.DisplayDialog("Play2Web", "Could not load init scene asset.", "OK");
                return;
            }

            EditorPrefs.SetBool(PendingPlayPref, true);
            GP_Play2WebWindow.ClearLog();
            GP_Play2WebWindow.PushLog("Play Test from " + GP_InitSceneUtility.Path);

            if (EditorApplication.isCompiling)
                return;

            EnterPlay();
        }

        static bool AssignStartScene()
        {
            var asset = AssetDatabase.LoadAssetAtPath<SceneAsset>(GP_InitSceneUtility.Path);
            if (asset == null)
                return false;
            EditorSceneManager.playModeStartScene = asset;
            EditorPrefs.SetBool(UsedStartScenePref, true);
            return true;
        }

        static void EnterPlay()
        {
            EditorPrefs.SetBool(PendingPlayPref, false);
            GP_Play2Web.Requested = true;
            EditorApplication.isPlaying = true;
        }

        static void TryPendingPlay()
        {
            if (!EditorPrefs.GetBool(PendingPlayPref, false))
                return;
            if (EditorApplication.isPlaying || EditorApplication.isCompiling)
                return;
            if (!AssignStartScene())
            {
                EditorPrefs.SetBool(PendingPlayPref, false);
                return;
            }
            EnterPlay();
        }

        static void OnPlayMode(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.EnteredEditMode)
                return;

            if (EditorPrefs.GetBool(PendingPlayPref, false))
            {
                EditorApplication.delayCall += TryPendingPlay;
                return;
            }

            ClearStartScene();
        }

        static void ClearStartScene()
        {
            if (!EditorPrefs.GetBool(UsedStartScenePref, false))
                return;
            EditorSceneManager.playModeStartScene = null;
            EditorPrefs.SetBool(UsedStartScenePref, false);
        }
    }
}
