using System;

namespace Xsolla.Core
{
	[Serializable]
	public class StoreItem
	{
		public string sku;
		public string name;
		public StoreItemGroup[] groups;
		public StoreItemAttribute[] attributes;
		public string type;
		public string virtual_item_type;
		public string description;
		public string long_description;
		public string image_url;
		public bool is_free;
		public Price price;
		public VirtualPrice[] virtual_prices;
		public InventoryOptions inventory_options;
		public int order;
		public MediaListItem[] media_list;
		public StoreItemPromotion[] promotions;
		public StoreItemLimits limits;

		public VirtualItemType VirtualItemType
		{
			get
			{
				if (string.IsNullOrEmpty(virtual_item_type))
					return VirtualItemType.None;

				switch (virtual_item_type)
				{
					case "consumable": return VirtualItemType.Consumable;
					case "non_consumable": return VirtualItemType.NonConsumable;
					case "non_renewing_subscription": return VirtualItemType.NonRenewingSubscription;
					default: return VirtualItemType.None;
				}
			}
		}

		[Serializable]
		public class InventoryOptions
		{
			public ConsumableOption consumable;
			public ExpirationPeriod expiration_period;

			[Serializable]
			public class ConsumableOption
			{
				public int? usages_count;
			}

			[Serializable]
			public class ExpirationPeriod
			{
				public string type;
				public int value;

				public TimeSpan ToTimeSpan()
				{
					var dt = DateTime.Now;
					switch (type)
					{
						case "minute":
							dt = dt.AddMinutes(value);
							break;
						case "hour":
							dt = dt.AddHours(value);
							break;
						case "day":
							dt = dt.AddDays(value);
							break;
						case "week":
							dt = dt.AddDays(7 * value);
							break;
						case "month":
							dt = dt.AddMonths(value);
							break;
						case "year":
							dt = dt.AddYears(value);
							break;
					}

					return dt - DateTime.Now;
				}

				public override string ToString()
				{
					var result = $"{value} {type}";
					if (value > 1)
						result += "s";
					return result;
				}
			}
		}
	}
}