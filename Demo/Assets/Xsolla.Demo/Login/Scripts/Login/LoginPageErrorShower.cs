using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageErrorShower : MonoBehaviour
	{
		[SerializeField] private Text ErrorText;
		[SerializeField] private SimpleTextButton ResendEmailButton;

		private void Awake()
		{
			if (ResendEmailButton)
			{
				ResendEmailButton.onClick += () => XsollaAuth.ResendConfirmationLink(LoginPageEnterController.LastUsername, null, null);
				ResendEmailButton.gameObject.SetActive(false);
			}
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

		public void ShowError(string errorMessage)
		{
			ErrorText.text = errorMessage;
		}

		public void ShowError(Error error)
		{
			if (ResendEmailButton)
				ResendEmailButton.gameObject.SetActive(error != null && error.ErrorType == ErrorType.UserIsNotActivated);

			if (error == null)
				ShowError("Unknown error");
			else if (!string.IsNullOrEmpty(error.errorMessage))
				ShowError(error.errorMessage);
			else
				ShowError(error.ErrorType.ToString());
		}
	}
}