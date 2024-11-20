using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(UserProfileEntryUI))]
	public class AndroidProfileEntryDisabler : MonoBehaviour
	{
#if UNITY_ANDROID
		private void Start()
		{
			XsollaAuth.GetUserInfo(
				info =>
				{
					if (string.IsNullOrEmpty(info.email))
						gameObject.SetActive(false);
				},
				null);
		}
#endif
	}
}