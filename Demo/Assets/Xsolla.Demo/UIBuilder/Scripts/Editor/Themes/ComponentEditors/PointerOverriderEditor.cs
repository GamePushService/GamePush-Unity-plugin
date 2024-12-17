using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class PointerOverriderEditor
	{
		private SerializedProperty PointerEvents { get; }

		private SerializedProperty TransitionMode { get; }

		private SerializedProperty IsOverrideHover { get; }

		private SerializedProperty HoverPropertyId { get; }

		private SerializedProperty IsOverridePress { get; }

		private SerializedProperty PressPropertyId { get; }

		public void DrawGUI<T>(IEnumerable<T> props, string mainName) where T : IUIItem
		{
			EditorGUILayout.PropertyField(TransitionMode);

			if (TransitionMode.enumValueIndex > 0)
			{
				EditorGUILayout.BeginHorizontal();
				EditorGUILayout.Space();
				EditorGUILayout.BeginVertical();

				EditorGUILayout.PropertyField(PointerEvents);
				var canEdit = PointerEvents.objectReferenceValue;

				GUI.enabled = canEdit;
				DrawOverride(IsOverrideHover, HoverPropertyId, "Override on hover", canEdit, props, mainName);
				DrawOverride(IsOverridePress, PressPropertyId, "Override on press", canEdit, props, mainName);
				GUI.enabled = true;

				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
		}

		private static void DrawOverride<T>(SerializedProperty isOverride, SerializedProperty propertyId, string label, bool canEdit, IEnumerable<T> props, string mainName) where T : IUIItem
		{
			EditorGUILayout.BeginHorizontal();
			isOverride.boolValue = EditorGUILayout.ToggleLeft(label, isOverride.boolValue, GUILayout.Width(EditorGUIUtility.labelWidth));

			if (canEdit && isOverride.boolValue)
			{
				var guiColor = GUI.color;

				var names = props.Select(x => x.Name).ToList();
				var ids = props.Select(x => x.Id).ToList();

				var index = ids.IndexOf(propertyId.stringValue);
				if (index < 0)
				{
					names.Insert(0, "UNDEFINED");
					ids.Insert(0, Guid.Empty.ToString());
					index = 0;
					GUI.color = Color.yellow;
				}

				var selectedIndex = EditorGUILayout.Popup(index, names.ToArray());
				if (selectedIndex != index)
				{
					propertyId.stringValue = ids[selectedIndex];
				}

				GUI.color = guiColor;
			}
			else
			{
				GUI.enabled = false;
				EditorGUILayout.LabelField(mainName);
				GUI.enabled = canEdit;
			}

			EditorGUILayout.EndHorizontal();
		}

		public PointerOverriderEditor(SerializedObject serializedObject)
		{
			var overrider = serializedObject.FindProperty("_pointerOverrider");
			PointerEvents = overrider.FindPropertyRelative("_pointerEvents");
			TransitionMode = overrider.FindPropertyRelative("_transitionMode");

			IsOverrideHover = overrider.FindPropertyRelative("_isOverrideHover");
			HoverPropertyId = overrider.FindPropertyRelative("_hoverPropertyId");

			IsOverridePress = overrider.FindPropertyRelative("_isOverridePress");
			PressPropertyId = overrider.FindPropertyRelative("_pressPropertyId");
		}
	}
}