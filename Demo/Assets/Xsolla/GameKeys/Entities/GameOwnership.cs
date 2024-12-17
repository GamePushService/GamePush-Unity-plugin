using System;
using Xsolla.Core;

namespace Xsolla.GameKeys
{
	[Serializable]
	public class GameOwnership
	{
		public bool has_more;
		public int total_items_count;
		public Item[] items;

		[Serializable]
		public class Item
		{
			public string name;
			public string description;
			public int project_id;
			public string game_sku;
			public string drm;
			public string image_url;
			public bool is_pre_order;
			public StoreItemAttribute[] attributes;
		}
	}
}