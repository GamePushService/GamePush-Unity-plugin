using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public class SpritesDrawer : PropertiesMetaDrawer<ThemeEditorWindow>
	{
		protected override IEnumerable<IUIItem> Props => ThemesLibrary.Current?.Sprites;

		protected override void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused)
		{
			var themes = ThemesLibrary.Themes;
			var fieldRect = CalculateFieldRect(rect, themes.Count + 1);
			var offsetPerField = CalculateOffsetPerField(fieldRect);

			var item = MetaItems[index];

			var name = EditorGUI.TextField(fieldRect, item.Name);
			if (name != item.Name && !string.IsNullOrEmpty(name))
			{
				ThemesManager.ChangeSpriteName(item.Id, name);
				item.Name = name;
			}
			fieldRect.x += offsetPerField;

			fieldRect.x += (fieldRect.width - ElementHeight) / 2f;
			fieldRect.height = ElementHeight;
			fieldRect.width = ElementHeight;

			foreach (var theme in themes)
			{
				var prop = theme.GetSpriteProperty(item.Id);
				prop.Value = (Sprite) EditorGUI.ObjectField(fieldRect, prop.Value, typeof(Sprite), false);
				fieldRect.x += offsetPerField;
			}
		}

		protected override void OnReorderElement(ReorderableList list, int oldIndex, int newIndex)
		{
			ThemesManager.ChangeSpritesOrder(oldIndex, newIndex);
			Window.OnGuiChanged();
			IsMetaDirty = true;
		}

		protected override void OnAddElement(ReorderableList list)
		{
			ThemesManager.CreateSprite();
			Window.OnGuiChanged();
			IsMetaDirty = true;
		}

		protected override void OnRemoveElement(ReorderableList list)
		{
			var item = MetaItems[list.index];
			ThemesManager.DeleteSprite(item.Id);
			Window.OnGuiChanged();
			IsMetaDirty = true;
		}

		public SpritesDrawer(ThemeEditorWindow window) : base(window)
		{
			HeaderTitle = "Sprites";
			ElementHeight = EditorGUIUtility.singleLineHeight * 3f;
		}
	}
}