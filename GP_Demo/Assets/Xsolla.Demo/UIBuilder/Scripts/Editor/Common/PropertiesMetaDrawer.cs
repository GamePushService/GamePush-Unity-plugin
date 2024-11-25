using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	public abstract class PropertiesMetaDrawer<TWindow>
	{
		protected readonly TWindow Window;

		public bool IsMetaDirty { get; set; } = true;

		protected List<PropertyMeta> MetaItems { get; set; }

		private ReorderableList MetaList { get; set; }

		protected float ElementHeight { get; set; }

		protected string HeaderTitle { get; set; }

		protected abstract IEnumerable<IUIItem> Props { get; }

		public void Draw()
		{
			if (IsMetaDirty)
			{
				RefreshMeta();
				IsMetaDirty = false;
			}

			MetaList.DoLayoutList();
		}

		private void RefreshMeta()
		{
			MetaItems = new List<PropertyMeta>();

			if (Props != null)
			{
				foreach (var prop in Props)
				{
					MetaItems.Add(new PropertyMeta(prop.Id, prop.Name));
				}
			}

			if (MetaList != null)
			{
				MetaList.drawElementCallback -= OnDrawElement;
				MetaList.drawHeaderCallback -= OnDrawHeader;
				MetaList.onReorderCallbackWithDetails -= OnReorderElement;
				MetaList.onAddCallback -= OnAddElement;
				MetaList.onRemoveCallback -= OnRemoveElement;
				MetaList.onCanRemoveCallback -= CanRemoveElement;
			}

			MetaList = new ReorderableList(MetaItems, typeof(PropertyMeta));
			if (ElementHeight > 0f)
			{
				MetaList.elementHeight = ElementHeight;
			}

			MetaList.drawElementCallback += OnDrawElement;
			MetaList.drawHeaderCallback += OnDrawHeader;
			MetaList.onReorderCallbackWithDetails += OnReorderElement;
			MetaList.onAddCallback += OnAddElement;
			MetaList.onRemoveCallback += OnRemoveElement;
			MetaList.onCanRemoveCallback += CanRemoveElement;
		}

		protected abstract void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused);

		private void OnDrawHeader(Rect rect)
		{
			EditorGUI.LabelField(rect, HeaderTitle);
		}

		protected abstract void OnReorderElement(ReorderableList list, int oldIndex, int newIndex);

		protected abstract void OnAddElement(ReorderableList list);

		protected abstract void OnRemoveElement(ReorderableList list);

		protected virtual bool CanRemoveElement(ReorderableList list)
		{
			return list.count > 0;
		}

		protected Rect CalculateFieldRect(Rect lineRect, int elementsCount)
		{
			return new Rect
			(
				lineRect.x + EditorGUIUtility.standardVerticalSpacing,
				lineRect.y + EditorGUIUtility.standardVerticalSpacing,
				lineRect.width / elementsCount - EditorGUIUtility.standardVerticalSpacing * 2,
				lineRect.height = EditorGUIUtility.singleLineHeight
			);
		}

		protected float CalculateOffsetPerField(Rect rect)
		{
			return rect.width + EditorGUIUtility.standardVerticalSpacing * 2;
		}

		protected PropertiesMetaDrawer(TWindow window)
		{
			Window = window;
		}
	}
}