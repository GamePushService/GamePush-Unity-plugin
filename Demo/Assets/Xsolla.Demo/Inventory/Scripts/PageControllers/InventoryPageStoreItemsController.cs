using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public class InventoryPageStoreItemsController : BasePageStoreItemsController
	{
		[SerializeField] private SimpleButton RefreshInventoryButton = default;
	
		private string _lastGroup;

		protected override bool IsShowContent => UserInventory.Instance.HasVirtualItems || UserInventory.Instance.HasPurchasedSubscriptions;

		protected override void Initialize()
		{
			UserInventory.Instance.RefreshEvent += OnUserInventoryRefresh;

			if (RefreshInventoryButton)
				RefreshInventoryButton.onClick += RefreshInventory;
		}

		private void OnDestroy()
		{
			//If UserInventory is not in destroying state (on app exit)
			if (UserInventory.Instance)
			{
				UserInventory.Instance.RefreshEvent -= OnUserInventoryRefresh;

				if (RefreshInventoryButton)
					RefreshInventoryButton.onClick -= RefreshInventory;
			}
		}

		private void RefreshInventory()
		{
			var isDone = false;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => isDone);

			UserInventory.Instance.Refresh(
				() => isDone = true,
				error =>
				{
					isDone = true;
					StoreDemoPopup.ShowError(error);
				}
			);
		}

		private void OnUserInventoryRefresh()
		{
			base.UpdatePage(_lastGroup);
		}

		protected override void InitializeItemUI(GameObject item, ItemModel model)
		{
			item.GetComponent<InventoryItemUI>().Initialize(model);
		}

		protected override List<ItemModel> GetItemsByGroup(string groupName)
		{
			_lastGroup = groupName;

			var isGroupAll = groupName.Equals(GROUP_ALL);

			return UserInventory.Instance.AllItems.Where(i =>
			{
				if (i.IsVirtualCurrency())
					return false;

				if (i.IsSubscription())
				{
					var model = UserInventory.Instance.Subscriptions.First(x => x.Sku.Equals(i.Sku));
					if (!(model.Status != UserSubscriptionModel.SubscriptionStatusType.None && model.Expired.HasValue))
					{
						return false; //This is a non-purchased subscription
					}
				}

				var itemGroups = i.Groups;

				if (base.CheckHideInAttribute(i, HideInFlag.Inventory))
					return false; //This item must be hidden by attribute

				if (isGroupAll)
					return true;
				else
					return itemGroups.Any(g => g.Equals(groupName));
			}).ToList();
		}

		protected override List<string> GetGroups()
		{
			var items = UserInventory.Instance.AllItems;

			var itemGroups = new HashSet<string>();

			foreach (var item in items)
			{
				if (item.IsSubscription() && item is UserSubscriptionModel subscription && subscription.Status == UserSubscriptionModel.SubscriptionStatusType.None)
				{
					//Do nothing, skip this item
					continue;
				}
				else
				{
					var currentItemGroups = item.Groups;

					foreach (var group in currentItemGroups)
						itemGroups.Add(group);
				}
			}

			return itemGroups.ToList();
		}
	}
}
