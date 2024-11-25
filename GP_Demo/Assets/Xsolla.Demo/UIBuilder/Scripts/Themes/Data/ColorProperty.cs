using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class ColorProperty : UIProperty<Color>
	{
		[SerializeField] private Color _color;

		public override Color Value
		{
			get => _color;
			set => _color = value;
		}

		public ColorProperty()
		{
			Name = "New Color";
		}
	}
}