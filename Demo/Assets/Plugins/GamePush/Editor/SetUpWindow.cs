using System.IO;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using GamePush;
using GamePush.Data;

namespace GamePushEditor
{
    public class SetUpWindow : EditorWindow
    {
        private const string SITE_URL = "https://gamepush.com";

        private const string VERSION = PluginData.SDK_VERSION;

        private static int _id;
        private static string _token;

        private static bool _showPreloaderAd;
        private static bool _gameReadyAuto;
        private static string _buildPlatform;

        private static SavedProjectData _projectData;

        private static Vector2 _scrollPos;

        private static GUIStyle _titleStyle;
        private static GUIStyle _buttonStyle;

        private static int _menuOpened;

        private static SavedDataSO DataLinker => Resources.Load<SavedDataSO>("GP_DataLinker");

        [MenuItem("Tools/GamePush/SetUp")]
        private static void ShowWindow()
        {
            var window = GetWindow<SetUpWindow>();
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
            _gameReadyAuto = _projectData.gameReadyAuto;
            _selectedIndex = _projectData.buildPlatformId;
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
            _projectData = new SavedProjectData(_id, _token, _showPreloaderAd, _gameReadyAuto, _selectedIndex);

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

            file.WriteLine("namespace GamePush.Data");
            file.WriteLine("{");
            file.WriteLine("    public static class ProjectData");
            file.WriteLine("    {");
            file.WriteLine($"        public static string SDK_VERSION = \"{VERSION}\";");
            file.WriteLine($"        public static string ID = \"{_id}\";");
            file.WriteLine($"        public static string TOKEN = \"{_token}\";");
            file.WriteLine($"        public static string BUILD_PLATFORM = \"{_platforms[_selectedIndex]}\";");
            file.WriteLine($"        public static bool GAMEREADY_AUTOCALL = {gameReadyBool};");
            file.WriteLine("    }");
            file.WriteLine("}");
            file.Close();
            AssetDatabase.Refresh();
        }

        private static void SaveProjectDataToJavaScript()
        {
            var path = AssetDatabase.GetAssetPath(DataLinker.jspreData);

            var pathJspre = path.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(DataLinker.jspreData)), "_dataFields.jspre");
            
            var file = new StreamWriter(path);

            file.WriteLine($"const dataProjectId = \'{_id}\';");
            file.WriteLine($"const dataPublicToken = \'{_token}\';");
            file.WriteLine($"const showPreloaderAd = \'{_showPreloaderAd}\';");
            file.WriteLine($"const autocallGameReady = \'{_gameReadyAuto}\';");
            file.WriteLine($"const buildPlatform = \'{_platforms[_selectedIndex]}\';");

            file.Close();

            var filePre = new StreamWriter(pathJspre);

            filePre.WriteLine($"const dataProjectId = \'{_id}\';");
            filePre.WriteLine($"const dataPublicToken = \'{_token}\';");
            filePre.WriteLine($"const showPreloaderAd = \'{_showPreloaderAd}\';");
            filePre.WriteLine($"const autocallGameReady = \'{_gameReadyAuto}\';");
            filePre.WriteLine($"const buildPlatform = \'{_platforms[_selectedIndex]}\';");

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

        private static int _selectedIndex;
        private static string[] _platforms = new string[] {
             PlatformTypes.ANDROID,
             PlatformTypes.GOOGLE_PLAY,
             PlatformTypes.APP_GALLERY,
             PlatformTypes.GALAXY_STORE,
             PlatformTypes.ONE_STORE,
             PlatformTypes.AMAZON_APPSTORE,
             PlatformTypes.XIAOMI_GETAPPS,
             PlatformTypes.APTOIDE,
             PlatformTypes.RUSTORE,
             PlatformTypes.CUSTOM_ANDROID,
        };
        private enum AndroidPlatform { ANDROID, GOOGLE_PLAY, HUAWEI}

        private void OnLoginGUI()
        {
            GUILayout.Space(20);
            GUILayout.Label("  Enter project ID and token", _titleStyle);
            GUILayout.Space(10);

            _id = EditorGUILayout.IntField("  Project ID", _id);
            GUILayout.Space(5);
            _token = EditorGUILayout.TextField("  Token", _token);

            GUILayout.Space(15);
            GUILayout.Label("  Additional settings", _titleStyle);
            GUILayout.Space(10);

            _showPreloaderAd = EditorGUILayout.Toggle("  Show Preloader Ad", _showPreloaderAd);
            GUILayout.Space(5);
            _gameReadyAuto = EditorGUILayout.Toggle("  GameReady Autocall", _gameReadyAuto);


            Rect rect = new Rect(10, 210, position.width - 20, 20);

            // Отображаем выпадающий список
            _selectedIndex = EditorGUI.Popup(rect, "Mobile target platform", _selectedIndex, _platforms);


            GUILayout.Space(55);


            if (GUILayout.Button("Save", GUILayout.Height(30)))
                SaveConfig();

            if (GUILayout.Button("Sync", GUILayout.Height(30)))
                SyncConfig();
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
            CoreSDK.SetProjectData(_id, _token);
            CoreSDK.SetAndroidPlatform(_platforms[_selectedIndex]);
            SaveProjectDataToScript();
            SaveProjectData();

            SetProjectDataToWebTemplate();

            GP_Logger.SystemLog("Data saved");
        }

        private static async void SyncConfig()
        {
            CoreSDK.SetProjectData(_id, _token);
            CoreSDK.SetAndroidPlatform(_platforms[_selectedIndex]);

            await CoreSDK.FetchEditorConfig();
            GP_Logger.SystemLog("Config fetched");
            CheckCustromAds();
        }

        private static void CheckCustromAds()
        {
            bool customAd = CoreSDK.platformConfig.customAdsConfigId != "";

            ChangeDefineSymbols("CUSTOM_ADS_MOBILE", customAd);

            bool isYAMON = false;
            if (customAd)
            {
                foreach (AdBanner adBanner in CoreSDK.platformConfig.customAdsConfig.configs.android.banners)
                {
                    if (adBanner.adServer.ToString() == AdServerType.YandexSimpleMonetization.ToString())
                    {
                        isYAMON = true;
                    }
                }
            }

            ChangeDefineSymbols("YANDEX_SIMPLE_MONETIZATION", isYAMON);
        }

        private static void ChangeDefineSymbols(string symbols, bool isSet)
        {
            if (isSet)
            {
                GP_Logger.SystemLog("Add Define Symbols: " + symbols);
                DefineSymbolsManager.AddScriptingDefineSymbol(symbols);
            }
            else
            {
                GP_Logger.SystemLog("Remove Define Symbols: " + symbols);
                DefineSymbolsManager.RemoveScriptingDefineSymbol(symbols);
            }
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

        
    }
}
