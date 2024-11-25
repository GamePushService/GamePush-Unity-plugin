using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Samples.Authorization
{
	public class SocialAuthorizationPage : MonoBehaviour
	{
		// Declaration of variables for SocialProvider and signIn button
		public SocialProvider SocialProvider = SocialProvider.Facebook;
		public Button SignInButton;

		private void Start()
		{
			// Handling the button click
			SignInButton.onClick.AddListener(() =>
			{
				// Call the social authorization method
				// Pass the social network provider and callback functions for success, error and cancel cases
				XsollaAuth.AuthViaSocialNetwork(SocialProvider, OnSuccess, OnError, OnCancel);
			});
		}

		private void OnSuccess()
		{
			Debug.Log("Social authorization successful");
			// Add actions taken in case of success
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Social authorization failed. Error: {error.errorMessage}");
			// Add actions taken in case of error
		}

		private void OnCancel()
		{
			Debug.Log("Social authorization cancelled by user.");
			// Add actions taken in case the user canceles authorization
		}
	}
}
