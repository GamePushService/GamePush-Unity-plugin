using System;
using UnityEngine;
using Object = UnityEngine.Object;
#if UNITY_WEBGL || UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
using System.Runtime.InteropServices;
#endif

namespace Xsolla.Core
{
	public static class XsollaWebBrowser
	{
		private static IInAppBrowser _inAppBrowser;
		private static GameObject _inAppBrowserGameObject;

		public static IInAppBrowser InAppBrowser
		{
			get
			{
#if !(UNITY_EDITOR || UNITY_STANDALONE)
				return null;
#else
				if (!XsollaSettings.InAppBrowserEnabled)
					return null;

				if (_inAppBrowser == null)
				{
					var prefab = Resources.Load<GameObject>(Constants.WEB_BROWSER_RESOURCE_PATH);
					if (!prefab)
					{
						XDebug.LogError("Prefab InAppBrowser not found in Resources folder.");
					}
					else
					{
						_inAppBrowserGameObject = Object.Instantiate(prefab);
						_inAppBrowserGameObject.name = "XsollaWebBrowser";
						Object.DontDestroyOnLoad(_inAppBrowserGameObject);
						_inAppBrowser = _inAppBrowserGameObject.GetComponent<IInAppBrowser>();
						_inAppBrowser.CloseEvent += info => Close();
					}
				}

				return _inAppBrowser;
#endif
			}
		}

		public static void OpenPurchaseUI(string paymentToken, bool forcePlatformBrowser = false, Action<BrowserCloseInfo> onBrowserClosed = null, PlatformSpecificAppearance platformSpecificAppearance = null)
		{
#if UNITY_ANDROID
			if (!Application.isEditor && XsollaSettings.InAppBrowserEnabled)
			{
				new AndroidPayments().Perform(
					paymentToken,
					isClosedManually => {
						var info = new BrowserCloseInfo {
							isManually = isClosedManually
						};
						onBrowserClosed?.Invoke(info);
					},
					platformSpecificAppearance?.AndroidActivityType);

				return;
			}

#elif UNITY_IOS
			if (!Application.isEditor && XsollaSettings.InAppBrowserEnabled)
			{
				new IosPayments().Perform(
					paymentToken,
					isClosedManually => {
						var info = new BrowserCloseInfo {
							isManually = isClosedManually
						};
						onBrowserClosed?.Invoke(info);
					});

				return;
			}

#elif UNITY_WEBGL && !UNITY_EDITOR
			if (XsollaSettings.InAppBrowserEnabled)
			{
				WebGLBrowserClosedCallback = isManually =>
				{
					var info = new BrowserCloseInfo {
						isManually = isManually
					};
					onBrowserClosed?.Invoke(info);
				};

				Screen.fullScreen = false;
				OpenPaystationWidget(paymentToken, XsollaSettings.IsSandbox);
				return;
			}
#endif
			var url = new PaystationUrlBuilder(paymentToken).Build();
			XDebug.Log($"Purchase url: {url}");
			Open(url, forcePlatformBrowser);

#if UNITY_STANDALONE || UNITY_EDITOR
			if (InAppBrowser != null && !forcePlatformBrowser)
			{
				InAppBrowser.AddInitHandler(() => InAppBrowser.UpdateSize(740, 760));
				InAppBrowser.CloseEvent += onBrowserClosed;
			}
#endif
		}

		public static void Open(string url, bool forcePlatformBrowser = false)
		{
			XDebug.Log($"WebBrowser. Open url: {url}");
#if UNITY_EDITOR || UNITY_STANDALONE
			if (InAppBrowser != null && !forcePlatformBrowser)
				InAppBrowser.Open(url);
			else
				Application.OpenURL(url);
#elif UNITY_WEBGL
#pragma warning disable 0618
			Application.ExternalEval($"window.open(\"{url}\",\"_blank\")");
#pragma warning restore 0618
			return;
#else
			Application.OpenURL(url);
#endif
		}

		public static void Close(float delay = 0)
		{
			_inAppBrowser?.Close(delay);

			if (!_inAppBrowserGameObject)
				Object.Destroy(_inAppBrowserGameObject);

			_inAppBrowser = null;
		}

#if UNITY_WEBGL
		[DllImport("__Internal")]
		private static extern void OpenPaystationWidget(string token, bool sandbox);

		[DllImport("__Internal")]
		private static extern void ClosePaystationWidget();

		private static Action<bool> WebGLBrowserClosedCallback;

		public static void ClosePaystationWidget(bool isManually)
		{
			WebGLBrowserClosedCallback?.Invoke(isManually);
			ClosePaystationWidget();
		}
#endif

#if UNITY_EDITOR_OSX || UNITY_STANDALONE_OSX
		[DllImport("XsollaSdkNative")]
		private static extern void OpenUrlInSafari(string url);

		public static void OpenSafari(string url)
		{
			OpenUrlInSafari(url);
		}
#endif
	}
}