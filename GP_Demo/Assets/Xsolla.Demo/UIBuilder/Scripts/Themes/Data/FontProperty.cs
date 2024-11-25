using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class FontProperty : UIProperty<Font>
	{
		[SerializeField] private Font _font;

		public override Font Value
		{
			get => _font;
			set => _font = value;
		}

		public FontProperty()
		{
			Name = "New Font";
		}
	}
}