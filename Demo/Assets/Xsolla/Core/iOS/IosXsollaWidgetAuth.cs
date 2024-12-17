#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Xsolla.Core
{
	internal class IosXsollaWidgetAuth
	{
		[DllImport("__Internal")]
		private static extern void _authWithXsollaWidget(string loginId, int clientId, string state, string redirectUri, string locale,
			IosCallbacks.ActionStringCallbackDelegate successCallback, IntPtr successActionPtr,
			IosCallbacks.ActionStringCallbackDelegate errorCallback, IntPtr errorActionPtr,
			IosCallbacks.ActionVoidCallbackDelegate cancelCallback, IntPtr cancelActionPtr);

		private Action OnSuccess;
		private Action<Error> OnError;
		private Action OnCancel;

		public void Perform(Action onSuccess, Action<Error> onError, Action onCancel, string locale)
		{
			IosUtils.ConfigureAnalytics();

			OnSuccess = onSuccess;
			OnError = onError;
			OnCancel = onCancel;

			Action<string> onSuccessNative = HandleSuccess;
			Action<string> onErrorNative = HandleError;
			Action onCancelNative = HandleCancel;

			_authWithXsollaWidget(
				XsollaSettings.LoginId,
				XsollaSettings.OAuthClientId,
				"xsollatest",
				RedirectUrlHelper.GetAuthDeepLinkUrl(),
				locale,
				IosCallbacks.ActionStringCallback, onSuccessNative.GetPointer(),
				IosCallbacks.ActionStringCallback, onErrorNative.GetPointer(),
				IosCallbacks.ActionVoidCallback, onCancelNative.GetPointer());
		}

		private void HandleSuccess(string tokenJson)
		{
			var response = ParseUtils.FromJson<TokenResponse>(tokenJson);
			XsollaToken.Create(response.access_token, response.refresh_token);
			OnSuccess?.Invoke();
		}

		private void HandleError(string error)
		{
			OnError?.Invoke(new Error(errorMessage: $"IosSocialAuth: {error}"));
		}

		private void HandleCancel()
		{
			OnCancel?.Invoke();
		}
	}
}
#endif