// #define AUTOFILLTOOL_DEBUG
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core.Editor.AutoFillSettings
{
	using Node = SettingsTree.TreeNode;

	public class AutoFillTool
	{
		private const string LOGIN_KEY = "XsollaAutoFillTool_login";
		private const string PASSWORD_KEY = "XsollaAutoFillTool_password";
		private static AutoFillTool _instance;
		private static bool IsAlive => _instance != null;
		private AutoFillUI _window;
		private AutoFillRequester _requester;
		private SettingsTree _tree;
		private ToolStep _currentStep;
		private SettingsType _currentProgress = (SettingsType)(-1);

		private string _currentToken;
		private string _currentChallengeID;
		private Dictionary<int,string[]> _oauthToRedirectMap;

		public static void OpenAutoFillTool()
		{
			if (_instance != null)
				_instance.OpenExisting();
			else
			{
				_instance = new AutoFillTool();
				_instance.OpenNew();
			}
		}

		private void OpenExisting()
		{
			AutoFillUI.OpenUI();
		}

		private void OpenNew()
		{
			if (_window != null) {
				OpenExisting();
				return;
			}

			if (EditorPrefs.HasKey(LOGIN_KEY))
				_window = AutoFillUI.OpenUI(EditorPrefs.GetString(LOGIN_KEY),EditorPrefs.GetString(PASSWORD_KEY),rememberMe: true);
			else
				_window = AutoFillUI.OpenUI();

			_window.OnLoginButton += ProcessLoginRequest;
			_window.OnCodeButton  += ProcessConfirmationCode;
			_window.OnSelectionChanged += ProcessSelectionChanged;
			_window.OnApplyButton += ApplySelection;
			_window.OnUiClosed += OnUiClosed;

			_requester = new AutoFillRequester();
			_tree = new SettingsTree();
			_oauthToRedirectMap = new Dictionary<int,string[]>();

			SetToolStep(ToolStep.Auth);
		}

		private void OnUiClosed()
		{
			_window.OnLoginButton -= ProcessLoginRequest;
			_window.OnCodeButton  -= ProcessConfirmationCode;
			_window.OnSelectionChanged -= ProcessSelectionChanged;
			_window.OnApplyButton -= ApplySelection;
			_window.OnUiClosed -= OnUiClosed;

			_window = null;
			_requester?.Dispose();
			_requester = null;
			_tree = null;
			_oauthToRedirectMap.Clear();
			_oauthToRedirectMap = null;
			_instance = null;

			XDebug.Log("AutoFullTool closed");
		}

		private void SetToolStep(ToolStep newStep)
		{
			_currentStep = newStep;
			_window?.SetUiStep(newStep);
		}

		private void ShowError(string errorMessage)
		{
			XDebug.LogError(errorMessage);
			_window?.ShowMessage(errorMessage);
			_window?.ClearProgress();
		}

		private void ShowError(Error error)
		{
			var errorMessage = error?.errorMessage ?? "UNKNOWN ERROR";
			ShowError(errorMessage);
		}

		private void ProcessLoginRequest(string login, string password, bool rememberMe)
		{
			if (_currentStep != ToolStep.Auth) {
				ShowError($"Error: ProcessLoginRequest raised on {_currentStep}");
				return;
			}
			if (string.IsNullOrEmpty(login)) {
				ShowError("Error: login is empty");
				return;
			}
			if (string.IsNullOrEmpty(password)) {
				ShowError("Error: password is empty");
				return;
			}

			_window?.ClearMessage();
			_window?.ShowProgress(0f);
			SetToolStep(ToolStep.Wait);

			if (rememberMe)
			{
				EditorPrefs.SetString(LOGIN_KEY,login);
				EditorPrefs.SetString(PASSWORD_KEY,password);
			}
			else if (EditorPrefs.HasKey(LOGIN_KEY))
			{
				EditorPrefs.DeleteKey(LOGIN_KEY);
				EditorPrefs.DeleteKey(PASSWORD_KEY);
			}

			Action<string> onTokenGet = token =>
			{
				if (!IsAlive) {return;}
				_currentToken = token;
				_window?.ShowProgress();
				GenerateAndShowSettingsTree();
			};

			Action<string> onConfirmationNeeded = challengeID =>
			{
				if (!IsAlive) {return;}
				_currentChallengeID = challengeID;
				_window?.ClearProgress();
				SetToolStep(ToolStep.Code);
			};

			Action<Error> onError = error =>
			{
				if (!IsAlive) {return;}
				var errorMessage = error?.errorMessage ?? "UNKNOWN ERROR";
				ShowError(errorMessage);
				SetToolStep(ToolStep.Auth);
			};

			XDebug.Log("Sending auth request. Please wait...");
			_requester.RequestToken(login,password,onTokenGet,onConfirmationNeeded,onError);
		}

		private void ProcessConfirmationCode(string code)
		{
			if (_currentStep != ToolStep.Code) {
				ShowError($"Error: ProcessConfirmationCode raised on {_currentStep}");
				return;
			}
			if (string.IsNullOrEmpty(_currentChallengeID)) {
				ShowError("Error: challenge ID is empty");
				return;
			}
			if (string.IsNullOrEmpty(code)) {
				ShowError("Error: code is empty");
				return;
			}

			_window?.ClearMessage();
			_window?.ShowProgress(0f);
			SetToolStep(ToolStep.Wait);

			Action<string> onTokenGet = token =>
			{
				if (!IsAlive) {return;}
				_currentToken = token;
				_window?.ShowProgress();
				GenerateAndShowSettingsTree();
			};

			Action<Error> onError = error =>
			{
				if (!IsAlive) {return;}
				var errorMessage = error?.errorMessage ?? "UNKNOWN ERROR";
				ShowError(errorMessage);
				SetToolStep(ToolStep.Code);
			};

			XDebug.Log("Sending confirmation code. Please wait...");
			_requester.ConfirmByCode(_currentChallengeID,code,onTokenGet,onError);
		}

		private void GenerateAndShowSettingsTree()
		{
			if (string.IsNullOrEmpty(_currentToken)) {
				ShowError("Error: GenerateAndShowSettingsTree: Token is empty.");
				return;
			}

			XDebug.Log("Requesting PA values. Please wait...");
			var defaultSelection = new int[Enum.GetValues(typeof(SettingsType)).Length];

			Action finalCallback = () =>
			{
				_window.ClearProgress();
				_window.UpdateSettings(_tree.GenerateTableView(defaultSelection),defaultSelection);
				SetToolStep(ToolStep.Settings);
			};

			RecursiveAddNode(_tree.Root,SettingsType.MerchantID,finalCallback);
		}

		private void RecursiveAddNode(Node curNode, SettingsType curLevel, Action finalCallback)
		{
			if (curLevel > _currentProgress) {
				_currentProgress = curLevel;
				_window?.ShowProgress();
			}

			switch (curLevel)
			{
				case SettingsType.MerchantID:
					XDebug.Log("Requesting MerchantIDs...");
					_requester.RequestMerchantIDs(_currentToken,
						onSuccess: containers =>
						{
							if (TryRepackContainers(containers,curNode,out Node[] childNodes))
								foreach (var child in childNodes)
									RecursiveAddNode(child,SettingsType.ProjectID,finalCallback);
							else
								finalCallback?.Invoke();
						},
						onError: ShowError);
					break;
				case SettingsType.ProjectID:
					XDebug.Log($"Requesting ProjectIDs for Merchant {curNode.value}...");
					_requester.RequestProjectIDs(_currentToken,
						merchantID: (int)curNode.value,
						onSuccess: containers =>
						{
							if (TryRepackContainers(containers,curNode,out Node[] childNodes))
								foreach (var child in childNodes)
									RecursiveAddNode(child,SettingsType.LoginID,finalCallback);
							else
								finalCallback?.Invoke();
						},
						onError: ShowError);
					break;
				case SettingsType.LoginID:
					XDebug.Log($"Requesting LoginIDs for Project {curNode.value}...");
					_requester.RequestLoginIDs(_currentToken,
						merchantID: (int)curNode.parent.value,
						projectID: (int)curNode.value,
						onSuccess: containers =>
						{
							if (TryRepackContainers(containers,curNode,out Node[] childNodes))
								foreach (var child in childNodes)
									RecursiveAddNode(child,SettingsType.OAuthID,finalCallback);
							else
								finalCallback?.Invoke();
						},
						onError: ShowError);
					break;
				case SettingsType.OAuthID:
					XDebug.Log($"Requesting OauthIDs for Login {curNode.value}...");
					_requester.RequestOAuthIDs(_currentToken,
						loginID: (string)curNode.value,
						onSuccess: containers =>
						{
							if (TryRepackContainers(containers,curNode,out Node[] childNodes))
								foreach (var child in childNodes)
									RecursiveAddNode(child,SettingsType.RedirectUrl,finalCallback);
							else
								finalCallback?.Invoke();
						},
						onError: ShowError);
					break;
				case SettingsType.RedirectUrl:
					XDebug.Log($"Assigning RedirectURIs for OAuth client {curNode.value}...");
					TryAssignRedirectUrls(OAuthID: (int)curNode.value, curNode, out Node[] _);
					finalCallback?.Invoke();
					break;
				default:
					XDebug.LogError($"Unexpected setting type {curLevel}");
					return;
			}
		}

		private bool TryRepackContainers(IntIdContainer[] containers, Node parentNode, out Node[] nodes)
		{
			if (containers == null || containers.Length == 0) {
				nodes = null;
				return false;
			}

			nodes = new Node[containers.Length];
			for (int i = 0; i < containers.Length; i++) {
				var cur = containers[i];
				nodes[i] = new Node(cur.id,$"{cur.name} ({cur.id})",parentNode);
			}

			parentNode.children = nodes;
			return true;
		}

		private bool TryRepackContainers(StringIdContainer[] containers, Node parentNode, out Node[] nodes)
		{
			if (containers == null || containers.Length == 0) {
				nodes = null;
				return false;
			}

			nodes = new Node[containers.Length];
			for (int i = 0; i < containers.Length; i++) {
				var cur = containers[i];
				nodes[i] = new Node(cur.id,$"{cur.name} ({cur.id})",parentNode);
			}

			parentNode.children = nodes;
			return true;
		}

		private bool TryRepackContainers(OAuthContainer[] containers, Node parentNode, out Node[] nodes)
		{
			if (containers == null || containers.Length == 0) {
				nodes = null;
				return false;
			}

			var result = new List<Node>(containers.Length);
			for (int i = 0; i < containers.Length; i++)
			{
				var cur = containers[i];
				if (!cur.is_public)
					continue;

				if (cur.redirect_uris != null && cur.redirect_uris.Length > 0)
					_oauthToRedirectMap[cur.id] = cur.redirect_uris;

				result.Add(new Node(cur.id,$"{cur.name} ({cur.id})",parentNode));
			}

			if (result.Count == 0) {
				nodes = null;
				return false;
			}

			nodes = result.ToArray();
			parentNode.children = nodes;
			return true;
		}

		private bool TryAssignRedirectUrls(int OAuthID, Node parentNode, out Node[] nodes)
		{
			if (!_oauthToRedirectMap.ContainsKey(OAuthID)) {
				nodes = null;
				return false;
			}

			var redirects = _oauthToRedirectMap[OAuthID];
			if (redirects == null || redirects.Length == 0) {
				nodes = null;
				return false;
			}

			nodes = new Node[redirects.Length];
			for (int i = 0; i < redirects.Length; i++)
				nodes[i] = new Node(redirects[i],redirects[i]?.Replace("/","\\"),parentNode);//Fix UI breaking because of '/' char

			parentNode.children = nodes;
			return true;
		}

		private void ProcessSelectionChanged(int[] selection, int changeIndex)
		{
			if (_currentStep != ToolStep.Settings)
				return;

			for (int i = (changeIndex+1); i < selection.Length; i++)
				selection[i] = 0;

			_window.UpdateSettings(_tree.GenerateTableView(selection),selection);
		}

		private void ApplySelection(int[] selection)
		{
			if (selection == null || selection.Length == 0) {
				ShowError($"Error: selection to apply is empty. Selection:'[{string.Join(",",_tree.GetValues(selection))}]'");
				return;
			}

			var expectedLength = Enum.GetValues(typeof(SettingsType)).Length;
			if (selection.Length != expectedLength) {
				ShowError($"Error: inconsistent selection. Lenght:'{selection.Length}'. Expected:'{expectedLength}'");
				return;
			}

			int? projectID = null;
			string loginID = null;
			int? OAuthID = null;
			string redirectURL = null;

			var values = _tree.GetValues(selection);
			try
			{
				for (int i = 1; i < values.Length; i++) {
					var curValue = values[i];
					switch ((SettingsType)i)
					{
						case SettingsType.ProjectID:
							projectID = curValue;
							break;
						case SettingsType.LoginID:
							loginID = curValue;
							break;
						case SettingsType.OAuthID:
							OAuthID = curValue;
							break;
						case SettingsType.RedirectUrl:
							redirectURL = curValue;
							break;
					}
				}
			}
			catch (Exception exception)
			{
				ShowError(exception.Message);
				return;
			}

#if AUTOFILLTOOL_DEBUG
			XDebug.Error($"Selection applied: [{string.Join(",",projectID,loginID,OAuthID,redirectURL)}]");
#else
			XsollaSettings.StoreProjectId = projectID?.ToString() ?? string.Empty;
			XsollaSettings.LoginId        = loginID ?? string.Empty;
			XsollaSettings.OAuthClientId  = OAuthID ?? default(int);
			XsollaSettings.CallbackUrl    = redirectURL ?? string.Empty;

			XDebug.Log("Settings applied");
#endif
		}
	}
}