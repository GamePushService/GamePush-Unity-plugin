using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	[CustomEditor(typeof(XsollaSettings))]
	public partial class XsollaSettingsEditor : UnityEditor.Editor
	{
		[MenuItem("Window/Xsolla/Edit Settings", false, 1000)]
		public static void Edit()
		{
			Selection.activeObject = GetSettingsAsset();
		}

		private GUIStyle GroupAreaStyle => GUI.skin.box;

		private GUIStyle GroupHeaderStyle => EditorStyles.boldLabel;

		public override void OnInspectorGUI()
		{
			var changed = AutoFillSettings() ||
				GeneralSettings() ||
				LoginSettings() ||
				PayStationSettings() ||
				AndroidSettings() ||
				DesktopSettings() ||
				EditorSettings();

			if (changed)
			{
				DropSavedTokens();
				SaveSettingsAsset();
			}
		}

		public static void DropSavedTokens()
		{
			XsollaToken.DeleteSavedInstance();
		}

		public static void SaveSettingsAsset()
		{
			EditorUtility.SetDirty(XsollaSettings.Instance);
			AssetDatabase.SaveAssets();
		}

		private static void DrawErrorBox(string message)
		{
			EditorGUILayout.HelpBox(message, MessageType.Error, true);
			EditorGUILayout.Space();
		}

		private static XsollaSettings GetSettingsAsset()
		{
			var instances = Resources.LoadAll<XsollaSettings>(string.Empty);
			if (instances.Length == 1)
				return instances[0];

			if (instances.Length > 1)
			{
				var paths = instances.Select(AssetDatabase.GetAssetPath);
				var joinedPaths = string.Join("\n", paths);

				throw new Exception("There are more than one `XsollaSettings` asset in the project. "
					+ "Please keep only one asset. "
					+ $"Founded {instances.Length} assets by paths:\n{joinedPaths}");
			}

			var instance = CreateInstance<XsollaSettings>();
			var assetPath = Path.Combine(Application.dataPath, "Resources", "XsollaSettings.asset");

			var directoryName = Path.GetDirectoryName(assetPath);
			if (!Directory.Exists(directoryName))
				Directory.CreateDirectory(directoryName);

			assetPath = assetPath.Replace(Application.dataPath, "Assets");
			AssetDatabase.CreateAsset(instance, assetPath);
			return instance;
		}
	}
}
