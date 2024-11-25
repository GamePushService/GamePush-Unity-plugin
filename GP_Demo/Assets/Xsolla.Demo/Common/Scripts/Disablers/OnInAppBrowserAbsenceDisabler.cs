using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class OnInAppBrowserAbsenceDisabler : MonoBehaviour
	{
		private void Awake()
		{
			if (XsollaWebBrowser.InAppBrowser == null)
				gameObject.SetActive(false);
		}
	}
}