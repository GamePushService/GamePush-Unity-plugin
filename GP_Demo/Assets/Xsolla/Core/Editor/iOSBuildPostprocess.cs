#if UNITY_IOS
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEditor.iOS.Xcode;

namespace Xsolla.Core.Editor
{
	public class iOSBuildPostprocess : IPostprocessBuildWithReport
	{
		public int callbackOrder => 2000;

		public void OnPostprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.iOS)
				return;

			XDebug.Log("Xsolla SDK is now processing iOS Project settings", true);
			SetupXcodeProject(report.summary.outputPath);
			SetupDeepLinking(report.summary.outputPath);
		}

		private static void SetupXcodeProject(string outputPath)
		{
			var projectPath = PBXProject.GetPBXProjectPath(outputPath);
			var project = new PBXProject();
			project.ReadFromFile(projectPath);

			var targetGuids = new List<string> {
				project.ProjectGuid(),
				project.GetUnityMainTargetGuid(),
				project.GetUnityFrameworkTargetGuid()
			};

			foreach (var targetGuid in targetGuids)
			{
				project.SetBuildProperty(targetGuid, "ENABLE_BITCODE", "NO");
			}

			project.WriteToFile(projectPath);
		}

		private static void SetupDeepLinking(string outputPath)
		{
			var plistPath = Path.Combine(outputPath, "Info.plist");
			var plistDoc = new PlistDocument();
			plistDoc.ReadFromString(File.ReadAllText(plistPath));

			var rootDic = plistDoc.root;
			var urlTypesArray = rootDic["CFBundleURLTypes"] as PlistElementArray
				?? rootDic.CreateArray("CFBundleURLTypes");

			var urlNamesDic = urlTypesArray.AddDict();
			urlNamesDic.SetString("CFBundleURLName", "");

			var urlSchemesArray = urlNamesDic.CreateArray("CFBundleURLSchemes");
			urlSchemesArray.AddString("app");

			File.WriteAllText(plistPath, plistDoc.WriteToString());
		}
	}
}
#endif