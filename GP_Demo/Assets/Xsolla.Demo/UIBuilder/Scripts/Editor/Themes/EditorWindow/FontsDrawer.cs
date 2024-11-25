using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class FontsDrawer : PropertiesMetaDrawer<ThemeEditorWindow>
	{
		protected override IEnumerable<IUIItem> Props => ThemesLibrary.Current?.Fonts;

		protected override void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var themes = ThemesLibrary.Themes;
			var fieldRect = CalculateFieldRect(rect, themes.Count + 1);
			var offsetPerField = CalculateOffsetPerField(fieldRect);

			var item = MetaItems[index];

			var name = EditorGUI.TextField(fieldRect, item.Name);
			if (name != item.Name && !string.IsNullOrEmpty(name))
			{
				ThemesManager.ChangeFontName(item.Id, name);
				item.Name = name;
			}
			fieldRect.x += offsetPerField;

			foreach (var theme in themes)
			{
				var prop = theme.GetFontProperty(item.Id);
				prop.Value = (Font) EditorGUI.ObjectField(fieldRect, prop.Value, typeof(Font), false);
				fieldRect.x += offsetPerField;
			}
		}

		protected override void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			ThemesManager.ChangeFontsOrder(oldIndex, newIndex);
			Window.OnGuiChanged();
			IsMetaDirty = true;
		}

		protected override void OnAddElement(ReorderableList list)
		{
			ThemesManager.CreateFont();
			Window.OnGuiChanged();
			IsMetaDirty = true;
		}

		protected override void OnRemoveElement(ReorderableList list)
		{
			var item = MetaItems[list.index];
			ThemesManager.DeleteFont(item.Id);
			Window.OnGuiChanged();
			IsMetaDirty = true;
		}

		public FontsDrawer(ThemeEditorWindow window) : base(window)
		{
			HeaderTitle = "Fonts";
		}
	}
}