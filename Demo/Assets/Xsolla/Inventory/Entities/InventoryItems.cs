using System;
using Xsolla.Core;

namespace Xsolla.Inventory
{
	[Serializable]
	public class InventoryItems
	{
		public InventoryItem[] items;
	}

	[Serializable]
	public class InventoryItem
	{
		public string sku;
		public string name;
		public string description;
		//Public object attributes; Don't use it yet.
		public string type;
		public string virtual_item_type;
		public StoreItemAttribute[] attributes;
		public StoreItemGroup[] groups;
		public string image_url;
		public int? quantity;
		public int? remaining_uses;
		public string instance_id;

		public VirtualItemType VirtualItemType
		{
			get
			{
				if (!string.IsNullOrEmpty(type) && type.Equals("virtual_currency"))
					return VirtualItemType.VirtualCurrency;

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
	}
}