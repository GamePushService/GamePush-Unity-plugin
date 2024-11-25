using UnityEngine;
using System;
using System.Collections.Generic;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class ColorProvider : UIPropertyProvider<ColorProperty, Color>
	{
		protected override IEnumerable<ColorProperty> Props => ThemesLibrary.Current.Colors;
	}
}