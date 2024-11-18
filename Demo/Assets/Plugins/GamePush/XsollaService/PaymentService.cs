using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace GamePush.Xsolla
{
	public static class PaymentService
	{
		public static void PurchaseItem(string itemId)
		{
			XsollaCatalog.Purchase(itemId, OnPurchaseSuccess, OnError);
		}

		private static void OnPurchaseSuccess(OrderStatus status)
		{
			Debug.Log("Purchase successful");
			// Add actions taken in case of success
		}

		private static void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}

}
