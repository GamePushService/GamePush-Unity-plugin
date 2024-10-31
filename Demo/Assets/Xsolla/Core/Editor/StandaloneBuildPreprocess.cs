using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Xsolla.Core.Editor
{
	public class StandaloneBuildPreprocess : IPreprocessBuildWithReport
	{
		public int callbackOrder => 3000;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.StandaloneOSX)
				return;

			var steamLibPath = "Assets/Xsolla/ThirdParty/Steamworks.NET-master/Plugins/steam_api.bundle";
			var steamLibAsset = AssetImporter.GetAtPath(steamLibPath) as PluginImporter;
			if (steamLibAsset != null)
			{
				steamLibAsset.SetPlatformData(BuildTarget.StandaloneOSX, "OS", "OSX");
				steamLibAsset.SetPlatformData(BuildTarget.StandaloneOSX, "CPU", "AnyCPU");
				steamLibAsset.SaveAndReimport();
			}
		}
	}
}