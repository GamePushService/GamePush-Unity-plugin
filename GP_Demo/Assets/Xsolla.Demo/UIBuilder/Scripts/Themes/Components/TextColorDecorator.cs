using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.UIBuilder
{
	[AddComponentMenu("Xsolla/UI Builder/Text Color Decorator", 100)]
	public class TextColorDecorator : ThemeDecorator<Text>
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