using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ItemUI : MonoBehaviour
    {
		[SerializeField] private Image ItemImage = default;
		[SerializeField] private GameObject ImageLoadingCircle = default;
		[SerializeField] private Text ItemName = default;
		[SerializeField] private Text ItemDescription = default;
		[SerializeField] private Text ItemQuantityText = default;
		[SerializeField] private int DescriptionLengthLimit = default;
		[SerializeField] private GameObject QuantityLabel = default;
		[SerializeField] private bool ShowQuantityOne = default;

		public void Initialize(ItemModel itemModel, int quantity = 1)
		{
			SetImage(itemModel.ImageUrl, itemModel.Sku);
			SetTextInfo(itemModel.Name, itemModel.Description, quantity, itemModel.Sku);
			ShowQuantity(quantity);
		}

		private void SetImage(string imageUrl, string itemSku)
		{
			if (!string.IsNullOrEmpty(imageUrl))
				ImageLoader.LoadSprite(imageUrl, LoadImageCallback);
			else
				XDebug.LogError($"Item with sku: '{itemSku}' has no image url");
		}

		private void LoadImageCallback(Sprite image)
		{
			if (ItemImage)
			{
				ItemImage.sprite = image;
				ImageLoadingCircle.SetActive(false);
				ItemImage.gameObject.SetActive(true);
			}
		}

		private void SetTextInfo(string itemName, string itemDescription, int quantity, string itemSku)
		{
			ItemName.text = itemName;
			ItemDescription.text = ShortenDescription(itemDescription, DescriptionLengthLimit, itemSku);
			ItemQuantityText.text = quantity.ToString();
		}

		private string ShortenDescription(string input, int limit, string itemSku)
		{
			if (string.IsNullOrEmpty(input))
				return string.Empty;

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

		private void ShowQuantity(int quantity)
		{
			var shouldShowQuantity = (quantity > 1) || (quantity == 1 && ShowQuantityOne);

			if (shouldShowQuantity)
				QuantityLabel.SetActive(true);
			else
				QuantityLabel.SetActive(false);
		}
	}
}
