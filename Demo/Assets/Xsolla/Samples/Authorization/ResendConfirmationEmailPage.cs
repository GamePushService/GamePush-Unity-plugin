using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Samples.Authorization
{
	public class ResendConfirmationEmailPage : MonoBehaviour
	{
		// Declaration of variables for UI elements
		public InputField UsernameInput;
		public Button ResendEmailButton;

		private void Start()
		{
			// Handling the button click
			ResendEmailButton.onClick.AddListener(() =>
			{
				// Get the username from the input field
				var username = UsernameInput.text;

				// Call the resend confirmation email method
				// Pass the username and callback functions for success and error cases
				XsollaAuth.ResendConfirmationLink(username, OnSuccess, OnError);
			});
		}

		private void OnSuccess()
		{
			Debug.Log("Resend confirmation email successful");
			// Add actions taken in case of success
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Resend confirmation email failed. Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}
}
