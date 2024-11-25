using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class SpriteProperty : UIProperty<Sprite>
	{
		[SerializeField] private Sprite _sprite;

		public override Sprite Value
		{
			get => _sprite;
			set => _sprite = value;
		}

		public SpriteProperty()
		{
			Name = "New Sprite";
		}
	}
}