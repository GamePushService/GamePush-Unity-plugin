using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.Inventory;

namespace Xsolla.Samples.DisplayItemsInInventory
{
	public class InventoryItemsPage : MonoBehaviour
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
			// Starting the items request from the store after successful authentication
			// Pass the callback functions for success and error cases
			XsollaInventory.GetInventoryItems(OnItemsRequestSuccess, OnError);
		}

		private void OnItemsRequestSuccess(InventoryItems inventoryItems)
		{
			// Iterating the items collection
			foreach (var inventoryItem in inventoryItems.items)
			{
				// Skipping virtual currency items
				if (inventoryItem.VirtualItemType == VirtualItemType.VirtualCurrency)
					continue;

				// Instantiating the widget prefab as child of the container
				var widgetGo = Instantiate(WidgetPrefab, WidgetsContainer, false);
				var widget = widgetGo.GetComponent<InventoryItemWidget>();

				// Assigning the values for UI elements
				widget.NameText.text = inventoryItem.name;
				widget.DescriptionText.text = inventoryItem.description;
				widget.QuantityText.text = inventoryItem.quantity.ToString();

				// Loading the item image and assigning it to the UI element
				ImageLoader.LoadSprite(inventoryItem.image_url, sprite => widget.IconImage.sprite = sprite);
			}
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}
}
