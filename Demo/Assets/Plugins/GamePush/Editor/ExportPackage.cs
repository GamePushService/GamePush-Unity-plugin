// ExportPackage.cs
using UnityEngine;
using UnityEditor;

namespace GamePushEditor
{
    public class GP_ExportPackage
    {
        //[MenuItem("Tools/Export package")]
        static void export()
        {
            string fileName = "GP_plugin.unitypackage";
            string[] paths = {
            "Assets/WebGLTemplates",
            "Assets/GamePush"
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

