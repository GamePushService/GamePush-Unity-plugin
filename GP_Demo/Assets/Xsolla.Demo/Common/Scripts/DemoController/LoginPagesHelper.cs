using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
    public class LoginPagesHelper : OnStateChangedHandler
	{
		private GameObject StateObject => base.StateMachine.StateObject;
		private bool IsAuthInProgress => StateObject.GetComponent<LoginPageEnterController>()?.IsAuthInProgress ?? false;

		protected sealed override void OnStateChanged(MenuState _, MenuState newState)
		{
			if (newState.IsPostAuthState())
				return;

			if (StateObject == null)
				return;

			var pageController = StateObject.GetComponent<LoginPageController>();
			var buttonsProvider = StateObject.GetComponent<LoginPageCommonButtonsProvider>();

			if (pageController == null && buttonsProvider == null)
				return;

			SetupButtons(buttonsProvider, newState);
			SetupPageChangingRules(newState, pageController, buttonsProvider);
		}

		public GameObject SetState(MenuState state)
		{
			return base.StateMachine.SetState(state);
		}

		private void SetupButtons(LoginPageCommonButtonsProvider buttonsProvider, MenuState newState)
		{
			if (buttonsProvider == null)
				return;

			if (buttonsProvider.LogInButton != null)
			{
				if (newState == MenuState.Authorization)
					buttonsProvider.LogInButton.onClick += () =>
					{
						if (!IsAuthInProgress)
							SetState(MenuState.Registration);
					};
				else
					buttonsProvider.LogInButton.onClick += () => SetState(MenuState.Authorization);
			}

			if (buttonsProvider.DemoUserButton != null)
				buttonsProvider.DemoUserButton.onClick += RunDemoUserAuth;
		}

		private void RunDemoUserAuth()
		{
			if (IsAuthInProgress)
				return;

			var proxyObject = new GameObject();
			var proxyScript = proxyObject.AddComponent<LoginProxyActionHolder>();

			proxyScript.ProxyAction = LoginProxyActions.RunDemoUserAuthDelegate;
			proxyScript.ProxyActionArgument = null;

			SetState(MenuState.Authorization);
		}

		private void SetupPageChangingRules(MenuState newState, LoginPageController pageController, LoginPageCommonButtonsProvider buttonsProvider)
		{
			switch (newState)
			{
				case MenuState.Authorization:
				{
					if (pageController != null)
					{
						pageController.OnSuccess = () => SetState(MenuState.Main);
						pageController.OnError = error => OnLoginError(error, MenuState.AuthorizationFailed);
					}

					if (buttonsProvider != null)
					{
						buttonsProvider.OKButton.onClick += () =>
						{
							if (IsAuthInProgress)
								return;
							else
								SetState(MenuState.ChangePassword);
						};
					}

					break;
				}
				case MenuState.AuthorizationFailed:
				{
					if (buttonsProvider != null)
						buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
					break;
				}
				case MenuState.Registration:
				{
					if (pageController != null)
					{
						pageController.OnSuccess = () => SetState(MenuState.RegistrationSuccess);
						pageController.OnError = error => OnLoginError(error, MenuState.RegistrationFailed);
					}
					break;
				}
				case MenuState.RegistrationSuccess:
				{
					if (buttonsProvider != null)
						buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
					break;
				}
				case MenuState.RegistrationFailed:
				{
					if (buttonsProvider != null)
						buttonsProvider.OKButton.onClick += () => SetState(MenuState.Registration);
					break;
				}
				case MenuState.ChangePassword:
				{
					if (pageController != null)
					{
						pageController.OnSuccess = () => SetState(MenuState.ChangePasswordSuccess);
						pageController.OnError = error => OnLoginError(error, MenuState.ChangePasswordFailed);
					}
					break;
				}
				case MenuState.ChangePasswordSuccess:
				{
					if (buttonsProvider != null)
						buttonsProvider.OKButton.onClick += () => SetState(MenuState.Authorization);
					break;
				}
				case MenuState.ChangePasswordFailed:
				{
					if (buttonsProvider != null)
						buttonsProvider.OKButton.onClick += () => SetState(MenuState.ChangePassword);
					break;
				}
			}
		}

		private void OnLoginError(Error error, MenuState stateToSet)
		{
			var newStateObject = SetState(stateToSet);
			if (newStateObject != null)
				newStateObject.GetComponent<LoginPageErrorShower>()?.ShowError(error);
		}
	}
}
