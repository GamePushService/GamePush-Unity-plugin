using System.Collections.Generic;
using UnityEditor;

namespace Xsolla.UIBuilder
{
	[CustomEditor(typeof(TextFontDecorator))]
	public class TextFontEditor : DecoratorEditor<FontProperty>
	{
		protected override IEnumerable<FontProperty> Props => ThemesLibrary.Current.Fonts;

		public TextFontEditor()
		{
			PropsLabel = "Font";
		}
	}
}