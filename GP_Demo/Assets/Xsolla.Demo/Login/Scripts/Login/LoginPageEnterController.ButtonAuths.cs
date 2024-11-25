using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public partial class LoginPageEnterController
	{
		[SerializeField] private InputField EmailInputField;
		[SerializeField] private InputField PasswordInputField;
		[SerializeField] private Toggle RememberMeCheckbox;
		[SerializeField] private SimpleButton LoginButton;

		[Space]
		[SerializeField] private SimpleSocialButton[] MainSocialLoginButtons;
		[SerializeField] private SimpleButton OtherSocialNetworksButton;
		[SerializeField] private SocialNetworksWidget SocialNetworksWidget;

		[Space]
		[SerializeField] private SimpleButton PasswordlessButton;
		[SerializeField] private PasswordlessWidget PasswordlessWidget;

		[Space]
		[SerializeField] private SimpleButton DeviceIDAuthButton;
		[SerializeField] private SimpleButton LoginWidgetAuthButton;

		private void Awake()
		{
			LoginButton.onClick += () => RunBasicAuth(EmailInputField.text, PasswordInputField.text, RememberMeCheckbox.isOn);

			SocialNetworksWidget.OnSocialButtonClick = RunSocialAuth;
			OtherSocialNetworksButton.onClick += () => SocialNetworksWidget.gameObject.SetActive(true);

			if (PasswordlessWidget)
			{
				PasswordlessButton.onClick += () => PasswordlessWidget.gameObject.SetActive(true);
				PasswordlessWidget.OnPhoneAccessRequest += RunPasswordlessAuth<PasswordlessPhoneAuth>;
				PasswordlessWidget.OnEmailAccessRequest += RunPasswordlessAuth<PasswordlessEmailAuth>;
			}

			foreach (var button in MainSocialLoginButtons)
			{
				button.onClick += () => RunSocialAuth(button.SocialProvider);
			}

			if (DeviceIDAuthButton)
				DeviceIDAuthButton.onClick += RunDeviceIDAuth;
			
			if (LoginWidgetAuthButton)
				LoginWidgetAuthButton.onClick += RunLoginWidgetAuth;
		}

		public void RunBasicAuth(string username, string password, bool rememberMe)
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			object[] args = {
				username,
				password
			};

			TryAuthBy<BasicAuth>(args, SuperComplete, ProcessError);
		}

		public void RunWidgetAuth()
		{
			TryAuthBy<LoginWidgetAuth>(null, SuperComplete, ProcessError);
		}

		public void RunSocialAuth(SocialProvider socialProvider)
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			object[] args = {socialProvider};

			TryAuthBy<SocialAuth>(args, SuperComplete, ProcessError);
		}

		public void RunManualSteamAuth()
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			TryAuthBy<SteamAuth>(null, SuperComplete, ProcessError);
		}

		private void RunDeviceIDAuth()
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			TryAuthBy<DeviceIdAuth>(null, SuperComplete, ProcessError);
		}

		private void RunLoginWidgetAuth()
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);

			TryAuthBy<LoginWidgetAuth>(null, SuperComplete, ProcessError);
		}

		private void RunPasswordlessAuth<T>(string value) where T : LoginAuthorization
		{
			var args = new object[] {
				PasswordlessWidget,
				value
			};

			var passwordlessAuth = GetComponent<T>();
			if (passwordlessAuth != null)
			{
				passwordlessAuth.TryAuth(args, SuperComplete, ProcessError);
				return;
			}

			TryAuthBy<T>(args, SuperComplete, ProcessError);
		}

		public void RunDemoUserAuth()
		{
			if (IsAuthInProgress)
				return;

			IsAuthInProgress = true;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);
			XDebug.LogWarning("!PLEASE WAIT! Process of creating new demo user can take up to 30 seconds");

			Action<DemoUserLoginResponse> onDemoUserSuccess = response =>
			{
				XsollaToken.Create(response.access_token, response.refresh_token);
				SuperComplete();
			};

			WebRequestHelper.Instance.GetRequest(
				sdkType: SdkType.Login,
				url: "https://us-central1-xsolla-sdk-demo.cloudfunctions.net/generateDemoUserToken",
				onComplete: onDemoUserSuccess,
				onError: ProcessError);
		}

		private void DisableCommonButtons()
		{
			var buttonsProvider = GetComponent<LoginPageCommonButtonsProvider>();
			if (buttonsProvider != null)
			{
				if (buttonsProvider.DemoUserButton != null)
					buttonsProvider.DemoUserButton.gameObject.SetActive(false);
				if (buttonsProvider.LogInButton != null)
					buttonsProvider.LogInButton.gameObject.SetActive(false);
			}
		}
	}
}