using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class AttributeItemUI : MonoBehaviour
	{
		[SerializeField] bool _isReadOnly = default;
		[SerializeField] InputField KeyInputField = default;
		[SerializeField] InputField ValueInputField = default;
		[SerializeField] Text KeyText = default;
		[SerializeField] Text ValueText = default;
		[SerializeField] SimpleButton RemoveButton = default;

		private string _key;
		private string _value;

		public static Action<AttributeItemUI, string, string> OnKeyChanged;
		public static Action<AttributeItemUI, string, string> OnValueChanged;
		public static Action<AttributeItemUI> OnRemoveRequest;

		public bool IsReadOnly
		{
			get => _isReadOnly;
			set
			{
				if (value == _isReadOnly)
					return;

				_isReadOnly = value;
				SetAccessibility(value);
			}
		}

		public string Key
		{
			get => _key;
			set
			{
				_key = value;
				KeyText.text = value;

				if (KeyInputField)
					KeyInputField.text = value;
			}
		}

		public string Value
		{
			get => _value;
			set
			{
				_value = value;
				ValueText.text = value;

				if (ValueInputField)
					ValueInputField.text = value;
			}
		}

		public bool IsPublic	{ get; set; }
		public bool IsNew		{ get; set; }
		public bool IsModified	{ get; set; }

		private void Awake()
		{
			SetAccessibility(_isReadOnly);

			KeyInputField?.onEndEdit.AddListener(SetNewKey);
			ValueInputField?.onEndEdit.AddListener(SetNewValue);

			if (RemoveButton)
				RemoveButton.onClick += () => OnRemoveRequest?.Invoke(this);
		}

		private void SetAccessibility(bool isReadOnly)
		{
			KeyInputField?.gameObject.SetActive(!isReadOnly);
			ValueInputField?.gameObject.SetActive(!isReadOnly);

			KeyText.gameObject.SetActive(isReadOnly);
			ValueText.gameObject.SetActive(isReadOnly);

			RemoveButton?.gameObject.SetActive(!isReadOnly);
		}

		private void SetNewKey(string newKey)
		{
			if(newKey == Key)
				return;

			var oldKey = Key;
			Key = newKey;
			OnKeyChanged?.Invoke(this, oldKey, newKey);
		}

		private void SetNewValue(string newValue)
		{
			if(newValue == Value)
				return;

			var oldValue = Value;
			Value = newValue;
			OnValueChanged?.Invoke(this, oldValue, newValue);
		}
	}
}
