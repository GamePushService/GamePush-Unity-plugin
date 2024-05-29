using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using GamePush;
using GamePush.Data;

namespace GamePushEditor
{
    public class GP_LoginWindow : EditorWindow
    {
        private const string SITE_URL = "https://gamepush.com";

        private const string VERSION = "2.0.1";

        private static bool _isDataFetch;
        private static string _id, _token;
        private static int _projectId;

        private static SavedProjectData _projectData;

        private static Vector2 _scrollPos;

        private static GUIStyle _titleStyle;

        private static int _menuOpened;

        private static SavedDataSO Config => Resources.Load<SavedDataSO>("ConfigSO");

        [MenuItem("Tools/GamePush/SetUp")]
        private static void ShowWindow()
        {
            var window = GetWindow<GP_LoginWindow>();
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
        }


        private static SavedProjectData GetSavedProjectData()
        {
            var path = AssetDatabase.GetAssetPath(Config.saveFile);
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
            _projectData = new SavedProjectData(_id, _token);

            var path = AssetDatabase.GetAssetPath(Config.saveFile);
            var json = JsonUtility.ToJson(_projectData);

            System.IO.File.WriteAllText(path, json);
            AssetDatabase.Refresh();
        }

        private static SavedProjectData GetProjectData()
        {
            if (ProjectData.ID == null)
            {
                SaveProjectData();
                return new SavedProjectData(_id, _token);
            }
            var savedProjectData = new SavedProjectData(ProjectData.ID, ProjectData.TOKEN);
            return savedProjectData;
        }

        private static void SaveProjectDataToScript()
        {
            _projectData = new SavedProjectData(_id, _token);

            var path = AssetDatabase.GetAssetPath(Config.projectData);
            var file = new System.IO.StreamWriter(path);

            file.WriteLine("namespace GamePush.Data");
            file.WriteLine("{");
            file.WriteLine("    public static class ProjectData");
            file.WriteLine("    {");
            file.WriteLine($"        public static string ID = \"{_projectData.id}\";");
            file.WriteLine($"        public static string TOKEN = \"{_projectData.token}\";");
            file.WriteLine("    }");
            file.WriteLine("}");
            file.Close();
            AssetDatabase.Refresh();
        }

        private static void DrawSeparator()
        {
            GUILayout.Space(10);
            var rect = EditorGUILayout.BeginHorizontal();
            Handles.color = Color.gray;
            Handles.DrawLine(new Vector2(rect.x - 15, rect.y), new Vector2(rect.width + 15, rect.y));
            EditorGUILayout.EndHorizontal();
            GUILayout.Space(10);
        }

        private void OnGUI()
        {
            _menuOpened = GUILayout.Toolbar(_menuOpened, new[] { "Setup", "Settings" });
            

            switch (_menuOpened)
            {
                case 0:
                    OnLoginGUI();
                    break;
                case 1:
                    OnSettingsGUI();
                    break;
            }

            GUILayout.Space(100);
            DrawSeparator();

            if (GUILayout.Button("<color=#04bc04>GamePush 2024</color>",
                    new GUIStyle { alignment = TextAnchor.LowerRight, richText = true }))
                Application.OpenURL(SITE_URL);

            GUILayout.Label($"<color=white>v{VERSION}</color>", new GUIStyle { alignment = TextAnchor.LowerRight });
        }

        private void OnLoginGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Enter project ID and token", _titleStyle);
            GUILayout.Space(10);

            _id = EditorGUILayout.TextField("ID", _id);
            GUILayout.Space(5);
            _token = EditorGUILayout.TextField("Token", _token);

            GUILayout.Space(20);
            if (GUILayout.Button("Save"))
                FetchConfig();
        }

        private void OnSettingsGUI()
        {
            GUILayout.Space(10);
            GUILayout.Label("Settings", _titleStyle);
            GUILayout.Space(10);
        }

        private static void FetchConfig()
        {
            Debug.Log("Save data");
            if (string.IsNullOrEmpty(_id) || string.IsNullOrEmpty(_token))
            {
                EditorUtility.DisplayDialog("GamePush Error", "Please fill all the fields.", "OK");
                return;
            }

            if (!ValidateId(_id)) return;
            if (!ValidateToken(_token)) return;

            int.TryParse(_id, out _projectId);

            Debug.Log("Set data");
            CoreSDK.SetProjectData(_projectId, _token);

            SetProjectDataToWebTemplate();
            SaveProjectData();
            SaveProjectDataToScript();

            Debug.Log("Fetch data");
            CoreSDK.FetchConfig();
            Debug.Log("Done");
        }

        private static bool ValidateId(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                EditorUtility.DisplayDialog("GamePush Error", "Please enter a name.", "OK");
                return false;
            }

            if (!Regex.IsMatch(input, @"^[0-9]+$"))
            {
                EditorUtility.DisplayDialog("GamePush Error", "The project ID can only contain numbers", "OK");
                return false;
            }
            return true;
        }

        private static bool ValidateToken(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                EditorUtility.DisplayDialog("GamePush Error", "Please enter a name.", "OK");
                return false;
            }

            if (!Regex.IsMatch(input, @"^[a-zA-Z0-9]+$"))
            {
                EditorUtility.DisplayDialog("GamePush Error", "The project token can only contain alphabetical letters and numbers", "OK");
                return false;
            }
            return true;
        }

        private static void SetProjectDataToWebTemplate()
        {
            PlayerSettings.SetTemplateCustomValue("PROJECT_ID", _id);
            PlayerSettings.SetTemplateCustomValue("TOKEN", _token);
        }
    }
}
