#if UNITY_IOS
using System;
using System.Runtime.InteropServices;

namespace Xsolla.Core
{
	internal class IosSocialAuth
	{
		[DllImport("__Internal")]
		private static extern void _authBySocialNetwork(string platform, int clientID, string state, string redirectUri,
			IosCallbacks.ActionStringCallbackDelegate successCallback, IntPtr successActionPtr,
			IosCallbacks.ActionStringCallbackDelegate errorCallback, IntPtr errorActionPtr,
			IosCallbacks.ActionVoidCallbackDelegate cancelCallback, IntPtr cancelActionPtr);

		private Action OnSuccess;
		private Action<Error> OnError;
		private Action OnCancel;

		public void Perform(SocialProvider socialProvider, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			IosUtils.ConfigureAnalytics();

			OnSuccess = onSuccess;
			OnError = onError;
			OnCancel = onCancel;

			Action<string> onSuccessNative = HandleSuccess;
			Action<string> onErrorNative = HandleError;
			Action onCancelNative = HandleCancel;

			_authBySocialNetwork(
				socialProvider.ToString().ToUpper(),
				XsollaSettings.OAuthClientId,
				"xsollatest",
				RedirectUrlHelper.GetAuthDeepLinkUrl(),
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