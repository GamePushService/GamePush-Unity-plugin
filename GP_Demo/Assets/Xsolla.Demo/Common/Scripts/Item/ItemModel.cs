using System;
using System.Collections.Generic;

namespace Xsolla.Demo
{
	[Serializable]
	public abstract class ItemModel
	{
		public string Sku;
		public string Name;
		public string Description;
		public string LongDescription;
		public string ImageUrl;
		public bool IsConsumable;
		public KeyValuePair<string, string>[] Attributes;
		public List<string> Groups;

		public abstract bool IsVirtualCurrency();
		public abstract bool IsSubscription();
		public abstract bool IsBundle();
	}
}
