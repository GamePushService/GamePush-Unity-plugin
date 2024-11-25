using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Inventory
{
	public static class XsollaInventory
	{
		private static string BaseUrl => $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}";

		/// <summary>
		/// Returns the current user’s balance of virtual currency. For each virtual currency, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/display-inventory/).</remarks>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after virtual currency balance was successfully received.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		public static void GetVirtualCurrencyBalance(Action<VirtualCurrencyBalances> onSuccess, Action<Error> onError, string platform = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/virtual_currency_balance")
				.AddPlatform(platform)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetVirtualCurrencyBalance(onSuccess, onError, platform)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of virtual items from the user’s inventory according to pagination settings. For each virtual item, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/display-inventory/).</remarks>
		/// <param name="onSuccess">Called after purchased virtual items were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of item's text fields.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		public static void GetInventoryItems(Action<InventoryItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string platform = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/inventory/items")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddPlatform(platform)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetInventoryItems(onSuccess, onError, limit, offset, locale, platform)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Consumes an inventory item. Use for only for consumable virtual items.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/consume-item/).</remarks>
		/// <param name="item">Contains consume parameters.</param>
		/// <param name="onSuccess">Called after successful inventory item consumption.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		public static void ConsumeInventoryItem(ConsumeItem item, Action onSuccess, Action<Error> onError, string platform = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/inventory/item/consume")
				.AddPlatform(platform)
				.Build();

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				item,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => ConsumeInventoryItem(item, onSuccess, onError, platform)),
				ErrorGroup.ConsumeItemErrors);
		}

		/// <summary>
		/// Returns a list of time-limited items from the user’s inventory. For each item, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/player-inventory/display-inventory/).</remarks>
		/// <param name="onSuccess">Called after list of user time limited items was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		public static void GetTimeLimitedItems(Action<TimeLimitedItems> onSuccess, Action<Error> onError, string platform = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/time_limited_items")
				.AddPlatform(platform)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetTimeLimitedItems(onSuccess, onError, platform)),
				ErrorGroup.ItemsListErrors);
		}
	}
}
