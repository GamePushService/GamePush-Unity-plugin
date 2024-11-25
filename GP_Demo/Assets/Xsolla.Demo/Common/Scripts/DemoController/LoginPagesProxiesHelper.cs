namespace Xsolla.Demo
{
	public class LoginPagesProxiesHelper : OnStateChangedHandler
	{
		protected sealed override void OnStateChanged(MenuState lastState, MenuState newState)
		{
			if (newState == MenuState.Authorization)
			{
#if UNITY_6000
				var proxyScript = FindAnyObjectByType<LoginProxyActionHolder>();
#else
				var proxyScript = FindObjectOfType<LoginProxyActionHolder>();
#endif
				var loginEnterScript = base.StateMachine.StateObject.GetComponent<LoginPageEnterController>();

				if (proxyScript != null)
				{
					if (loginEnterScript != null)
						loginEnterScript.RunLoginProxyAction(proxyScript.ProxyAction, proxyScript.ProxyActionArgument);

					Destroy(proxyScript.gameObject);
				}
			}
			else if (newState == MenuState.LoginSettingsError)
			{
#if UNITY_6000
                var proxyScript = FindAnyObjectByType<LoginSettingsErrorHolder>();
#else
				var proxyScript = FindObjectOfType<LoginSettingsErrorHolder>();
#endif
				var errorShower = base.StateMachine.StateObject.GetComponent<LoginPageErrorShower>();

				if (proxyScript != null)
				{
					if (errorShower != null)
						errorShower.ShowError(proxyScript.LoginSettingsError);

					Destroy(proxyScript.gameObject);
				}
			}
		}
	}
}
