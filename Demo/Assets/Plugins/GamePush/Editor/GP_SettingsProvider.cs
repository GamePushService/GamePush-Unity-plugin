using GamePush;
using UnityEditor;

namespace Plugins.GamePush.Editor
{
    sealed class GP_SettingsProvider : SettingsProvider
    {
        public  GP_SettingsProvider()
            : base("Project/GamePush", SettingsScope.Project) {}

        public override void OnGUI(string search)
        {
            var settings = GP_Settings.instance;
            var viewLogs = settings.viewLogs;
            var platformSettings = settings.platformSettings;
            var paymentsStub = settings.paymentsStub;
            EditorGUI.BeginChangeCheck();
            viewLogs = EditorGUILayout.Toggle("View logs", viewLogs);
            platformSettings = (GP_PlatformSettings)EditorGUILayout.ObjectField("Platform settings", platformSettings, typeof(GP_PlatformSettings));
            paymentsStub = (GP_PaymentsStub)EditorGUILayout.ObjectField("Payments stub", paymentsStub, typeof(GP_PaymentsStub));
            if (EditorGUI.EndChangeCheck())
            {
                settings.viewLogs = viewLogs;
                settings.platformSettings = platformSettings;
                settings.paymentsStub = paymentsStub;
                settings.Save();
            }
        }

        [SettingsProvider]
        public static SettingsProvider CreateCustomSettingsProvider()
            => new GP_SettingsProvider();
    }
}
