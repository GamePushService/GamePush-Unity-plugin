using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(LogInHotkeys))]
	public class OnTabInputFieldSwitcher : MonoBehaviour
	{
		[SerializeField] InputField[] InputFields = default;

		private int _currentField = -1;

		private void Awake()
		{
			this.GetComponent<LogInHotkeys>().TabKeyPressedEvent += SwitchToNextInputField;

			for (int i = 0; i < InputFields.Length; i++)
			{
				int index = i;
				InputFields[i].onValueChanged.AddListener(_ => SetCurrentField(index));
			}
		}

		private void SwitchToNextInputField()
		{
			_currentField++;

			if(_currentField >= InputFields.Length)
				_currentField = 0;

			InputFields[_currentField].Select();
		}

		private void SetCurrentField(int index)
		{
			_currentField = index;
		}
	}
}
