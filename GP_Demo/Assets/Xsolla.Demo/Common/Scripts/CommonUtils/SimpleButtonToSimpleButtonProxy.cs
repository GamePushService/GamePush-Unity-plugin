using UnityEngine;

namespace Xsolla.Demo
{
    public class SimpleButtonToSimpleButtonProxy : MonoBehaviour
    {
		[SerializeField] private SimpleButton OriginalButton = default;
		[SerializeField] private SimpleButton[] ButtonsToInvoke = default;

		private void Awake()
		{
			OriginalButton.onClick += RaiseAllButtons;
		}

		private void RaiseAllButtons()
		{
			foreach (var button in ButtonsToInvoke)
				button.onClick?.Invoke();
		}
	}
}
