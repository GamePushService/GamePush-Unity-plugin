using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class UserInventory : MonoSingleton<UserInventory>
	{
		public event Action RefreshEvent;
		public event Action<List<InventoryItemModel>> UpdateItemsEvent;
		public event Action<List<VirtualCurrencyBalanceModel>> UpdateVirtualCurrencyBalanceEvent;
		public event Action<List<UserSubscriptionModel>> UpdateSubscriptionsEvent;

		public List<ItemModel> AllItems { get; private set; }
		public List<InventoryItemModel> VirtualItems { get; private set; }
		public List<VirtualCurrencyBalanceModel> Balance { get; private set; }
		public List<UserSubscriptionModel> Subscriptions { get; private set; }

		public bool HasVirtualItems => VirtualItems.Any();
		public bool HasPurchasedSubscriptions => Subscriptions.FindAll(s => s.Status != UserSubscriptionModel.SubscriptionStatusType.None).Any();

		public override void Init()
		{
			base.Init();
			IsUpdated = false;
			AllItems = new List<ItemModel>();
			VirtualItems = new List<InventoryItemModel>();
			Balance = new List<VirtualCurrencyBalanceModel>();
			Subscriptions = new List<UserSubscriptionModel>();
		}

		partial void RefreshInventory(Action onSuccess, [CanBeNull] Action<Error> onError)
		{
			IsUpdated = false;
			StartCoroutine(WaitItemUpdatingCoroutine(onSuccess, onError));
		}

		private IEnumerator WaitItemUpdatingCoroutine(Action onSuccess = null, Action<Error> onError = null)
		{
			var balanceUpdated = false;
			var itemsUpdated = false;
			var subscriptionsUpdated = false;

			if (!XsollaToken.Exists)
				yield break;

			InventoryLogic.Instance.GetVirtualCurrencyBalance(balance =>
			{
				Balance = balance;
				balanceUpdated = true;
			}, onError);

			if (!XsollaToken.Exists)
				yield break;

			InventoryLogic.Instance.GetInventoryItems(items =>
			{
				VirtualItems = items.Where(i => !i.IsVirtualCurrency() && !i.IsSubscription()).ToList();
				itemsUpdated = true;
			}, onError);

			if (!XsollaToken.Exists)
				yield break;

			InventoryLogic.Instance.GetUserSubscriptions(items =>
			{
				Subscriptions = items;
				subscriptionsUpdated = true;
			}, onError);

			yield return new WaitUntil(() => balanceUpdated && itemsUpdated && subscriptionsUpdated);

			IsUpdated = true;
			HandleInventoryUpdate(onSuccess);
		}

		private void HandleInventoryUpdate(Action callback)
		{
			AllItems.Clear();
			AllItems.AddRange(VirtualItems);
			AllItems.AddRange(Subscriptions);

			UpdateVirtualCurrencyBalanceEvent?.Invoke(Balance);
			UpdateItemsEvent?.Invoke(VirtualItems);
			UpdateSubscriptionsEvent?.Invoke(Subscriptions);
			RefreshEvent?.Invoke();
			callback?.Invoke();
		}
	}
}