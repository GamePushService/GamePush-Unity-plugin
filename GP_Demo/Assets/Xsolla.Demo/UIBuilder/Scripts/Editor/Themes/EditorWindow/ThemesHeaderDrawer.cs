using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ThemesHeaderDrawer
	{
		private readonly ThemeEditorWindow Window;

		public void Draw()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginHorizontal(GUILayout.MinWidth(Window.PropLabelsWidth));
			Window.IsAutoRefresh = EditorGUILayout.ToggleLeft("Auto refresh", Window.IsAutoRefresh);
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			GUI.enabled = !Window.IsAutoRefresh;
			if (GUILayout.Button("Refresh"))
			{
				Window.Refresh(true);
			}
			GUI.enabled = true;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();

			EditorGUILayout.BeginHorizontal(GUILayout.Width(Window.PropLabelsWidth));
			EditorGUILayout.LabelField("Themes");
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.BeginHorizontal();
			var guiColor = GUI.color;

			var themes = ThemesLibrary.Themes;
			var current = ThemesLibrary.Current;

			foreach (var theme in themes)
			{
				GUI.color = theme == current ? Color.yellow : guiColor;

				if (GUILayout.Button(theme.Name))
				{
					ThemesLibrary.Current = theme;
					Window.Refresh(true);
				}
			}

			GUI.color = guiColor;
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndHorizontal();
			EditorGUILayout.Space();
		}

		public ThemesHeaderDrawer(ThemeEditorWindow window)
		{
			Window = window;
		}
	}
}