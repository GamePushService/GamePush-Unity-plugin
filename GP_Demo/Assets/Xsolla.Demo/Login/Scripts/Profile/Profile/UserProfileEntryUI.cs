using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class UserProfileEntryUI : MonoBehaviour
	{
		[SerializeField] private GameObject SetStateObjects;
		[SerializeField] private GameObject UnsetStateObjects;
		[SerializeField] private GameObject EditStateObjects;

		[SerializeField] private Text CurrentValueText;

		[SerializeField] private SimpleButton[] EditButtons;
		[SerializeField] private UserProfileEntryEditor EntryEditor;
		[SerializeField] private BaseUserProfileValueConverter ValueConverter;

		private UserProfileEntryType _entryType;
		private EntryState _currentState;

		private string CurrentValue
		{
			get => CurrentValueText.text;
			set
			{
				CurrentValueText.text = value;
				SetState(value);
			}
		}

		public static event Action<UserProfileEntryUI, UserProfileEntryType, string, string> UserEntryEdited;
		public static event Action<UserProfileEntryUI> UserEntryEditStarted;

		public static void RaiseUserEntryEditStarted(UserProfileEntryUI entryUI)
		{
			UserEntryEditStarted?.Invoke(entryUI);
		}

		public void InitializeEntry(UserProfileEntryType entryType, string value)
		{
			_entryType = entryType;

			if (ValueConverter != null && !string.IsNullOrEmpty(value))
				value = ValueConverter.Convert(value);

			if (EntryEditor)
				EntryEditor.SetInitial(value);

			CurrentValue = value;
		}

		private void Awake()
		{
			foreach (var button in EditButtons)
				button.onClick += StartEdit;

			UserEntryEditStarted += OnEntryEdit;

			if (EntryEditor)
				EntryEditor.UserProfileEntryEdited += OnEntryEdited;
		}

		private void StartEdit()
		{
			SetState(EntryState.Edit);
			UserEntryEditStarted?.Invoke(this);
		}

		private void OnEntryEdit(UserProfileEntryUI editingEntry)
		{
			if (_currentState == EntryState.Edit && (editingEntry == null || !editingEntry.Equals(this)))
			{
				SetState(CurrentValueText.text);
			}
		}

		private void SetState(string entryValue)
		{
			if (!string.IsNullOrEmpty(entryValue))
				SetState(EntryState.Set);
			else
			{
				if (UnsetStateObjects)
					SetState(EntryState.Unset);
				else
					SetState(EntryState.Edit);
			}
		}

		private void SetState(EntryState entryState)
		{
			if (SetStateObjects)
				SetActive(SetStateObjects, entryState == EntryState.Set);
			if (UnsetStateObjects)
				SetActive(UnsetStateObjects, entryState == EntryState.Unset);
			if (EditStateObjects)
				SetActive(EditStateObjects, entryState == EntryState.Edit);

			_currentState = entryState;
		}

		private void SetActive(GameObject gameObject, bool targetState)
		{
			if (targetState != gameObject.activeSelf)
				gameObject.SetActive(targetState);
		}

		private void OnEntryEdited(string newValue)
		{
			var isConverterUsed = ValueConverter != null;

			if (isConverterUsed && !string.IsNullOrEmpty(newValue))
				newValue = ValueConverter.ConvertBack(newValue);

			var isValueChanged = isConverterUsed
				? newValue == ValueConverter.ConvertBack(CurrentValue)
				: newValue == CurrentValue;

			if (isValueChanged)
			{
				CurrentValue = isConverterUsed ? ValueConverter.Convert(newValue) : newValue;
			}
			else
			{
				var oldValue = CurrentValue;
				CurrentValue = isConverterUsed ? ValueConverter.Convert(newValue) : newValue;

				UserEntryEdited?.Invoke(this, _entryType, oldValue, newValue);
			}
		}

		private enum EntryState
		{
			Set,
			Unset,
			Edit
		}
	}
}