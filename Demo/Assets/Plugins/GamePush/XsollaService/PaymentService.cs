using System;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Catalog;
using Xsolla.Core;

namespace GamePush.Xsolla
{
	public static class PaymentService
	{
		private static Action<string> _onPurchaseSuccess;
		private static Action _onPurchaseError;

		public static void PurchaseItem(string itemId, Action<string> onPurchaseSuccess = null, Action onPurchaseError = null)
		{
			_onPurchaseSuccess = onPurchaseSuccess;
			_onPurchaseError = onPurchaseError;

			XsollaCatalog.Purchase(itemId, OnPurchaseSuccess, OnError);
		}

		private static void OnPurchaseSuccess(OrderStatus status)
		{
			Debug.Log("Purchase successful");
			_onPurchaseSuccess?.Invoke(status.ToString());
			// Add actions taken in case of success
		}

		private static void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			_onPurchaseError?.Invoke();
			// Add actions taken in case of error
		}

	}

}
