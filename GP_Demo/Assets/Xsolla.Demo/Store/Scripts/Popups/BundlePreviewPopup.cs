using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	public class BundlePreviewPopup : MonoBehaviour, IBundlePreviewPopup
	{
		[SerializeField] Image bundleImage = default;
		[SerializeField] GameObject loadingCircle = default;
		[SerializeField] Text bundleName = default;
		[SerializeField] Text bundleDescription = default;
		[SerializeField] Text bundleInfo = default;
		[SerializeField] Text bundlePrice = default;
		[SerializeField] Text bundlePriceWithoutDiscount = default;
		[SerializeField] Image bundlePriceVcImage = default;
		[SerializeField] Text bundlePriceVc = default;
		[SerializeField] Image bundlePriceVcWithoutDiscountImage = default;
		[SerializeField] Text bundlePriceVcWithoutDiscount = default;

		[SerializeField] SimpleButton closeButton = default;
		[SerializeField] SimpleTextButton buyButton = default;

		[SerializeField] GameObject itemPrefab = default;
		[SerializeField] ItemContainer itemsContainer = default;

		public IBundlePreviewPopup SetBundleInfo(CatalogBundleItemModel bundle)
		{
			bundleName.text = bundle.Name;

			if (bundleDescription != null)
				bundleDescription.text = bundle.Description;

			bundleInfo.text = $"This bundle includes '{bundle.Content.Count}' item{(bundle.Content.Count > 1 ? "s" : "")}:";

			if (!string.IsNullOrEmpty(bundle.ImageUrl))
			{
				ImageLoader.LoadSprite(bundle.ImageUrl, LoadImageCallback);
			}
			else
			{
				XDebug.LogError($"Bundle with sku = '{bundle.Sku}' has no image!");
			}

			if (bundle.VirtualPrice != null)
				InitializeVirtualCurrencyPrice(bundle);
			else
				InitializeRealPrice(bundle);

			bundle.Content.ForEach(AddBundleContentItem);

			closeButton.onClick = () => { Destroy(gameObject, 0.001F); };

			AttachBuyButtonHandler(bundle);

			return this;
		}

		private void EnableRealPrice(bool bEnable)
		{
			bundlePrice.gameObject.SetActive(bEnable);
			bundlePriceWithoutDiscount.gameObject.SetActive(bEnable);
		}

		private void EnableVirtualCurrencyPrice(bool bEnable)
		{
			bundlePriceVcImage.gameObject.SetActive(bEnable);
			bundlePriceVc.gameObject.SetActive(bEnable);
			bundlePriceVcWithoutDiscountImage.gameObject.SetActive(bEnable);
			bundlePriceVcWithoutDiscount.gameObject.SetActive(bEnable);
		}

		private void InitializeVirtualCurrencyPrice(CatalogBundleItemModel bundle)
		{
			EnableRealPrice(false);
			EnableVirtualCurrencyPrice(true);

			bundlePriceVc.text = bundle.VirtualPrice?.Value.ToString();
			bundlePriceVcWithoutDiscount.text = bundle.ContentVirtualPriceWithoutDiscount?.Value.ToString();

			if (!bundle.ContentVirtualPriceWithoutDiscount.HasValue)
			{
				bundlePriceVcWithoutDiscountImage.gameObject.SetActive(false);
				bundlePriceVcWithoutDiscount.gameObject.SetActive(false);
			}

			InitializeVcImages(bundle);
		}

		private void InitializeRealPrice(CatalogBundleItemModel bundle)
		{
			EnableRealPrice(true);
			EnableVirtualCurrencyPrice(false);

			var realPrice = bundle.RealPrice;
			if (realPrice == null)
			{
				XDebug.LogError($"Bundle with sku = {bundle.Sku} has no price!");
				return;
			}

			var valuePair = realPrice.Value;
			var currency = valuePair.Key;
			var price = valuePair.Value;

			bundlePrice.text = PriceFormatter.FormatPrice(currency, price);

			var contentRealPrice = bundle.ContentRealPrice;
			if (contentRealPrice == null)
			{
				XDebug.LogError($"Bundle with sku = {bundle.Sku} has no content price!");
				return;
			}

			var contentValuePair = contentRealPrice.Value;
			var contentCurrency = contentValuePair.Key;
			var contentPrice = contentValuePair.Value;

			bundlePriceWithoutDiscount.text = PriceFormatter.FormatPrice(contentCurrency, contentPrice);
		}

		private void LoadImageCallback(Sprite sprite)
		{
			if (!bundleImage)
				return;

			loadingCircle.SetActive(false);
			bundleImage.gameObject.SetActive(true);
			bundleImage.sprite = sprite;
		}

		private void InitializeVcImages(CatalogBundleItemModel bundle)
		{
			var currencySku = bundle.VirtualPrice?.Key;
			var currency = UserCatalog.Instance.VirtualCurrencies.First(vc => vc.Sku.Equals(currencySku));
			if (!string.IsNullOrEmpty(currency.ImageUrl))
			{
				ImageLoader.LoadSprite(currency.ImageUrl, sprite =>
				{
					if (bundlePriceVcImage)
					{
						bundlePriceVcImage.sprite = sprite;
						bundlePriceVcWithoutDiscountImage.sprite = sprite;
					}
				});
			}
			else
			{
				XDebug.LogError($"Bundle with sku = '{bundle.Sku}' virtual currency price has no image!");
			}
		}

		private void AddBundleContentItem(BundleContentItem bundleContentItem)
		{
			itemsContainer.AddItem(itemPrefab).GetComponent<BundleItemUI>().Initialize(bundleContentItem);
		}

		private void AttachBuyButtonHandler(CatalogItemModel virtualItem)
		{
			if (virtualItem.VirtualPrice == null)
			{
				buyButton.onClick = () =>
				{
					Destroy(gameObject, 0.001F);
					StoreLogic.PurchaseForRealMoney(virtualItem, null, null);
				};
			}
			else
			{
				buyButton.onClick = () =>
				{
					Destroy(gameObject, 0.001F);
					StoreLogic.PurchaseForVirtualCurrency(virtualItem, null, null);
				};
			}
		}
	}
}