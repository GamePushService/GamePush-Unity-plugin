using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Xsolla.Demo
{
	public class ItemContainer : MonoBehaviour
	{
		[SerializeField] Transform itemParent = default;
		[SerializeField] Text emptyMessageText = default;

		private readonly List<GameObject> _items = new List<GameObject>();

		private void Awake()
		{
			DisableEmptyContainerMessage();
		}

		public bool IsEmpty()
		{
			return _items.Count == 0;
		}

		public GameObject AddItem(GameObject itemPrefab)
		{
			var itemObject = Instantiate(itemPrefab, itemParent);
			_items.Add(itemObject);
			return itemObject;
		}

		public void RemoveItem(GameObject itemForRemove)
		{
			if (!_items.Contains(itemForRemove)) return;
			_items.Remove(itemForRemove);
			Destroy(itemForRemove);
		}

		public void Clear()
		{
			_items.ForEach(Destroy);
			_items.Clear();
		}

		public void EnableEmptyContainerMessage(string text = null)
		{
			if(emptyMessageText == null) return;
			emptyMessageText.gameObject.SetActive(true);
			if (!string.IsNullOrEmpty(text))
				emptyMessageText.text = text;
		}

		public void DisableEmptyContainerMessage()
		{
			if(emptyMessageText == null) return;
			emptyMessageText.gameObject.SetActive(false);
		}
	}
}
