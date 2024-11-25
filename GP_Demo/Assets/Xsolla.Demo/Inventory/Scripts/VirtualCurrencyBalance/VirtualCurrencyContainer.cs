using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public class VirtualCurrencyContainer : MonoBehaviour
	{
		[SerializeField] private WidgetProvider virtualCurrencyBalanceProvider;

		private readonly Dictionary<string, VirtualCurrencyBalanceUI> _currencies =
			new Dictionary<string, VirtualCurrencyBalanceUI>();

		private void Awake()
		{
			if (virtualCurrencyBalanceProvider.GetValue() != null) return;
			XDebug.LogWarning("VirtualCurrencyBalancePrefab is missing!");
			Destroy(gameObject);
		}

		private void Start()
		{
			if (UserCatalog.Instance.IsUpdated && UserInventory.Instance.IsUpdated)
			{
				SetCurrencies(UserCatalog.Instance.VirtualCurrencies);
				SetCurrenciesBalance(UserInventory.Instance.Balance);
			}
			UserCatalog.Instance.UpdateVirtualCurrenciesEvent += SetCurrencies;
			UserInventory.Instance.UpdateVirtualCurrencyBalanceEvent += SetCurrenciesBalance;
		}

		private void OnDestroy()
		{
			if(UserCatalog.IsExist)
				UserCatalog.Instance.UpdateVirtualCurrenciesEvent -= SetCurrencies;
			if(UserInventory.IsExist)
				UserInventory.Instance.UpdateVirtualCurrencyBalanceEvent -= SetCurrenciesBalance;
		}

		private void SetCurrencies(List<VirtualCurrencyModel> items)
		{
			_currencies.Values.ToList().ForEach(c =>
			{
				if (c.gameObject != null)
					Destroy(c.gameObject);
			});
			_currencies.Clear();
			items.ForEach(i => AddCurrency(i));
		}

		private VirtualCurrencyBalanceUI AddCurrency(VirtualCurrencyModel item)
		{
			if (item == null) return null;
			if (_currencies.ContainsKey(item.Sku)) return _currencies[item.Sku];
			if (string.IsNullOrEmpty(item.ImageUrl)) return null;
			var currencyBalance = Instantiate(virtualCurrencyBalanceProvider.GetValue(), transform);
			var balanceUi = currencyBalance.GetComponent<VirtualCurrencyBalanceUI>();
			balanceUi.Initialize(item);
			_currencies.Add(item.Sku, balanceUi);
			return balanceUi;
		}

		private void SetCurrenciesBalance(List<VirtualCurrencyBalanceModel> balance)
		{
			balance.ForEach(SetCurrencyBalance);
		}

		private void SetCurrencyBalance(VirtualCurrencyBalanceModel balance)
		{
			AddCurrency(new VirtualCurrencyModel
			{
				Sku = balance.Sku,
				ImageUrl = balance.ImageUrl
			})?.SetBalance(balance.Amount);
		}
	}
}