using System.Runtime.InteropServices;
using PuppeteerSharp;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.XsollaBrowser
{
	public static class BrowserSettings
	{
		public static Vector2Int LaunchViewportSize { get; set; } = new Vector2Int(1280, 1024);

		public static bool Headless { get; set; } = true;
		public static bool DevTools { get; set; } = false;

		public static string[] BrowserLaunchArgs { get; set; } = {
			"--mute-audio",
			"--no-sandbox",
			"--disable-setuid-sandbox",
			"--disable-dev-shm-usage",
			"--disable-accelerated-2d-canvas",
			"--disable-background-timer-throttling"
		};

		public static FetchOptions GetFetchOptions(Platform? platform = null)
		{
			if (!platform.HasValue)
				platform = GetCurrentPlatform();

			return new FetchOptions {
				Product = Product.Chrome,
				Revision = Constants.BROWSER_REVISION,
				Platform = platform.Value,
				Path = GetFetchPath()
			};
		}

		private static string GetFetchPath()
		{
#if UNITY_EDITOR
			return Application.persistentDataPath;
#else
			if (!XsollaSettings.PackInAppBrowserInBuild)
				return Application.persistentDataPath;

			var appDir = System.IO.Directory.GetParent(Application.dataPath);
			if (appDir == null)
				throw new System.Exception("Failed to get parent directory of the data path");

			return System.IO.Path.Combine(appDir.FullName, "local-chromium");
#endif
		}

		public static LaunchOptions GetLaunchOptions()
		{
			return new LaunchOptions {
				Headless = Headless,
				DevTools = DevTools,
				Args = BrowserLaunchArgs,
				ViewportWidth = LaunchViewportSize.x,
				ViewportHeight = LaunchViewportSize.y
			};
		}

		private static Platform GetCurrentPlatform()
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
				return Platform.MacOS;

			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				return RuntimeInformation.OSArchitecture == Architecture.X64
					? Platform.Win64
					: Platform.Win32;
			}

			return RuntimeInformation.IsOSPlatform(OSPlatform.Linux)
				? Platform.Linux
				: Platform.Unknown;
		}
	}
}