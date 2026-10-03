using System.IO;
using System.Text.RegularExpressions;
using GamePush;
using UnityEditor;
using UnityEngine;

namespace Plugins.GamePush.Editor
{
    sealed class GP_SettingsProvider : SettingsProvider
    {
        const string ProjectPath = "Assets/Plugins/GamePush/Data/ProjectData.cs";
        const string JsprePath = "Assets/Plugins/GamePush/JS/_dataFields.jspre";

        public  GP_SettingsProvider()
            : base("Project/GamePush", SettingsScope.Project) {}

        public override void OnGUI(string search)
        {
            var wrap = GP_SettingsWrap.instance;
            if (wrap.settings == null)
                wrap.settings = new GP_Settings();
            var settings = wrap.settings;
            var viewLogs = settings.viewLogs;
            var fullLogs = settings.fullLogs;
            var platformSettings = settings.platformSettings;
            var paymentsStub = settings.paymentsStub;
            EditorGUI.BeginChangeCheck();
            viewLogs = EditorGUILayout.Toggle("View logs", viewLogs);
            fullLogs = EditorGUILayout.Toggle("Full Logs", fullLogs);
            EditorGUILayout.HelpBox(
                "Verbose SDK and multiplayer traces in the Unity / browser console. Player builds pick this up after a rebuild.",
                MessageType.None);
            platformSettings = (GP_PlatformSettings)EditorGUILayout.ObjectField("Platform settings", platformSettings, typeof(GP_PlatformSettings), false);
            paymentsStub = (GP_PaymentsStub)EditorGUILayout.ObjectField("Payments stub", paymentsStub, typeof(GP_PaymentsStub), false);
            if (EditorGUI.EndChangeCheck())
            {
                var fullLogsChanged = settings.fullLogs != fullLogs;
                settings.viewLogs = viewLogs;
                settings.fullLogs = fullLogs;
                settings.platformSettings = platformSettings;
                settings.paymentsStub = paymentsStub;
                wrap.settings = settings;
                GP_Settings.instance = settings;
                wrap.Save();
                if (fullLogsChanged)
                    BakeFullLogs(fullLogs);
            }

            EditorGUILayout.Space(12);
            EditorGUILayout.LabelField("Editor", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox(
                "Play2Web starts only from Play Test in its window, not from the regular Play button.",
                MessageType.None);
            if (GUILayout.Button("Open Play2Web"))
                GamePushEditor.Play2Web.GP_Play2WebWindow.Open();
            if (GUILayout.Button("Open GamePush settings"))
                EditorApplication.ExecuteMenuItem("Tools/GamePush");
        }

        static void BakeFullLogs(bool enabled)
        {
            var flag = enabled ? "true" : "false";
            if (File.Exists(ProjectPath))
            {
                var text = File.ReadAllText(ProjectPath);
                if (Regex.IsMatch(text, @"public static bool FULL_LOGS = (true|false);"))
                    text = Regex.Replace(text, @"public static bool FULL_LOGS = (true|false);",
                        "public static bool FULL_LOGS = " + flag + ";");
                else
                    text = text.Replace("        public static bool SDK_LIVE",
                        "        public static bool FULL_LOGS = " + flag + ";\n        public static bool SDK_LIVE");
                File.WriteAllText(ProjectPath, text);
            }
            if (File.Exists(JsprePath))
            {
                var js = File.ReadAllText(JsprePath);
                if (Regex.IsMatch(js, @"const gpFullLogs = (true|false);"))
                    js = Regex.Replace(js, @"const gpFullLogs = (true|false);",
                        "const gpFullLogs = " + flag + ";");
                else
                    js = js.TrimEnd() + "\nconst gpFullLogs = " + flag + ";\n";
                File.WriteAllText(JsprePath, js);
            }
            AssetDatabase.Refresh();
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
            => new GP_SettingsProvider();
    }
}
