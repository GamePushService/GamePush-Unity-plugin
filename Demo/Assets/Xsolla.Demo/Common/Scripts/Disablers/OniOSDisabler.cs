using UnityEngine;

namespace Xsolla.Demo
{
	public class OniOSDisabler : MonoBehaviour
	{
#if UNITY_IOS
		private void Awake()
		{
			this.gameObject.SetActive(false);
		}
#endif
	}
}