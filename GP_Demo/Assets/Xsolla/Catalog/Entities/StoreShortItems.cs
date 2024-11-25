using System;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	[Serializable]
	public class StoreShortItems
	{
		public StoreShortItem[] items;
	}

	[Serializable]
	public class StoreShortItem
	{
		public string sku;
		public string name;
		public string description;
		public StoreItemGroup[] groups;
		public StoreItemPromotion[] promotions;
	}
}