using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ItemsController : MonoBehaviour
	{
		[SerializeField] GameObject itemsContainerPrefab = default;
		[SerializeField] GameObject inventoryContainerPrefab = default;
		[SerializeField] Transform content = default;

		private readonly Dictionary<string, GameObject> _containers = new Dictionary<string, GameObject>();
		private bool _isEmptyCatalog;

		private void Awake()
		{
			var defaultContainers = GetDefaultContainers();
			defaultContainers.ToList().ForEach(container => { AddContainer(container.Value, container.Key); });
			_isEmptyCatalog = true;
		}

		private Dictionary<string, GameObject> GetDefaultContainers()
		{
			Dictionary<string, GameObject> itemContainers = GetDefaultItemContainers();
			Dictionary<string, GameObject> otherContainers = new Dictionary<string, GameObject>()
			{
				{StoreConstants.INVENTORY_CONTAINER_NAME, inventoryContainerPrefab}
			};
			itemContainers.ToList().ForEach((container) => { otherContainers.Add(container.Key, container.Value); });
			return otherContainers;
		}

		private Dictionary<string, GameObject> GetDefaultItemContainers()
		{
			return new Dictionary<string, GameObject>()
			{
				{StoreConstants.EMPTY_CONTAINER_NAME, itemsContainerPrefab}
			};
		}

		GameObject AddContainer(GameObject itemContainerPref, string containerName)
		{
			var newContainer = Instantiate(itemContainerPref, content);
			newContainer.name = containerName;
			//newContainer.GetComponent<IContainer>().SetStoreImplementation(_demoImplementation);
			newContainer.SetActive(false);
			_containers.Add(containerName, newContainer);
			return newContainer;
		}

		public void ActivateContainer(string groupId)
		{
			GameObject activeContainer = InternalActivateContainer(
				_containers.ContainsKey(groupId) ? groupId : StoreConstants.EMPTY_CONTAINER_NAME
			);
			activeContainer.GetComponent<IContainer>().Refresh();
			CheckForEmptyCatalogMessage(activeContainer);
		}

		private GameObject InternalActivateContainer(string containerName)
		{
			_containers.Values.ToList().ForEach(o => o.SetActive(false));
			_containers[containerName].SetActive(true);
			return _containers[containerName];
		}

		private void CheckForEmptyCatalogMessage(GameObject activeContainer)
		{
			ItemContainer itemContainer = activeContainer.GetComponent<ItemContainer>();
			if (_isEmptyCatalog && (itemContainer != null) && itemContainer.IsEmpty())
			{
				itemContainer.EnableEmptyContainerMessage();
			}
		}
	}
}
