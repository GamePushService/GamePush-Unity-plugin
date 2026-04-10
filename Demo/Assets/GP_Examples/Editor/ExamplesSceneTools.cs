using System.Text;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Examples.EditorTools
{
    public static class ExamplesSceneTools
    {
        private const string ExamplesScenePath = "Assets/GP_Examples/Scenes/ExamplesScene.unity";

        public static void PrintExamplesHierarchy()
        {
            var scene = EditorSceneManager.OpenScene(ExamplesScenePath, OpenSceneMode.Single);
            var builder = new StringBuilder();

            builder.AppendLine($"Scene: {scene.path}");
            foreach (var root in scene.GetRootGameObjects())
            {
                AppendTransform(builder, root.transform, 0);
            }

            Debug.Log(builder.ToString());
        }

        private static void AppendTransform(StringBuilder builder, Transform transform, int depth)
        {
            builder.Append(' ', depth * 2);
            builder.Append("- ");
            builder.Append(transform.name);
            builder.AppendLine();

            for (int index = 0; index < transform.childCount; index++)
            {
                AppendTransform(builder, transform.GetChild(index), depth + 1);
            }
        }
    }
}
