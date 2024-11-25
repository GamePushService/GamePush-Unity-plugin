using System;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginActionsCreatePageProxer : MonoBehaviour
	{
		[SerializeField] private SimpleSocialButton[] MainSocialLoginButtons = default;
		[SerializeField] private SimpleButton OtherSocialNetworksButton = default;
		[SerializeField] private SocialNetworksWidget SocialNetworksWidget = default;

		private void Awake()
		{
			SocialNetworksWidget.OnSocialButtonClick = RequestSocialAuth;
			OtherSocialNetworksButton.onClick += () => SocialNetworksWidget.gameObject.SetActive(true);
			
			foreach (var button in MainSocialLoginButtons)
			{
				button.onClick += () => RequestSocialAuth(button.SocialProvider);
			}
		}

		private void RequestSocialAuth(SocialProvider provider)
		{
			ExecuteProxyRequest(LoginProxyActions.RunSocialAuthDelegate, provider);
		}

		private void RequestSteamAuth()
		{
			ExecuteProxyRequest(LoginProxyActions.RunSteamAuthDelegate, null);
		}

		private void ExecuteProxyRequest(Action<LoginPageEnterController, object> proxyRequest, object proxyArgument)
		{
			var proxyObject = new GameObject();
			var proxyScript = proxyObject.AddComponent<LoginProxyActionHolder>();

			proxyScript.ProxyAction = proxyRequest;
			proxyScript.ProxyActionArgument = proxyArgument;

			DemoController.Instance.SetState(MenuState.Authorization);
		}
	}
}
