using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class SpriteProvider : UIPropertyProvider<SpriteProperty, Sprite>
	{
		protected override IEnumerable<SpriteProperty> Props => ThemesLibrary.Current.Sprites;
	}
}