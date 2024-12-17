using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public class WidgetProperty : UIProperty<GameObject>
	{
		[SerializeField] private GameObject _prefab;

		public override GameObject Value
		{
			get => _prefab;
			set => _prefab = value;
		}

		public WidgetProperty()
		{
			Name = "New Widget";
		}
	}
}