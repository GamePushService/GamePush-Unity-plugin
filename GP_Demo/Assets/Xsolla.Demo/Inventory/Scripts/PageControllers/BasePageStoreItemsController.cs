using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public abstract class BasePageStoreItemsController : MonoBehaviour
	{
		protected const string GROUP_ALL = "ALL";

		[SerializeField] protected WidgetProvider ItemPrefabProvider = new WidgetProvider();
		[SerializeField] protected GroupsController groupsController = default;
		[SerializeField] protected ItemContainer itemsContainer = default;
		[SerializeField] protected GameObject emptyMessage = default;

		private GameObject ItemPrefab => ItemPrefabProvider.GetValue();

		protected abstract bool IsShowContent {get;}

		private void Start()
		{
			Initialize();
			groupsController.GroupSelectedEvent += ShowGroupItems;
			UpdatePage();
		}

		protected void ShowGroupItems(string groupName)
		{
			var items = GetItemsByGroup(groupName);

			itemsContainer.Clear();
			items.ForEach(item =>
			{
				var itemGameObject = itemsContainer.AddItem(ItemPrefab);
				InitializeItemUI(itemGameObject, item);
			});
		}

		protected void UpdatePage(string groupToSelect = null)
		{
			StartCoroutine(UpdatePageCoroutine(IsShowContent, groupToSelect));
		}

		private IEnumerator UpdatePageCoroutine(bool showContent, string groupToSelect)
		{
			yield return StartCoroutine(FillGroups());

			UpdateContentVisibility(showContent);

			if (!string.IsNullOrEmpty(groupToSelect))
				groupsController.SelectGroup(groupToSelect);
			else
				groupsController.SelectDefault();
		}

		private IEnumerator FillGroups()
		{
			yield return new WaitUntil(() => UserCatalog.Instance.IsUpdated);
			yield return new WaitUntil(() => UserInventory.Instance.IsUpdated);

			var groups = GetGroups();

			groupsController.RemoveAll();
			groupsController.AddGroup(GROUP_ALL);
			groups.ForEach(g => groupsController.AddGroup(g));
			
			LayoutRebuilder.ForceRebuildLayoutImmediate(groupsController.transform as RectTransform);
		}

		protected bool CheckHideInAttribute(ItemModel item, HideInFlag flag)
		{
			var attributes = item.Attributes;

			if (attributes == null)
				return false;

			foreach (var attribute in attributes)
			{
				var isHideKey = (attribute.Key == "HideIn");
				var isFlagValue = (attribute.Value == flag.ToString());

				if (isHideKey && isFlagValue)
					return true;
			}

			//else
			return false;
		}

		protected virtual void Initialize() { }

		protected abstract void InitializeItemUI(GameObject item, ItemModel model);
		protected abstract List<ItemModel> GetItemsByGroup(string groupName);
		protected abstract List<string> GetGroups();

		protected enum HideInFlag
		{
			Store,
			Inventory
		}

		private void ShowEmptyMessage(bool showEmptyMessage)
		{
			emptyMessage.SetActive(showEmptyMessage);
		}

		private void ShowContent(bool showContent)
		{
			groupsController.gameObject.SetActive(showContent);
			itemsContainer.gameObject.SetActive(showContent);
		}

		private void UpdateContentVisibility(bool showContent)
		{
			ShowEmptyMessage(!showContent);
			ShowContent(showContent);
		}
	}
}
