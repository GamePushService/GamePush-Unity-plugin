using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace Xsolla.Core.Editor.AutoFillSettings
{
	public class AutoFillUI : EditorWindow
	{
		private const string MESSAGE_TEMPLATE = "<color=red><i>{0}</i></color>";

		private string _login;
		private string _password;
		private bool   _rememberMe;
		private string _confirmationCode;
		private string _message;

		private ToolStep _currentStep = ToolStep.Undefined;
		private bool _isCodeRequired = false;

		private readonly float _defaultProgressIncrimination;
		private float _currentProgress;

		private List<string[]> _settings;
		private readonly int _settingsCount;
		private readonly Dictionary<SettingsType,string> _settingsLabels;
		private readonly string[] _empty = new string[]{""};
		private readonly int[] _selectionCur;
		private readonly int[] _selectionNew;

		private readonly GUIStyle _labelStyle = EditorStyles.boldLabel;
		private readonly GUILayoutOption _buttonStyle = GUILayout.Height(27);
		private GUIStyle _errorStyle;

		public event Action<string,string,bool> OnLoginButton;
		public event Action<string> OnCodeButton;
		public event Action<int[],int> OnSelectionChanged;
		public event Action<int[]> OnApplyButton;
		public event Action OnUiClosed;

		private AutoFillUI()
		{
			base.titleContent = new GUIContent("Xsolla Auto Fill Settings");
			base.minSize = new Vector2(350,500);

			_settingsLabels = new Dictionary<SettingsType,string>()
			{
				{SettingsType.MerchantID, "Merchant ID"},
				{SettingsType.ProjectID,  "Project ID"},
				{SettingsType.LoginID,    "Login ID"},
				{SettingsType.OAuthID,    "OAuth Client ID"},
				{SettingsType.RedirectUrl,"Callback URL"}
			};

			_settingsCount = Enum.GetValues(typeof(SettingsType)).Length;
			_selectionCur = new int[_settingsCount];
			_selectionNew = new int[_settingsCount];
			_defaultProgressIncrimination = 1f / (_settingsCount+1);
			_currentProgress = -_defaultProgressIncrimination;
		}

		private void Awake()
		{
			//Can only be set here or in OnEnable
			_errorStyle = new GUIStyle(){richText=true};
		}

		private void OnDestroy()
		{
			OnUiClosed?.Invoke();
		}

		public static AutoFillUI OpenUI(string login = null, string password = null, bool? rememberMe = null)
		{
			var ui = GetWindow<AutoFillUI>();

			if (login != null)
				ui._login = login;
			if (password != null)
				ui._password = password;
			if (rememberMe.HasValue)
				ui._rememberMe = rememberMe.Value;
			
			return ui;
		}

		public void SetUiStep(ToolStep toolStep)
		{
			if (toolStep == ToolStep.Code)
				_isCodeRequired = true;

			_currentStep = toolStep;
			this.Repaint();
		}

		public void UpdateSettings(List<string[]> newSettings, int[] newSelection)
		{
			_settings = newSettings;
			for (int i = 0; i < _settingsCount; i++) {
				_selectionCur[i] = newSelection[i];
				_selectionNew[i] = newSelection[i];
			}
		}

		public void ShowMessage(string message)
		{
			if (!string.IsNullOrEmpty(message))
				_message = string.Format(MESSAGE_TEMPLATE,message);
			else
				_message = null;

			this.Repaint();
		}

		public void ClearMessage()
		{
			ShowMessage(null);
		}

		public void ShowProgress(float? progress = null)
		{
			if (!progress.HasValue)
				progress = _currentProgress + _defaultProgressIncrimination;

			if (progress < 0f)
				progress = 0f;
			else if (progress > 1f)
				progress = 1f;

			_currentProgress = progress.Value;
			this.Repaint();
		}

		public void ClearProgress()
		{
			_currentProgress = -_defaultProgressIncrimination;
			this.Repaint();
		}

		private void OnGUI()
		{
			//Error message on top of the window
			if (!string.IsNullOrEmpty(_message)) {
				GUILayout.Label(_message,_errorStyle);
				Space();
			}

			//Authorization
			GUILayout.Label("Autorization",_labelStyle);
			GuiLockStart(ToolStep.Auth);
			_login = EditorGUILayout.TextField("Login", _login);
			_password = EditorGUILayout.PasswordField("Password", _password);
			_rememberMe = EditorGUILayout.Toggle("Remember me", _rememberMe);
			if (GUILayout.Button("Log In",_buttonStyle)) {RaiseOnLogin();}
			Space();
			GuiLockEnd();

			//Confirmation code
			if (_isCodeRequired) {
				GuiLockStart(ToolStep.Code);
				_confirmationCode = EditorGUILayout.TextField ("Confirmation code", _confirmationCode);
				if (GUILayout.Button("Confirm",_buttonStyle)) {RaiseOnCode();}
				Space();
				GuiLockEnd();
			}

			//Settings
			GUILayout.Label ("Publisher Account", _labelStyle);
			GuiLockStart(ToolStep.Settings);
			for (int i = 0; i < _settingsCount; i++) {
				var label = _settingsLabels.TryGetValue((SettingsType)i, out string value) ? value : "UNKNOWN";
				_selectionNew[i] = EditorGUILayout.Popup(new GUIContent(label),_selectionCur[i],GetSettingsList(i));
			}
			CheckSelectionChange();
			if (GUILayout.Button("Apply settings",_buttonStyle)) {RaiseOnApply();}
			GuiLockEnd();

			//Progress bar
			if (_currentProgress >= 0f) {
				var verticalPos = _isCodeRequired ? (176+65) : 176;
				EditorGUI.ProgressBar(
					position: new Rect(x:30, y:verticalPos, width:(position.width - 60), height:30),
					value: _currentProgress,
					text: "Loading..."
				);
			}
		}

		private string[] GetSettingsList(int index)
		{
			if (_settings == null || _settings.Count == 0 || index >= _settings.Count)
				return _empty;

			var result = _settings[index];
			if (result != null)
				return result;
			else
				return _empty;
		}

		private void CheckSelectionChange()
		{
			var changeIndex = -1;
			for (int i = 0; i < _settingsCount; i++) {
				if (_selectionNew[i] != _selectionCur[i]) {
					_selectionCur[i] = _selectionNew[i];
					changeIndex = i;
				}
			}

			if (changeIndex == -1)
				return;
			
			if (OnSelectionChanged != null)
				OnSelectionChanged.Invoke(_selectionCur,changeIndex);
			else
				ShowMessage($"DEBUG: Selection changed [{string.Join(",",_selectionCur)}] index:{changeIndex}");
		}

		private void RaiseOnLogin()
		{
			if (OnLoginButton != null)
				OnLoginButton.Invoke(_login,_password,_rememberMe);
			else
			{
				ShowMessage($"DEBUG: Login sent [{_login},{_password},{_rememberMe}]");
				_isCodeRequired = true;
				_currentStep = ToolStep.Code;
			}
		}

		private void RaiseOnCode()
		{
			if (OnCodeButton != null)
				OnCodeButton.Invoke(_confirmationCode);
			else
			{
				ShowMessage($"DEBUG: Code sent {_confirmationCode}");
				_currentStep = ToolStep.Settings;
			}
		}

		private void RaiseOnApply()
		{
			if (OnApplyButton != null)
				OnApplyButton.Invoke(_selectionCur);
			else
				ShowMessage($"DEBUG: Settings applied [{string.Join(",",_selectionCur)}]");
		}

		private void Space()
		{
			EditorGUILayout.Space(15,true);
		}

		private void GuiLockStart(ToolStep toolStep)
		{
			if (toolStep != _currentStep)
				GUI.enabled = false;
		}

		private void GuiLockEnd()
		{
			GUI.enabled = true;
		}
	}
}