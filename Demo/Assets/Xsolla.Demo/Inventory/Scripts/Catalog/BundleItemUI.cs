using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class BundleItemUI : MonoBehaviour
	{
		[SerializeField] private Image itemImage = default;
		[SerializeField] private Text itemName = default;
		[SerializeField] private Text itemDescription = default;
		[SerializeField] private Text itemQuantity = default;

		public void Initialize(BundleContentItem item)
		{
			itemName.text = item.Name;
			itemDescription.text = item.Description;
			itemQuantity.text = item.Quantity.ToString();

			if (!string.IsNullOrEmpty(item.ImageUrl))
			{
				ImageLoader.LoadSprite(item.ImageUrl, LoadImageCallback);
			}
			else
			{
				XDebug.LogError($"Bundle content item with sku = '{item.Sku}' without image!");
			}
		}

		void LoadImageCallback(Sprite sprite)
		{
			if (itemImage)
				itemImage.sprite = sprite;
		}
	}
}
