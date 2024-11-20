using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Demo
{
	public class UserSubscriptions : MonoSingleton<UserSubscriptions>
	{
		private List<TimeLimitedItem> _items = new List<TimeLimitedItem>();

		public List<TimeLimitedItem> GetItems() => _items;

		public bool IsEmpty()
		{
			return !GetItems().Any();
		}

		public bool IsSubscription(string sku)
		{
			return GetItems().Exists(i => i.sku == sku);
		}

		public void UpdateSupscriptions(Action<List<TimeLimitedItem>> onSuccess = null, Action<Error> onError = null)
		{
			XsollaInventory.GetTimeLimitedItems(items =>
				{
					_items = items.items.ToList();
					onSuccess?.Invoke(GetItems());
				},
				onError);
		}
	}
}
