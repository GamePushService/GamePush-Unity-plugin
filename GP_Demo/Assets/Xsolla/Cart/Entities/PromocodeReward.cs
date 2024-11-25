using System;

namespace Xsolla.Cart
{
	[Serializable]
	public class PromocodeReward
	{
		public BonusItem[] bonus;
		public Discount discount;
		public DiscountedItems discounted_items;
		public bool is_selectable;

		[Serializable]
		public class BonusItem
		{
			public RewardItem item;
			public int quantity;
		}

		[Serializable]
		public class Discount
		{
			public string percent;
		}

		[Serializable]
		public class DiscountedItems
		{
			public string sku;
			public Discount discount;
		}

		[Serializable]
		public class RewardItem
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public UnitItem[] unit_items;
		}

		[Serializable]
		public class UnitItem
		{
			public string sku;
			public string type;
			public string name;
			public string drm_name;
			public string drm_sku;
		}
	}
}