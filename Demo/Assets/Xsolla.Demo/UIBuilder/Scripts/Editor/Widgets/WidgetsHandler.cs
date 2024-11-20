using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Xsolla.UIBuilder
{
	public static class WidgetsHandler
	{
		public static void Handle(string assetPath)
		{
			var prefab = PrefabUtility.LoadPrefabContents(assetPath);
			if (IsPrefabVariant(prefab))
			{
				PrefabUtility.UnloadPrefabContents(prefab);
				return;
			}

			var isDirty = false;

			var containers = prefab.GetComponentsInChildren<WidgetContainer>();
			foreach (var container in containers)
			{
				if (Handle(container))
				{
					isDirty = true;
				}
			}

			if (isDirty)
			{
				PrefabUtility.SaveAsPrefabAsset(prefab, assetPath);
			}

			PrefabUtility.UnloadPrefabContents(prefab);
		}

		public static bool Handle(WidgetContainer container)
		{
			if (!container)
			{
				return false;
			}

			if (PrefabUtility.IsPartOfAnyPrefab(container))
			{
				return false;
			}

			var widgetPrefab = WidgetsLibrary.GetWidgetProperty(container.PropertyId)?.Value;
			var widgetPrefabPath = widgetPrefab ? GetAssetPath(widgetPrefab) : string.Empty;

			var currentPrefabPath = container.Current ? GetAssetPath(container.Current) : string.Empty;
			if (string.IsNullOrEmpty(widgetPrefabPath) || currentPrefabPath == widgetPrefabPath)
			{
				return false;
			}

			var siblingIndex = ClearCurrentWidget(container);
			SetCurrentWidget(container, widgetPrefab, siblingIndex);
			AlignWidgetByContainer(container.Container, container.Current);

			return true;
		}

		private static int ClearCurrentWidget(WidgetContainer container)
		{
			var siblingIndex = 0;

			if (container.Current)
			{
				siblingIndex = container.Current.GetSiblingIndex();

				if (PrefabUtility.IsPrefabAssetMissing(container.Current.gameObject))
				{
					Object.Destroy(container.Current.gameObject);
				}
				else
				{
					Object.DestroyImmediate(container.Current.gameObject);
				}
			}

			container.Current = null;
			return siblingIndex;
		}

		private static void SetCurrentWidget(WidgetContainer container, GameObject target, int siblingIndex)
		{
			if (container && target)
			{
				container.Current = CreateAsChild(container.Container, target).transform;
				container.Current.SetSiblingIndex(siblingIndex);
			}
		}

		#region Utils

		private static bool IsPrefabVariant(Object obj)
		{
			return PrefabUtility.GetCorrespondingObjectFromOriginalSource(obj) != null;
		}

		private static string GetAssetPath(Object obj)
		{
			return PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
		}

		private static GameObject CreateAsChild(Transform parent, GameObject prefab)
		{
			var gameObject = PrefabUtility.InstantiatePrefab(prefab, parent) as GameObject;
			if (!gameObject || gameObject == null)
			{
				throw new Exception();
			}

			gameObject.name = prefab.name;
			gameObject.transform.SetParent(parent, false);

			return gameObject;
		}

		private static void AlignWidgetByContainer(Transform parent, Transform child)
		{
			if (!parent || !child)
			{
				return;
			}

			var rectTransform = child.transform as RectTransform;
			if (!rectTransform || rectTransform == null)
			{
				return;
			}

			rectTransform.SetParent(parent, false);

			rectTransform.anchorMin = Vector2.zero;
			rectTransform.anchorMax = Vector2.one;

			rectTransform.offsetMin = Vector2.zero;
			rectTransform.offsetMax = Vector2.zero;

			rectTransform.sizeDelta = Vector2.zero;
		}

		#endregion
	}
}