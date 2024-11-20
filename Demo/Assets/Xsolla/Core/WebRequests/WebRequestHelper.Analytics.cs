using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		private const string ANALYTICS_URL_TEMPLATE = "engine=unity&engine_v={0}&sdk={1}&sdk_v={2}&build_platform={3}{4}";

		//Uncomment and fill the values if you want to hardcode referral info
		private KeyValuePair<string, string>? _referralAnalytics /* = new KeyValuePair<string, string>("MyReferralPlugin", "0.0.1")*/;

		public void SetReferralAnalytics(string referralPlugin, string referralVersion)
		{
			_referralAnalytics = new KeyValuePair<string, string>(referralPlugin, referralVersion);
		}

		public string AppendAnalyticsToUrl(SdkType analyticsType, string url)
		{
			var dividingSymbol = url.Contains("?") ? "&" : "?";
			GetUnityParameters(toUpper: false, out string engineVersion, out string buildPlatform);
			GetXsollaSdkParameters(analyticsType, toUpper: false, out string sdkType, out string sdkVersion);

			var referralAnalytics = default(string);

			if (_referralAnalytics.HasValue) //if (_referralAnalytics != null)
			{
				string referralPlugin = _referralAnalytics.Value.Key;
				string referralVersion = _referralAnalytics.Value.Value;
				referralAnalytics = $"&ref={referralPlugin}&ref_v={referralVersion}";
			}

			var analyticsAddition = string.Format(ANALYTICS_URL_TEMPLATE, engineVersion, sdkType, sdkVersion, buildPlatform, referralAnalytics);

			var result = $"{url}{dividingSymbol}{analyticsAddition}";
			return result;
		}

		public List<WebRequestHeader> AppendAnalyticHeaders(SdkType analyticsType, List<WebRequestHeader> headers)
		{
			var analyticHeaders = GetAnalyticHeaders(analyticsType);
			var result = default(List<WebRequestHeader>);

			if (headers != null)
			{
				result = headers;
				result.AddRange(analyticHeaders);
			}
			else
			{
				result = analyticHeaders;
			}

			return result;
		}

		public List<WebRequestHeader> AppendAnalyticHeaders(SdkType analyticsType, WebRequestHeader header)
		{
			var analyticHeaders = GetAnalyticHeaders(analyticsType);
			var result = default(List<WebRequestHeader>);

			if (header != null)
			{
				result = new List<WebRequestHeader>() {header};
				result.AddRange(analyticHeaders);
			}
			else
			{
				result = analyticHeaders;
			}

			return result;
		}

		private List<WebRequestHeader> GetAnalyticHeaders(SdkType analyticsType)
		{
			//These headers cause network error in WebGL environment
			//'Access to XMLHttpRequest ... has been blocked by CORS policy: Request header field x-engine is not allowed by Access-Control-Allow-Headers in preflight response.'
			if (Application.platform == RuntimePlatform.WebGLPlayer)
				return new List<WebRequestHeader>();

			GetUnityParameters(toUpper: true, out string engineVersion, out string buildPlatform);
			GetXsollaSdkParameters(analyticsType, toUpper: true, out string sdkType, out string sdkVersion);

			var resultCapacity = _referralAnalytics.HasValue ? 6 : 4;
			var result = new List<WebRequestHeader>(capacity: resultCapacity) {
				new WebRequestHeader() {Name = "X-ENGINE", Value = "UNITY"},
				new WebRequestHeader() {Name = "X-ENGINE-V", Value = engineVersion},
				new WebRequestHeader() {Name = "X-SDK", Value = sdkType},
				new WebRequestHeader() {Name = "X-SDK-V", Value = sdkVersion},
				new WebRequestHeader() {Name = "X-BUILD-PLATFORM", Value = buildPlatform}
			};

			if (_referralAnalytics.HasValue) //if (_referralAnalytics != null)
			{
				string referralPlugin = _referralAnalytics.Value.Key;
				string referralVersion = _referralAnalytics.Value.Value;

				result.Add(new WebRequestHeader() {Name = "X-REF", Value = referralPlugin.ToUpper()});
				result.Add(new WebRequestHeader() {Name = "X-REF-V", Value = referralVersion.ToUpper()});
			}

			return result;
		}

		private void GetXsollaSdkParameters(SdkType analyticsType, bool toUpper, out string sdkType, out string sdkVersion)
		{
			switch (analyticsType)
			{
				case SdkType.Login:
					sdkType = toUpper ? "LOGIN" : "login";
					break;
				case SdkType.Store:
					sdkType = toUpper ? "STORE" : "store";
					break;
				case SdkType.Subscriptions:
					sdkType = toUpper ? "SUBSCRIPTIONS" : "subscriptions";
					break;
				case SdkType.SettingsFillTool:
					sdkType = toUpper ? "SETTINGS-FILL-TOOL" : "settings-fill-tool";
					break;
				default:
					XDebug.LogError($"Unexpected analyticsType: '{analyticsType}'");
					sdkType = string.Empty;
					break;
			}

			if (XsollaMarker.HasDemoPart)
				sdkVersion = $"{Constants.SDK_VERSION}{(toUpper ? "_DEMO" : "_demo")}";
			else
				sdkVersion = Constants.SDK_VERSION;
		}

		private void GetUnityParameters(bool toUpper, out string engineVersion, out string buildPlatform)
		{
			engineVersion = Application.unityVersion;
#if UNITY_EDITOR
			buildPlatform = EditorUserBuildSettings.activeBuildTarget.ToString();
#else
			buildPlatform = Application.platform.ToString();
#endif

			if (toUpper)
			{
				engineVersion = engineVersion.ToUpper();
				buildPlatform = buildPlatform.ToUpper();
			}
			else
				buildPlatform = buildPlatform.ToLower();
		}
	}
}