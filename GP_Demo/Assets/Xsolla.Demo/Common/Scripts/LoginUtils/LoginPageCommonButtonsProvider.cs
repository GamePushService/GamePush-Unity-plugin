using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LoginPageCommonButtonsProvider : MonoBehaviour
	{
		public SimpleButton OKButton;
		public SimpleButton DemoUserButton;
		public SimpleButton LogInButton;

		private void Start()
		{
			if (DemoUserButton)
			{
				var isDefaultProject = XsollaSettings.LoginId == Constants.DEFAULT_LOGIN_ID
				                       && XsollaSettings.StoreProjectId == Constants.DEFAULT_PROJECT_ID;

				DemoUserButton.gameObject.SetActive(isDefaultProject);
			}
		}
	}
}