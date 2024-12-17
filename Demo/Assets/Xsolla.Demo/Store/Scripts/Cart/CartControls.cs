using System;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class CartControls : MonoBehaviour
	{
		[SerializeField] private SimpleTextButton buyButton = default;
		[SerializeField] private SimpleButton clearCartButton = default;

		[SerializeField] private Text totalPriceText = default;
		[SerializeField] private Text subtotalPriceText = default;
		[SerializeField] private Text discountPriceText = default;
		[SerializeField] private Text totalLabelText = default;
		[SerializeField] private Text subtotalLabelText = default;
		[SerializeField] private Text discountLabelText = default;

		private GameObject _loaderObject;

		public Action OnBuyCart
		{
			set => buyButton.onClick = value;
		}

		public Action OnClearCart
		{
			set => clearCartButton.onClick = value;
		}

		public void Initialize(float totalPrice = 0f, float discount = 0f)
		{
			totalPriceText.text = PriceFormatter.FormatPrice(totalPrice);
			ShowTotal(totalPrice >= 0.01f);

			if (discount >= 0.01f)
				ShowDiscount(discount, totalPrice + discount);
			else
				HideDiscount();
		}

		private void ShowTotal(bool show)
		{
			totalLabelText.gameObject.SetActive(show);
			totalPriceText.gameObject.SetActive(show);
		}

		private void HideDiscount()
		{
			subtotalLabelText.gameObject.SetActive(false);
			subtotalPriceText.gameObject.SetActive(false);
			discountLabelText.gameObject.SetActive(false);
			discountPriceText.gameObject.SetActive(false);
		}

		private void ShowDiscount(float discount, float subtotal)
		{
			subtotalLabelText.gameObject.SetActive(true);
			subtotalPriceText.gameObject.SetActive(true);
			discountLabelText.gameObject.SetActive(true);
			discountPriceText.gameObject.SetActive(true);
			discountPriceText.text = $"-{PriceFormatter.FormatPrice(discount)}";
			subtotalPriceText.text = PriceFormatter.FormatPrice(subtotal);
		}
	}
}
