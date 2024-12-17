using System;

namespace Xsolla.Core
{
	[Serializable]
	public class VirtualPrice
	{
		public string sku;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public string amount;
		public string amount_without_discount;
		public CalculatedPrice calculated_price;
		public bool is_default;

		public int GetAmount()
		{
			return int.Parse(amount.ToLowerInvariant());
		}

		public int GetAmountWithoutDiscount()
		{
			return int.Parse(amount_without_discount.ToLowerInvariant());
		}

		public string GetAmountRaw()
		{
			return calculated_price.amount;
		}

		public string GetAmountWithoutDiscountRaw()
		{
			return calculated_price.amount_without_discount;
		}

		[Serializable]
		public class CalculatedPrice
		{
			public string amount = default;
			public string amount_without_discount = default;
		}
	}
}