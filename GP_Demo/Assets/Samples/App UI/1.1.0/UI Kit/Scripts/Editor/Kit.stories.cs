using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace Unity.AppUI.Editor
{
    public class KitPage : StoryBookPage
    {
        public override string displayName => "Kit";

        public override Type componentType => null;

        public KitPage()
        {
            m_Stories.Add(new StoryBookStory("Main", () =>
            {
                var element = new VisualElement();
                var tree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(AssetDatabase.GUIDToAssetPath("d8e9b51f49d24be8a1cf19ae6c5a7c37"));
                tree.CloneTree(element);
                var root = element.Q<VisualElement>("root-main");
                root.styleSheets.Add(
                    AssetDatabase.LoadAssetAtPath<StyleSheet>(AssetDatabase.GUIDToAssetPath("b763f743b4824058b4e329c1a2592529")));
                Samples.Examples.SetupDataBinding(root);
                root.Query(className: "example-context-switcher-panel").ForEach(visualElement => 
                    visualElement.style.display = DisplayStyle.None);
                return root;
            }));
        }
    }
}
