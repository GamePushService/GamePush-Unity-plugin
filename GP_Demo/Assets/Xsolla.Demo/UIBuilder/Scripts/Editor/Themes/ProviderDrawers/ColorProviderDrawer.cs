using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[CustomPropertyDrawer(typeof(ColorProvider))]
	public class ColorProviderDrawer : PropertyProviderDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(ThemesLibrary.Current.Colors, position, property, label);
		}
	}
}