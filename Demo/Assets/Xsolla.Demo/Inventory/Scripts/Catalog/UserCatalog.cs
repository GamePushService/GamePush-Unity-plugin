using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Catalog;


namespace Xsolla.Demo
{
	public partial class UserCatalog : MonoSingleton<UserCatalog>
	{
		public event Action<List<VirtualCurrencyModel>> UpdateVirtualCurrenciesEvent;
		public event Action<List<CatalogVirtualItemModel>> UpdateItemsEvent;
		public event Action<List<CatalogVirtualCurrencyModel>> UpdateVirtualCurrencyPackagesEvent;
		public event Action<List<CatalogBundleItemModel>> UpdateBundlesEvent;
		public event Action<List<CatalogSubscriptionItemModel>> UpdateSubscriptionsEvent;

		public List<VirtualCurrencyModel> VirtualCurrencies { get; private set; }
		public List<CatalogItemModel> AllItems { get; private set; }
		public List<CatalogVirtualItemModel> VirtualItems { get; private set; }
		public List<CatalogVirtualCurrencyModel> CurrencyPackages { get; private set; }
		public List<CatalogBundleItemModel> Bundles { get; private set; }
		public List<CatalogSubscriptionItemModel> Subscriptions { get; private set; }
		
		public bool HasVirtualItems => VirtualItems.Any();
		public bool HasBundles => Bundles.Any();
		public bool HasCurrencyPackages => CurrencyPackages.Any();

		public override void Init()
		{
			base.Init();
			VirtualCurrencies = new List<VirtualCurrencyModel>();
			AllItems = new List<CatalogItemModel>();
			VirtualItems = new List<CatalogVirtualItemModel>();
			CurrencyPackages = new List<CatalogVirtualCurrencyModel>();
			Bundles = new List<CatalogBundleItemModel>();
			Subscriptions = new List<CatalogSubscriptionItemModel>();
		}

		public void UpdateItems([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			IsUpdated = false;
			StartCoroutine(UpdateItemsCoroutine(onSuccess, onError));
		}

		IEnumerator UpdateItemsCoroutine([CanBeNull] Action onSuccess = null, Action<Error> onError = null)
		{
			var isError = false;
			Action<Error> errorCallback = error =>
			{
				isError = true;
				onError?.Invoke(error);
			};

			yield return StartCoroutine(UpdateVirtualCurrenciesCoroutine(errorCallback));
			yield return StartCoroutine(UpdateVirtualItemsCoroutine(errorCallback));
			yield return StartCoroutine(UpdateVirtualCurrencyPackagesCoroutine(errorCallback));
			yield return StartCoroutine(UpdateSubscriptionsCoroutine(errorCallback));
			yield return StartCoroutine(UpdateBundlesCoroutine(errorCallback));
		
			if (isError) yield break;
			IsUpdated = true;
			onSuccess?.Invoke();
		}

		private IEnumerator UpdateSomeItemsCoroutine<T>(Action<Action<List<T>>, Action<Error>> method,  Action<List<T>> onSuccess, Action<Error> onError = null)
			where T: CatalogItemModel
		{
			var busy = true;
			if (method != null && XsollaToken.Exists)
				method.Invoke(items =>
				{
					busy = false;
					AddUniqueItemsFrom(items);
					onSuccess?.Invoke(items);
				}, onError);
			else 
				busy = false;
			yield return new WaitWhile(() => busy);
		}

		private void AddUniqueItemsFrom<T>(List<T> items) where T : CatalogItemModel
		{
			var uniqueItems = items.Where(i => !AllItems.Any(a => a.Sku.Equals(i.Sku))).ToList();
			if(uniqueItems.Any())
				AllItems.AddRange(uniqueItems);
		}

		private IEnumerator UpdateVirtualCurrenciesCoroutine(Action<Error> onError = null)
		{
			var busy = true;
			CatalogLogic.Instance.GetVirtualCurrencies(items =>
			{
				VirtualCurrencies = items;
				UpdateVirtualCurrenciesEvent?.Invoke(items);
				busy = false;
			}, onError);
			yield return new WaitWhile(() => busy);
		}
	
		private IEnumerator UpdateVirtualItemsCoroutine(Action<Error> onError = null)
		{
			yield return StartCoroutine(UpdateSomeItemsCoroutine<CatalogVirtualItemModel>(
				CatalogLogic.Instance.GetCatalogVirtualItems, items =>
				{
					VirtualItems = items;
					UpdateItemsEvent?.Invoke(items);
				}, onError));
		}

		private IEnumerator UpdateVirtualCurrencyPackagesCoroutine(Action<Error> onError = null)
		{
			yield return StartCoroutine(UpdateSomeItemsCoroutine<CatalogVirtualCurrencyModel>(
			CatalogLogic.Instance.GetCatalogVirtualCurrencyPackages, items =>
			{
				CurrencyPackages = items;
				UpdateVirtualCurrencyPackagesEvent?.Invoke(items);
			}, onError));
		}
	
		private IEnumerator UpdateBundlesCoroutine(Action<Error> onError = null)
		{
			yield return StartCoroutine(UpdateSomeItemsCoroutine<CatalogBundleItemModel>(
				CatalogLogic.Instance.GetCatalogBundles, items =>
				{
					Bundles = items;
					UpdateBundlesEvent?.Invoke(items);
				}, onError));
		}
	
		private IEnumerator UpdateSubscriptionsCoroutine(Action<Error> onError = null)
		{
			yield return StartCoroutine(UpdateSomeItemsCoroutine<CatalogSubscriptionItemModel>(
			CatalogLogic.Instance.GetCatalogSubscriptions, items =>
			{
				Subscriptions = items;
				UpdateSubscriptionsEvent?.Invoke(items);
			}, onError));
		}
	}
}
