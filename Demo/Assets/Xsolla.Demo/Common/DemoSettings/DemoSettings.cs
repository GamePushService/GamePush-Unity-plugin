using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class DemoSettings : ScriptableObject
	{
		[SerializeField] private bool _requestNicknameOnAuth = true;
		[SerializeField] private string webStoreUrl = Constants.DEFAULT_WEB_STORE_URL;

		[SerializeField] private bool useSteamAuth;
		[SerializeField] private string steamAppId = "480";
		[SerializeField] private PaymentsFlow paymentsFlow = PaymentsFlow.XsollaPayStation;

		[SerializeField] private PlatformType platform = PlatformType.Xsolla;
		[SerializeField] private bool useConsoleAuth;
		[SerializeField] private string usernameFromConsole;

		private static DemoSettings _instance;

		public static DemoSettings Instance
		{
			get
			{
				if (!_instance)
					_instance = Resources.Load<DemoSettings>("XsollaDemoSettings");

				return _instance;
			}
		}

		public static bool RequestNicknameOnAuth
		{
			get => Instance._requestNicknameOnAuth;
			set
			{
				Instance._requestNicknameOnAuth = value;
				MarkAssetDirty();
			}
		}

		private const string WebStoreUrlKey = nameof(webStoreUrl);

		public static string WebStoreUrl
		{
			get
			{
				if (PlayerPrefs.HasKey(WebStoreUrlKey))
					return PlayerPrefs.GetString(WebStoreUrlKey);
				else
					return Instance.webStoreUrl;
			}
			set
			{
				PlayerPrefs.SetString(WebStoreUrlKey, value);

				if (!Application.isPlaying)
				{
					Instance.webStoreUrl = value;
					MarkAssetDirty();
				}
			}
		}

		public static bool UseSteamAuth
		{
			get => Instance.useSteamAuth;
			set
			{
				Instance.useSteamAuth = value;
				MarkAssetDirty();
			}
		}

		public static string SteamAppId
		{
			get => Instance.steamAppId;
			set
			{
				Instance.steamAppId = value;
				MarkAssetDirty();
			}
		}

		public static PaymentsFlow PaymentsFlow
		{
			get => Instance.paymentsFlow;
			set
			{
				Instance.paymentsFlow = value;
				MarkAssetDirty();
			}
		}

		public static PlatformType Platform
		{
			get => Instance.platform;
			set
			{
				Instance.platform = value;
				MarkAssetDirty();
			}
		}

		public static bool UseConsoleAuth
		{
			get => Instance.useConsoleAuth;
			set
			{
				Instance.useConsoleAuth = value;
				MarkAssetDirty();
			}
		}

		public static string UsernameFromConsolePlatform
		{
			get => Instance.usernameFromConsole;
			set
			{
				Instance.usernameFromConsole = value;
				MarkAssetDirty();
			}
		}

		private static void MarkAssetDirty()
		{
#if UNITY_EDITOR
			EditorUtility.SetDirty(Instance);
#endif
		}
	}
}