using UnityEngine;

namespace Xsolla.Demo
{
	public class OnWebGLDisabler : MonoBehaviour
	{
#if UNITY_WEBGL
		private void Awake()
		{
			this.gameObject.SetActive(false);
		}
#endif
	}
}
