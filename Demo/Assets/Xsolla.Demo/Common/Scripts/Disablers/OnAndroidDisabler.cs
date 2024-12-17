using UnityEngine;

namespace Xsolla.Demo
{
	public class OnAndroidDisabler : MonoBehaviour
	{
#if UNITY_ANDROID
		private void Awake()
		{
			this.gameObject.SetActive(false);
		}
#endif
	}
}
