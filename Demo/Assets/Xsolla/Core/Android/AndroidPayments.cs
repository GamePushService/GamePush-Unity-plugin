#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidPayments
	{
		public void Perform(string paymentToken, Action<bool> onBrowserClose, AndroidActivityType? activityType)
		{
			try
			{
				var helper = new AndroidHelper();

				string redirectScheme = null;
				string redirectHost = null;
				var redirectPolicy = XsollaSettings.AndroidRedirectPolicySettings;
				if (!redirectPolicy.UseSettingsFromPublisherAccount && !string.IsNullOrEmpty(redirectPolicy.ReturnUrl))
				{
					var uri = new Uri(redirectPolicy.ReturnUrl);
					redirectScheme = uri.Scheme;
					redirectHost = uri.Host;
				}

				var browserCloseCallback = new AndroidBrowserCallback {
					OnBrowserClosed = isManually => helper.MainThreadExecutor.Enqueue(() => onBrowserClose?.Invoke(isManually))
				};

				string activityTypeName = activityType?.ToString();

				var proxyActivity = new AndroidJavaClass($"{Application.identifier}.androidProxies.PaymentsProxyActivity");
				proxyActivity.CallStatic("perform",
					helper.CurrentActivity,
					paymentToken,
					XsollaSettings.IsSandbox,
					redirectScheme,
					redirectHost,
					XsollaSettings.PaystationVersion,
					browserCloseCallback,
					activityTypeName);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSDKPaymentsHelper.PerformPayment: {e.Message}", e);
			}
		}
	}
}
#endif