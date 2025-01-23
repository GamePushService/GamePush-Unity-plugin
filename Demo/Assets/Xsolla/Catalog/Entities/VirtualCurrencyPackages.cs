using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	[Serializable]
	public class VirtualCurrencyPackages
	{
		public bool has_more;
		public VirtualCurrencyPackage[] items;
	}

	[Serializable]
	public class VirtualCurrencyPackage : StoreItem
	{
		public string bundle_type;
		public List<Content> content;

		[Serializable]
		public class Content
		{
			public string sku;
			public string name;
			public string type;
			public string description;
			public string image_url;
			public int quantity;
			public InventoryOptions inventory_options;
		}
	}
}