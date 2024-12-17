using System;
using Xsolla.Core;

namespace Xsolla.Auth
{
	internal class StandaloneXsollaWidgetAuth
	{
		private Action OnSuccess;
		private Action<Error> OnError;
		private Action OnCancel;
		private IInAppBrowser BrowserInstance;

		public void Perform(Action onSuccess, Action<Error> onError, Action onCancel, string locale)
		{
			OnSuccess = onSuccess;
			OnError = onError;
			OnCancel = onCancel;

			var url = new UrlBuilder("https://login-widget.xsolla.com/latest/")
				.AddProjectId(XsollaSettings.LoginId)
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddResponseType("code")
				.AddState("xsollatest")
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(null))
				.AddScope("offline")
				.AddLocale(locale)
				.Build();

			XsollaWebBrowser.Open(url);

			BrowserInstance = XsollaWebBrowser.InAppBrowser;
			BrowserInstance.CloseEvent += OnBrowserClose;
			BrowserInstance.UrlChangeEvent += OnBrowserUrlChange;

			XsollaWebBrowser.InAppBrowser.UpdateSize(820, 840);
		}

		private void OnBrowserUrlChange(string newUrl)
		{
			if (!ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.code, out var parsedCode))
				return;

			UnsubscribeFromBrowser();
			XsollaWebBrowser.Close();

			XsollaAuth.ExchangeCodeToToken(
				parsedCode,
				() => OnSuccess?.Invoke(),
				error => OnError?.Invoke(error));
		}

		private void OnBrowserClose(BrowserCloseInfo info)
		{
			OnCancel?.Invoke();
			UnsubscribeFromBrowser();
		}

		private void UnsubscribeFromBrowser()
		{
			var browser = XsollaWebBrowser.InAppBrowser;
			browser.CloseEvent -= OnBrowserClose;
			browser.UrlChangeEvent -= OnBrowserUrlChange;
		}
	}
}