// ExportPackage.cs
using UnityEngine;
using UnityEditor;

namespace GamePushEditor
{
    public class GP_ExportPackage
    {
        [MenuItem("Tools/GamePush/Export package")]
        static void export()
        {
            string fileName = "GP_plugin.unitypackage";
            string[] paths = {
            "Assets/WebGLTemplates",
            "Assets/Examples",
            "Assets/Plugins/GamePush",
            "Packages/com.unity.nuget.newtonsoft-json",
            "Packages/com.lastabyss.simplegraphql"

        };

            AssetDatabase.ExportPackage(
                paths,
                fileName,
                ExportPackageOptions.Interactive |
                ExportPackageOptions.Recurse |
                ExportPackageOptions.IncludeDependencies);
        }
    }

}

