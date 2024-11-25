#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Xsolla.Core
{
	internal class IosPayments
	{
		[DllImport("__Internal")]
		private static extern void _performPayment(
			string token,
			bool isSandbox,
			string redirectUrl,
			IosCallbacks.ActionStringCallbackDelegate onErrorCallback, IntPtr onErrorActionPtr,
			IosCallbacks.ActionBoolCallbackDelegate browserCallback, IntPtr browserCallbackActionPtr);

		private Action<bool> OnBrowserClose { get; set; }

		public void Perform(string paymentToken, Action<bool> onBrowserClose)
		{
			IosUtils.ConfigureAnalytics();
			IosUtils.SetPaystationVersion(XsollaSettings.PaystationVersion);

			OnBrowserClose = onBrowserClose;

			Action<string> onErrorNative = HandleError;
			Action<bool> onBrowserClosedNative = HandleBrowserClose;

			_performPayment(
				paymentToken,
				XsollaSettings.IsSandbox,
				RedirectUrlHelper.GetPaymentDeepLinkUrl(XsollaSettings.IosRedirectPolicySettings),
				IosCallbacks.ActionStringCallback, onErrorNative.GetPointer(),
				IosCallbacks.ActionBoolCallback, onBrowserClosedNative.GetPointer());
		}

		private static void HandleError(string error)
		{
			XDebug.LogError($"IosPayments. Error: {error}");
		}

		private void HandleBrowserClose(bool isManually)
		{
			OnBrowserClose?.Invoke(isManually);
		}
	}
}
#endif