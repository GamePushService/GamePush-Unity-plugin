using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class ThemesDrawer : PropertiesMetaDrawer<ThemeEditorWindow>
	{
		protected override IEnumerable<IUIItem> Props => ThemesLibrary.Themes;

		protected override void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var fieldRect = CalculateFieldRect(rect, 1);
			var item = MetaItems[index];

			var name = EditorGUI.TextField(fieldRect, item.Name);
			if (name != item.Name && !string.IsNullOrEmpty(name))
			{
				ThemesManager.ChangeThemeName(item.Id, name);
				item.Name = name;
			}
		}

		protected override void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			ThemesManager.ChangeThemesOrder(oldIndex, newIndex);
			Window.OnGuiChanged();
			IsMetaDirty = true;
		}

		protected override void OnAddElement(ReorderableList list)
		{
			ThemesManager.CreateTheme();
			Window.OnGuiChanged();
			Window.MarkAllMetaDirty();
			IsMetaDirty = true;
		}

		protected override void OnRemoveElement(ReorderableList list)
		{
			var index = list.index;
			if (index >= 0 && index < MetaItems.Count)
			{
				var item = MetaItems[index];
				ThemesManager.DeleteTheme(item.Id);
			}

			Window.OnGuiChanged();
			Window.MarkAllMetaDirty();
			IsMetaDirty = true;
		}

		protected override bool CanRemoveElement(ReorderableList list)
		{
			return list.count > 1;
		}

		public ThemesDrawer(ThemeEditorWindow window) : base(window)
		{
			HeaderTitle = "Themes";
		}
	}
}