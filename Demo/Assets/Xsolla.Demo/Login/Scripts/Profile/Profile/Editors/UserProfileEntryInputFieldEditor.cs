using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(InputField))]
	public class UserProfileEntryInputFieldEditor : UserProfileEntryEditor
	{
		[SerializeField] string Template = default;
		[SerializeField] string AcceptedSymbolsRegex = default;

		private InputField _inputField;
		private Regex _inputRegex;
		private Regex _templateRegex;
		private bool _skipNextCheck; //Assigning a new modified value to _inputField.text during templating results in StackOverflowException, this is a workaround
		private List<string> _charsToTemplate;
		private int _lastInputLength;

		public override void SetInitial(string value)
		{
			_skipNextCheck = true;
			if (_inputField == null)
				_inputField = GetComponent<InputField>();
			if (_charsToTemplate != null)
				_charsToTemplate.Clear();

			_inputField.text = value;
			_lastInputLength = 0;

			_skipNextCheck = false;
		}

		private void Awake()
		{
			if (_inputField == null)
				_inputField = GetComponent<InputField>();

			_inputField.onEndEdit.AddListener(OnEndEdit);

			var isUseFilter = !string.IsNullOrEmpty(AcceptedSymbolsRegex);
			var isUseTemplate = !string.IsNullOrEmpty(Template);

			if (isUseFilter || isUseTemplate)
				_inputField.onValidateInput += ProcessInput;

			if (isUseFilter)
				_inputRegex = new Regex(AcceptedSymbolsRegex);

			if (isUseTemplate)
			{
				_charsToTemplate = new List<string>();
				_templateRegex = new Regex("_");
				_inputField.onValueChanged.AddListener(DecreaseCachedCharsIfNeeded);
			}
		}

		private char ProcessInput(string currentInput, int charIndex, char newChar)
		{
			if (_skipNextCheck)
				return newChar;

			if (_inputRegex != null && !_inputRegex.IsMatch($"{newChar}"))
				return '\0';

			if (_templateRegex != null)
			{
				_charsToTemplate.Add($"{newChar}");
				string templatedInput = Template;

				for (int index = 0; index < _charsToTemplate.Count; index++)
				{
					if (templatedInput.IndexOf('_') >= 0)
						templatedInput = _templateRegex.Replace(input: templatedInput, replacement: _charsToTemplate[index], count: 1);
					else
						templatedInput += _charsToTemplate[index];
				}

				_skipNextCheck = true;
				_inputField.text = templatedInput;
				_skipNextCheck = false;

				int caretPosIndex = templatedInput.IndexOf('_');
				if(caretPosIndex > 0)
					_inputField.caretPosition = caretPosIndex;

				return '\0';
			}

			return newChar;
		}

		private void DecreaseCachedCharsIfNeeded(string newInputFieldValue)
		{
			if (newInputFieldValue.Length < _lastInputLength)
			{
				int remainingAcceptedChars = 0;

				for (Match m = _inputRegex.Match(newInputFieldValue); m.Success; m = m.NextMatch())
					remainingAcceptedChars++;

				while (remainingAcceptedChars < _charsToTemplate.Count)
					_charsToTemplate.RemoveAt(_charsToTemplate.Count - 1);
			}

			_lastInputLength = newInputFieldValue.Length;
		}

		private void OnEndEdit(string value)
		{
			_charsToTemplate?.Clear();

			if (value != Template)
				base.RaiseEntryEdited(value);
			else
				base.RaiseEntryEdited(string.Empty);
		}

		private void OnEnable()
		{
			_inputField.Select();
		}
	}
}
