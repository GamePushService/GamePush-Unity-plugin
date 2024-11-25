using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class XsollaSettingsValidator : MonoBehaviour
	{
		public bool ValidateXsollaSettings()
		{
			if (string.IsNullOrEmpty(XsollaSettings.LoginId))
			{
				var errorMessage = "Please copy the Login project ID from your Publisher Account and add it to your project settings";
				GenerateLoginSettingsError(errorMessage);
				return false;
			}

			if (string.IsNullOrEmpty(XsollaSettings.StoreProjectId))
			{
				var errorMessage = "Please copy the Store project ID from your Publisher Account and add it to your project settings";
				GenerateLoginSettingsError(errorMessage);
				return false;
			}

			var isDefaultLoginID = XsollaSettings.LoginId == Constants.DEFAULT_LOGIN_ID;
			var isDefaultProjectID = XsollaSettings.StoreProjectId == Constants.DEFAULT_PROJECT_ID;

			if (isDefaultLoginID && !isDefaultProjectID)
			{
				var errorMessage = $"You've changed [XsollaSettings->ProjectID] to '{XsollaSettings.StoreProjectId}', but did not change LoginID. Change LoginID from '{XsollaSettings.LoginId}' to correct value.";
				GenerateLoginSettingsError(errorMessage);
				return false;
			}

			return true;
		}

		private void GenerateLoginSettingsError(string errorMessage)
		{
			var proxyObject = new GameObject();
			var proxyScript = proxyObject.AddComponent<LoginSettingsErrorHolder>();
			proxyScript.LoginSettingsError = errorMessage;
		}
	}
}
