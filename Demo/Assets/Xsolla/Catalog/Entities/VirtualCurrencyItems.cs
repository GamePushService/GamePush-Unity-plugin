using System;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	[Serializable]
	public class VirtualCurrencyItems
	{
		public bool has_more;
		public VirtualCurrencyItem[] items;
	}

	[Serializable]
	public class VirtualCurrencyItem : StoreItem { }
}