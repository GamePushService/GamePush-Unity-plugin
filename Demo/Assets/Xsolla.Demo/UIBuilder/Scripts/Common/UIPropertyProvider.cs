using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public abstract class UIPropertyProvider<TProp, TValue> where TProp : UIProperty<TValue>
	{
		[SerializeField] private string _id;

		public string Id
		{
			get => _id;
			set => _id = value;
		}

		protected abstract IEnumerable<TProp> Props { get; }

		public TProp GetProperty()
		{
			return Props.FirstOrDefault(x => x.Id == Id);
		}

		public TValue GetValue()
		{
			return GetProperty().Value;
		}
	}
}