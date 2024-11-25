using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public class InventoryItemContainer : MonoBehaviour, IContainer
	{
		[SerializeField] private GameObject itemPrefab = default;
		[SerializeField] private Transform itemParent = default;

		private List<GameObject> _items;

		private void Awake()
		{
			_items = new List<GameObject>();
			UserInventory.Instance.UpdateItemsEvent += RefreshInternal;
		}

		public void Refresh()
		{
			RefreshInternal(UserInventory.Instance.VirtualItems);
		}

		private void RefreshInternal(List<InventoryItemModel> items)
		{
			ClearItems();
			items.ForEach(AddItem);
		}

		private void ClearItems()
		{
			_items.ForEach(Destroy);
			_items.Clear();
		}

		private void AddItem(InventoryItemModel itemInformation)
		{
			var newItem = Instantiate(itemPrefab, itemParent);
			newItem.GetComponent<InventoryItemUI>().Initialize(itemInformation);
			_items.Add(newItem);
		}
	}
}
