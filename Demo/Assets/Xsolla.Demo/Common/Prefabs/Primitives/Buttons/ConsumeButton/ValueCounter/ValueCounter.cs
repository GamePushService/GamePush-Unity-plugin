using System;
using UnityEngine;

namespace Xsolla.Demo
{
	public class ValueCounter : MonoBehaviour
	{
		public event Action<int> ValueChanged;
		private int _value;

		public int GetValue()
		{
			return _value;
		}

		public void ResetValue()
		{
			_value = 1;
			ValueChanged?.Invoke(_value);
		}

		public void IncreaseValue(int delta)
		{
			_value += delta;
			ValueChanged?.Invoke(_value);
		}

		public void DecreaseValue(int delta)
		{
			if (delta > _value)
				delta = _value;
			_value -= delta;
			ValueChanged?.Invoke(_value);
		}

		public override string ToString()
		{
			return _value.ToString();
		}

		public static ValueCounter operator ++(ValueCounter value)
		{
			value.IncreaseValue(1);
			return value;
		}

		public static ValueCounter operator --(ValueCounter value)
		{
			value.DecreaseValue(1);
			return value;
		}

		public static implicit operator int(ValueCounter value)
		{
			return value.GetValue();
		}
	}
}
