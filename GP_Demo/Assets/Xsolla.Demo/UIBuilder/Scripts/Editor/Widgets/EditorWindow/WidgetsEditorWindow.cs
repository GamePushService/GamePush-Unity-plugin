using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class WidgetsEditorWindow : EditorWindow
	{
		private readonly WidgetsHeaderDrawer HeaderDrawer;

		private readonly WidgetsDrawer WidgetsDrawer;

		private Vector2 ScrollPosition { get; set; }

		private const string _isAutoRefreshKey = "ui_widgets_is_auto_refresh";

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

		private void OnGUI()
		{
			EditorGUIUtility.labelWidth = 1f;

			EditorGUILayout.Space();
			HeaderDrawer.Draw();

			ScrollPosition = EditorGUILayout.BeginScrollView(ScrollPosition);
			EditorGUI.BeginChangeCheck();

			EditorGUILayout.Space();
			WidgetsDrawer.Draw();

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
			EditorUtility.SetDirty(WidgetsLibrary.Instance);

			if (isRefreshAssets)
			{
				HandleSceneObjects();
				HandleAssets();
			}

			AssetDatabase.SaveAssets();

			if (isRefreshAssets)
			{
				AssetDatabase.Refresh();
			}
		}

		private void HandleSceneObjects()
		{
#if UNITY_6000
			var containers = FindObjectsByType<WidgetContainer>(FindObjectsSortMode.None);
#else
			var containers = FindObjectsOfType<WidgetContainer>();
#endif
			
			foreach (var container in containers)
			{
				if (!PrefabUtility.IsPartOfAnyPrefab(container))
				{
					WidgetsHandler.Handle(container);
				}
			}
		}

		private void HandleAssets()
		{
			var guids = AssetDatabase.FindAssets(
				"t:prefab",
				new[]
				{
					"Assets"
				}
			);

			var paths = new List<string>();

			foreach (var guid in guids)
			{
				var assetPath = AssetDatabase.GUIDToAssetPath(guid);
				var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetPath);
				if (gameObject.GetComponentsInChildren<WidgetContainer>().Length > 0)
				{
					paths.Add(assetPath);
				}
			}

			foreach (var path in paths)
			{
				WidgetsHandler.Handle(path);
			}
		}

		[MenuItem("Window/Xsolla/UI Widgets Editor", false, 1110)]
		private static void OpenWindow()
		{
			GetWindow<WidgetsEditorWindow>("UI Widgets Editor");
		}

		private WidgetsEditorWindow()
		{
			HeaderDrawer = new WidgetsHeaderDrawer(this);
			WidgetsDrawer = new WidgetsDrawer(this);
		}
	}
}