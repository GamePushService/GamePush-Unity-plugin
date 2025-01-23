using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Samples.DisplayVirtualCurrencyBalance
{
	public class VirtualCurrenciesPage : MonoBehaviour
	{
		// Declaration of variables for widget's container and prefab
		public Transform WidgetsContainer;
		public GameObject WidgetPrefab;

		private void Start()
		{
			// Starting the authentication process
			// Pass the credentials and callback functions for success and error cases
			// The credentials (username and password) are hard-coded for simplicity
			XsollaAuth.SignIn("xsolla", "xsolla", OnAuthenticationSuccess, OnError);
		}

		private void OnAuthenticationSuccess()
		{
			// Starting the virtual currencies request from the store after successful authentication
			// Pass the callback functions for success and error cases
			XsollaInventory.GetVirtualCurrencyBalance(OnBalanceRequestSuccess, OnError);
		}

		private void OnBalanceRequestSuccess(VirtualCurrencyBalances balance)
		{
			// Iterating the virtual currency balances collection
			foreach (var balanceItem in balance.items)
			{
				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<VirtualCurrencyWidget>();

				// Assigning the values for UI elements
				widget.NameText.text = balanceItem.name;
				widget.AmountText.text = balanceItem.amount.ToString();

				ImageLoader.LoadSprite(balanceItem.image_url, sprite => widget.IconImage.sprite = sprite);
			}
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}
}
