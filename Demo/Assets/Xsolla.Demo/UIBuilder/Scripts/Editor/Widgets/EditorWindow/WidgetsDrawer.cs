using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class WidgetsDrawer : PropertiesMetaDrawer<WidgetsEditorWindow>
	{
		protected override IEnumerable<IUIItem> Props => WidgetsLibrary.Widgets;

		protected override void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var item = MetaItems[index];
			var widget = WidgetsLibrary.Widgets.First(x => x.Id == item.Id);

			var fieldRect = CalculateFieldRect(rect, 2);
			var name = EditorGUI.TextField(fieldRect, item.Name);
			if (name != item.Name && !string.IsNullOrEmpty(name))
			{
				WidgetsManager.ChangeWidgetName(item.Id, name);
				item.Name = name;
			}
			fieldRect.x += rect.width / 2;

			var prefab = EditorGUI.ObjectField(fieldRect, widget.Value, typeof(GameObject), false) as GameObject;
			if (prefab != widget.Value)
			{
				widget.Value = prefab;
				IsMetaDirty = true;
			}
		}

		protected override void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			WidgetsManager.ChangeWidgetsOrder(oldIndex, newIndex);
			IsMetaDirty = true;
			Window.OnGuiChanged();
		}

		protected override void OnAddElement(ReorderableList list)
		{
			WidgetsManager.CreateWidget();
			IsMetaDirty = true;
			Window.OnGuiChanged();
		}

		protected override void OnRemoveElement(ReorderableList list)
		{
			var item = MetaItems[list.index];
			WidgetsManager.DeleteWidget(item.Id);
			IsMetaDirty = true;
			Window.OnGuiChanged();
		}

		public WidgetsDrawer(WidgetsEditorWindow window) : base(window)
		{
			HeaderTitle = "Widgets";
		}
	}
}