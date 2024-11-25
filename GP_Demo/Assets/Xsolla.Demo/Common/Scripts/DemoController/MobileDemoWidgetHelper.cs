namespace Xsolla.Demo
{
	public class MobileDemoWidgetHelper : OnStateChangedHandler
	{
		protected sealed override void OnStateChanged(MenuState lastState, MenuState newState)
		{
			if (lastState == newState || newState != MenuState.Authorization)
				return;

			var loginEnterScript = base.StateMachine.StateObject.GetComponent<LoginPageEnterControllerMobile>();
			if (loginEnterScript == null)
				return;

			if (lastState == MenuState.ChangePassword)
			{
				if (LoginPageChangePasswordControllerMobile.IsBackNavigationTriggered)
					loginEnterScript.ShowDefaultLoginWidget(true);
			}
			else if (lastState == MenuState.Registration)
			{
				if (LoginPageCreateControllerMobile.IsLoginNavigationTriggered)
					loginEnterScript.ShowDefaultLoginWidget(true);
			}
		}
	}
}
