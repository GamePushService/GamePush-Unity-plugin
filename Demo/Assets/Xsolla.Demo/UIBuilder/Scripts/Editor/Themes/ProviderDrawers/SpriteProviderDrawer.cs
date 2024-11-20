using UnityEditor;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[CustomPropertyDrawer(typeof(SpriteProvider))]
	public class SpriteProviderDrawer : PropertyProviderDrawer
	{
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			Draw(ThemesLibrary.Current.Sprites, position, property, label);
		}
	}
}