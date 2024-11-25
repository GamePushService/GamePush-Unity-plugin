using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string LOG_LEVEL_LABEL = "Print to Console";
		private const string LOG_LEVEL_TOOLTIP = "Types of SDK messages to display in logs.";

		private readonly string[] LogLevelOptions = {
			"Info, Warnings and Errors",
			"Warnings and Errors",
			"Only Errors"
		};

		private bool EditorSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Debugging", GroupHeaderStyle);

			XsollaSettings.LogLevel = (LogLevel) EditorGUILayout.Popup(new GUIContent(LOG_LEVEL_LABEL, LOG_LEVEL_TOOLTIP), (int) XsollaSettings.LogLevel, LogLevelOptions);

			EditorGUILayout.EndVertical();
			return false;
		}
	}
}