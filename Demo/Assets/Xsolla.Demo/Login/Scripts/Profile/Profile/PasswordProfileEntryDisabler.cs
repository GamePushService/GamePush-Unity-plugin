using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class PasswordProfileEntryDisabler : MonoBehaviour
	{
		private void Start()
		{
			XsollaAuth.GetUserInfo(
				info =>
				{
					var isEmailPresented = !string.IsNullOrEmpty(info.email);
					var isUsernamePresented = !string.IsNullOrEmpty(info.username);
					if (!isEmailPresented && !isUsernamePresented)
						gameObject.SetActive(false);
				},
				error => XDebug.Log("Could not get user info"));
		}
	}
}