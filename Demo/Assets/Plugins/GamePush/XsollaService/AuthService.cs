using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Auth;

namespace GamePush.Xsolla
{
	public static class AuthService
	{
		public static event Action OnLoginComplete;
		public static event Action<string> OnLoginError;

		public static void Login(Action onLoginComplete = null, Action<string> onLoginError = null)
		{
			OnLoginComplete = onLoginComplete;
			OnLoginError = onLoginError;

			XsollaAuth.AuthWithXsollaWidget(OnSuccess, OnError, OnCancel);
		}

		private static void OnSuccess()
		{
			Debug.Log("Authorization successful");
			OnLoginComplete?.Invoke();
		}

		private static void OnError(Error error)
		{
			Debug.LogError($"Authorization failed. Error: {error.errorMessage}");
			OnLoginError?.Invoke(error.errorMessage);
		}

		private static void OnCancel()
		{
			Debug.Log("Authorization cancel");
			OnLoginError?.Invoke("Cancel");
		}

		public static void Register(string username, string password, string email, Action<LoginLink> OnSuccess = null)
        {
			XsollaAuth.Register(username, password, email, OnSuccess, OnError);
		}

	}

}
