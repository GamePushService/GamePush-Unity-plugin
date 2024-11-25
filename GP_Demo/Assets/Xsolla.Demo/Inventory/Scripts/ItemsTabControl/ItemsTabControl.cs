using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class ItemsTabControl : MonoBehaviour
	{
		[SerializeField] private MenuButton storeButton = default;
		[SerializeField] private MenuButton inventoryButton = default;

		private ItemsController _itemsController;
		private GroupsController _groupsController;

		public void Init()
		{
#if UNITY_6000
			_groupsController = FindAnyObjectByType<GroupsController>();
			_itemsController = FindAnyObjectByType<ItemsController>();
#else
			_groupsController = FindObjectOfType<GroupsController>();
			_itemsController = FindObjectOfType<ItemsController>();
#endif

			storeButton.gameObject.SetActive(true);
			inventoryButton.gameObject.SetActive(true);

			storeButton.Select(false);
			storeButton.onClick = _ => InternalActivateStoreTab();
			inventoryButton.onClick = _ => InternalActivateInventoryTab();

			InitHotKeys();
		}

		private void InternalActivateStoreTab()
		{
			inventoryButton.Deselect();

			var selectedGroup = _groupsController.GetSelectedGroup();
			if (selectedGroup != null)
			{
				_itemsController.ActivateContainer(selectedGroup.Id);
			}
		}

		private void InternalActivateInventoryTab()
		{
			storeButton.Deselect();
			_itemsController.ActivateContainer(StoreConstants.INVENTORY_CONTAINER_NAME);
		}

		public void ActivateStoreTab()
		{
			storeButton.Select(false);
			inventoryButton.Deselect();
		}
	}
}
