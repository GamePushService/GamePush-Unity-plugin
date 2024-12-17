using System.Collections.Generic;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(ImageColorDecorator))]
	public class ImageColorEditor : DecoratorEditor<ColorProperty>
	{
		protected override IEnumerable<ColorProperty> Props => ThemesLibrary.Current.Colors;

		public ImageColorEditor()
		{
			PropsLabel = "Color";
		}
	}
}