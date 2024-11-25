using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class OnNotDefaultProjectDisabler : MonoBehaviour
	{
		private void Start()
		{
			var isDefaultProject = XsollaSettings.LoginId == Constants.DEFAULT_LOGIN_ID
				&& XsollaSettings.StoreProjectId == Constants.DEFAULT_PROJECT_ID;

			gameObject.SetActive(isDefaultProject);
		}
	}
}