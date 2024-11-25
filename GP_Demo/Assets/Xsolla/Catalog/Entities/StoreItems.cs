using System;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	[Serializable]
	public class StoreItems
	{
		public bool has_more;
		public StoreItem[] items;
	}
}