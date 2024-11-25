using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public class StoreItemUI : MonoBehaviour
	{
		[SerializeField] Image itemImage = default;
		[SerializeField] GameObject loadingCircle = default;
		[SerializeField] Text itemName = default;
		[SerializeField] Text itemDescription = default;
		[SerializeField] int itemDescriptionLength = default;
		[SerializeField] Text itemPrice = default;
		[SerializeField] Text itemPriceWithoutDiscount = default;
		[SerializeField] Image itemPriceVcImage = default;
		[SerializeField] Text itemPriceVcText = default;
		[SerializeField] GameObject expirationTimeObject = default;
		[SerializeField] Text expirationTimeText = default;
		[SerializeField] SimpleTextButton buyButton = default;
		[SerializeField] SimpleTextButton previewButton = default;
		[SerializeField] AddToCartButton cartButton = default;
		[SerializeField] SimpleTextButton checkoutButtonButton = default;
		[SerializeField] GameObject prices = default;
		[SerializeField] GameObject purchasedText = default;
		[SerializeField] GameObject freePrice = default;

		private CatalogItemModel _itemInformation;

		public bool IsAlreadyPurchased { get; private set; }

		public event Action<CatalogItemModel> OnInitialized;

		public void Initialize(CatalogItemModel virtualItem)
		{
			itemPrice.gameObject.SetActive(false);
			itemPriceWithoutDiscount.gameObject.SetActive(false);
			itemPriceVcImage.gameObject.SetActive(false);
			itemPriceVcText.gameObject.SetActive(false);
			expirationTimeObject.SetActive(false);
			freePrice.SetActive(false);

			_itemInformation = virtualItem;

			if (virtualItem.VirtualPrice != null)
				InitializeVirtualCurrencyPrice(virtualItem);
			else
				InitializeRealPrice(virtualItem);

			InitializeVirtualItem(virtualItem);

			var isBundle = virtualItem.IsBundle();
			DisableBuy(isBundle);
			if (!isBundle)
				AttachBuyButtonHandler(virtualItem);
			else
				AttachPreviewButtonHandler(virtualItem);

			var isPurchased = CheckIfItemPurchased(virtualItem);
			DisablePrice(isPurchased);

			if (UserCart.Instance.Contains(virtualItem.Sku))
				EnableCheckout(true);

			checkoutButtonButton.onClick = () => DemoController.Instance.SetState(MenuState.Cart);

			OnInitialized?.Invoke(virtualItem);
		}

		private bool CheckIfItemPurchased(CatalogItemModel virtualItem)
		{
			if (!virtualItem.IsConsumable)
			{
				var sameItemFromInventory = UserInventory.Instance.VirtualItems.FirstOrDefault(i => i.Sku.Equals(_itemInformation.Sku));
				IsAlreadyPurchased = sameItemFromInventory != null;
				return IsAlreadyPurchased;
			}

			//else
			return false;
		}

		private void DisablePrice(bool disable)
		{
			prices.SetActive(!disable);
			purchasedText.SetActive(disable);
		}

		private void DisableBuy(bool disable)
		{
			buyButton.gameObject.SetActive(!disable);
			previewButton.gameObject.SetActive(disable);
		}

		private void EnablePrice(bool isVirtualPrice)
		{
			itemPrice.gameObject.SetActive(!isVirtualPrice);
			itemPriceWithoutDiscount.gameObject.SetActive(false);
			itemPriceVcImage.gameObject.SetActive(isVirtualPrice);
			itemPriceVcText.gameObject.SetActive(isVirtualPrice);
		}

		private void EnableCheckout(bool enableCheckout)
		{
			checkoutButtonButton.gameObject.SetActive(enableCheckout);
			previewButton.gameObject.SetActive(!enableCheckout);
			cartButton.gameObject.SetActive(!enableCheckout);
			buyButton.gameObject.SetActive(!enableCheckout);
		}

		private void InitializeVirtualCurrencyPrice(CatalogItemModel virtualItem)
		{
			EnablePrice(true);
			cartButton.gameObject.SetActive(false);
			itemPriceVcText.text = virtualItem.VirtualPrice?.Value.ToString();
			InitializeVcImage(virtualItem);
		}

		private void InitializeVcImage(CatalogItemModel virtualItem)
		{
			StartCoroutine(WaitCatalogUpdate(() =>
			{
				var currencySku = virtualItem.VirtualPrice?.Key;
				var currency = UserCatalog.Instance.VirtualCurrencies.First(vc => vc.Sku.Equals(currencySku));

				if (!string.IsNullOrEmpty(currency.ImageUrl))
					ImageLoader.LoadSprite(currency.ImageUrl, sprite =>
					{
						if (itemPriceVcImage)
							itemPriceVcImage.sprite = sprite;
					});
				else
					XDebug.LogError($"Virtual currency item with sku = '{virtualItem.Sku}' without image!");
			}));
		}

		IEnumerator WaitCatalogUpdate(Action callback)
		{
			yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
			callback?.Invoke();
		}

		IEnumerator WaitInventoryUpdate(Action callback)
		{
			yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);
			callback?.Invoke();
		}

		private void InitializeRealPrice(CatalogItemModel virtualItem)
		{
			cartButton.gameObject.SetActive(true);
			if (UserCart.Instance.Contains(virtualItem.Sku))
				cartButton.Select(true);
			cartButton.onClick = isSelected =>
			{
				if (isSelected)
					UserCart.Instance.AddItem(_itemInformation);
				else
					UserCart.Instance.RemoveItem(_itemInformation);

				EnableCheckout(isSelected);
			};

			var realPrice = virtualItem.RealPrice;
			if (realPrice == null)
			{
				freePrice.SetActive(true);
				return;
			}

			EnablePrice(false);

			var valuePair = realPrice.Value;
			var currency = valuePair.Key;
			var price = valuePair.Value;
			itemPrice.text = PriceFormatter.FormatPrice(currency, price);

			var priceWithoutDiscountContainer = virtualItem.RealPriceWithoutDiscount;
			if (priceWithoutDiscountContainer == null || !priceWithoutDiscountContainer.HasValue || priceWithoutDiscountContainer.Value.Value == default(float))
				return;

			var priceWithoutDiscount = priceWithoutDiscountContainer.Value.Value;
			if (priceWithoutDiscount == price)
				return;

			itemPriceWithoutDiscount.text = PriceFormatter.FormatPrice(currency, priceWithoutDiscount);
			itemPriceWithoutDiscount.gameObject.SetActive(true);
		}

		private void InitializeVirtualItem(CatalogItemModel virtualItem)
		{
			if (string.IsNullOrEmpty(virtualItem.Name))
			{
				XDebug.LogError($"Try initialize item with sku = {virtualItem.Sku} without name!");
				virtualItem.Name = virtualItem.Sku;
			}

			itemName.text = virtualItem.Name;
			itemDescription.text = ShortenDescription(virtualItem.Description, itemDescriptionLength);
			gameObject.name = "Item_" + virtualItem.Name.Replace(" ", "");
			if (!string.IsNullOrEmpty(virtualItem.ImageUrl))
			{
				ImageLoader.LoadSprite(virtualItem.ImageUrl, LoadImageCallback);
			}
			else
			{
				XDebug.LogError($"Virtual item item with sku = '{virtualItem.Sku}' without image!");
			}
		}

		private void LoadImageCallback(Sprite image)
		{
			if (!itemImage)
				return;

			loadingCircle.SetActive(false);
			itemImage.gameObject.SetActive(true);
			itemImage.sprite = image;

			var aspectRatioFitter = itemImage.GetComponent<AspectRatioFitter>();
			if (aspectRatioFitter)
				aspectRatioFitter.aspectRatio = image.bounds.size.x / image.bounds.size.y;

			InitExpirationTime(_itemInformation);
		}

		private void InitExpirationTime(CatalogItemModel virtualItem)
		{
			expirationTimeObject.SetActive(false);
			if (!virtualItem.IsSubscription()) return;
			var subscription = UserCatalog.Instance.Subscriptions.First(s => s.Sku.Equals(virtualItem.Sku));
			if (subscription == null)
			{
				XDebug.LogError($"Something went wrong... Can not find subscription item with sku = '{virtualItem.Sku}'!");
				return;
			}

			expirationTimeObject.SetActive(true);
			expirationTimeText.text = subscription.ExpirationPeriodText;
		}

		private void AttachBuyButtonHandler(CatalogItemModel virtualItem)
		{
			Action<CatalogItemModel> onPurchased = item => { StartCoroutine(WaitInventoryUpdate(() => CheckIfItemPurchased(item))); };

			if (virtualItem.VirtualPrice != null)
				buyButton.onClick = () => StoreLogic.PurchaseForVirtualCurrency(virtualItem, onPurchased, null);
			else if (virtualItem.Price != null)
				buyButton.onClick = () => StoreLogic.PurchaseForRealMoney(virtualItem, onPurchased, null);
			else
				buyButton.onClick = () => StoreLogic.PurchaseFreeItem(virtualItem, onPurchased, null);
		}

		private void AttachPreviewButtonHandler(CatalogItemModel virtualItem)
		{
			previewButton.onClick = () => { PopupFactory.Instance.CreateBundlePreview().SetBundleInfo((CatalogBundleItemModel) virtualItem); };
		}

		private string ShortenDescription(string input, int limit)
		{
			if (input.Length <= limit)
				return input;

			if (limit > input.Length - 1)
				limit = input.Length - 1;

			var spacePosition = input.LastIndexOf(" ", limit);
			var result = default(string);

			if (spacePosition != -1)
				result = input.Substring(0, spacePosition);
			else
				result = input.Substring(0, limit);

			return result;
		}
	}
}