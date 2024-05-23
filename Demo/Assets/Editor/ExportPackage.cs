// ExportPackage.cs
using UnityEngine;
using UnityEditor;

public class ExportPackage
{
    [MenuItem("Tools/Export/Export GamePush package")]
    static void export()
    {
        string fileName = "GP_plugin.unitypackage";
        string[] paths = {
            "Assets/WebGLTemplates",
            "Packages/com.gamepush.gp-unity-plugin",
            "Packages/com.lastabyss.simplegraphql",
        };

        AssetDatabase.ExportPackage(
            paths,
            fileName,
            ExportPackageOptions.Interactive |
            ExportPackageOptions.Recurse |
            ExportPackageOptions.IncludeDependencies);
    }
}

