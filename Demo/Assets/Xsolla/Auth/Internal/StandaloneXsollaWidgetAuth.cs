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
			          .AddParam("login_url", RedirectUrlHelper.GetRedirectUrl(null))
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
			if (!ParseUtils.TryGetValueFromUrl(newUrl, ParseParameter.token, out var parsedToken))
				return;

			XsollaToken.Create(parsedToken);
			OnSuccess?.Invoke();

			UnsubscribeFromBrowser();
			XsollaWebBrowser.Close();
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