using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserProfileEntryGenderEditor : UserProfileEntryEditor
	{
		[SerializeField] Toggle MaleCheckbox = default;
		[SerializeField] Toggle FemaleCheckbox = default;
		[SerializeField] Toggle OtherCheckbox = default;
		[SerializeField] Toggle PreferNotCheckbox = default;

		private bool _skipNextCheck;
		private Toggle[] _checkboxes;

		private void Awake()
		{
			_checkboxes = new Toggle[]
			{
				MaleCheckbox, FemaleCheckbox, OtherCheckbox, PreferNotCheckbox
			};

			for (int i = 0; i < _checkboxes.Length; i++)
			{
				var checkboxType = (CheckboxType)i;
				_checkboxes[i].onValueChanged.AddListener( newValue => ProcessValueChange(checkboxType, newValue) );
			}
		}

		public override void SetInitial(string value)
		{
			if (string.IsNullOrEmpty(value))
				return;

			var checkboxTypeToActivate = default(CheckboxType);

			switch (value)
			{
				case UserProfileGender.MALE:
					checkboxTypeToActivate = CheckboxType.Male;
					break;
				case UserProfileGender.FEMALE:
					checkboxTypeToActivate = CheckboxType.Female;
					break;
				case UserProfileGender.OTHER:
					checkboxTypeToActivate = CheckboxType.Other;
					break;
				case UserProfileGender.PREFER_NOT:
					checkboxTypeToActivate = CheckboxType.PreferNot;
					break;
				default:
					XDebug.LogWarning($"Could not set initial value: {value}");
					return;
			}

			_skipNextCheck = true;
			_checkboxes[(int)checkboxTypeToActivate].isOn = true;
			DisableCheckboxesExcept(checkboxTypeToActivate);
			_skipNextCheck = false;
		}

		private void ProcessValueChange(CheckboxType checkboxType, bool newValue)
		{
			if(newValue/*== true*/ && !_skipNextCheck)
			{
				DisableCheckboxesExcept(checkboxType);
				var result = default(string);

				switch (checkboxType)
				{
					case CheckboxType.Male:
						result = UserProfileGender.MALE;
						break;
					case CheckboxType.Female:
						result = UserProfileGender.FEMALE;
						break;
					case CheckboxType.Other:
						result = UserProfileGender.OTHER;
						break;
					case CheckboxType.PreferNot:
						result = UserProfileGender.PREFER_NOT;
						break;
					default:
						XDebug.LogError($"Unexpected checkboxType: {checkboxType.ToString()}");
						break;
				}

				base.RaiseEntryEdited(result);
			}
		}

		private void DisableCheckboxesExcept(CheckboxType activeCheckbox)
		{
			var activeCheckboxIndex = (int)activeCheckbox;

			for (int i = 0; i < _checkboxes.Length; i++)
				if (i != activeCheckboxIndex && _checkboxes[i].isOn)
					_checkboxes[i].isOn = false;
		}

		private enum CheckboxType
		{
			Male, Female, Other, PreferNot
		}
	}
}
