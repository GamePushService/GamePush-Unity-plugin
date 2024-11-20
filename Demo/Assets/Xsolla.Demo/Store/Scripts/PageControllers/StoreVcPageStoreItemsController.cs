using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Xsolla.Demo
{
	public class StoreVcPageStoreItemsController : BaseStorePageStoreItemsController
	{
		protected override bool IsShowContent => UserCatalog.Instance.HasCurrencyPackages;

		protected override List<ItemModel> GetItemsByGroup(string groupName)
		{
			var items = (groupName.Equals(GROUP_ALL))
				? UserCatalog.Instance.CurrencyPackages
				: UserCatalog.Instance.CurrencyPackages.Where(i =>
				{
					var currency = UserCatalog.Instance.VirtualCurrencies.First(vc => vc.Name.Equals(groupName));
					return i.CurrencySku.Equals(currency.Sku);
				}).ToList();

			return items.Cast<ItemModel>().ToList();
		}

		protected override List<string> GetGroups()
		{
			var items = UserCatalog.Instance.CurrencyPackages;

			return UserCatalog.Instance.VirtualCurrencies
				.Where(vc => items.Any(i => i.CurrencySku.Equals(vc.Sku)))
				.Select(vc => vc.Name).ToList();
		}
	}
}