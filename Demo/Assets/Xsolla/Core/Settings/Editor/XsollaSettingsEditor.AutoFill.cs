using UnityEditor;
using UnityEngine;
using Xsolla.Core.Editor.AutoFillSettings;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private bool AutoFillSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Auto fill settings", GroupHeaderStyle);

			if (GUILayout.Button("Fill settings by PA", GUILayout.Height(30)))
				AutoFillTool.OpenAutoFillTool();

			EditorGUILayout.EndVertical();
			return false;
		}
	}
}