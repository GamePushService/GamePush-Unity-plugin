using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using GamePush;
using GamePush.Data;
using GamePushEditor.Adapters;
using Plugins.GamePush.Editor;

namespace GamePushEditor
{
    public class GP_Window : EditorWindow
    {
        private const string SITE_URL = "https://gamepush.com";
        private const string VERSION = PluginData.SDK_VERSION;
        private const string ApiSecretPref = "GamePush.ApiSecret";

        private static readonly string[] TabNames = { "Main", "Android", "Platform emulator", "In-apps" };
        private static readonly string[] AndroidPlatforms =
        {
            "ANDROID", "GOOGLE_PLAY", "RUSTORE", "APP_GALLERY", "GALAXY_STORE",
            "ONE_STORE", "AMAZON_APPSTORE", "XIAOMI_GETAPPS", "APTOIDE",
            "XIAOMI_GAMECENTER", "CUSTOM_ANDROID"
        };
        private static readonly string[] AndroidPlatformLabels =
        {
            "Android (generic)", "Google Play", "RuStore", "AppGallery", "Galaxy Store",
            "One Store", "Amazon Appstore", "Xiaomi GetApps", "Aptoide",
            "Xiaomi Game Center", "Custom Android"
        };

        private static SavedProjectData _projectData;
        private static GUIStyle _titleStyle;
        private static GUIStyle _providerBox;
        private static GUIStyle _providerTitle;
        private static GUIStyle _providerActiveStatus;
        private static bool _providerStylesPro;
        private static int _menuOpened;

        private bool _revealProjectId;
        private bool _revealToken;
        private bool _revealApiKey;
        private string _apiKey = string.Empty;
        private bool _loadingProducts;

        private Vector2 _emulatorScroll;
        private Vector2 _paymentsScroll;
        private Vector2 _androidScroll;
        private Editor _platformEditor;
        private Editor _paymentsEditor;
        private GP_PlatformSettings _platformSettings;
        private GP_PaymentsStub _paymentsStub;

        private static SavedDataSO DataLinker => Resources.Load<SavedDataSO>("GP_DataLinker");

        [MenuItem("Tools/GamePush")]
        private static void ShowWindow()
        {
            var window = GetWindow<GP_Window>();
            window.minSize = new Vector2(360, 480);
            window.titleContent = new GUIContent("GamePush Settings");
            window.Show();
        }

        private void OnEnable()
        {
            _projectData = GetSavedProjectData();
            _apiKey = EditorPrefs.GetString(ApiSecretPref, string.Empty);
            EnsureAssetEditors();
        }

        private void OnBecameVisible()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState { textColor = Color.white }
                };
            }
        }

        private void OnDisable()
        {
            DestroyEditor(ref _platformEditor);
            DestroyEditor(ref _paymentsEditor);
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
                return new SavedProjectData(){ id = _projectData.id, token = _projectData.token };
            }

            var savedProjectData = JsonUtility.FromJson<SavedProjectData>(json);
            if (json.IndexOf("\"adsStubs\"", StringComparison.Ordinal) < 0)
                savedProjectData.adsStubs = true;
            if (json.IndexOf("\"paymentsStubs\"", StringComparison.Ordinal) < 0)
                savedProjectData.paymentsStubs = true;
            if (string.IsNullOrEmpty(savedProjectData.androidPlatform))
                savedProjectData.androidPlatform = "ANDROID";
            if (savedProjectData.androidPlatformTag == null)
                savedProjectData.androidPlatformTag = "";
            if (json.IndexOf("\"nativeDebugConsole\"", StringComparison.Ordinal) < 0)
                savedProjectData.nativeDebugConsole = true;
            if (json.IndexOf("\"nativeOverlays\"", StringComparison.Ordinal) < 0)
                savedProjectData.nativeOverlays = true;
            if (json.IndexOf("\"autoPauseOnOverlay\"", StringComparison.Ordinal) < 0)
                savedProjectData.autoPauseOnOverlay = true;
            return savedProjectData;
        }

        private static void SaveProjectData()
        {
            var path = AssetDatabase.GetAssetPath(DataLinker.saveFile);
            var json = JsonUtility.ToJson(_projectData);
            if (GP_EditorFiles.WriteIfChanged(path, json))
                AssetDatabase.Refresh();
        }

        private static void SetProjectDataToWebTemplate()
        {
            PlayerSettings.SetTemplateCustomValue("PROJECT_ID", _projectData.id.ToString());
            PlayerSettings.SetTemplateCustomValue("TOKEN", _projectData.token.ToString());
            PlayerSettings.SetTemplateCustomValue("SHOW_PRELOADER_AD", _projectData.showPreloadAd.ToString());
        }

        private static void SaveProjectDataToScript(bool refresh = true)
        {
            SaveProjectDataToJavaScript();
            SaveProjectDataToSharp(refresh);
        }

        private static void SaveProjectDataToSharp(bool refresh = true)
        {
            var path = AssetDatabase.GetAssetPath(DataLinker.projectData);
            string showStickyBool = _projectData.showStickyOnStart.ToString().ToLower();
            string waitPluginBool = _projectData.waitPluginReady.ToString().ToLower();
            string autoPauseBool = _projectData.autoPause.ToString().ToLower();
            string adsStubsBool = _projectData.adsStubs.ToString().ToLower();
            string paymentsStubsBool = _projectData.paymentsStubs.ToString().ToLower();
            string sdkLiveBool = _projectData.sdkLive.ToString().ToLower();
            string fullLogsBool = (GP_Settings.instance != null && GP_Settings.instance.fullLogs)
                .ToString().ToLower();
            string nativeDebugConsoleBool = _projectData.nativeDebugConsole.ToString().ToLower();
            string nativeOverlaysBool = _projectData.nativeOverlays.ToString().ToLower();
            string autoPauseOnOverlayBool = _projectData.autoPauseOnOverlay.ToString().ToLower();
            var androidPlatform = string.IsNullOrEmpty(_projectData.androidPlatform)
                ? "ANDROID"
                : _projectData.androidPlatform;

            var file = new StringBuilder();
            file.AppendLine("namespace GamePush.Data");
            file.AppendLine("{");
            file.AppendLine("    public static class ProjectData");
            file.AppendLine("    {");
            file.AppendLine($"        public static string SDK_VERSION = \"{VERSION}\";");
            file.AppendLine($"        public static string ID = \"{_projectData.id}\";");
            file.AppendLine($"        public static string TOKEN = \"{_projectData.token}\";");
            file.AppendLine($"        public static bool SHOW_STICKY_ON_START = {showStickyBool};");
            file.AppendLine($"        public static bool WAIT_PLAGIN_READY = {waitPluginBool};");
            file.AppendLine($"        public static bool AUTO_PAUSE_ON_ADS = {autoPauseBool};");
            file.AppendLine($"        public static bool ADS_STUBS = {adsStubsBool};");
            file.AppendLine($"        public static bool PAYMENTS_STUBS = {paymentsStubsBool};");
            file.AppendLine($"        public static bool SDK_LIVE = {sdkLiveBool};");
            file.AppendLine($"        public static bool FULL_LOGS = {fullLogsBool};");
            file.AppendLine($"        public static bool NATIVE_DEBUG_CONSOLE = {nativeDebugConsoleBool};");
            file.AppendLine($"        public static bool NATIVE_OVERLAYS = {nativeOverlaysBool};");
            file.AppendLine($"        public static bool AUTO_PAUSE_ON_OVERLAY = {autoPauseOnOverlayBool};");
            file.AppendLine($"        public static string ANDROID_PLATFORM = \"{CsString(androidPlatform)}\";");
            file.AppendLine($"        public static string ANDROID_PLATFORM_TAG = \"{CsString(_projectData.androidPlatformTag)}\";");
            file.AppendLine("    }");
            file.AppendLine("}");
            if (!GP_EditorFiles.WriteIfChanged(path, file.ToString()))
                return;
            if (refresh)
                AssetDatabase.Refresh();
        }

        private static void SaveProjectDataToJavaScript()
        {
            var pathToJS = AssetDatabase.GetAssetPath(DataLinker.jsAnchor);
            var pathJspre = pathToJS.Replace(Path.GetFileName(AssetDatabase.GetAssetPath(DataLinker.jsAnchor)), "_dataFields.jspre");
            var filePre = new StringBuilder();
            filePre.AppendLine($"const dataProjectId = \'{_projectData.id}\';");
            filePre.AppendLine($"const dataPublicToken = \'{_projectData.token}\';");
            filePre.AppendLine($"const showPreloaderAd = \'{_projectData.showPreloadAd}\';");
            filePre.AppendLine("const gpFullLogs = " +
                              (GP_Settings.instance != null && GP_Settings.instance.fullLogs ? "true" : "false") +
                              ";");
            if (!GP_EditorFiles.WriteIfChanged(pathJspre, filePre.ToString()))
                return;
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }

        #region GUI

        private void OnGUI()
        {
            if (_titleStyle == null)
            {
                _titleStyle = new GUIStyle
                {
                    fontSize = 20,
                    fontStyle = FontStyle.Bold,
                    normal = new GUIStyleState { textColor = Color.white }
                };
            }

            if (_projectData == null)
                _projectData = GetSavedProjectData();

            EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);
            _menuOpened = GUILayout.Toolbar(
                _menuOpened,
                TabNames,
                EditorStyles.toolbarButton,
                GUI.ToolbarButtonSize.FitToContents);
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            switch (_menuOpened)
            {
                case 1:
                    OnAndroidGUI();
                    break;
                case 2:
                    OnPlatformEmulatorGUI();
                    break;
                case 3:
                    OnPaymentsGUI();
                    break;
                default:
                    OnLoginGUI();
                    break;
            }

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

            _projectData.id = DrawSecretIntField("Project ID", _projectData.id, ref _revealProjectId);
            GUILayout.Space(5);
            _projectData.token = DrawSecretTextField("Token", _projectData.token, ref _revealToken);

            GUILayout.Space(15);
            GUILayout.Label(" Additional settings", _titleStyle);
            GUILayout.Space(10);

            _projectData.showPreloadAd = EditorGUILayout.Toggle("Show Preloader Ad", _projectData.showPreloadAd);
            GUILayout.Space(5);
            _projectData.showStickyOnStart = EditorGUILayout.Toggle("Show Sticky on Start", _projectData.showStickyOnStart);
            GUILayout.Space(5);
            _projectData.waitPluginReady = EditorGUILayout.Toggle("Await plugin ready", _projectData.waitPluginReady);
            GUILayout.Space(5);
            _projectData.autoPause = EditorGUILayout.Toggle("Pause music on ads", _projectData.autoPause);

            GUILayout.Space(15);
            GUILayout.Label(" Editor Settings", _titleStyle);
            GUILayout.Space(10);

            _projectData.adsStubs = EditorGUILayout.Toggle("Ads stubs", _projectData.adsStubs);
            GUILayout.Space(5);
            _projectData.paymentsStubs = EditorGUILayout.Toggle("Payments stubs", _projectData.paymentsStubs);
            GUILayout.Space(5);
            _projectData.sdkLive = EditorGUILayout.Toggle("SDK Live", _projectData.sdkLive);
            GUILayout.Space(5);
            if (GUILayout.Button("Open Play2Web"))
                GamePushEditor.Play2Web.GP_Play2WebWindow.Open();
            EditorGUILayout.HelpBox(
                "SDK Live: editor play talks to the real GamePush SDK instead of stubs. Overlay still starts only from Play Test.",
                MessageType.None);

            GUILayout.Space(15);
            GUILayout.Label(" Native plugin", _titleStyle);
            GUILayout.Space(10);
            _projectData.nativeDebugConsole = EditorGUILayout.Toggle("Debug console", _projectData.nativeDebugConsole);
            EditorGUILayout.HelpBox(
                "Bottom-right overlay on native Android. Not shown in the Unity Editor. On by default. Turn off, Save, then rebuild the APK to hide it in release.",
                MessageType.None);

            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            _projectData.nativeOverlays = EditorGUILayout.Toggle("UI overlays", _projectData.nativeOverlays);
            if (GUILayout.Button("Configure", GUILayout.Width(96f)))
                GamePushEditor.Overlays.GP_OverlaySettingsWindow.Open();
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.HelpBox(
                "uGUI screens for achievements, leaderboards, chat, documents, game collections, feedback and confirm on Android/Windows. Turn off to keep the previous no-op behaviour.",
                MessageType.None);

            using (new EditorGUI.DisabledScope(!_projectData.nativeOverlays))
                _projectData.autoPauseOnOverlay =
                    EditorGUILayout.Toggle("Pause game on overlay", _projectData.autoPauseOnOverlay);
            EditorGUILayout.HelpBox(
                "Sets Time.timeScale to 0 while any overlay is open, the same way ads already do.",
                MessageType.None);

            if (!GamePushEditor.Overlays.GP_OverlayPrefabBuilder.IsTextMeshProReady)
                EditorGUILayout.HelpBox(
                    "TextMeshPro Essential Resources are missing. The overlay prefabs need them; the rebuild button will offer the import.",
                    MessageType.Warning);

            GUILayout.Space(25);

            if (GUILayout.Button("Save", GUILayout.Height(30)))
                SaveConfig();
        }

        private void OnAndroidGUI()
        {
            _androidScroll = EditorGUILayout.BeginScrollView(_androidScroll);

            var androidTarget = EditorUserBuildSettings.activeBuildTarget == BuildTarget.Android;
            if (!androidTarget)
            {
                GUILayout.Space(16);
                EditorGUILayout.HelpBox(
                    "Active build target is " + EditorUserBuildSettings.activeBuildTarget +
                    ". Switch the Unity platform to Android (File → Build Profiles) to edit store, providers, Save and Sync.",
                    MessageType.Warning);
            }

            EditorGUI.BeginDisabledGroup(!androidTarget);

            GUILayout.Space(16);
            DrawAndroidEnvironment();

            GUILayout.Space(16);
            GUILayout.Label(" Mobile platform", _titleStyle);
            GUILayout.Space(8);
            EditorGUILayout.HelpBox(
                "One APK = one store. Save or Sync stores the selected store and packs providers. That bake stays until the next Save or Sync. Auth providers stay live (WebView in APK).",
                MessageType.Info);

            var platform = string.IsNullOrEmpty(_projectData.androidPlatform) ? "ANDROID" : _projectData.androidPlatform;
            var index = Array.IndexOf(AndroidPlatforms, platform);
            if (index < 0)
                index = 0;
            index = EditorGUILayout.Popup("Store", index, AndroidPlatformLabels);
            _projectData.androidPlatform = AndroidPlatforms[index];
            if (_projectData.androidPlatform == "CUSTOM_ANDROID")
                _projectData.androidPlatformTag = EditorGUILayout.TextField("Platform tag", _projectData.androidPlatformTag ?? "");

            GUILayout.Space(20);
            DrawStoreSnapshot();

            GUILayout.Space(20);
            GUILayout.Label(" Ad providers", _titleStyle);
            GUILayout.Space(6);
            EditorGUILayout.HelpBox(
                "Yandex Mobile Ads is packed only if this store uses it. Unit IDs come from the GamePush dashboard.",
                MessageType.None);
            DrawAdapterGrid(GP_AdapterRegistry.Ads);

            GUILayout.Space(14);
            GUILayout.Label(" Auth providers", _titleStyle);
            GUILayout.Space(6);
            EditorGUILayout.HelpBox(
                "Shared WebView, packed if this store has Android auth. Google / Yandex / Xsolla switch at launch from the live config.",
                MessageType.None);
            DrawAdapterGrid(GP_AdapterRegistry.Auth);

            GUILayout.Space(14);
            GUILayout.Label(" Payment providers", _titleStyle);
            GUILayout.Space(6);
            EditorGUILayout.HelpBox(
                "Robokassa/Xsolla/Stripe share the auth WebView; Google Play uses Unity IAP; One Store uses the official plugin. Unused SDKs stay out of the APK.",
                MessageType.None);
            DrawAdapterGrid(GP_AdapterRegistry.Payments);

            if (!string.IsNullOrEmpty(GP_YandexMobileAdsInstaller.Status))
                EditorGUILayout.HelpBox(GP_YandexMobileAdsInstaller.Status, MessageType.None);
            if (!string.IsNullOrEmpty(GP_OneStoreInstaller.Status))
                EditorGUILayout.HelpBox(GP_OneStoreInstaller.Status, MessageType.None);
            if (!string.IsNullOrEmpty(GP_PlatformSnapshot.Status))
                EditorGUILayout.HelpBox(GP_PlatformSnapshot.Status, MessageType.None);

            GUILayout.Space(20);
            GUILayout.Label(" Debug console", _titleStyle);
            GUILayout.Space(6);
            _projectData.nativeDebugConsole = EditorGUILayout.Toggle("Show overlay", _projectData.nativeDebugConsole);
            EditorGUILayout.HelpBox(
                "Bottom-right native log overlay on Android. Not shown in the Unity Editor. On by default. Uncheck, Save, and rebuild to hide it in a release APK.",
                MessageType.None);

            GUILayout.Space(20);
            if (GUILayout.Button("Save", GUILayout.Height(30)))
                SaveConfig();

            EditorGUI.EndDisabledGroup();
            EditorGUILayout.EndScrollView();
        }

        private static void DrawAndroidEnvironment()
        {
            GUILayout.Label(" Android toolchain", _titleStyle);
            GUILayout.Space(8);

            if (!GP_AndroidTooling.AndroidModuleInstalled)
            {
                EditorGUILayout.HelpBox(
                    "Android Build Support is missing for this Unity version. Hub → this editor → Add modules → Android Build Support (SDK, NDK, OpenJDK).",
                    MessageType.Error);
            }
            else if (!GP_AndroidTooling.HasWorkingJava())
            {
                EditorGUILayout.HelpBox(
                    "Yandex Ads' EDM4U needs Java. This PC has no JAVA_HOME. Unity's OpenJDK can be used for this Editor session only.",
                    MessageType.Warning);
                if (GUILayout.Button("Use Unity JDK for this session", GUILayout.Height(24)))
                    GP_AndroidTooling.UseUnityJdkForThisSession();
            }
            else
            {
                EditorGUILayout.HelpBox("Java: " + GP_AndroidTooling.JavaHome, MessageType.None);
            }

            EditorGUILayout.HelpBox(
                "If the editor recompiles every time it gains focus: Assets → External Dependency Manager → Android Resolver → Settings → uncheck Enable Auto-Resolution.",
                MessageType.Info);

            EditorGUI.BeginDisabledGroup(!GP_AdapterRegistry.IsYandexAdsInstalled);
            if (GUILayout.Button("Resolve Android dependencies now", GUILayout.Height(24)))
            {
                if (!GP_AndroidTooling.TryResolveNow())
                    EditorUtility.DisplayDialog("GamePush",
                        "Could not start Android Resolver. Check Java above, then use Assets → External Dependency Manager → Android Resolver → Resolve.",
                        "OK");
                else
                {
                    var snapshot = GP_PlatformSnapshot.Load();
                    if (snapshot != null)
                        GP_AndroidBuildAdapters.ApplyGradle(snapshot);
                }
            }
            EditorGUI.EndDisabledGroup();
        }

        private static void DrawStoreSnapshot()
        {
            var snapshot = GP_PlatformSnapshot.Load();
            EditorGUILayout.HelpBox(GP_PlatformSnapshot.Describe(snapshot), MessageType.Info);
            EditorGUI.BeginDisabledGroup(GP_PlatformSnapshot.Busy || _projectData.id == 0 || string.IsNullOrEmpty(_projectData.token));
            if (GUILayout.Button(GP_PlatformSnapshot.Busy ? "Syncing…" : "Sync platform config", GUILayout.Height(24)))
                SaveConfig();
            EditorGUI.EndDisabledGroup();
        }

        private static void DrawAdapterGrid(GP_AdapterInfo[] adapters)
        {
            if (adapters == null || adapters.Length == 0)
                return;

            const float gap = 6f;
            const float minTile = 148f;
            var width = Mathf.Max(minTile, EditorGUIUtility.currentViewWidth - 36f);
            var columns = Mathf.Max(1, Mathf.FloorToInt((width + gap) / (minTile + gap)));
            var tile = (width - gap * (columns - 1)) / columns;
            var snapshot = GP_PlatformSnapshot.Load();

            for (var i = 0; i < adapters.Length; i++)
            {
                if (i % columns == 0)
                {
                    if (i > 0)
                        GUILayout.Space(gap);
                    EditorGUILayout.BeginHorizontal();
                }
                else
                    GUILayout.Space(gap);

                DrawAdapterTile(adapters[i], tile, snapshot);

                if (i % columns == columns - 1 || i == adapters.Length - 1)
                {
                    GUILayout.FlexibleSpace();
                    EditorGUILayout.EndHorizontal();
                }
            }
        }

        static bool IsProviderActive(GP_AdapterInfo adapter, GP_AndroidAdapterSnapshot snapshot)
        {
            if (adapter == null || snapshot == null || string.IsNullOrEmpty(adapter.SdkId))
                return false;
            if (adapter.Kind == GP_AdapterKind.Ads)
                return adapter.SdkId == snapshot.adsService ||
                       (snapshot.needYandexAds && adapter.Id == GP_AdapterRegistry.YandexAdsId);
            if (adapter.Kind == GP_AdapterKind.Auth)
                return adapter.SdkId == snapshot.authService;
            if (adapter.Kind == GP_AdapterKind.Payments)
                return adapter.SdkId == snapshot.paymentsService;
            return false;
        }

        static void EnsureProviderStyles()
        {
            var pro = EditorGUIUtility.isProSkin;
            if (_providerBox != null && _providerStylesPro == pro)
                return;

            _providerBox = new GUIStyle(EditorStyles.helpBox)
            {
                padding = new RectOffset(8, 8, 5, 5),
                margin = new RectOffset(0, 0, 0, 0)
            };
            _providerTitle = new GUIStyle(EditorStyles.boldLabel)
            {
                wordWrap = true,
                clipping = TextClipping.Clip
            };
            _providerActiveStatus = new GUIStyle(EditorStyles.miniLabel)
            {
                fontStyle = FontStyle.Bold,
                wordWrap = true
            };
            _providerActiveStatus.normal.textColor = pro
                ? new Color(0.55f, 0.95f, 0.65f)
                : new Color(0.05f, 0.42f, 0.16f);
            _providerStylesPro = pro;
        }

        static void DrawRectBorder(Rect rect, Color color, float thickness)
        {
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.yMax - thickness, rect.width, thickness), color);
            EditorGUI.DrawRect(new Rect(rect.x, rect.y, thickness, rect.height), color);
            EditorGUI.DrawRect(new Rect(rect.xMax - thickness, rect.y, thickness, rect.height), color);
        }

        private static void DrawAdapterTile(GP_AdapterInfo adapter, float size, GP_AndroidAdapterSnapshot snapshot)
        {
            EnsureProviderStyles();
            var active = IsProviderActive(adapter, snapshot);
            string version = null;
            var installed = IsAdapterInstalled(adapter, out version);
            var busy = AdapterInstallBusy(adapter);
            var status = active ? "Active"
                : adapter.Installable ? (installed ? "Installed" : "Not installed")
                : adapter.ComingSoon ? "Soon"
                : "Built-in";

            var prevBg = GUI.backgroundColor;
            if (active)
            {
                GUI.backgroundColor = EditorGUIUtility.isProSkin
                    ? new Color(0.28f, 0.58f, 0.36f, 1f)
                    : new Color(0.58f, 0.88f, 0.64f, 1f);
            }

            EditorGUILayout.BeginVertical(_providerBox, GUILayout.Width(size));
            GUI.backgroundColor = prevBg;
            GUILayout.Label(new GUIContent(adapter.Title, adapter.Description), _providerTitle);

            EditorGUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent(status, adapter.Description),
                active ? _providerActiveStatus : EditorStyles.miniLabel);
            GUILayout.FlexibleSpace();
            if (adapter.Installable)
            {
                EditorGUI.BeginDisabledGroup(installed || busy);
                if (GUILayout.Button(new GUIContent(
                        busy ? "…" : (installed ? version : "Install"),
                        adapter.Description), GUILayout.Width(64), GUILayout.Height(18)))
                    InstallAdapter(adapter);
                EditorGUI.EndDisabledGroup();
            }
            if (!string.IsNullOrEmpty(adapter.DocsUrl))
            {
                if (GUILayout.Button(new GUIContent("Docs", adapter.Description),
                        GUILayout.Width(44), GUILayout.Height(18)))
                    Application.OpenURL(adapter.DocsUrl);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();

            if (active && Event.current.type == EventType.Repaint)
            {
                var accent = EditorGUIUtility.isProSkin
                    ? new Color(0.42f, 0.92f, 0.55f)
                    : new Color(0.12f, 0.52f, 0.26f);
                DrawRectBorder(GUILayoutUtility.GetLastRect(), accent, 2f);
            }
        }

        static bool IsAdapterInstalled(GP_AdapterInfo adapter, out string version)
        {
            version = null;
            if (adapter == null || !adapter.Installable)
                return false;
            if (adapter.Id == GP_AdapterRegistry.OneStoreId)
            {
                if (!GP_AdapterRegistry.IsOneStoreInstalled)
                    return false;
                version = GP_AdapterRegistry.OneStoreVersion;
                return true;
            }
            return GP_AdapterRegistry.TryGetInstalledVersion(adapter.PackageName, out version);
        }

        static bool AdapterInstallBusy(GP_AdapterInfo adapter)
        {
            if (adapter == null)
                return false;
            if (adapter.Id == GP_AdapterRegistry.OneStoreId)
                return GP_OneStoreInstaller.Busy;
            if (adapter.Id == GP_AdapterRegistry.YandexAdsId)
                return GP_YandexMobileAdsInstaller.Busy;
            return false;
        }

        static void InstallAdapter(GP_AdapterInfo adapter)
        {
            if (adapter == null)
                return;
            if (adapter.Id == GP_AdapterRegistry.OneStoreId)
                GP_OneStoreInstaller.Install();
            else if (adapter.Id == GP_AdapterRegistry.YandexAdsId)
                GP_YandexMobileAdsInstaller.Install();
        }

        private void OnPlatformEmulatorGUI()
        {
            EnsureAssetEditors();
            if (_platformEditor == null)
            {
                EditorGUILayout.HelpBox("GP_PlatformSettings asset was not found in Resources or Project Settings.", MessageType.Warning);
                return;
            }

            _emulatorScroll = EditorGUILayout.BeginScrollView(_emulatorScroll);
            _platformEditor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }

        private void OnPaymentsGUI()
        {
            EnsureAssetEditors();

            EditorGUILayout.Space(8);
            _apiKey = DrawSecretTextField("API key", _apiKey, ref _revealApiKey);
            EditorGUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(_loadingProducts);
            if (GUILayout.Button("Load products", GUILayout.Height(24)))
                LoadProductsFromApi();
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("Clear key", GUILayout.Width(80), GUILayout.Height(24)))
            {
                _apiKey = string.Empty;
                EditorPrefs.DeleteKey(ApiSecretPref);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import JSON", GUILayout.Height(24)))
                ImportProductsFile("json");
            if (GUILayout.Button("Import CSV", GUILayout.Height(24)))
                ImportProductsFile("csv");
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(8);
            if (_paymentsEditor == null)
            {
                EditorGUILayout.HelpBox("GP_PaymentsStub asset was not found in Resources or Project Settings.", MessageType.Warning);
                return;
            }

            _paymentsScroll = EditorGUILayout.BeginScrollView(_paymentsScroll);
            _paymentsEditor.OnInspectorGUI();
            EditorGUILayout.EndScrollView();
        }

        private void EnsureAssetEditors()
        {
            _platformSettings = ResolvePlatformSettings();
            _paymentsStub = ResolvePaymentsStub();

            if (_platformEditor == null || _platformEditor.target != _platformSettings)
            {
                DestroyEditor(ref _platformEditor);
                if (_platformSettings != null)
                    _platformEditor = Editor.CreateEditor(_platformSettings);
            }

            if (_paymentsEditor == null || _paymentsEditor.target != _paymentsStub)
            {
                DestroyEditor(ref _paymentsEditor);
                if (_paymentsStub != null)
                    _paymentsEditor = Editor.CreateEditor(_paymentsStub);
            }
        }

        private static GP_PlatformSettings ResolvePlatformSettings()
        {
            GP_Settings settings = GP_SettingsWrap.instance != null ? GP_SettingsWrap.instance.settings : null;
            if (settings != null && settings.platformSettings != null)
                return settings.platformSettings;
            return Resources.Load<GP_PlatformSettings>("GP_PlatformSettings");
        }

        private static GP_PaymentsStub ResolvePaymentsStub()
        {
            GP_Settings settings = GP_SettingsWrap.instance != null ? GP_SettingsWrap.instance.settings : null;
            if (settings != null && settings.paymentsStub != null)
                return settings.paymentsStub;
            return Resources.Load<GP_PaymentsStub>("GP_PaymentsStub");
        }

        private static void DestroyEditor(ref Editor editor)
        {
            if (editor == null)
                return;
            DestroyImmediate(editor);
            editor = null;
        }

        private void ImportProductsFile(string extension)
        {
            string path = EditorUtility.OpenFilePanel("Import GamePush products", "", extension);
            if (string.IsNullOrEmpty(path))
                return;

            try
            {
                string text = File.ReadAllText(path);
                List<FetchProducts> products = GP_ProductCatalogMapper.Parse(text, CurrentPlatform());
                ApplyProducts(products);
            }
            catch (Exception)
            {
                EditorUtility.DisplayDialog("GamePush Error", "Failed to import products file.", "OK");
            }
        }

        private async void LoadProductsFromApi()
        {
            if (_projectData == null || _projectData.id == 0)
            {
                EditorUtility.DisplayDialog("GamePush Error", "Save a Project ID on the Main tab first.", "OK");
                return;
            }

            if (string.IsNullOrWhiteSpace(_apiKey))
            {
                EditorUtility.DisplayDialog("GamePush Error", "Enter the account API key (Account Settings → API Keys).", "OK");
                return;
            }

            EditorPrefs.SetString(ApiSecretPref, _apiKey);
            _loadingProducts = true;
            Repaint();
            try
            {
                List<FetchProducts> products = await GP_ProductCatalogClient.FetchProducts(
                    _projectData.id,
                    _apiKey,
                    CurrentPlatform());
                ApplyProducts(products);
            }
            catch (Exception exception)
            {
                EditorUtility.DisplayDialog("GamePush Error", exception.Message, "OK");
            }
            finally
            {
                _loadingProducts = false;
                Repaint();
            }
        }

        private void ApplyProducts(List<FetchProducts> products)
        {
            if (_paymentsStub == null)
            {
                EditorUtility.DisplayDialog("GamePush Error", "GP_PaymentsStub asset was not found.", "OK");
                return;
            }

            if (products == null || products.Count == 0)
            {
                EditorUtility.DisplayDialog("GamePush", "No products found.", "OK");
                return;
            }

            Undo.RecordObject(_paymentsStub, "Import GamePush products");
            _paymentsStub.MergeProducts(products);
            EditorUtility.SetDirty(_paymentsStub);
            AssetDatabase.SaveAssets();
            DestroyEditor(ref _paymentsEditor);
            _paymentsEditor = Editor.CreateEditor(_paymentsStub);
            EditorUtility.DisplayDialog("GamePush", $"Imported {products.Count} products.", "OK");
        }

        private static Platform CurrentPlatform()
        {
            GP_PlatformSettings settings = ResolvePlatformSettings();
            return settings != null ? settings.PlatformToEmulate : Platform.NONE;
        }

        private static int DrawSecretIntField(string label, int value, ref bool reveal)
        {
            EditorGUILayout.BeginHorizontal();
            if (reveal)
            {
                value = EditorGUILayout.IntField(label, value);
            }
            else
            {
                string raw = EditorGUILayout.PasswordField(label, value == 0 ? string.Empty : value.ToString());
                if (string.IsNullOrEmpty(raw))
                    value = 0;
                else if (int.TryParse(raw, out int parsed))
                    value = parsed;
            }

            if (DrawRevealToggle(reveal))
                reveal = !reveal;
            EditorGUILayout.EndHorizontal();
            return value;
        }

        private static string DrawSecretTextField(string label, string value, ref bool reveal)
        {
            EditorGUILayout.BeginHorizontal();
            value = reveal
                ? EditorGUILayout.TextField(label, value)
                : EditorGUILayout.PasswordField(label, value ?? string.Empty);
            if (DrawRevealToggle(reveal))
                reveal = !reveal;
            EditorGUILayout.EndHorizontal();
            return value;
        }

        private static bool DrawRevealToggle(bool revealed)
        {
            return GUILayout.Button(
                RevealButtonContent(revealed),
                EditorStyles.miniButton,
                GUILayout.Width(24),
                GUILayout.Height(EditorGUIUtility.singleLineHeight));
        }

        private static GUIContent RevealButtonContent(bool revealed)
        {
            string iconName = revealed ? "d_scenevis_visible" : "d_scenevis_hidden";
            GUIContent icon = EditorGUIUtility.IconContent(iconName);
            if (icon != null && icon.image != null)
            {
                icon.tooltip = revealed ? "Hide value" : "Show value";
                return icon;
            }

            return new GUIContent(revealed ? "Hide" : "Show", revealed ? "Hide value" : "Show value");
        }

        private static string CsString(string value)
        {
            return (value ?? "").Replace("\\", "\\\\").Replace("\"", "\\\"");
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
        #endregion

        private static void SaveConfig()
        {
            //Console.Log("Saving data");
            if (_projectData.id == 0 || string.IsNullOrEmpty(_projectData.token))
            {
                EditorUtility.DisplayDialog("GamePush Error", "Please fill all the fields.", "OK");
                return;
            }

            if (!ValidateToken(_projectData.token)) return;

            if (string.IsNullOrEmpty(_projectData.androidPlatform))
                _projectData.androidPlatform = "ANDROID";
            if (_projectData.androidPlatformTag == null)
                _projectData.androidPlatformTag = "";

            SaveProjectData();

            SetProjectDataToWebTemplate();
            SaveProjectDataToScript(refresh: false);

            IniSceneHandle();

            GP_PlatformSnapshot.FetchAndApply(_projectData.id, _projectData.token,
                _projectData.androidPlatform, _projectData.androidPlatformTag);

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

        #region InitScene

        internal static void SyncWaitPluginReady(bool enabled)
        {
            if (_projectData != null)
                _projectData.waitPluginReady = enabled;
        }

        private static void IniSceneHandle()
        {
            if (_projectData.waitPluginReady)
                GP_InitSceneUtility.PlaceAtBuildIndexZero();
            else
                GP_InitSceneUtility.RemoveFromBuildSettings();
        }

        #endregion
        
    }
}
