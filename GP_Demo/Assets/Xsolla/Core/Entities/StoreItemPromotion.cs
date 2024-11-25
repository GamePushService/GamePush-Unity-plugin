using System;

namespace Xsolla.Core
{
	[Serializable]
	public class StoreItemPromotion
	{
		public string name;
		public string date_start;
		public string date_end;
		public Discount discount;
		public Bonus[] bonus;
		public PromotionLimits limits;

		[Serializable]
		public class Discount
		{
			public string percent;
			public string value;
		}

		[Serializable]
		public class Bonus
		{
			public string sku;
			public int quantity;
		}

		[Serializable]
		public class PromotionLimits
		{
			public PerUser per_user;

			[Serializable]
			public class PerUser
			{
				public int available;
				public int total;
			}
		}
	}
}