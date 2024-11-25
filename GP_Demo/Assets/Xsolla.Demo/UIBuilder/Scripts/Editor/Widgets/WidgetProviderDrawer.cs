using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[CustomPropertyDrawer(typeof(WidgetProvider))]
	public class WidgetProviderDrawer : PropertyProviderDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(WidgetsLibrary.Widgets, position, property, label);
		}
	}
}