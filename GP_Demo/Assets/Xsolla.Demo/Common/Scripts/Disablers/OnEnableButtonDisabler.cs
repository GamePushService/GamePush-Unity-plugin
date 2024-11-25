using System.Collections;
using UnityEngine;

namespace Xsolla.Demo
{
	public class OnEnableButtonDisabler : MonoBehaviour
    {
		[SerializeField] private SimpleTextButtonDisableable Button = default;

		private void OnEnable()
		{
			StartCoroutine(DisableAfterDelay());
		}

		private IEnumerator DisableAfterDelay()
		{
			yield return new WaitForEndOfFrame();
			Button.Disable();
		}
	}
}
