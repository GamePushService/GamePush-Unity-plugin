using UnityEngine;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(LogInHotkeys))]
	public class OnEnterButtonPresser : MonoBehaviour
	{
		[SerializeField] SimpleButton Button = default;

		private void Awake()
		{
			this.GetComponent<LogInHotkeys>().EnterKeyPressedEvent += TryPressButton;
		}

		private void TryPressButton()
		{
			if (Button is SimpleButtonLockDecorator lockableButton)
			{
				if (!lockableButton.IsLocked())
					lockableButton.onClick?.Invoke();
			}
			else
			{
				Button.onClick?.Invoke();
			}
		}
	}
}
