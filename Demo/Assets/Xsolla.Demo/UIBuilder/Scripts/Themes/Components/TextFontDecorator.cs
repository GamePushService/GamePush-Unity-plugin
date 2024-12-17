using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Text Font Decorator", 100)]
	public class TextFontDecorator : ThemeDecorator<Text>
	{
		protected override IEnumerable<IUIItem> Props => ThemesLibrary.Current.Fonts;

		protected override void ApplyProperty(Theme theme, string id)
		{
			var prop = theme.GetFontProperty(id);
			if (prop != null)
			{
				Comp.font = prop.Value;
			}
		}
	}
}