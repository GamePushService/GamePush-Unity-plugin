using System;
using Xsolla.Core;

namespace Xsolla.GameKeys
{
	[Serializable]
	public class GameKeyItems
	{
		public GameKeyItem[] items;

		[Serializable]
		public class GameKeyItem
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public bool is_free;
			public Price price;
			public VirtualPrice[] virtual_prices;
			public string drm_name;
			public string drm_sku;
			public bool has_keys;
			public bool is_pre_order;
			public string release_date;
			public StoreItemGroup[] groups;
			public StoreItemAttribute[] attributes;
		}
	}
}