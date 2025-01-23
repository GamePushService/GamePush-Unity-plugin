using UnityEngine;
using Xsolla.Catalog;
using Xsolla.Core;

namespace Xsolla.Samples.Steam
{
	public class SellViaSteamGateway : MonoBehaviour
	{
		// Function for starting the purchase process via Steam Gateway
		public void PurchaseItem(string itemSku)
		{
			// Get additional headers for the request from `SteamUtils` class
			var additionalHeaders = SteamUtils.GetAdditionalCustomHeaders();

			// Starting the purchase process
			// Pass the `itemSku` parameter and callback functions for success and error cases
			// Pass `additionalHeaders` variable as the optional `customHeaders` parameter
			XsollaCatalog.Purchase(itemSku, OnPurchaseSuccess, OnError, customHeaders: additionalHeaders);
		}

		private void OnPurchaseSuccess(OrderStatus status)
		{
			Debug.Log("Purchase successful");
			// Add actions taken in case of success
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}
}
