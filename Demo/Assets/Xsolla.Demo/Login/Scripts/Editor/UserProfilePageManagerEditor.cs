using UnityEditor;
using System;

namespace Xsolla.Demo
{
	[CustomEditor(typeof(UserProfilePageManager))]
	public class UserProfilePageManagerEditor : Editor
	{
		private static bool _unfold = false;

		public override void OnInspectorGUI()
		{
			_unfold = EditorGUILayout.Foldout(_unfold, "User-friendly entry assignment");

			if (_unfold)
			{
				var userProfielManager = target as UserProfilePageManager;
				var typeNames = Enum.GetNames(typeof(UserProfileEntryType));
				userProfielManager.UserProfileEntries = ResizeArrayIfNeeded(typeNames.Length, userProfielManager.UserProfileEntries);

				for (int i = 0; i < typeNames.Length; i++)
				{
					var value = EditorGUILayout.ObjectField(label: typeNames[i], userProfielManager.UserProfileEntries[i], typeof(UserProfileEntryUI), allowSceneObjects: true);

					if (value != userProfielManager.UserProfileEntries[i])
						userProfielManager.UserProfileEntries[i] = value as UserProfileEntryUI;
				}
			}

			base.DrawDefaultInspector();
		}

		private UserProfileEntryUI[] ResizeArrayIfNeeded(int targetLength, UserProfileEntryUI[] array)
		{
			if (array.Length == targetLength)
				return array;
			else
			{
				UserProfileEntryUI[] newArray = new UserProfileEntryUI[targetLength];
				var length = targetLength > array.Length ? array.Length : targetLength;

				Array.Copy(sourceArray: array, destinationArray: newArray, length);

				return newArray;
			}
		}
	}
}
