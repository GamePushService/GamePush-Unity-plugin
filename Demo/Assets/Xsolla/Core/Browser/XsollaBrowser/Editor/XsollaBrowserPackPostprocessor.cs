using System;
using System.IO;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace Xsolla.Core.Browser
{
	public class XsollaBrowserPackPostprocessor : IPostprocessBuildWithReport
	{
		public int callbackOrder { get; }

		public void OnPostprocessBuild(BuildReport report)
		{
			if (report.summary.platformGroup != BuildTargetGroup.Standalone ||
			    !XsollaSettings.InAppBrowserEnabled ||
			    !XsollaSettings.PackInAppBrowserInBuild)
				return;

			var browserPlatform = string.Empty;
			switch (report.summary.platform)
			{
				case BuildTarget.StandaloneOSX:
					browserPlatform = "MacOS";
					break;
				case BuildTarget.StandaloneWindows:
					browserPlatform = "Win32";
					break;
				case BuildTarget.StandaloneWindows64:
					browserPlatform = "Win64";
					break;
				case BuildTarget.StandaloneLinux64:
					browserPlatform = "Linux";
					break;
			}

			if (string.IsNullOrEmpty(browserPlatform))
			{
				XDebug.LogWarning($"Build target \"{report.summary.platform}\" is not supported. Packing browser in the build is skipped", true);
				return;
			}

			var buildBrowserDirectory = Path.GetDirectoryName(report.summary.outputPath);
			if (string.IsNullOrEmpty(buildBrowserDirectory))
				throw new Exception(nameof(buildBrowserDirectory));

			buildBrowserDirectory = Path.Combine(buildBrowserDirectory, ".local-chromium");

			try
			{
				if (Directory.Exists(buildBrowserDirectory))
					Directory.Delete(buildBrowserDirectory, true);
			}
			catch (Exception e)
			{
				XDebug.LogWarning($"Can't delete existing browser directory. Packing browser in the build is skipped. Exception: {e}", true);
				return;
			}

			const string browserRevision = Constants.BROWSER_REVISION;
			var sourceBrowserDirectory = Path.Combine(Application.persistentDataPath, $"{browserPlatform}-{browserRevision}");
			if (Directory.Exists(sourceBrowserDirectory))
			{
				buildBrowserDirectory = Path.Combine(buildBrowserDirectory, $"{browserPlatform}-{browserRevision}");

				foreach (var dirPath in Directory.GetDirectories(sourceBrowserDirectory, "*", SearchOption.AllDirectories))
					Directory.CreateDirectory(dirPath.Replace(sourceBrowserDirectory, buildBrowserDirectory));

				foreach (var filePath in Directory.GetFiles(sourceBrowserDirectory, "*.*", SearchOption.AllDirectories))
					File.Copy(filePath, filePath.Replace(sourceBrowserDirectory, buildBrowserDirectory), true);
			}
			else
			{
				if (Application.internetReachability == NetworkReachability.NotReachable)
				{
					XDebug.LogWarning("Internet connection is unavailable. Packing browser in the build is skipped", true);
					return;
				}

				var fetcher = new XsollaBrowserFetcher {
					Platform = browserPlatform,
					Path = buildBrowserDirectory,
					Revision = Constants.BROWSER_REVISION
				};

				Task.Run(async () => await fetcher.DownloadAsync()).Wait();
			}
		}
	}
}