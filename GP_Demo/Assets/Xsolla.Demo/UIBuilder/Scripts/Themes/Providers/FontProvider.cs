using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class FontProvider : UIPropertyProvider<FontProperty, Font>
	{
		protected override IEnumerable<FontProperty> Props => ThemesLibrary.Current.Fonts;
	}
}