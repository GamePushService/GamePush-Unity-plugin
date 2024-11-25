using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class CartDiscountUI : MonoBehaviour
	{
		[SerializeField]
		Text discountAmount = default;

		public void Initialize(float discount)
		{
			XDebug.Log("Discount = " + discount);
			discountAmount.text = $"- {PriceFormatter.FormatPrice(discount)}";
		}
	}
}
