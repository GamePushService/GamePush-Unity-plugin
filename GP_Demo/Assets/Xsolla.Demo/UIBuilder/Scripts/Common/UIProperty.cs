using System;
using UnityEngine;

namespace Xsolla.UIBuilder
{
	[Serializable]
	public abstract class UIProperty<TValue> : IUIItem
	{
		[SerializeField] private string _name;

		[SerializeField] private string _id;

		public string Id
		{
			get => _id;
			set => _id = value;
		}

		public string Name
		{
			get => _name;
			set => _name = value;
		}

		public abstract TValue Value { get; set; }

		protected UIProperty()
		{
			Id = Guid.NewGuid().ToString();
		}

		protected bool Equals(UIProperty<TValue> other)
		{
			return _id == other._id;
		}

		public override bool Equals(object obj)
		{
			if (ReferenceEquals(null, obj))
				return false;

			if (ReferenceEquals(this, obj))
				return true;

			if (obj.GetType() != GetType())
				return false;

			return Equals((UIProperty<TValue>) obj);
		}

		public override int GetHashCode()
		{
			return _id != null ? _id.GetHashCode() : 0;
		}
	}
}