using System;
using Xsolla.Core;

namespace Xsolla.GameKeys
{
	public static class XsollaGameKeys
	{
		private static string BaseUrl => $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}";

		/// <summary>
		/// Gets a games list for building a catalog.
		/// </summary>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetGamesList(Action<GameItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = "long_description")
		{
			var url = new UrlBuilder($"{BaseUrl}/items/game")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				onError,
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Gets a game for the catalog.
		/// </summary>
		/// <param name="itemSku">Item SKU.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetGameForCatalog(string itemSku, Action<GameItem> onSuccess, Action<Error> onError, string locale = null, string country = null, string additionalFields = "long_description")
		{
			var url = new UrlBuilder($"{BaseUrl}/items/game/sku/{itemSku}")
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				onError,
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Gets a game key for the catalog.
		/// </summary>
		/// <param name="itemSku">Item SKU.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetGameKeyForCatalog(string itemSku, Action<GameKeyItems> onSuccess, Action<Error> onError, string locale = null, string country = null, string additionalFields = "long_description")
		{
			var url = new UrlBuilder($"{BaseUrl}/items/game/key/sku/{itemSku}")
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				onError,
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Gets a games list from the specified group for building a catalog.
		/// </summary>
		/// <param name="groupId">Group external ID.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetGamesListBySpecifiedGroup(string groupId, Action<GameItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = "long_description")
		{
			var url = new UrlBuilder($"{BaseUrl}/items/game/group/{groupId}")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				onError,
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Gets a game key list from the specified group for building a catalog.
		/// </summary>
		/// <param name="groupId">Group external ID.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language. Two-letter lowercase language code per ISO 639-1.</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). If you do not specify the country explicitly, it will be defined by the user IP address.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetGameKeysListBySpecifiedGroup(string groupId, Action<GameKeyItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = "long_description")
		{
			var url = new UrlBuilder($"{BaseUrl}/items/game/key/group/{groupId}")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				onError,
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Grants entitlement by a provided game code.
		/// </summary>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetDrmList(Action<DrmItems> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/items/game/drm";
			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url, onSuccess,
				onError,
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Get the list of games owned by the user. The response will contain an array of games owned by a particular user.
		/// </summary>
		/// <param name="sandbox">What type of entitlements should be returned. If the parameter is set to true, the entitlements received by the user in the sandbox mode only are returned. If the parameter isn't passed or is set to false, the entitlements received by the user in the live mode only are returned.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetOwnedGames(bool sandbox, Action<GameOwnership> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string additionalFields = "long_description")
		{
			var sandboxValue = sandbox ? "1" : "0";
			var url = new UrlBuilder($"{BaseUrl}/entitlement")
				.AddParam("sandbox", sandboxValue)
				.AddLimit(limit)
				.AddOffset(offset)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				onError,
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Grants entitlement by a provided game code.
		/// </summary>
		/// <param name="gameCode">Game code.</param>
		/// <param name="sandbox">Redeem game code in the sandbox mode. The option is available for those users who are specified in the list of company users.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void RedeemGameCode(string gameCode, bool sandbox, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/entitlement/redeem";

			var requestData = new RedeemGameCodeRequest {
				code = gameCode,
				sandbox = sandbox
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RedeemGameCode(gameCode, sandbox, onSuccess, onError)),
				ErrorGroup.LoginErrors);
		}
	}
}