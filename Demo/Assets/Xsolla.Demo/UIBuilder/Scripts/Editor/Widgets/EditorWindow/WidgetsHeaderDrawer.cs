using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class WidgetsHeaderDrawer
	{
		private readonly WidgetsEditorWindow Window;

		public void Draw()
		{
			EditorGUILayout.BeginHorizontal();

			Window.IsAutoRefresh = EditorGUILayout.ToggleLeft("Auto refresh", Window.IsAutoRefresh);

			GUI.enabled = !Window.IsAutoRefresh;
			if (GUILayout.Button("Refresh"))
			{
				Window.Refresh(true);
			}
			GUI.enabled = true;

			EditorGUILayout.EndHorizontal();
		}

		public WidgetsHeaderDrawer(WidgetsEditorWindow window)
		{
			Window = window;
		}
	}
}