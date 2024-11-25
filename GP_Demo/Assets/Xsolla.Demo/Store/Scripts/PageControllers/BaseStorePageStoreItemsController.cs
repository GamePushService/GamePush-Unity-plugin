using UnityEngine;

namespace Xsolla.Demo
{
	public abstract class BaseStorePageStoreItemsController : BasePageStoreItemsController
	{
		protected override void InitializeItemUI(GameObject item, ItemModel model)
		{
			item.GetComponent<StoreItemUI>().Initialize((CatalogItemModel)model);
		}
	}
}
