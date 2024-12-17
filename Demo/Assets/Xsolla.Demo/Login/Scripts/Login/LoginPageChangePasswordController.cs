using System;
using Xsolla.Core;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;

namespace Xsolla.Demo
{
	public class LoginPageChangePasswordController : LoginPageController
	{
		[SerializeField] InputField EmailInputField = default;
		[SerializeField] SimpleButton ChangePasswordButton = default;

		public static string LastEmail { get; private set; }

		private bool IsPasswordChangeInProgress
		{
			get => base.IsInProgress;
			set
			{
				if (value)
					XDebug.Log("LoginPageChangePasswordController: Password reset started");
				else
					XDebug.Log("LoginPageChangePasswordController: Password reset ended");

				base.IsInProgress = value;
			}
		}

		private void Awake()
		{
			if (ChangePasswordButton != null)
				ChangePasswordButton.onClick += PrepareAndRunPasswordChange;
		}

		private void PrepareAndRunPasswordChange()
		{
			RunPasswordChange(EmailInputField.text);
		}

		public void RunPasswordChange(string email)
		{
			if (IsPasswordChangeInProgress)
				return;

			LastEmail = email;

			IsPasswordChangeInProgress = true;

			Action onSuccessfulPasswordChange = () =>
			{
				XDebug.Log("LoginPageChangePasswordController: Password change success");
				OnSuccess?.Invoke();
			};

			Action<Error> onFailedPasswordChange = error =>
			{
				XDebug.LogError($"LoginPageChangePasswordController: Password change error: {error.ToString()}");
				OnError?.Invoke(error);
			};

			XsollaAuth.ResetPassword(email, onSuccessfulPasswordChange, onFailedPasswordChange);
			IsPasswordChangeInProgress = false;
		}
	}
}