using System;

namespace Xsolla.Catalog
{
	[Serializable]
	public class CouponRedeemedItems
	{
		public CouponRedeemedItem[] items;
	}

	[Serializable]
	public class CouponRedeemedItem
	{
		public string sku;
		public Group[] groups;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public int quantity;
		public bool is_free;

		[Serializable]
		public class Group
		{
			public string external_id;
			public string name;
		}
	}
}