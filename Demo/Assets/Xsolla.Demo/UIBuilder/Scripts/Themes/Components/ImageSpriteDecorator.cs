using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Image Sprite Decorator", 100)]
	public class ImageSpriteDecorator : ThemeDecorator<Image>
	{
		protected override IEnumerable<IUIItem> Props => ThemesLibrary.Current.Sprites;

		protected override void ApplyProperty(Theme theme, string id)
		{
			var prop = theme.GetSpriteProperty(id);
			if (prop != null)
			{
				Comp.sprite = prop.Value;
			}
		}
	}
}