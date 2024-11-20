using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public abstract class PropertyProviderDrawer : PropertyDrawer
	{
		protected void Draw<T>(IEnumerable<T> props, Rect position, SerializedProperty property, GUIContent label) where T : IUIItem
		{
			EditorGUI.BeginProperty(position, label, property);
			var guiColor = GUI.color;

			var idProp = property.FindPropertyRelative("_id");

			var names = props.Select(x => x.Name).ToList();
			var ids = props.Select(x => x.Id).ToList();

			var index = ids.IndexOf(idProp.stringValue);
			if (index < 0)
			{
				names.Insert(0, "UNDEFINED");
				ids.Insert(0, Guid.Empty.ToString());
				index = 0;
				GUI.color = Color.yellow;
			}

			var selectedIndex = EditorGUI.Popup(position, property.name, index, names.ToArray());
			if (selectedIndex != index)
			{
				idProp.stringValue = ids[selectedIndex];
			}

			GUI.color = guiColor;
			EditorGUI.EndProperty();
		}
	}
}