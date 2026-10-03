using UnityEditor;
using UnityEngine;
using GamePush.Overlays;

namespace GamePushEditor.Overlays
{
    [CustomEditor(typeof(GP_OverlaySkin))]
    sealed class GP_OverlaySkinEditor : Editor
    {
        static readonly Color SelectionFill = new Color(1f, 0.82f, 0.18f, 0.22f);

        public override void OnInspectorGUI()
        {
            GP_OverlayPalette.LastChangeWasPalette = false;
            serializedObject.Update();
            var property = serializedObject.GetIterator();
            var enterChildren = true;
            while (property.NextVisible(enterChildren))
            {
                enterChildren = false;
                if (property.propertyPath == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                        EditorGUILayout.PropertyField(property, false);
                    continue;
                }

                var slot = GP_OverlayPalette.SlotFromProperty(property.name);
                if (slot == GP_OverlayColorRole.None || property.propertyType != SerializedPropertyType.Color)
                {
                    EditorGUI.BeginChangeCheck();
                    EditorGUILayout.PropertyField(property, true);
                    if (EditorGUI.EndChangeCheck())
                        GP_OverlayPalette.LastChangeWasPalette = false;
                    continue;
                }

                var group = GP_OverlayPalette.GroupTitle(property.name);
                if (!string.IsNullOrEmpty(group))
                {
                    EditorGUILayout.Space(6f);
                    EditorGUILayout.LabelField(group, EditorStyles.boldLabel);
                }

                var label = new GUIContent(GP_OverlayPalette.Label(slot),
                    string.IsNullOrEmpty(property.tooltip)
                        ? GP_OverlayPalette.Tooltip(slot)
                        : property.tooltip);
                var height = EditorGUI.GetPropertyHeight(property, label, true);
                var rect = EditorGUILayout.GetControlRect(true, height);
                var line = EditorGUIUtility.singleLineHeight;
                var fieldRect = new Rect(rect.x, rect.yMax - line, rect.width, line);
                if (GP_OverlayPalette.Selected == slot)
                    EditorGUI.DrawRect(fieldRect, SelectionFill);

                if (Event.current.type == EventType.MouseDown && fieldRect.Contains(Event.current.mousePosition))
                {
                    GP_OverlayPalette.Selected = GP_OverlayPalette.Selected == slot
                        ? GP_OverlayColorRole.None
                        : slot;
                    RepaintOverlayWindows();
                }

                EditorGUI.BeginChangeCheck();
                EditorGUI.PropertyField(rect, property, label, true);
                if (EditorGUI.EndChangeCheck())
                {
                    GP_OverlayPalette.LastChangeWasPalette = true;
                    GP_OverlayPalette.Selected = GP_OverlayColorRole.None;
                    RepaintOverlayWindows();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        internal static void RepaintOverlayWindows()
        {
            var windows = Resources.FindObjectsOfTypeAll<GP_OverlaySettingsWindow>();
            for (var i = 0; i < windows.Length; i++)
                windows[i].Repaint();
        }
    }
}
