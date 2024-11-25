using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;

public class LoginPageEnterControllerMobile : MonoBehaviour
{
	[SerializeField] SimpleButton openDefaultLoginButton = default;
	[SerializeField] SimpleButton closeDefaultLoginButton = default;

	[SerializeField] InputField emailInputField = default;
	[SerializeField] InputField passwordInputField = default;
	
	[SerializeField] SimpleTextButtonDisableable loginButton = default; 

	[Space]
	[SerializeField] GameObject defaultLoginWidget = default;

	private void Awake()
	{
		openDefaultLoginButton.onClick += OnOpenDefaultLoginWidget;
		closeDefaultLoginButton.onClick += OnCloseDefaultLoginWidget;
	}

	public void ShowDefaultLoginWidget(bool bShow)
	{
		defaultLoginWidget.SetActive(bShow);
	}

	private void OnOpenDefaultLoginWidget()
	{
		ShowDefaultLoginWidget(true);

		loginButton.Disable();
	}

	private void OnCloseDefaultLoginWidget()
	{
		ShowDefaultLoginWidget(false);

		emailInputField.text = string.Empty;
		passwordInputField.text = string.Empty;
	}
}