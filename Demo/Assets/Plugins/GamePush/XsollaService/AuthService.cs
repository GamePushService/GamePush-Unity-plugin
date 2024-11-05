using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Auth;

namespace GamePush.Services
{
	public static class AuthService
	{
		public static void Login()
		{
			XsollaAuth.AuthWithXsollaWidget(OnSuccess, OnError, OnCancel);
		}

		private static void OnSuccess()
		{
			Debug.Log("Authorization successful");
			// Add actions taken in case of success
		}

		private static void OnError(Error error)
		{
			Debug.LogError($"Authorization failed. Error: {error.errorMessage}");
			// Add actions taken in case of error
		}

		private static void OnCancel()
		{
			Debug.Log("Authorization cancel");
			// Add actions taken in case of success
		}
	}

}
