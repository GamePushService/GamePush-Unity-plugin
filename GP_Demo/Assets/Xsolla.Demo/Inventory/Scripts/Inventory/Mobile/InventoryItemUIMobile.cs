using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(InventoryItemUI))]
	public class InventoryItemUIMobile : MonoBehaviour
	{
		private ItemModel _itemInformation;

		private void Awake()
		{
			var itemUI = GetComponent<InventoryItemUI>();
			itemUI.OnInitialized += OnItemInitialized;
		}

		private void OnItemInitialized(ItemModel itemInformation)
		{
			_itemInformation = itemInformation;
		}

		public void OnItemClicked()
		{
			if (_itemInformation == null)
				return;

			InventoryItemInfoMobile.ShowItem(_itemInformation);
		}
	}
}