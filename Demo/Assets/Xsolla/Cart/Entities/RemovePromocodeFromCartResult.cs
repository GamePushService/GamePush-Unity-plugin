using System;
using Xsolla.Core;

namespace Xsolla.Cart
{
	[Serializable]
	public class RemovePromocodeFromCartResult
	{
		public string cart_id;
		public Price price;
		public bool is_free;
		public Item[] items;

		[Serializable]
		public class Item
		{
			public string sku;
			public StoreItemGroup[] groups;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public int quantity;
			public bool is_free;
		}
	}
}