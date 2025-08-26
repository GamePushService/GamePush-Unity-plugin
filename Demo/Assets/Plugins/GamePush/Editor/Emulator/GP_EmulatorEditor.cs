using UnityEditor;
using UnityEngine;

using GamePush.ConsoleController;

namespace GamePush
{
    [CustomEditor(typeof(GP_ConsoleController))]
    public class GP_EmulatorEditor : Editor
    {
        SerializedProperty LogSwitches;

        private void OnEnable()
        {
            LogSwitches = serializedObject.FindProperty("logSwitches");
        }

        public override void OnInspectorGUI()
        {
            GP_ConsoleController controller = (GP_ConsoleController)target;

            serializedObject.Update();

            EditorGUILayout.Space(10);

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Enable All"))
                controller.SwitchAll(true);
            if (GUILayout.Button("Disable All"))
                controller.SwitchAll(false);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(LogSwitches);

            serializedObject.ApplyModifiedProperties();
        }
    }
}


