using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public abstract class DecoratorEditor<TProp> : Editor where TProp : IUIItem
	{
		private SerializedProperty CompProp { get; set; }

		private SerializedProperty PropertyId { get; set; }

		private PointerOverriderEditor PointerOverriderEditor { get; set; }

		protected abstract IEnumerable<TProp> Props { get; }

		protected string PropsLabel { get; set; }

		protected void OnEnable()
		{
			CompProp = serializedObject.FindProperty("_comp");
			PropertyId = serializedObject.FindProperty("_propertyId");
			PointerOverriderEditor = new PointerOverriderEditor(serializedObject);
		}

		public override void OnInspectorGUI()
		{
			serializedObject.Update();

			EditorGUILayout.PropertyField(CompProp);
			var guiColor = GUI.color;

			var names = Props.Select(x => x.Name).ToList();
			var ids = Props.Select(x => x.Id).ToList();

			var index = ids.IndexOf(PropertyId.stringValue);
			if (index < 0)
			{
				names.Insert(0, "UNDEFINED");
				ids.Insert(0, Guid.Empty.ToString());
				index = 0;
				GUI.color = Color.yellow;
			}

			var selectedIndex = EditorGUILayout.Popup(PropsLabel, index, names.ToArray());
			if (selectedIndex != index)
			{
				PropertyId.stringValue = ids[selectedIndex];
			}

			GUI.color = guiColor;
			PointerOverriderEditor.DrawGUI(Props, names[selectedIndex]);

			serializedObject.ApplyModifiedProperties();
		}
	}
}