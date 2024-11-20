using UnityEditor;
using System;

namespace Xsolla.Demo
{
	[CustomEditor(typeof(UrlContainer))]
	public class UrlContainerEditor : Editor
	{
		private static bool _unfold = false;

		public override void OnInspectorGUI()
		{
			_unfold = EditorGUILayout.Foldout(_unfold, "URLs for store demo buttons");

			if (_unfold)
			{
				var urlContainer = target as UrlContainer;
				var typeNames = Enum.GetNames(typeof(UrlType));
				urlContainer.Urls = ResizeArrayIfNeeded(typeNames.Length, urlContainer.Urls);

				for (int i = 0; i < typeNames.Length; i++)
				{
					var value = EditorGUILayout.TextField(typeNames[i], urlContainer.Urls[i]);

					if(value != urlContainer.Urls[i])
					{
						var serializedTarget = new SerializedObject(target);
						var property = serializedTarget.FindProperty("Urls");
						property.GetArrayElementAtIndex(i).stringValue = value;
						serializedTarget.ApplyModifiedProperties();
					}
				}
			}
		}

		private string[] ResizeArrayIfNeeded(int targetLength, string[] array)
		{
			if (array.Length == targetLength)
				return array;
			else
			{
				string[] newArray = new string[targetLength];
				var length = targetLength > array.Length ? array.Length : targetLength;

				Array.Copy(sourceArray: array, destinationArray: newArray, length);

				return newArray;
			}
		}
	}
}
