using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageCreateController : LoginPageController
	{
		[SerializeField] private InputField UsernameInputField = default;
		[SerializeField] private InputField EmailInputField = default;
		[SerializeField] private InputField PasswordInputField = default;
		[SerializeField] private SimpleButton CreateButton = default;

		public static string LastUsername { get; private set; }
		public static string LastEmail { get; private set; }

		public static void DropLastCredentials()
		{
			LastUsername = null;
			LastEmail = null;
		}

		private bool IsCreateInProgress
		{
			get => base.IsInProgress;
			set
			{
				if (value)
					XDebug.Log("LoginPageCreateController: Create started");
				else
					XDebug.Log("LoginPageCreateController: Create ended");

				base.IsInProgress = value;
			}
		}

		private void Awake()
		{
			if (CreateButton != null)
				CreateButton.onClick += PrepareAndRunCreate;
		}

		private void Start()
		{
			if (!string.IsNullOrEmpty(LastUsername))
				UsernameInputField.text = LastUsername;

			if (!string.IsNullOrEmpty(LastEmail))
				EmailInputField.text = LastEmail;
		}

		private void PrepareAndRunCreate()
		{
			RunCreate(UsernameInputField.text, EmailInputField.text, PasswordInputField.text);
		}

		public void RunCreate(string username, string email, string password)
		{
			if (IsCreateInProgress)
				return;

			LastEmail = email;
			LastUsername = username;

			IsCreateInProgress = true;

			var isFieldsFilled = !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password);
			var isEmailValid = ValidateEmail(email);

			if (isFieldsFilled && isEmailValid)
			{
				Action<LoginLink> onSuccessfulCreate = loginLink =>
				{
					XDebug.Log("LoginPageCreateController: Create success");

					if (loginLink.login_url != null)
					{
#if UNITY_6000
						var loginPageHelper = FindAnyObjectByType<LoginPagesHelper>();
#else
						var loginPageHelper = FindObjectOfType<LoginPagesHelper>();
#endif
						loginPageHelper.SetState(MenuState.Authorization);

#if UNITY_6000

						var loginPageEnterController = FindAnyObjectByType<LoginPageEnterController>();
#else
						var loginPageEnterController = FindObjectOfType<LoginPageEnterController>();
#endif
						loginPageEnterController.RunBasicAuth(username, password, true);
					}
					else
					{
						OnSuccess?.Invoke();
					}
				};

				Action<Error> onFailedCreate = error =>
				{
					XDebug.LogError($"LoginPageCreateController: Create error: {error}");
					OnError?.Invoke(error);
				};

				XsollaAuth.Register(
					username,
					password,
					email,
					onSuccessfulCreate,
					onFailedCreate,
					acceptConsent: true,
					promoEmailAgreement: true);
			}
			else if (!isEmailValid)
			{
				XDebug.Log($"Invalid email: {email}");
				Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: "Invalid email");
				OnError?.Invoke(error);
			}
			else
			{
				XDebug.LogError($"Fields are not filled. Username: '{username}' Password: '{password}'");
				Error error = new Error(errorType: ErrorType.RegistrationNotAllowedException, errorMessage: $"Not all fields are filled");
				base.OnError?.Invoke(error);
			}

			IsCreateInProgress = false;
		}
	}
}