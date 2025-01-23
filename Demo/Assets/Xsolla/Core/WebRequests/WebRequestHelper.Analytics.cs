using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		private const string ANALYTICS_QUERY_TEMPLATE = "engine=unity&engine_v={0}&sdk={1}&sdk_v={2}&build_platform={3}";

		public string AppendAnalyticsToUrl(SdkType sdkType, string url)
		{
			var analyticsQuery = string.Format(ANALYTICS_QUERY_TEMPLATE,
				GetEngineVersion(),
				GetSdkType(sdkType),
				GetSdkVersion(),
				GetBuildPlatform());

			var dividingSymbol = url.Contains("?") ? "&" : "?";
			return $"{url}{dividingSymbol}{analyticsQuery}";
		}

		public List<WebRequestHeader> AppendAnalyticHeaders(SdkType sdkType, List<WebRequestHeader> originHeaders)
		{
			var analyticHeaders = GetAnalyticHeaders(sdkType);

			if (originHeaders != null)
				analyticHeaders.AddRange(originHeaders);

			return analyticHeaders;
		}

		public List<WebRequestHeader> AppendAnalyticHeaders(SdkType sdkType, WebRequestHeader originHeader)
		{
			var analyticHeaders = GetAnalyticHeaders(sdkType);

			if (originHeader != null)
				analyticHeaders.Add(originHeader);

			return analyticHeaders;
		}

		public List<WebRequestHeader> GetAnalyticHeaders(SdkType sdkType)
		{
			// These headers cause network error in WebGL environment
			//'Access to XMLHttpRequest ... has been blocked by CORS policy: Request header field x-engine is not allowed by Access-Control-Allow-Headers in preflight response.'
			if (Application.platform == RuntimePlatform.WebGLPlayer)
				return new List<WebRequestHeader>();

			return new List<WebRequestHeader> {
				new WebRequestHeader { Name = "X-ENGINE", Value = "UNITY" },
				new WebRequestHeader { Name = "X-ENGINE-V", Value = GetEngineVersion(true) },
				new WebRequestHeader { Name = "X-SDK", Value = GetSdkType(sdkType, true) },
				new WebRequestHeader { Name = "X-SDK-V", Value = GetSdkVersion(true) },
				new WebRequestHeader { Name = "X-BUILD-PLATFORM", Value = GetBuildPlatform(true) }
			};
		}

		private static string GetEngineVersion(bool toUpper = false)
		{
			var engineVersion = Application.unityVersion;
			return toUpper
				? engineVersion.ToUpperInvariant()
				: engineVersion.ToLowerInvariant();
		}

		private static string GetSdkType(SdkType sdkType, bool toUpper = false)
		{
			var sdkTypeValue = string.Empty;

			switch (sdkType)
			{
				case SdkType.Login:
					sdkTypeValue = "login";
					break;
				case SdkType.Store:
					sdkTypeValue = "store";
					break;
				case SdkType.Subscriptions:
					sdkTypeValue = "subscriptions";
					break;
				case SdkType.SettingsFillTool:
					sdkTypeValue = "settings-fill-tool";
					break;
				default:
					XDebug.LogError($"Unexpected analyticsType: '{sdkType}'");
					break;
			}

			return toUpper
				? sdkTypeValue.ToUpperInvariant()
				: sdkTypeValue.ToLowerInvariant();
		}

		private static string GetSdkVersion(bool toUpper = false)
		{
			var sdkVersion = Constants.SDK_VERSION;

			if (XsollaMarker.HasDemoPart)
				sdkVersion = $"{sdkVersion}_demo";

			return toUpper
				? sdkVersion.ToUpperInvariant()
				: sdkVersion.ToLowerInvariant();
		}

		private static string GetBuildPlatform(bool toUpper = false)
		{
#if UNITY_EDITOR
			var buildPlatform = UnityEditor.EditorUserBuildSettings.activeBuildTarget.ToString();
#else
			var buildPlatform = Application.platform.ToString();
#endif

			return toUpper
				? buildPlatform.ToUpperInvariant()
				: buildPlatform.ToLowerInvariant();
		}
	}
}