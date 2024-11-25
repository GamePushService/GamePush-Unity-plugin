using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class CouponRewardItemUI : MonoBehaviour
	{
		[SerializeField] Image itemImage = default;
		[SerializeField] Text itemName = default;
		[SerializeField] Text itemDescription = default;
		[SerializeField] Text itemQuantity = default;

		public void Initialize(CouponRedeemedItemModel rewardItem)
		{
			itemName.text = rewardItem.Name;
			itemDescription.text = rewardItem.Description;
			itemQuantity.text = rewardItem.Quantity.ToString();

			if (itemImage.sprite != null)
			{
				if (!string.IsNullOrEmpty(rewardItem.ImageUrl))
				{
					ImageLoader.LoadSprite(rewardItem.ImageUrl, image =>
					{
						if (itemImage)
							itemImage.sprite = image;
					});
				}
				else
				{
					XDebug.LogError($"Coupon reward item item with sku = '{rewardItem.Sku}' without image!");
				}
			}
		}
	}
}
