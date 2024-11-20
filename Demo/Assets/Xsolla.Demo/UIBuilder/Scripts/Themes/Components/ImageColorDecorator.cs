using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Image Color Decorator", 100)]
	public class ImageColorDecorator : ThemeDecorator<Image>
	{
		protected override IEnumerable<IUIItem> Props => ThemesLibrary.Current.Colors;

		protected override void ApplyProperty(Theme theme, string id)
		{
			var prop = theme.GetColorProperty(id);
			if (prop != null)
			{
				Comp.color = prop.Value;
			}
		}
	}
}