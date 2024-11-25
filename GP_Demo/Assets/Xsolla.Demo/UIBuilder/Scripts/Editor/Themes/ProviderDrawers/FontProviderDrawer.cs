using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[CustomPropertyDrawer(typeof(FontProvider))]
	public class FontProviderDrawer : PropertyProviderDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(ThemesLibrary.Current.Fonts, position, property, label);
		}
	}
}