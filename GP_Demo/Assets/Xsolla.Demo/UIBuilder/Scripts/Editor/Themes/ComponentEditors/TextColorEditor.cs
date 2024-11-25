using System.Collections.Generic;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(TextColorDecorator))]
	public class TextColorEditor : DecoratorEditor<ColorProperty>
	{
		protected override IEnumerable<ColorProperty> Props => ThemesLibrary.Current.Colors;

		public TextColorEditor()
		{
			PropsLabel = "Color";
		}
	}
}