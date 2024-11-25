using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class PasswordToggleDecorator : MonoBehaviour
	{
		[SerializeField] Toggle ShowPasswordToggle = default;
		[SerializeField] Image Eye = default;
		[SerializeField] Sprite OpenedEye = default;
		[SerializeField] Sprite ClosedEye = default;

		private void Start()
		{
			ShowPasswordToggle.onValueChanged.AddListener(ToggleEye);
			ToggleEye(ShowPasswordToggle.isOn);
		}

		private void ToggleEye(bool showPassword)
		{
			Eye.sprite = showPassword ? OpenedEye : ClosedEye;
		}
	}
}
