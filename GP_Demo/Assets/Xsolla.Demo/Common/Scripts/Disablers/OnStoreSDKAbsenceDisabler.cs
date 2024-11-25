using UnityEngine;

namespace Xsolla.Demo
{
	public class OnStoreSDKAbsenceDisabler : MonoBehaviour
	{
		private void Awake()
		{
			if (!DemoMarker.IsStorePartAvailable)
				this.gameObject.SetActive(false);
		}
	}
}
