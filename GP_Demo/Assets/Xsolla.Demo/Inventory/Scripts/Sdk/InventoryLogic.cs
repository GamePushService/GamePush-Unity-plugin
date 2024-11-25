using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Demo
{
	public class InventoryLogic : MonoSingleton<InventoryLogic>
    {
		public void GetInventoryItems(Action<List<InventoryItemModel>> onSuccess, Action<Error> onError = null)
		{
			XsollaInventory.GetInventoryItems(items =>
			{
				var inventoryItems = items.items.Where(i => !i.type.Equals("virtual_currency") && i.VirtualItemType != VirtualItemType.NonRenewingSubscription).Select(
					i => new InventoryItemModel
					{
						Sku = i.sku,
						Description = i.description,
						Name = i.name,
						ImageUrl = i.image_url,
						IsConsumable = i.VirtualItemType == VirtualItemType.Consumable,
						InstanceId = i.instance_id,
						RemainingUses = (uint?)i.quantity,
						Attributes = ItemInfoConverter.ConvertAttributes(i.attributes),
						Groups = ItemInfoConverter.ConvertGroups(i.groups)
					}).ToList();
				onSuccess?.Invoke(inventoryItems);
			}, onError);
		}

		public void GetVirtualCurrencyBalance(Action<List<VirtualCurrencyBalanceModel>> onSuccess, Action<Error> onError = null)
		{
			XsollaInventory.GetVirtualCurrencyBalance(balances =>
			{
				var result = balances.items.ToList().Select(b => new VirtualCurrencyBalanceModel
				{
					Sku = b.sku,
					Description = b.description,
					Name = b.name,
					ImageUrl = b.image_url,
					IsConsumable = false,
					Amount = b.amount
				}).ToList();
				onSuccess?.Invoke(result);
			}, onError);
		}

		public void GetUserSubscriptions(Action<List<UserSubscriptionModel>> onSuccess, Action<Error> onError = null)
		{
			XsollaInventory.GetTimeLimitedItems(items =>
			{
				var subscriptionItems = items.items.Select(i => new UserSubscriptionModel
				{
					Sku = i.sku,
					Description = i.description,
					Name = i.name,
					ImageUrl = i.image_url,
					IsConsumable = false,
					Status = GetSubscriptionStatus(i.status),
					Expired = i.expired_at.HasValue ? UnixTimeToDateTime(i.expired_at.Value) : (DateTime?)null,
					Groups = ItemInfoConverter.ConvertGroups(null)
				}).ToList();
				onSuccess?.Invoke(subscriptionItems);
			}, onError);
		}

		public void ConsumeInventoryItem(InventoryItemModel item, int? count = 1, Action<InventoryItemModel> onSuccess = null, Action<Error> onError = null)
		{
			var consumeItem = new ConsumeItem
			{
				sku = item.Sku,
				instance_id = item.InstanceId,
				quantity = count
			};

			XsollaInventory.ConsumeInventoryItem(consumeItem,
				onSuccess: () => onSuccess?.Invoke(item),
				onError: onError);
		}

		public void RedeemCouponCode(string couponCode, Action<List<CouponRedeemedItemModel>> onSuccess, Action<Error> onError)
		{
			XsollaCatalog.RedeemCouponCode(
				couponCode,
				onSuccess: redeemedItems =>
				{
					var redeemedItemModels = redeemedItems.items.Select(
						i => new CouponRedeemedItemModel
						{
							Sku = i.sku,
							Description = i.description,
							Name = i.name,
							ImageUrl = i.image_url,
							Quantity = i.quantity,
						}).ToList();
					onSuccess?.Invoke(redeemedItemModels);
				},
				onError: onError);
		}

		private UserSubscriptionModel.SubscriptionStatusType GetSubscriptionStatus(string status)
		{
			if (string.IsNullOrEmpty(status)) return UserSubscriptionModel.SubscriptionStatusType.None;
			switch (status)
			{
				case "active": return UserSubscriptionModel.SubscriptionStatusType.Active;
				case "expired": return UserSubscriptionModel.SubscriptionStatusType.Expired;
				default: return UserSubscriptionModel.SubscriptionStatusType.None;
			}
		}

		private DateTime UnixTimeToDateTime(long unixTime)
		{
			DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
			dtDateTime = dtDateTime.AddSeconds(unixTime).ToLocalTime();
			return dtDateTime;
		}
	}
}
