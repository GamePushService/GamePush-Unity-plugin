#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidAuthCallback : AndroidJavaProxy
	{
		private readonly AndroidHelper AndroidHelper;
		private readonly Action OnSuccess;
		private readonly Action<Error> OnError;
		private readonly Action OnCancel;

		public AndroidAuthCallback(AndroidHelper androidHelper, Action onSuccess, Action<Error> onError, Action onCancel) : base("com.xsolla.android.login.callback.AuthCallback")
		{
			AndroidHelper = androidHelper;
			OnSuccess = onSuccess;
			OnError = onError;
			OnCancel = onCancel;
		}

		public void onSuccess()
		{
			try
			{
				var accessToken = AndroidHelper.Xlogin.CallStatic<string>("getToken");
				var refreshToken = AndroidHelper.Xlogin.CallStatic<string>("getRefreshToken");
				XsollaToken.Create(accessToken, refreshToken);
				OnSuccess?.Invoke();
			}
			catch (Exception e)
			{
				OnError?.Invoke(new Error(errorMessage: e.Message));
			}
		}

		public void onError(AndroidJavaObject _, string errorMessage)
		{
			if (!string.IsNullOrEmpty(errorMessage) && errorMessage == "CANCELLED")
			{
				OnCancel?.Invoke();
			}
			else
			{
				OnError?.Invoke(new Error(errorMessage: errorMessage));
			}
		}
	}
}
#endif