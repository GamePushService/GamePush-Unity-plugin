#if UNITY_ANDROID
using System.IO;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Editor
{
	public class AndroidManifestPreprocess : IPreprocessBuildWithReport
	{
		public int callbackOrder => 1000;

		public void OnPreprocessBuild(BuildReport report)
		{
			if (report.summary.platform != BuildTarget.Android)
				return;

			XDebug.Log("Xsolla SDK is now processing android manifest", true);
			SetupWeChat();
			SetupProxyActivity("PaymentsProxyActivity");
			SetupProxyActivity("SocialAuthProxyActivity");
			SetupProxyActivity("XsollaWidgetAuthProxyActivity");
		}

		private static void SetupWeChat()
		{
			var androidPackageName = Application.identifier;
			var wechatActivityName = $"{androidPackageName}.wxapi.WXEntryActivity";
			var wechatActivity = new ActivityNode(wechatActivityName);
			wechatActivity.AddAttribute(AndroidManifestConstants.ExportedAttribute, "true");

			var manifest = LoadManifestWrapper();
			var manifestChanged = false;

			// cleanup manifest in case WeChat activity was added previously
			if (manifest.ContainsNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(wechatActivity)))
			{
				manifest.RemoveNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(wechatActivity));
				manifestChanged = true;
			}

			if (!string.IsNullOrEmpty(XsollaSettings.WeChatAppId))
			{
				manifest.AddNode(wechatActivity, new FindByTag(AndroidManifestConstants.ApplicationTag));
				manifestChanged = true;
			}

			if (manifestChanged)
				manifest.SaveManifest();
		}

		private static void SetupProxyActivity(string activityName)
		{
			var fullActivityName = $"{Application.identifier}.androidProxies.{activityName}";
			var activityNode = new ActivityNode(fullActivityName);
			activityNode.AddAttribute(AndroidManifestConstants.ExportedAttribute, "true");
			activityNode.AddAttribute(AndroidManifestConstants.ConfigChanges, "orientation|screenSize|keyboardHidden");

			var manifest = LoadManifestWrapper();

			// cleanup manifest in case activity node was added previously
			if (manifest.ContainsNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(activityNode)))
				manifest.RemoveNode(new FindByTag(AndroidManifestConstants.ApplicationTag), new FindByName(activityNode));

			manifest.AddNode(activityNode, new FindByTag(AndroidManifestConstants.ApplicationTag));
			manifest.SaveManifest();
		}

		private static AndroidManifestWrapper LoadManifestWrapper()
		{
			const string fileName = "AndroidManifest.xml";
			var manifestPath = GetTargetFilePath(fileName);
			if (!File.Exists(manifestPath))
			{
				var manifestDirectory = Path.GetDirectoryName(manifestPath);
				if (manifestDirectory != null && !Directory.Exists(manifestDirectory))
					Directory.CreateDirectory(manifestDirectory);

				var templatePath = GetTemplateFilePath();
				File.Copy(templatePath, manifestPath);
			}

			return new AndroidManifestWrapper(manifestPath);
		}

		private static string GetTargetFilePath(string fileName)
		{
			return Path.Combine(Application.dataPath, "Plugins", "Android", fileName);
		}

		private static string GetTemplateFilePath()
		{
			var guids = AssetDatabase.FindAssets($"t:Script {nameof(AndroidScriptsPreprocess)}");
			if (guids.Length == 0)
				throw new FileNotFoundException($"Can't find {nameof(AndroidScriptsPreprocess)} script");

			var path = AssetDatabase.GUIDToAssetPath(guids[0]);
			path = path.Replace("Assets", Application.dataPath);

			path = Path.GetDirectoryName(path);
			if (path == null)
				throw new DirectoryNotFoundException("Can't find directory with android file templates");
			
			var fileName = "AndroidManifest.xml";
#if UNITY_6000
			fileName = "AndroidManifest_6000.xml";
#endif
			return Path.Combine(path, "AndroidTemplateFiles", $"{fileName}");
		}
	}
}
#endif