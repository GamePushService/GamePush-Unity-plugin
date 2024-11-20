using System.Collections.Generic;
using System.Linq;

namespace Xsolla.Demo
{
	public class StorePageStoreItemsController : BaseStorePageStoreItemsController
	{
		protected override bool IsShowContent => UserCatalog.Instance.HasVirtualItems || UserCatalog.Instance.HasBundles;

		protected override List<ItemModel> GetItemsByGroup(string groupName)
		{
			var isGroupAll = groupName.Equals(GROUP_ALL);

			var items = UserCatalog.Instance.AllItems.Where(item =>
			{
				if (item.IsVirtualCurrency())
					return false;
				else
				{
					var itemGroups = item.Groups;

					if (base.CheckHideInAttribute(item, HideInFlag.Store))
						return false; //This item must be hidden by attribute

					return isGroupAll || itemGroups.Contains(groupName);
				}
			}).ToList();

			return items.Cast<ItemModel>().ToList();
		}

		protected override List<string> GetGroups()
		{
			var items = UserCatalog.Instance.AllItems;
			var groups = new List<string>();

			foreach (var item in items)
			{
				foreach (var group in item.Groups)
				{
					if (!groups.Contains(group))
						groups.Add(group);
				}
			}

			groups.Remove(GROUP_ALL);
			return groups;
		}
	}
}
