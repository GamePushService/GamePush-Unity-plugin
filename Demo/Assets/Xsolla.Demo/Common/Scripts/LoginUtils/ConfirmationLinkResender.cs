using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ConfirmationLinkResender : MonoBehaviour
	{
		[SerializeField] private SimpleButton resendButton;
		[SerializeField] private Text emailText;

		void Awake()
		{
			switch (DemoController.Instance.GetState())
			{
				case MenuState.RegistrationSuccess:
					resendButton.onClick += ResendRegistrationConfirmationEmail;
					ReplaceEmailText(LoginPageCreateController.LastEmail);
					break;
				case MenuState.ChangePasswordSuccess:
					resendButton.onClick += ResendPasswordResetEmail;
					ReplaceEmailText(LoginPageChangePasswordController.LastEmail);
					break;
				default:
					XDebug.LogWarning("Unexpected state");
					break;
			}
		}

		private void ResendRegistrationConfirmationEmail()
		{
			XsollaAuth.ResendConfirmationLink(LoginPageCreateController.LastEmail, null, null);
		}

		private void ResendPasswordResetEmail()
		{
			XsollaAuth.ResetPassword(LoginPageChangePasswordController.LastEmail, null, null);
		}

		private void ReplaceEmailText(string email)
		{
			if (!string.IsNullOrEmpty(email))
			{
				var currentMessage = emailText.text;
				var modifiedMessage = currentMessage.Replace("{email@domen.com}", email);
				emailText.text = modifiedMessage;
			}
		}
	}
}