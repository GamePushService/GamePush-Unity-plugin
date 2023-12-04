using UnityEditor;

namespace GamePush
{
    [FilePath("UserSettings/GP_Settings.asset",
        FilePathAttribute.Location.ProjectFolder)]
    public sealed class GP_Settings : ScriptableSingleton<GP_Settings>
    {
        public bool viewLogs = true;
        public GP_PlatformSettings platformSettings;
        public void Save() => Save(true);

        public PlatformSettings GetPlatformSettings(){
            if (platformSettings != null)
            {
                return platformSettings.GetPlatformSettings();
            }
            Console.Log("PLATFORM SETTINGS: ", "DEFAULT");
            return new PlatformSettings();
        }
        
        private void OnDisable() => Save();

        public Language GetLanguage()
        {
            if (platformSettings != null)
            {
                return platformSettings.Language;
            }
            Console.Log("PLATFORM LANGUAGE: ", "DEFAULT - ENGLISH");
            return Language.English;
        }
    }
}