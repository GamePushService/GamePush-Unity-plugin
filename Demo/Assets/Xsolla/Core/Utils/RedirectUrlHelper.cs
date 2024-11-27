using UnityEngine;

namespace Xsolla.Core
{
	internal static class RedirectUrlHelper
	{
		public static string GetRedirectUrl(string redirectUri)
		{
			if (!string.IsNullOrEmpty(redirectUri))
				return redirectUri;

			return !string.IsNullOrEmpty(XsollaSettings.CallbackUrl)
				? XsollaSettings.CallbackUrl
				: Constants.DEFAULT_REDIRECT_URL;
		}

		public static string GetAuthDeepLinkUrl()
		{
			return string.IsNullOrEmpty(XsollaSettings.CallbackUrl)
				? $"app://xlogin.{Application.identifier}"
				: XsollaSettings.CallbackUrl;
		}

		public static string GetPaymentDeepLinkUrl(RedirectPolicySettings redirectPolicy)
		{
			var defaultUrl = $"app://xpayment.{Application.identifier}";

			if (redirectPolicy.UseSettingsFromPublisherAccount)
				return defaultUrl;

			var customUrl = redirectPolicy.ReturnUrl;
			return string.IsNullOrEmpty(customUrl)
				? defaultUrl
				: customUrl;
		}
	}
}