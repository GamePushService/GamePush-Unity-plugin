using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(StoreItemUI))]
    public class StoreItemDiscountLabel : MonoBehaviour
    {
		[SerializeField] private GameObject DiscountLabel = default;
		[SerializeField] private Text DiscountText = default;
		[SerializeField] private int RoundTo = 5;

		private StoreItemUI _connectedItemUI;

		private void Awake()
		{
			_connectedItemUI = GetComponent<StoreItemUI>();
			_connectedItemUI.OnInitialized += ShowDiscountIfAny;
		}

		private void ShowDiscountIfAny(CatalogItemModel itemModel)
		{
			DiscountLabel.SetActive(false);

			if (_connectedItemUI.IsAlreadyPurchased)
				return;

			var realPrice = itemModel.RealPrice;
			if (realPrice == null)
				return;

			var priceWithoutDiscountContainer = itemModel.RealPriceWithoutDiscount;
			if (priceWithoutDiscountContainer == null ||
				!priceWithoutDiscountContainer.HasValue ||
				priceWithoutDiscountContainer.Value.Value == default(float))
				return;

			var price = realPrice.Value.Value;
			var priceWithoutDiscount = priceWithoutDiscountContainer.Value.Value;

			if (priceWithoutDiscount != price)
			{
				var roundedDiscount = RoundDiscount(price, priceWithoutDiscount, RoundTo);
				var discountValue = $"-{roundedDiscount}%";
				DiscountText.text = discountValue;
				DiscountLabel.SetActive(true);
			}
		}

		private float RoundDiscount(float currentPrice, float initialPrice, int roundTo)
		{
			var currentPercents = (currentPrice / initialPrice) * 100f;
			var discountRaw = 100f - currentPercents;

			var discountRounded = (int)discountRaw;
			var count = discountRounded / roundTo;
			var remnant = discountRounded % roundTo;
			var middle = roundTo >> 1;

			if (remnant >= middle)
				count += 1;

			var result = count * roundTo;
			return result;
		}
	}
}
