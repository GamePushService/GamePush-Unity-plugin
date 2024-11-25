using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ThemeEditorWindow : EditorWindow
	{
		private readonly ThemesHeaderDrawer HeaderDrawer;

		private readonly ThemesDrawer ThemesDrawer;

		private readonly ColorsDrawer ColorsDrawer;

		private readonly SpritesDrawer SpritesDrawer;

		private readonly FontsDrawer FontsDrawer;

		private Vector2 ScrollPosition { get; set; }

		private const string _isAutoRefreshKey = "ui_themes_is_auto_refresh";

		public bool IsAutoRefresh
		{
			get
			{
				if (!EditorPrefs.HasKey(_isAutoRefreshKey))
				{
					EditorPrefs.SetBool(_isAutoRefreshKey, true);
				}

				return EditorPrefs.GetBool(_isAutoRefreshKey);
			}
			set => EditorPrefs.SetBool(_isAutoRefreshKey, value);
		}

		public float PropLabelsWidth { get; private set; }

		private void OnGUI()
		{
			var themes = ThemesLibrary.Themes;
			PropLabelsWidth = position.width / (themes.Count + 1);

			EditorGUIUtility.labelWidth = 1f;
			
			EditorGUILayout.Space();
			HeaderDrawer.Draw();

			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space();
			ThemesDrawer.Draw();

			EditorGUILayout.Space();
			ColorsDrawer.Draw();

			EditorGUILayout.Space();
			SpritesDrawer.Draw();

			EditorGUILayout.Space();
			FontsDrawer.Draw();

			if (GUI.changed)
			{
				OnGuiChanged();
			}

			EditorGUI.EndChangeCheck();
			EditorGUILayout.EndScrollView();
			EditorGUIUtility.labelWidth = 0;
			EditorGUILayout.Space();
		}

		public void OnGuiChanged()
		{
			Refresh(IsAutoRefresh);
			Repaint();
		}

		public void Refresh(bool isRefreshAssets)
		{
			EditorUtility.SetDirty(ThemesLibrary.Instance);

			if (isRefreshAssets)
			{
				AssetDatabase.SaveAssets();
				AssetDatabase.Refresh();
			}
		}

		public void MarkAllMetaDirty()
		{
			var window = GetWindow<ThemeEditorWindow>();
			window.ThemesDrawer.IsMetaDirty = true;
			window.ColorsDrawer.IsMetaDirty = true;
			window.SpritesDrawer.IsMetaDirty = true;
		}

		[MenuItem("Window/Xsolla/UI Themes Editor", false, 1100)]
		private static void OpenWindow()
		{
			GetWindow<ThemeEditorWindow>("UI Themes Editor");
		}

		private ThemeEditorWindow()
		{
			HeaderDrawer = new ThemesHeaderDrawer(this);
			ThemesDrawer = new ThemesDrawer(this);
			ColorsDrawer = new ColorsDrawer(this);
			SpritesDrawer = new SpritesDrawer(this);
			FontsDrawer = new FontsDrawer(this);
		}
	}
}