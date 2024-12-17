using System;
using UnityEngine;

namespace Xsolla.Core
{
	public class XsollaSettings : ScriptableObject
	{
		[SerializeField] private string loginId = string.Empty;
		[SerializeField] private string callbackUrl;
		[SerializeField] private bool invalidateExistingSessions;
		[SerializeField] private int oauthClientId;

		[SerializeField] private string storeProjectId = string.Empty;
		[SerializeField] private bool isSandbox = true;
		[SerializeField] private bool inAppBrowserEnabled = true;
		[SerializeField] private bool packInAppBrowserInBuild = true;

		[SerializeField] private RedirectPolicySettings desktopRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings webglRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings androidRedirectPolicySettings = new RedirectPolicySettings();
		[SerializeField] private RedirectPolicySettings iosRedirectPolicySettings = new RedirectPolicySettings();

		[SerializeField] private PayStationUISettings desktopPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings webglPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings androidPayStationUISettings = new PayStationUISettings();
		[SerializeField] private PayStationUISettings iosPayStationUISettings = new PayStationUISettings();

		[SerializeField] private string facebookAppId;
		[SerializeField] private string facebookClientToken;
		[SerializeField] private string googleServerId;
		[SerializeField] private string wechatAppId;
		[SerializeField] private string qqAppId;

		[SerializeField] private LogLevel logLevel = LogLevel.InfoWarningsErrors;

		public static bool PayStationGroupFoldout { get; set; }

		public static bool RedirectPolicyGroupFoldout { get; set; }

		public static int PaystationVersion { get; set; } = 4;

		public static string LoginId
		{
			get => Instance.loginId;
			set => Instance.loginId = value;
		}

		public static bool InvalidateExistingSessions
		{
			get => Instance.invalidateExistingSessions;
			set => Instance.invalidateExistingSessions = value;
		}

		public static int OAuthClientId
		{
			get => Instance.oauthClientId;
			set => Instance.oauthClientId = value;
		}

		public static string CallbackUrl
		{
			get => Instance.callbackUrl;
			set => Instance.callbackUrl = value;
		}

		public static string StoreProjectId
		{
			get => Instance.storeProjectId;
			set => Instance.storeProjectId = value;
		}

		public static bool IsSandbox
		{
			get => Instance.isSandbox;
			set => Instance.isSandbox = value;
		}

		public static bool InAppBrowserEnabled
		{
			get => Instance.inAppBrowserEnabled;
			set => Instance.inAppBrowserEnabled = value;
		}

		public static bool PackInAppBrowserInBuild
		{
			get => Instance.packInAppBrowserInBuild;
			set => Instance.packInAppBrowserInBuild = value;
		}

		public static RedirectPolicySettings DesktopRedirectPolicySettings
		{
			get => Instance.desktopRedirectPolicySettings;
			set => Instance.desktopRedirectPolicySettings = value;
		}

		public static RedirectPolicySettings WebglRedirectPolicySettings
		{
			get => Instance.webglRedirectPolicySettings;
			set => Instance.webglRedirectPolicySettings = value;
		}

		public static RedirectPolicySettings AndroidRedirectPolicySettings
		{
			get => Instance.androidRedirectPolicySettings;
			set => Instance.androidRedirectPolicySettings = value;
		}

		public static RedirectPolicySettings IosRedirectPolicySettings
		{
			get => Instance.iosRedirectPolicySettings;
			set => Instance.iosRedirectPolicySettings = value;
		}

		public static PayStationUISettings DesktopPayStationUISettings
		{
			get => Instance.desktopPayStationUISettings;
			set => Instance.desktopPayStationUISettings = value;
		}

		public static PayStationUISettings WebglPayStationUISettings
		{
			get => Instance.webglPayStationUISettings;
			set => Instance.webglPayStationUISettings = value;
		}

		public static PayStationUISettings AndroidPayStationUISettings
		{
			get => Instance.androidPayStationUISettings;
			set => Instance.androidPayStationUISettings = value;
		}

		public static PayStationUISettings IosPayStationUISettings
		{
			get => Instance.iosPayStationUISettings;
			set => Instance.iosPayStationUISettings = value;
		}

		public static string FacebookAppId
		{
			get => Instance.facebookAppId;
			set => Instance.facebookAppId = value;
		}

		public static string FacebookClientToken
		{
			get => Instance.facebookClientToken;
			set => Instance.facebookClientToken = value;
		}

		public static string GoogleServerId
		{
			get => Instance.googleServerId;
			set => Instance.googleServerId = value;
		}

		public static string WeChatAppId
		{
			get => Instance.wechatAppId;
			set => Instance.wechatAppId = value;
		}

		public static string QqAppId
		{
			get => Instance.qqAppId;
			set => Instance.qqAppId = value;
		}

		public static LogLevel LogLevel
		{
			get => Instance.logLevel;
			set => Instance.logLevel = value;
		}

		private static XsollaSettings _instance;

		public static XsollaSettings Instance
		{
			get
			{
				if (!_instance)
				{
					var instances = Resources.LoadAll<XsollaSettings>(string.Empty);

					if (instances.Length == 0)
						throw new Exception("`XsollaSettings` asset not found. Please create it.");

					if (instances.Length > 1)
						throw new Exception("Multiple `XsollaSettings` assets found. Please keep only one asset."); 

					_instance = instances[0];
				}

				return _instance;
			}
		}
	}
}
