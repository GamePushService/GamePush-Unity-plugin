using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class CatalogLogic : MonoSingleton<CatalogLogic>
	{
		private const uint CATALOG_CACHE_TIMEOUT = 500;

		private List<StoreItem> _itemsCache;
		private DateTime _itemsCacheTime = DateTime.Now;
		private bool _refreshItemsInProgress;

		private List<CatalogBundleItemModel> _bundlesCache;
		private DateTime _bundlesCacheTime = DateTime.Now;
		private bool _refreshBundlesInProgress;

		public void GetVirtualCurrencies(Action<List<VirtualCurrencyModel>> onSuccess, Action<Error> onError = null)
		{
			XsollaCatalog.GetVirtualCurrencyList(items =>
			{
				var currencies = items.items.ToList();
				if (currencies.Any())
				{
					var result = currencies.Select(c =>
					{
						var model = new VirtualCurrencyModel();
						FillItemModel(model, c);
						return model;
					}).ToList();
					onSuccess?.Invoke(result);
				}
				else onSuccess?.Invoke(new List<VirtualCurrencyModel>());
			}, onError);
		}

		public void GetCatalogVirtualItems(Action<List<CatalogVirtualItemModel>> onSuccess, Action<Error> onError = null)
		{
			RequestStoreItems(items =>
			{
				var virtualItems = new List<CatalogVirtualItemModel>();

				items.Where(i => i.VirtualItemType != VirtualItemType.NonRenewingSubscription).ToList().ForEach(i =>
				{
					virtualItems.Add(new CatalogVirtualItemModel {
						IsConsumable = i.VirtualItemType == VirtualItemType.Consumable
					});
					FillCatalogItem(virtualItems.Last(), i);
				});
				onSuccess?.Invoke(virtualItems);
			}, onError);
		}

		public void GetCatalogVirtualCurrencyPackages(Action<List<CatalogVirtualCurrencyModel>> onSuccess, Action<Error> onError = null)
		{
			XsollaCatalog.GetVirtualCurrencyPackagesList(packages =>
			{
				var currencies = new List<CatalogVirtualCurrencyModel>();
				foreach (var package in packages.items)
				{
					var currency = new CatalogVirtualCurrencyModel {
						IsConsumable = package.VirtualItemType == VirtualItemType.Consumable,
						Amount = (uint) package.content.First().quantity,
						CurrencySku = package.content.First().sku
					};
					FillCatalogItem(currency, package);
					currencies.Add(currency);
				}

				onSuccess?.Invoke(currencies);
			}, onError);
		}

		public void GetCatalogSubscriptions(Action<List<CatalogSubscriptionItemModel>> onSuccess, Action<Error> onError = null)
		{
			RequestStoreItems(items =>
			{
				var subscriptionItems = new List<CatalogSubscriptionItemModel>();
				items.Where(i => i.VirtualItemType == VirtualItemType.NonRenewingSubscription).ToList().ForEach(i =>
				{
					subscriptionItems.Add(new CatalogSubscriptionItemModel {
						IsConsumable = false,
						ExpirationPeriod = i.inventory_options.expiration_period.ToTimeSpan(),
						ExpirationPeriodText = i.inventory_options.expiration_period.ToString()
					});
					FillCatalogItem(subscriptionItems.Last(), i);
				});
				onSuccess?.Invoke(subscriptionItems);
			}, onError);
		}

		public void GetCatalogBundles(Action<List<CatalogBundleItemModel>> onSuccess, Action<Error> onError = null)
		{
			if (_bundlesCache == null || (DateTime.Now - _bundlesCacheTime).TotalMilliseconds > CATALOG_CACHE_TIMEOUT)
			{
				if (!_refreshBundlesInProgress)
				{
					XsollaCatalog.GetBundles(bundles =>
					{
						var bundleItems = new List<CatalogBundleItemModel>();
						bundles.items.ToList().ForEach(b =>
						{
							bundleItems.Add(new CatalogBundleItemModel {
								Sku = b.sku,
								Name = b.name,
								Description = b.description,
								ImageUrl = b.image_url
							});
							FillBundleItem(bundleItems.Last(), b);
							bundleItems.Last().Content = new List<BundleContentItem>();
						});
						_bundlesCache = bundleItems;

						bundles.items.ToList().ForEach(b =>
						{
							var model = _bundlesCache.First(c => c.Sku.Equals(b.sku));
							b.content.ToList().ForEach(c =>
							{
								if (_itemsCache.Any(i => i.sku.Equals(c.sku)))
								{
									var item = _itemsCache.First(i => i.sku.Equals(c.sku));
									var catalogItem = new CatalogVirtualItemModel {
										IsConsumable = item.VirtualItemType == VirtualItemType.Consumable
									};
									model.Content.Add(new BundleContentItem(catalogItem, c.quantity));
									FillCatalogItem(model.Content.Last().Item, item);
								}
								else if (_bundlesCache.Any(i => i.Sku.Equals(c.sku)))
								{
									var item = _bundlesCache.First(i => i.Sku.Equals(c.sku));
									model.Content.Add(new BundleContentItem(item, c.quantity));
								}
							});
						});

						_bundlesCacheTime = DateTime.Now;
						_refreshBundlesInProgress = false;
						onSuccess?.Invoke(bundleItems);
					}, onError);
				}
				else StartCoroutine(WaitBundlesCoroutine(onSuccess));
			}
			else onSuccess?.Invoke(_bundlesCache);
		}

		private void RequestStoreItems(Action<List<StoreItem>> onSuccess, Action<Error> onError = null)
		{
			if (_itemsCache == null || (DateTime.Now - _itemsCacheTime).TotalMilliseconds > CATALOG_CACHE_TIMEOUT)
			{
				if (!_refreshItemsInProgress)
				{
					_refreshItemsInProgress = true;
					XsollaCatalog.GetCatalog(items =>
					{
						_refreshItemsInProgress = false;
						_itemsCacheTime = DateTime.Now;
						_itemsCache = items.items.ToList();
						onSuccess?.Invoke(_itemsCache);
					}, onError);
				}
				else
					StartCoroutine(WaitItemsCoroutine(onSuccess));
			}
			else
				onSuccess?.Invoke(_itemsCache);
		}

		private IEnumerator WaitItemsCoroutine(Action<List<StoreItem>> onSuccess)
		{
			yield return new WaitWhile(() => _refreshItemsInProgress);
			onSuccess?.Invoke(_itemsCache);
		}

		private IEnumerator WaitBundlesCoroutine(Action<List<CatalogBundleItemModel>> onSuccess)
		{
			yield return new WaitWhile(() => _refreshBundlesInProgress);
			onSuccess?.Invoke(_bundlesCache);
		}

		private void FillItemModel(ItemModel model, StoreItem item)
		{
			model.Sku = item.sku;
			model.Name = item.name;
			model.Description = item.description;
			model.LongDescription = item.long_description;
			model.ImageUrl = item.image_url;
			model.Attributes = ItemInfoConverter.ConvertAttributes(item.attributes);
			model.Groups = ItemInfoConverter.ConvertGroups(item.groups);
		}

		private void FillCatalogItem(CatalogItemModel model, StoreItem item)
		{
			FillItemModel(model, item);

			model.RealPrice = GetRealPrice(item, out var realPriceWithoutDiscount);
			model.RealPriceWithoutDiscount = realPriceWithoutDiscount;

			model.VirtualPrice = GetVirtualPrice(item, out var virtualPriceWithoutDiscount);
			model.VirtualPriceWithoutDiscount = virtualPriceWithoutDiscount;
		}

		private void FillBundleItem(CatalogBundleItemModel model, BundleItem item)
		{
			model.Sku = item.sku;
			model.Name = item.name;
			model.Description = item.description;
			model.ImageUrl = item.image_url;
			model.Attributes = ItemInfoConverter.ConvertAttributes(item.attributes);
			model.Groups = ItemInfoConverter.ConvertGroups(item.groups);

			model.RealPrice = GetBundleRealPrice(item, out var realPriceWithoutDiscount);
			model.RealPriceWithoutDiscount = realPriceWithoutDiscount;

			model.VirtualPrice = GetBundleVirtualPrice(item, out var virtualPriceWithoutDiscount);
			model.VirtualPriceWithoutDiscount = virtualPriceWithoutDiscount;

			model.ContentRealPrice = GetBundleContentRealPrice(item, out var contentRealPriceWithoutDiscount);
			model.ContentRealPriceWithoutDiscount = contentRealPriceWithoutDiscount;

			model.ContentVirtualPrice = GetBundleContentVirtualPrice(item, out var contentVirtualPriceWithoutDiscount);
			model.ContentVirtualPriceWithoutDiscount = contentVirtualPriceWithoutDiscount;
		}

		private KeyValuePair<string, float>? GetRealPrice(StoreItem item, out KeyValuePair<string, float>? priceWithoutDiscount)
		{
			if (item.price == null)
			{
				priceWithoutDiscount = null;
				return null;
			}

			priceWithoutDiscount = new KeyValuePair<string, float>(item.price.currency, item.price.GetAmountWithoutDiscount());
			return new KeyValuePair<string, float>(item.price.currency, item.price.GetAmount());
		}

		private KeyValuePair<string, int>? GetVirtualPrice(StoreItem item, out KeyValuePair<string, int>? priceWithoutDiscount)
		{
			var virtualPrices = item.virtual_prices.ToList();

			if (!virtualPrices.Any())
			{
				priceWithoutDiscount = null;
				return null;
			}

			var virtualPrice = virtualPrices.Count(v => v.is_default) == 0
				? virtualPrices.First()
				: virtualPrices.First(v => v.is_default);

			priceWithoutDiscount = new KeyValuePair<string, int>(virtualPrice.sku, virtualPrice.GetAmountWithoutDiscount());
			return new KeyValuePair<string, int>(virtualPrice.sku, virtualPrice.GetAmount());
		}

		private KeyValuePair<string, float>? GetBundleRealPrice(BundleItem item, out KeyValuePair<string, float>? priceWithoutDiscount)
		{
			if (item.price == null)
			{
				priceWithoutDiscount = null;
				return null;
			}

			priceWithoutDiscount = new KeyValuePair<string, float>(item.price.currency, item.price.GetAmountWithoutDiscount());
			return new KeyValuePair<string, float>(item.price.currency, item.price.GetAmount());
		}

		private KeyValuePair<string, int>? GetBundleVirtualPrice(BundleItem item, out KeyValuePair<string, int>? priceWithoutDiscount)
		{
			var virtualPrices = item.virtual_prices.ToList();

			if (!virtualPrices.Any())
			{
				priceWithoutDiscount = null;
				return null;
			}

			var virtualPrice = virtualPrices.Count(v => v.is_default) == 0
				? virtualPrices.First()
				: virtualPrices.First(v => v.is_default);

			priceWithoutDiscount = new KeyValuePair<string, int>(virtualPrice.sku, virtualPrice.GetAmountWithoutDiscount());
			return new KeyValuePair<string, int>(virtualPrice.sku, virtualPrice.GetAmount());
		}

		private KeyValuePair<string, float>? GetBundleContentRealPrice(BundleItem bundle, out KeyValuePair<string, float>? priceWithoutDiscount)
		{
			if (bundle.total_content_price == null)
			{
				priceWithoutDiscount = null;
				return null;
			}

			priceWithoutDiscount = new KeyValuePair<string, float>(bundle.total_content_price.currency, bundle.total_content_price.GetAmountWithoutDiscount());
			return new KeyValuePair<string, float>(bundle.total_content_price.currency, bundle.total_content_price.GetAmount());
		}

		private KeyValuePair<string, int>? GetBundleContentVirtualPrice(BundleItem bundle, out KeyValuePair<string, int>? priceWithoutDiscount)
		{
			var bundleContent = bundle.content.ToList();

			if (!bundleContent.Any())
			{
				priceWithoutDiscount = null;
				return null;
			}

			var contentVirtualPrice = 0;
			var contentVirtualPriceWithoutDiscount = 0;
			var virtualCurrency = "";

			foreach (var bundleContentItem in bundleContent)
			{
				var virtualPrices = bundleContentItem.virtual_prices.ToList();

				if (!virtualPrices.Any())
				{
					priceWithoutDiscount = null;
					return null;
				}

				var virtualPrice = virtualPrices.Count(v => v.is_default) == 0
					? virtualPrices.First()
					: virtualPrices.First(v => v.is_default);

				if (string.IsNullOrEmpty(virtualCurrency))
				{
					virtualCurrency = virtualPrice.sku;
				}

				if (virtualCurrency != virtualPrice.sku)
				{
					priceWithoutDiscount = null;
					return null;
				}

				contentVirtualPrice += virtualPrice.GetAmount();
				contentVirtualPriceWithoutDiscount += virtualPrice.GetAmountWithoutDiscount();
			}

			priceWithoutDiscount = new KeyValuePair<string, int>(virtualCurrency, contentVirtualPriceWithoutDiscount);
			return new KeyValuePair<string, int>(virtualCurrency, contentVirtualPrice);
		}
	}
}