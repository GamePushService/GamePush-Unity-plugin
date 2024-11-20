using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class DemoController : MonoSingleton<DemoController>
	{
		partial void UpdateStore()
		{
			UserCatalog.Instance.UpdateItems(
			onSuccess: () =>
			{
				if (UserInventory.IsExist)
				{
					UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
					// This method used for fastest async image loading
					StartLoadItemImages(UserCatalog.Instance.AllItems);
				}
			},
			onError: StoreDemoPopup.ShowError);
		}

		partial void DestroyStore()
		{
			if (UserCatalog.IsExist)
				Destroy(UserCatalog.Instance.gameObject);
			if (UserSubscriptions.IsExist)
				Destroy(UserSubscriptions.Instance.gameObject);
			if (UserCart.IsExist)
				Destroy(UserCart.Instance.gameObject);
		}

		private void StartLoadItemImages(List<CatalogItemModel> items)
		{
			items.ForEach(i =>
			{
				if (!string.IsNullOrEmpty(i.ImageUrl))
					ImageLoader.LoadSprite(i.ImageUrl, null);
				else
					XDebug.LogError($"Catalog item with sku = '{i.Sku}' without image!");
			});
		}
	}
}
