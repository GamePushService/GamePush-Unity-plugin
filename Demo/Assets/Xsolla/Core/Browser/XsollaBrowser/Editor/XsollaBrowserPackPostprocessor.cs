using System;
using System.IO;
using System.Threading.Tasks;
using PuppeteerSharp;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.XsollaBrowser
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

			var browserPlatform = GetTargetPlatform(report.summary.platform);
			if (browserPlatform == Platform.Unknown)
			{
				XDebug.LogWarning($"Build target \"{report.summary.platform}\" is not supported. Packing browser in the build is skipped", true);
				return;
			}

			var buildBrowserDirectory = Path.GetDirectoryName(report.summary.outputPath);
			if (report.summary.platform == BuildTarget.StandaloneOSX)
			{
				buildBrowserDirectory = report.summary.outputPath + "/";
			}

			if (string.IsNullOrEmpty(buildBrowserDirectory))
				throw new Exception(nameof(buildBrowserDirectory));

			buildBrowserDirectory = Path.Combine(buildBrowserDirectory, "local-chromium");

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

				Task.Run(async () => await FetchBrowserAsync(browserPlatform, buildBrowserDirectory, Constants.BROWSER_REVISION)).Wait();
			}
		}

		private static Platform GetTargetPlatform(BuildTarget target)
		{
			switch (target)
			{
				case BuildTarget.StandaloneOSX:       return Platform.MacOS;
				case BuildTarget.StandaloneWindows:   return Platform.Win32;
				case BuildTarget.StandaloneWindows64: return Platform.Win64;
				case BuildTarget.StandaloneLinux64:   return Platform.Linux;
				default:                              return Platform.Unknown;
			}
		}

		private static Task FetchBrowserAsync(Platform platform, string path, string revision)
		{
			var options = new BrowserFetcherOptions {
				Product = Product.Chrome,
				Platform = platform,
				Path = path
			};

			return new BrowserFetcher(options).DownloadAsync(revision);
		}
	}
}