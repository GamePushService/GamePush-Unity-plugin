using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class InventoryItemUI : MonoBehaviour
	{
		[SerializeField] private Image itemImage = default;
		[SerializeField] private GameObject loadingCircle = default;
		[SerializeField] private Text itemName = default;
		[SerializeField] private Text itemDescription = default;
		[SerializeField] private GameObject itemQuantityImage = default;
		[SerializeField] private Text itemQuantityText = default;
		[SerializeField] private ConsumeButton consumeButton = default;
		[SerializeField] private Text purchasedStatusText = default;
		[SerializeField] private GameObject remainingTimeObject = default;
		[SerializeField] private Text remainingTimeTimeText = default;
		[SerializeField] private GameObject expiredTimeObject = default;
		[SerializeField] private Text expiredTimeText = default;
		[SerializeField] private bool shortSubsctiptionTimeFormat = false;
#pragma warning disable 0414
		[SerializeField] private SimpleTextButton renewSubscriptionButton = default;
#pragma warning restore 0414

		private ItemModel _itemInformation;
		private bool _canRenewSubscription = true;

		public event Action<ItemModel> OnInitialized;

		private void Awake()
		{
			DisableConsumeButton();
		}

		public void Initialize(ItemModel itemInformation)
		{
			_itemInformation = itemInformation;

			if (itemName)
				itemName.text = _itemInformation.Name;
			if (itemDescription)
				itemDescription.text = _itemInformation.Description;

			LoadImage(_itemInformation.ImageUrl);

			if (_itemInformation.IsSubscription() && renewSubscriptionButton)
				AttachRenewSubscriptionHandler();

			RefreshUi();

			OnInitialized?.Invoke(itemInformation);
		}

		partial void AttachRenewSubscriptionHandler();

		private void LoadImage(string url)
		{
			if (!string.IsNullOrEmpty(url))
				ImageLoader.LoadSprite(url, LoadImageCallback);
			else
			{
				XDebug.LogWarning($"Inventory item with sku = '{_itemInformation.Sku}' has no image!");
				LoadImageCallback(null);
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
		}

		private void RefreshUi()
		{
			DisableQuantityImage();
			DisableConsumeButton();
			DisableExpirationText();
			DisablePurchasedStatusText();

			if (_itemInformation.IsSubscription())
				DrawSubscriptionItem();
			else if (_itemInformation.IsConsumable)
				DrawConsumableVirtualItem();
			else
				DrawNonConsumableVirtualItem();
		}

		private void DrawSubscriptionItem()
		{
			if (TryDowncastTo<UserSubscriptionModel>(_itemInformation, out var model))
			{
				if (model.Status != UserSubscriptionModel.SubscriptionStatusType.None && model.Expired.HasValue)
				{
					if (model.Status == UserSubscriptionModel.SubscriptionStatusType.Expired || model.Expired <= DateTime.Now)
						EnableExpiredTimeText(GetPassedTime(model.Expired.Value));
					else
						EnableRemainingTimeText(GetRemainingTime(model.Expired.Value));
				}
				else
					EnablePurchasedStatusText(isPurchased: false);
			}
		}

		private void EnableRemainingTimeText(string text)
		{
			if (remainingTimeObject)
				remainingTimeObject.SetActive(true);
			if (expiredTimeObject)
				expiredTimeObject.SetActive(false);
			if (renewSubscriptionButton)
				renewSubscriptionButton.gameObject.SetActive(false);

			if (remainingTimeTimeText != null)
				remainingTimeTimeText.text = text;
		}

		private void EnableExpiredTimeText(string text)
		{
			if (remainingTimeObject)
				remainingTimeObject.SetActive(false);
			if (expiredTimeObject)
				expiredTimeObject.SetActive(true);

			if (renewSubscriptionButton)
			{
				if (_canRenewSubscription)
					renewSubscriptionButton.gameObject.SetActive(true);
				else
					renewSubscriptionButton.gameObject.SetActive(false);
			}

			if (expiredTimeText)
				expiredTimeText.text = $"Expired {text}";
		}

		private string GetRemainingTime(DateTime expiredDateTime)
		{
			var timeLeft = expiredDateTime - DateTime.Now;
			StartCoroutine(RemainingTimeCoroutine(timeLeft.TotalSeconds > 60 ? 60 : 1));
			var suffix = shortSubsctiptionTimeFormat ? string.Empty : "remaining";
			if (timeLeft.TotalDays >= 30)
				return $"{(int) (timeLeft.TotalDays / 30)} month{(timeLeft.TotalDays > 60 ? "s" : "")} {suffix}";
			if (timeLeft.TotalDays > 1)
				return $"{(uint) (timeLeft.TotalDays)} day{(timeLeft.TotalDays > 1 ? "s" : "")} {suffix}";
			if (timeLeft.TotalHours > 1)
				return $"{(uint) (timeLeft.TotalHours)} hour{(timeLeft.TotalHours > 1 ? "s" : "")} {suffix}";
			if (timeLeft.TotalMinutes > 1)
				return $"{(uint) (timeLeft.TotalMinutes)} minute{(timeLeft.TotalMinutes > 1 ? "s" : "")} {suffix}";
			return $"{(uint) (timeLeft.TotalSeconds)} second{(timeLeft.TotalSeconds > 1 ? "s" : "")} {suffix}";
		}

		private string GetPassedTime(DateTime expiredDateTime)
		{
			var timePassed = DateTime.Now - expiredDateTime;
			StartCoroutine(RemainingTimeCoroutine(timePassed.TotalSeconds > 60 ? 60 : 1));
			var suffix = shortSubsctiptionTimeFormat ? string.Empty : "ago";
			if (timePassed.TotalDays >= 30)
				return $"{(int) (timePassed.TotalDays / 30)} month{(timePassed.TotalDays > 60 ? "s" : "")} {suffix}";
			if (timePassed.TotalDays > 1)
				return $"{(uint) (timePassed.TotalDays)} day{(timePassed.TotalDays > 1 ? "s" : "")} {suffix}";
			if (timePassed.TotalHours > 1)
				return $"{(uint) (timePassed.TotalHours)} hour{(timePassed.TotalHours > 1 ? "s" : "")} {suffix}";
			if (timePassed.TotalMinutes > 1)
				return $"{(uint) (timePassed.TotalMinutes)} minute{(timePassed.TotalMinutes > 1 ? "s" : "")} {suffix}";
			return $"{(uint) (timePassed.TotalSeconds)} second{(timePassed.TotalSeconds > 1 ? "s" : "")} {suffix}";
		}

		private IEnumerator RemainingTimeCoroutine(float waitSeconds)
		{
			yield return new WaitForSeconds(waitSeconds);
			RefreshUi();
		}

		private void DrawConsumableVirtualItem()
		{
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out var model))
			{
				DrawItemsCount(model);
				EnableConsumeButton();
			}
		}

		private void DrawNonConsumableVirtualItem()
		{
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out var model))
			{
				DrawItemsCount(model);
				EnablePurchasedStatusText(isPurchased: true);
			}
		}

		private void DrawItemsCount(InventoryItemModel model)
		{
			if (model.RemainingUses == null || itemQuantityImage == null || itemQuantityText == null) return;
			EnableQuantityImage();
			itemQuantityText.text = model.RemainingUses.Value.ToString();
		}

		private void EnableQuantityImage()
		{
			if (itemQuantityImage != null)
				itemQuantityImage.SetActive(true);
		}

		private void DisableQuantityImage()
		{
			if (itemQuantityImage != null)
				itemQuantityImage.SetActive(false);
		}

		private void EnablePurchasedStatusText(bool isPurchased)
		{
			if (purchasedStatusText != null)
			{
				purchasedStatusText.text = isPurchased ? "Purchased" : "Not purchased";
				purchasedStatusText.gameObject.SetActive(true);
			}
		}

		private void DisablePurchasedStatusText()
		{
			if (purchasedStatusText != null)
				purchasedStatusText.gameObject.SetActive(false);
		}

		private void DisableExpirationText()
		{
			if (remainingTimeObject != null)
				remainingTimeObject.SetActive(false);
			if (expiredTimeObject != null)
				expiredTimeObject.SetActive(false);
			if (renewSubscriptionButton != null)
				renewSubscriptionButton.gameObject.SetActive(false);
		}

		private void EnableConsumeButton()
		{
			if (consumeButton != null)
			{
				consumeButton.gameObject.SetActive(true);
				consumeButton.onClick = ConsumeHandler;
				if (consumeButton.counter < 1)
					consumeButton.counter.IncreaseValue(1 - consumeButton.counter.GetValue());
				consumeButton.counter.ValueChanged += Counter_ValueChanged;
			}
		}

		private void DisableConsumeButton()
		{
			if (consumeButton != null)
			{
				consumeButton.counter.ValueChanged -= Counter_ValueChanged;
				consumeButton.gameObject.SetActive(false);
			}
		}

		private void ConsumeHandler()
		{
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out var model))
			{
				DisableConsumeButton();
				DemoInventory.Instance.ConsumeInventoryItem(model, consumeButton.counter.GetValue(),
					_ =>
					{
						UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
						consumeButton.counter.ResetValue();
					}, _ => EnableConsumeButton());
			}
		}

		private void Counter_ValueChanged(int newValue)
		{
			if (TryDowncastTo<InventoryItemModel>(_itemInformation, out var model))
			{
				if (newValue > model.RemainingUses)
				{
					var delta = (int) model.RemainingUses - newValue;
					StartCoroutine(ChangeConsumeQuantityCoroutine(delta));
				}
				else
				{
					if (newValue == 0)
						StartCoroutine(ChangeConsumeQuantityCoroutine(1));
				}
			}
		}

		private IEnumerator ChangeConsumeQuantityCoroutine(int deltaValue)
		{
			yield return new WaitForEndOfFrame();
			if (deltaValue < 0)
				consumeButton.counter.DecreaseValue(-deltaValue);
			else
				consumeButton.counter.IncreaseValue(deltaValue);
		}

		private bool TryDowncastTo<T>(ItemModel itemModel, out T result) where T : ItemModel
		{
			if (itemModel is T)
			{
				result = (T) itemModel;
				return true;
			}
			else
			{
				XDebug.LogError($"Item model incorrect for item with SKU '{itemModel.Sku}', expected type is '{typeof(T)}'");
				result = null;
				return false;
			}
		}
	}
}