using System;
using System.Globalization;

namespace Xsolla.Core
{
	[Serializable]
	public class Price
	{
		public string amount;
		public string amount_without_discount;
		public string currency;

		public float GetAmount()
		{
			return string.IsNullOrEmpty(amount)
				? 0F
				: float.Parse(amount, CultureInfo.InvariantCulture);
		}

		public float GetAmountWithoutDiscount()
		{
			return string.IsNullOrEmpty(amount_without_discount)
				? 0F
				: float.Parse(amount_without_discount, CultureInfo.InvariantCulture);
		}
	}
}