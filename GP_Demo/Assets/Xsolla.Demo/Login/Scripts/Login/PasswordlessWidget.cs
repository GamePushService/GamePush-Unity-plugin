using System;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
    public class PasswordlessWidget : MonoBehaviour, ICodeRequester
	{
		[SerializeField] private GameObject ChoiceState = default;
		[SerializeField] private GameObject PhoneState = default;
		[SerializeField] private GameObject EmailState = default;
		[SerializeField] private GameObject CodeState = default;
		[SerializeField] private SimpleButton BackButton = default;
		[Space]
		[SerializeField] private SimpleButton PhoneChoiceButton = default;
		[SerializeField] private SimpleButton EmailChoiceButton = default;
		[Space]
		[SerializeField] private LoginPageEnterController LoginPageEnterController = default;
		[SerializeField] private SimpleButton LoginWidgetAuthButton = default;
		[Space]
		[SerializeField] private InputField PhoneInputField = default;
		[SerializeField] private UserProfileEntryPhoneValueConverter PhoneConverter = default;
		[SerializeField] private SimpleTextButtonDisableable PhoneSendButton = default;
		[Space]
		[SerializeField] private InputField EmailInputField = default;
		[SerializeField] private SimpleTextButtonDisableable EmailSendButton = default;
		[Space]
		[SerializeField] private InputField CodeInputField = default;
		[SerializeField] private SimpleTextButtonDisableable CodeSendButton = default;
		[SerializeField] private SimpleButton ResendButton = default;
		[SerializeField] private CountdownTimer Timer = default;

		private AuthType _currentAuthType = AuthType.None;

		public event Action<string> OnPhoneAccessRequest;
		public event Action<string> OnEmailAccessRequest;

		private void Awake()
		{
			SetState(ChoiceState);
			BackButton.onClick += () => SetState(ChoiceState);

			PhoneChoiceButton.onClick += OnPhoneChoice;
			PhoneInputField.onValueChanged.AddListener(OnPhoneInput);
			PhoneSendButton.onClick += OnPhoneRequest;

			EmailChoiceButton.onClick += OnEmailChoice;
			EmailInputField.onValueChanged.AddListener(OnEmailInput);
			EmailSendButton.onClick += OnEmailRequest;

			CodeInputField.onValueChanged.AddListener(OnCodeInput);
			ResendButton.onClick += OnResendButton;
			Timer.TimeIsUp += OnTimerEnd;

			LoginWidgetAuthButton.onClick += LoginPageEnterController.RunWidgetAuth;
		}

		public void RequestCode(Action<string> onCode)
		{
			SetState(CodeState);
			CodeInputField.text = string.Empty;
			CodeSendButton.Disable();
			CodeSendButton.onClick = () => onCode?.Invoke(CodeInputField.text);
			Timer.ResetTimer();
		}

		public void RaiseOnError(Error error)
		{
			if (error.ErrorType == ErrorType.InvalidAuthorizationCode)
				StoreDemoPopup.ShowError(error);
			else
				StoreDemoPopup.ShowError(error, CloseWidget);
		}

		private void OnPhoneChoice()
		{
			SetState(PhoneState);
			OnPhoneInput(PhoneInputField.text);
		}

		private void OnPhoneInput(string text)
		{
			var phone = PhoneConverter.ConvertBack(text);

			if (IsPhoneValid(phone))
				PhoneSendButton.Enable();
			else
				PhoneSendButton.Disable();
		}

		private bool IsPhoneValid(string phone)
		{
			if (string.IsNullOrEmpty(phone))
				return false;

			var regex = new Regex("^\\+(\\d){5,25}$");
			return regex.IsMatch(phone);
		}

		private void OnPhoneRequest()
		{
			var phone = PhoneConverter.ConvertBack(PhoneInputField.text);
			XDebug.Log($"PasswordlessWidget: phone is '{phone}'");
			_currentAuthType = AuthType.Phone;
			OnPhoneAccessRequest?.Invoke(phone);
		}

		private void OnEmailChoice()
		{
			SetState(EmailState);
			OnEmailInput(EmailInputField.text);
		}

		private void OnEmailInput(string text)
		{
			if (text.Length > 1 && text.Length < 255)
				EmailSendButton.Enable();
			else
				EmailSendButton.Disable();
		}

		private void OnEmailRequest()
		{
			var email = EmailInputField.text;
			XDebug.Log($"PasswordlessWidget: email is '{email}'");
			_currentAuthType = AuthType.Email;
			OnEmailAccessRequest?.Invoke(email);
		}

		private void OnCodeInput(string code)
		{
			if (code.Length >= 1)
				CodeSendButton.Enable();
			else
				CodeSendButton.Disable();
		}

		private void OnResendButton()
		{
			switch (_currentAuthType)
			{
				case AuthType.Phone:
					OnPhoneRequest();
					break;
				case AuthType.Email:
					OnEmailRequest();
					break;
				default:
					XDebug.LogError($"PasswordlessWidget.OnResendButton: Unexpected auth type '{_currentAuthType}'");
					break;
			}

			Timer.ResetTimer();
		}

		private void OnTimerEnd()
		{
			StoreDemoPopup.ShowWarning(
				error: new Error(errorMessage: "Code expired"),
				buttonCallback: OnResendButton);
		}

		private void CloseWidget()
		{
			if (BackButton)
				BackButton.onClick?.Invoke();
		}

		private void SetState(GameObject targetState)
		{
			Timer.StopTimer();

			var allStates = new GameObject[] { ChoiceState, PhoneState, EmailState, CodeState };
			foreach (var state in allStates)
				SetActive(state, state.Equals(targetState));
		}

		private void SetActive(GameObject stateObject, bool targetState)
		{
			if (stateObject.activeSelf != targetState)
				stateObject.SetActive(targetState);
		}

		private enum AuthType : byte
		{
			None, Phone, Email
		}
	}
}
