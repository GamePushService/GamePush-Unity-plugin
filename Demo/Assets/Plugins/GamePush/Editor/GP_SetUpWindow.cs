using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using GamePush;
using GamePush.Data;

namespace GamePushEditor
{
    public class GP_Window : EditorWindow
    {
        private const string SITE_URL = "https://gamepush.com";

        private const string VERSION = PluginData.SDK_VERSION;

        private const string INIT_SCENE = "Assets/Plugins/GamePush/InitScene/AwaitInit.unity";

        //private static bool _isDataFetch;

        private static int _id;
        private static string _token;

        private static bool _showPreloaderAd;
        private static bool _showStickyOnStart;
        private static bool _waitPluginReady;
        private static bool _gameReadyAuto;

        private static SavedProjectData _projectData;

        private static Vector2 _scrollPos;

        private static GUIStyle _titleStyle;
        private static GUIStyle _buttonStyle;

        private static int _menuOpened;

        private static SavedDataSO DataLinker => Resources.Load<SavedDataSO>("GP_DataLinker");

        [MenuItem("Tools/GamePush")]
        private static void ShowWindow()
        {
            var window = GetWindow<GP_Window>();
            window.minSize = new Vector2(300, 350);
            window.titleContent = new GUIContent("GamePush Settings");
            window.Show();

            //CheckVersion();
        }

        private void OnBecameVisible()
        {
            _titleStyle = new GUIStyle
            {
                fontSize = 20,
                fontStyle = FontStyle.Bold,
                normal = new GUIStyleState { textColor = Color.white }
            };

            _projectData = GetSavedProjectData();
            _id = _projectData.id;

            _token = _projectData.token;
            _showPreloaderAd = _projectData.showPreAd;
            _showStickyOnStart = _projectData.showStickyOnStart;
            _waitPluginReady = _projectData.waitPluginReady;
            _gameReadyAuto = _projectData.gameReadyAuto;
        }


        private static SavedProjectData GetSavedProjectData()
        {
            var path = AssetDatabase.GetAssetPath(DataLinker.saveFile);
            var file = new System.IO.StreamReader(path);
            var json = file.ReadToEnd();
            file.Close();

            if (string.IsNullOrWhiteSpace(json))
            {
                SaveProjectData();
                return new SavedProjectData(_id, _token);
            }

            var savedProjectData = JsonUtility.FromJson<SavedProjectData>(json);
            return savedProjectData;
        }

        private static void SaveProjectData()
        {
            _projectData = new SavedProjectData(
                _id,
                _token,
                _showPreloaderAd,
                _showStickyOnStart,
                _waitPluginReady,
                _gameReadyAuto
                );

            var path = AssetDatabase.GetAssetPath(DataLinker.saveFile);
            var json = JsonUtility.ToJson(_projectData);

            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        private static void SetProjectDataToWebTemplate()
        {
            PlayerSettings.SetTemplateCustomValue("PROJECT_ID", _id.ToString());
            PlayerSettings.SetTemplateCustomValue("TOKEN", _token.ToString());
            PlayerSettings.SetTemplateCustomValue("SHOW_PRELOADER_AD", _showPreloaderAd.ToString());
            PlayerSettings.SetTemplateCustomValue("GAMEREADY_AUTOCALL", _gameReadyAuto.ToString());
        }

        private static void SaveProjectDataToScript()
        {
            SaveProjectDataToJavaScript();
            SaveProjectDataToSharp();
        }

        private static void SaveProjectDataToSharp()
        {
            var path = AssetDatabase.GetAssetPath(DataLinker.projectData);
            var file = new System.IO.StreamWriter(path);

            string gameReadyBool = _gameReadyAuto.ToString().ToLower();
            string showStickyBool = _showStickyOnStart.ToString().ToLower();
            string waitPluginBool = _waitPluginReady.ToString().ToLower();

            file.WriteLine("namespace GamePush.Data");
            file.WriteLine("{");
            file.WriteLine("    public static class ProjectData");
            file.WriteLine("    {");
            file.WriteLine($"        public static string SDK_VERSION = \"{VERSION}\";");
            file.WriteLine($"        public static string ID = \"{_id}\";");
            file.WriteLine($"        public static string TOKEN = \"{_token}\";");
            file.WriteLine($"        public static bool GAMEREADY_AUTOCALL = {gameReadyBool};");
            file.WriteLine($"        public static bool SHOW_STICKY_ON_START = {showStickyBool};");
            file.WriteLine($"        public static bool WAIT_PLAGIN_READY = {waitPluginBool};");
            file.WriteLine("    }");
            file.WriteLine("}");
            file.Close();
            AssetDatabase.Refresh();
        }

        private static void SaveProjectDataToJavaScript()
        {
            var pathToJS = AssetDatabase.GetAssetPath(DataLinker.jsAnchor);
            var pathJspre = pathToJS.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(DataLinker.jsAnchor)), "_dataFields.jspre");
            

            var filePre = new StreamWriter(pathJspre);

            filePre.WriteLine($"const dataProjectId = \'{_id}\';");
            filePre.WriteLine($"const dataPublicToken = \'{_token}\';");
            filePre.WriteLine($"const showPreloaderAd = \'{_showPreloaderAd}\';");
            filePre.WriteLine($"const showStickyOnStart = \'{_showStickyOnStart}\';");
            filePre.WriteLine($"const waitPluginReady = \'{_waitPluginReady}\';");
            filePre.WriteLine($"const autocallGameReady = \'{_gameReadyAuto}\';");

            filePre.Close();

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region GUI

        private void OnGUI()
        {
            OnLoginGUI();
            //_menuOpened = GUILayout.Toolbar(_menuOpened, new[] { "Setup", "Settings" });


            //switch (_menuOpened)
            //{
            //    case 0:
            //        OnLoginGUI();
            //        break;
            //    case 1:
            //        OnSettingsGUI();
            //        break;
            //}

            GUILayout.Space(30);
            DrawSeparator();

            if (GUILayout.Button("<color=#04bc04>GamePush 2024</color>",
                    new GUIStyle { alignment = TextAnchor.LowerRight, richText = true }))
                Application.OpenURL(SITE_URL);

            GUILayout.Label($"<color=white>v{VERSION}</color>", new GUIStyle { alignment = TextAnchor.LowerRight });
        }

        private void OnLoginGUI()
        {
            GUILayout.Space(20);
            GUILayout.Label(" Enter project ID and token", _titleStyle);
            GUILayout.Space(10);

            _id = EditorGUILayout.IntField("Project ID", _id);
            GUILayout.Space(5);
            _token = EditorGUILayout.TextField("Token", _token);

            GUILayout.Space(15);
            GUILayout.Label(" Additional settings", _titleStyle);
            GUILayout.Space(10);

            _showPreloaderAd = EditorGUILayout.Toggle("Show Preloader Ad", _showPreloaderAd);
            GUILayout.Space(5);
            _showStickyOnStart = EditorGUILayout.Toggle("Show Sticky on Start", _showStickyOnStart);
            GUILayout.Space(5);
            _gameReadyAuto = EditorGUILayout.Toggle("GameReady Autocall", _gameReadyAuto);
            GUILayout.Space(5);
            _waitPluginReady = EditorGUILayout.Toggle("Await plugin ready", _waitPluginReady);

            GUILayout.Space(25);

            if (GUILayout.Button("Save", GUILayout.Height(30)))
                SaveConfig();
        }

        //private void OnSettingsGUI()
        //{
        //    GUILayout.Space(10);
        //    GUILayout.Label("Settings", _titleStyle);
        //    GUILayout.Space(10);

        //    _showPreloaderAd = EditorGUILayout.Toggle("Show Preloader Ad", _showPreloaderAd);
        //    GUILayout.Space(5);
        //    _gameReadyDelay = EditorGUILayout.IntField("Game Ready Delay", _gameReadyDelay);
        //}

        private static void DrawSeparator()
        {
            GUILayout.Space(10);
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }
        #endregion

        private static void SaveConfig()
        {
            //Console.Log("Saving data");
            if (_id == 0 || string.IsNullOrEmpty(_token))
            {
                EditorUtility.DisplayDialog("GamePush Error", "Please fill all the fields.", "OK");
                return;
            }

            if (!ValidateToken(_token)) return;

            SaveProjectData();

            SetProjectDataToWebTemplate();
            SaveProjectDataToScript();

            IniSceneHandle();

            GP_Logger.SystemLog("Data saved");
        }

        private static bool ValidateToken(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                EditorUtility.DisplayDialog("GamePush Error", "Please enter project token.", "OK");
                return false;
            }

            if (!Regex.IsMatch(input, @"^[a-zA-Z0-9]+$"))
            {
                EditorUtility.DisplayDialog("GamePush Error", "The project token can only contain alphabetical letters and numbers", "OK");
                return false;
            }
            return true;
        }

        private static void IniSceneHandle()
        {
            if (_waitPluginReady)
                AddSceneToBuildSettings();
            else
                RemoveSceneToBuildSettings();
        }

        private static void AddSceneToBuildSettings()
        {
            if (!System.IO.File.Exists(INIT_SCENE))
                return;

            var scenes = EditorBuildSettings.scenes;

            // Проверяем, добавлена ли уже сцена
            foreach (var scene in scenes)
            {
                if (scene.path == INIT_SCENE)
                {
                    return;
                }
            }

            // Добавляем сцену в список Build Settings
            var newScenes = new EditorBuildSettingsScene[scenes.Length + 1];
            newScenes[0] = new EditorBuildSettingsScene(INIT_SCENE, true);
            for (int i = 0; i < scenes.Length; i++)
            {
                newScenes[i+1] = scenes[i];
            }
            
            EditorBuildSettings.scenes = newScenes;
        }

        private static void RemoveSceneToBuildSettings()
        {
            var scenes = EditorBuildSettings.scenes;
            bool needToRemove = false;

            foreach (var scene in scenes)
            {
                if (scene.path == INIT_SCENE)
                {
                    needToRemove = true;
                    break;
                }
            }

            if (needToRemove)
            {
                var newScenes = new EditorBuildSettingsScene[scenes.Length - 1];
                for (int i = 0; i < newScenes.Length; i++)
                {
                    newScenes[i] = scenes[i+1];
                }

                EditorBuildSettings.scenes = newScenes;
            }
        }
    }
}
