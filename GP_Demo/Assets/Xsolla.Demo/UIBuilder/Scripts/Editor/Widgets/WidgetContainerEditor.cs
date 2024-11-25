using System;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(WidgetContainer))]
	public class WidgetContainerEditor : Editor
	{
		private SerializedProperty PropertyId { get; set; }

		private SerializedProperty Container { get; set; }

		private SerializedProperty Current { get; set; }

		private void OnEnable()
		{
			PropertyId = serializedObject.FindProperty("_propertyId");
			Container = serializedObject.FindProperty("_container");
			Current = serializedObject.FindProperty("_current");
		}

		public override void OnInspectorGUI()
		{
			var isPartOfPrefab = PrefabUtility.IsPartOfAnyPrefab(target);
			var isPreviewMode = EditorSceneManager.IsPreviewSceneObject(target);
			if (isPartOfPrefab && !isPreviewMode)
			{
				EditorGUILayout.HelpBox("Open prefab for editing support", MessageType.Info);
			}
			else
			{
				serializedObject.Update();

				EditorGUILayout.PropertyField(Container);
				GUI.enabled = Container.objectReferenceValue != null;

				EditorGUILayout.PropertyField(Current);

				var widgets = WidgetsLibrary.Widgets;
				var names = widgets.Select(x => x.Name).ToList();
				var ids = widgets.Select(x => x.Id).ToList();

				var guiColor = GUI.color;
				var index = ids.IndexOf(PropertyId.stringValue);
				if (index < 0)
				{
					names.Insert(0, "UNDEFINED");
					ids.Insert(0, Guid.Empty.ToString());
					index = 0;
					GUI.color = Color.yellow;
				}

				var selectedIndex = EditorGUILayout.Popup("Widgets", index, names.ToArray());
				if (selectedIndex != index)
				{
					PropertyId.stringValue = ids[selectedIndex];
					serializedObject.ApplyModifiedProperties();

					var container = target as WidgetContainer;
					if (container)
					{
						WidgetsHandler.Handle(container);
					}
				}
				
				GUI.color = guiColor;
				GUI.enabled = true;
				
				serializedObject.ApplyModifiedProperties();
			}
		}
	}
}