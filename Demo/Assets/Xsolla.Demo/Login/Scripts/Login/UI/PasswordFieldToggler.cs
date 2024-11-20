using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(InputField))]
	public class PasswordFieldToggler : MonoBehaviour
	{
		private Toggle _showPasswordToggle;
		private InputField _passwordInputField;

		// Start is called before the first frame update
		void Start()
		{
			_passwordInputField = this.GetComponent<InputField>();
			_showPasswordToggle = this.GetComponentInChildren<Toggle>();

			if (_showPasswordToggle != null)
				_showPasswordToggle.onValueChanged.AddListener(ToggleShowPassword);
			else
				XDebug.LogError("Show password toggle is missing");
		}

		void ToggleShowPassword(bool show)
		{
			_passwordInputField.contentType = show ? InputField.ContentType.Standard : InputField.ContentType.Password;
			_passwordInputField.ForceLabelUpdate();
		}
	}
}
