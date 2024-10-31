using System;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string PROJECT_ID_LABEL = "Project ID";
		private const string PROJECT_ID_TOOLTIP = "Can be found in Publisher Account beside the name of your project.";

		private const string LOGIN_ID_LABEL = "Login ID";
		private const string LOGIN_ID_TOOLTIP = "Can be found in Publisher Account. " +
		                                        "To get it, go to the \"Login -> Dashbord\" section and click \"Copy ID\" near the name of the Login project. " +
		                                        "If you don't use Xsolla Login, leave this field blank.";

		private const string IN_APP_BROWSER_LABEL = "Enable In-App Browser";
		private const string IN_APP_BROWSER_TOOLTIP = "If the option is enabled, the in-app browser is used to open Pay Station or a window for social login.";

		private bool GeneralSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("General", GroupHeaderStyle);

			var changed = false;

			var projectId = EditorGUILayout.TextField(new GUIContent(PROJECT_ID_LABEL, PROJECT_ID_TOOLTIP), XsollaSettings.StoreProjectId);
			if (projectId != XsollaSettings.StoreProjectId)
			{
				XsollaSettings.StoreProjectId = projectId;
				changed = true;
			}

			var regex = new Regex(@"^[1-9]\d*$");
			if (!regex.IsMatch(projectId))
				DrawErrorBox("Project ID has incorrect value");

			var loginId = EditorGUILayout.TextField(new GUIContent(LOGIN_ID_LABEL, LOGIN_ID_TOOLTIP), XsollaSettings.LoginId);
			if (loginId != XsollaSettings.LoginId)
			{
				XsollaSettings.LoginId = loginId;
				changed = true;
			}

			regex = new Regex(@"^[^\s].+[^\s]$");
			if (!regex.IsMatch(loginId) || !Guid.TryParse(loginId, out _))
				DrawErrorBox("Login ID has incorrect value");

			var inAppBrowserEnabled = EditorGUILayout.Toggle(new GUIContent(IN_APP_BROWSER_LABEL, IN_APP_BROWSER_TOOLTIP), XsollaSettings.InAppBrowserEnabled);
			if (inAppBrowserEnabled != XsollaSettings.InAppBrowserEnabled)
			{
				XsollaSettings.InAppBrowserEnabled = inAppBrowserEnabled;
				changed = true;
			}

			EditorGUILayout.EndVertical();
			return changed;
		}
	}
}