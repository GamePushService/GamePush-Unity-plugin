using UnityEditor;

namespace GamePushEditor
{
    public class GP_ExportPackage
    {
        [MenuItem("Tools/GamePush/Export plugin")]
        static void export()
        {
            string[] paths = {
                "Assets/WebGLTemplates",
                "Assets/Plugins/GamePush",
                "Assets/Plugins/Android",
                "Assets/Plugins/iOS",
                "Assets/TextMesh Pro",
                "Packages/com.unity.nuget.newtonsoft-json",
                "Packages/com.lastabyss.simplegraphql"
            };

            string fileName = "GP_plugin.unitypackage";

            AssetDatabase.ExportPackage(
                paths,
                fileName,
                ExportPackageOptions.Interactive |
                ExportPackageOptions.Recurse |
                ExportPackageOptions.IncludeDependencies
            );
        }
    }

}
