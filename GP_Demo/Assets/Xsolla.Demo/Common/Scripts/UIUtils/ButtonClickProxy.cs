using UnityEngine;

namespace Xsolla.Demo
{
    public class ButtonClickProxy : MonoBehaviour
    {
		[SerializeField] private SimpleButton ProxyButton = default;
		[SerializeField] private SimpleButton[] ButtonsToClick = default;

		private void Awake()
		{
			ProxyButton.onClick += InvokeButtonsClick;
		}

		private void InvokeButtonsClick()
		{
			foreach (var button in ButtonsToClick)
			{
				button.onClick?.Invoke();
			}
		}
	}
}
