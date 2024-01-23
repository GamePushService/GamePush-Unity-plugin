using GamePush;
using UnityEditor;

namespace Plugins.GamePush.Editor
{
    [FilePath("Assets/Plugins/GamePush/Data/GP_SettingsWrap.asset",
        FilePathAttribute.Location.ProjectFolder)]
    public sealed class GP_SettingsWrap : ScriptableSingleton<GP_SettingsWrap>
    {
        public GP_Settings settings;
        
        private void OnDisable() => Save();
        public void Save() => Save(true);

        private void Awake()
        {
            GP_Settings.instance = settings;
        }

        private void OnValidate()
        {
            GP_Settings.instance = settings;
        }
    }
}