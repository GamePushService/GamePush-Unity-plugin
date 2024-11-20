using System.Collections.Generic;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(ImageSpriteDecorator))]
	public class ImageSpriteEditor : DecoratorEditor<SpriteProperty>
	{
		protected override IEnumerable<SpriteProperty> Props => ThemesLibrary.Current.Sprites;

		public ImageSpriteEditor()
		{
			PropsLabel = "Sprite";
		}
	}
}