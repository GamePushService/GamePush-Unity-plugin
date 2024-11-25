using System;
using Newtonsoft.Json;

namespace Xsolla.Inventory
{
	[Serializable]
	public class TimeLimitedItems
	{
		public TimeLimitedItem[] items;
	}

	[Serializable]
	public class TimeLimitedItem
	{
		public string sku;
		public string name;
		public string type;
		public string description;
		public string image_url;
		public string status;
		public long? expired_at;
		public string virtual_item_type;
		[JsonProperty("class")]
		public string subscription_class;

		public SubscriptionStatusType Status
		{
			get
			{
				if (string.IsNullOrEmpty(status))
					return SubscriptionStatusType.None;

				switch (status)
				{
					case "none": return SubscriptionStatusType.None;
					case "active": return SubscriptionStatusType.Active;
					case "expired": return SubscriptionStatusType.Expired;
					default: return SubscriptionStatusType.None;
				}
			}
		}
	}
}