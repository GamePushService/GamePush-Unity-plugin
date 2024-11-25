using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class CharacterLevelManager : MonoBehaviour
	{
		[SerializeField] private string CurrencySkuOrName = default;
		[SerializeField] private bool TryUseDefaultCurrencyOnFailure = default;
		[SerializeField] private int LevelUpPrice = default;
		[SerializeField] private VirtualCurrencyBalanceUI VirtualCurrencyPrefab = default;
		[SerializeField] private Transform PriceTagPlacement = default;
		[SerializeField] private SimpleButton LevelUpButton = default;
		[SerializeField] private Text LevelText = default;

		private VirtualCurrencyModel _targetCurrency;
		private string _characterLevelEntry;
		private int _characterLevel;

		private void Awake()
		{
			LevelUpButton.onClick += TryUpTheLevel;
		}

		private void Start()
		{

			Action<UserInfo> successCallback = info =>
			{
				var levelEntryHead = info.email ?? info.username ?? info.nickname ?? info.last_name;
				_characterLevelEntry = $"{levelEntryHead}:character_level";
				InitializeUI();
			};

			Action<Error> errorCallback = error =>
			{
				XDebug.LogError("Could not get info for character level save/load");
				InitializeUI();
			};

			XsollaAuth.GetUserInfo(successCallback, errorCallback);
		}

		private void InitializeUI()
		{
			_targetCurrency = GetTargetCurrency();

			if(_targetCurrency == null)
			{
				XDebug.Log("Could not obtain target currency");
				LevelUpButton.onClick -= TryUpTheLevel;
				return;
			}

			EnableUI();

			_characterLevel = LoadLevel(_characterLevelEntry);
			ShowLevel(_characterLevel);
			DrawPrice(_targetCurrency, LevelUpPrice);
		}

		private void EnableUI()
		{
			LevelUpButton.gameObject.SetActive(true);
			LevelText.gameObject.SetActive(true);
		}

		private void TryUpTheLevel()
		{
			var userCurrency = UserInventory.Instance.Balance?.Find(currency => _targetCurrency.Sku == currency.Sku);
			if (userCurrency == null)
			{
				XDebug.Log("UserInventory does not contain required currency");
				return;
			}

			if (userCurrency.Amount >= LevelUpPrice)
			{
				var levelUpPayment = new InventoryItemModel()
				{
					Name = _targetCurrency.Name,
					Sku = _targetCurrency.Sku,
				};

				Action onSuccessConsume = () =>
				{
					_characterLevel++;
					SaveLevel(_characterLevelEntry, _characterLevel);
					ShowLevel(_characterLevel);

					if (UserInventory.IsExist)
						UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);
				};

				if (DemoMarker.IsInventoryPartAvailable)
				{
					DemoInventory.Instance.ConsumeInventoryItem(
						levelUpPayment,
						(int)LevelUpPrice,
						onSuccess: _ => onSuccessConsume.Invoke(),
						onError: _ => XDebug.Log("Could not consume virtual currency"));
				}
				else
				{
					XDebug.LogError("Character level up supposed to be hidden in case InventoryDemo was not provided");
				}
			}
			else
			{
				var errorMessage = "Not enough currency amount to up the level";
				XDebug.Log(errorMessage);
				StoreDemoPopup.ShowWarning(new Error(errorMessage: errorMessage));
			}
		}

		private VirtualCurrencyModel GetTargetCurrency()
		{
			var allCurrencies = UserCatalog.Instance.VirtualCurrencies;
			if (allCurrencies == null)
			{
				XDebug.LogWarning("UserCatalog returned null as VirtualCurrencies");
				return null;
			}

			VirtualCurrencyModel targetCurrency = null;

			if (!string.IsNullOrEmpty(CurrencySkuOrName))
			{
				targetCurrency = allCurrencies.Find(currency => { return CurrencySkuOrName == currency.Sku || CurrencySkuOrName == currency.Name; });

				if (/*still*/targetCurrency == null)
				{
					XDebug.LogWarning($"Could not find specified virtual currency: {CurrencySkuOrName}");

					if (TryUseDefaultCurrencyOnFailure)
					{
						XDebug.Log("Will try to get default project's currency");
						targetCurrency = allCurrencies.Count > 0 ? allCurrencies[0] : null;
					}
				}
			}
			else
				targetCurrency = allCurrencies.Count > 0 ? allCurrencies[0] : null;

			if (targetCurrency == null)
				XDebug.LogWarning("Could not find virtual currency");

			return targetCurrency;
		}

		private void DrawPrice(VirtualCurrencyModel priceCurrency, int price)
		{
			var currencyObject = Instantiate(VirtualCurrencyPrefab.gameObject, PriceTagPlacement);
			var currencyUI = currencyObject.GetComponent<VirtualCurrencyBalanceUI>();
			currencyUI.Initialize(priceCurrency);
			currencyUI.SetBalance(price);
		}

		private int LoadLevel(string levelEntry)
		{
			if (!string.IsNullOrEmpty(levelEntry))
				return PlayerPrefs.GetInt(levelEntry, 1);
			else
			{
				XDebug.LogError("Could not load level. Level entry is null or empty");
				return 1;
			}
		}

		private void SaveLevel(string levelEntry, int characterLevel)
		{
			if (!string.IsNullOrEmpty(levelEntry))
				PlayerPrefs.SetInt(levelEntry, characterLevel);
			else
				XDebug.LogError("Could not save level. Level entry is null or empty");
		}

		private void ShowLevel(int characterLevel)
		{
			LevelText.text = $"{_characterLevel} LVL";
		}

	#if UNITY_EDITOR
		[ContextMenu("Drop the level")]
		private void DropTheLevel()
		{
			if (EditorApplication.isPlaying && !string.IsNullOrEmpty(_characterLevelEntry))
			{
				_characterLevel = 1;
				SaveLevel(_characterLevelEntry, _characterLevel);
				ShowLevel(_characterLevel);
			}
		}
	#endif
	}
}
