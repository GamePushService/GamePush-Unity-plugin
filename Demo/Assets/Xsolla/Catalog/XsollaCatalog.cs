using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Catalog
{
	public static class XsollaCatalog
	{
		private static string BaseUrl => $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}";

		/// <summary>
		/// Returns a list of virtual items according to pagination settings.
		/// The list includes items for which display in the store is enabled in the settings. For each virtual item, complete data is returned.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual items were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		///     Leave empty to use the default value.</param>
		/// <param name="country">Country to calculate regional prices and restrictions to catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Calculated based on the user's IP address if not specified.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetCatalog(Action<StoreItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = "long_description")
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_items")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetCatalog(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a full list of virtual items. The list includes items for which display in the store is enabled in the settings. For each virtual item, the SKU, name, description, and data about the groups it belongs to are returned.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Response language. <br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		///     Leave empty to use the default value.</param>
		public static void GetCatalogSimplified(Action<StoreShortItems> onSuccess, Action<Error> onError, string locale = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_items/all")
				.AddLocale(locale)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetCatalogSimplified(onSuccess, onError, locale)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of items for the specified group according to pagination settings. The list includes items for which display in the store is enabled in the settings. In the settings of the group, the display in the store must be enabled.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="groupExternalId">Group external ID.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		public static void GetGroupItems(string groupExternalId, Action<StoreItems> onSuccess, Action<Error> onError, int? limit = null, int? offset = null, string locale = null, string country = null, string additionalFields = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_items/group/{groupExternalId}")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetGroupItems(groupExternalId, onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a full list of virtual item groups. The list includes groups for which display in the store is enabled in the settings.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual item groups were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of the item text fields.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		public static void GetItemGroups(Action<Groups> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/groups")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetItemGroups(onSuccess, onError, limit, offset, locale)));
		}

		/// <summary>
		/// Returns a list of virtual currencies according to pagination settings.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual currencies were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetVirtualCurrencyList(Action<VirtualCurrencyItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_currency")
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
				error => TokenAutoRefresher.Check(error, onError, () => GetVirtualCurrencyList(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of virtual currency packages according to pagination settings. The list includes packages for which display in the store is enabled in the settings.
		/// If used after user authentication, the method returns packages that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/).</remarks>
		/// <param name="onSuccess">Called after virtual currency packages were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Response language.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		/// <param name="additionalFields">The list of additional fields. These fields will be in a response if you send them in a request. Available fields `media_list`, `order`, and `long_description`.</param>
		public static void GetVirtualCurrencyPackagesList(Action<VirtualCurrencyPackages> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/virtual_currency/package")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetVirtualCurrencyPackagesList(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns a list of bundles according to pagination settings. The list includes bundles for which display in the store is enabled in the settings.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/#unreal_engine_sdk_how_to_bundles).</remarks>
		/// <param name="onSuccess">Called after bundles are successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page. The maximum number of elements on a page is 50.</param>
		/// <param name="offset">Number of the element from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Defines localization of the item text fields.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		/// <param name="additionalFields">The list of additional fields. This fields will be in a response if you send its in a request. Available fields `media_list`, `order`, `long_description`.</param>
		public static void GetBundles(Action<BundleItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null, string country = null, string additionalFields = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/bundle")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddAdditionalFields(additionalFields)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetBundles(onSuccess, onError, limit, offset, locale, country, additionalFields)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Returns information about the contents of the specified bundle. In the bundle settings, display in the store must be enabled.
		/// If used after user authentication, the method returns items that match the personalization rules for the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/catalog/catalog-display/#unreal_engine_sdk_how_to_bundles).</remarks>
		/// <param name="sku">Bundle SKU.</param>
		/// <param name="onSuccess">Called after the cart is successfully filled.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Defines localization of the item text fields.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="country">Country used to calculate regional prices and restrictions for the catalog. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public static void GetBundle(string sku, Action<BundleItem> onSuccess, Action<Error> onError, string locale = null, string country = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/items/bundle/sku/{sku}")
				.AddLocale(locale)
				.AddCountry(country)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetBundle(sku, onSuccess, onError, locale, country)),
				ErrorGroup.ItemsListErrors);
		}

		/// <summary>
		/// Redeems the coupon code and delivers a reward to the user in one of the following ways:
		/// - to their inventory (virtual items, virtual currency packages, or bundles)
		/// - via email (game keys)
		/// - to the entitlement system (game keys)
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/coupons).</remarks>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after successful coupon redemption.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void RedeemCouponCode(string couponCode, Action<CouponRedeemedItems> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/coupon/redeem";

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			var requestData = new CouponCodeRequest {
				coupon_code = couponCode
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RedeemCouponCode(couponCode, onSuccess, onError)),
				ErrorGroup.CouponErrors);
		}

		/// <summary>
		/// Returns a list of items that can be credited to the user when the coupon is redeemed. Can be used to let users choose one of many items as a bonus. The usual case is choosing a DRM if the coupon contains a game as a bonus.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/coupons).</remarks>
		/// <param name="couponCode">Unique case sensitive code. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after receiving coupon rewards successfully.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetCouponRewards(string couponCode, Action<CouponReward> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/coupon/code/{couponCode}/rewards";

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetCouponRewards(couponCode, onSuccess, onError)),
				ErrorGroup.CouponErrors);
		}

		/// <summary>
		/// Creates an order with a specified item. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/one-click-purchase/).</remarks>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		public static void CreateOrder(string itemSku, Action<OrderData> onSuccess, Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var url = $"{BaseUrl}/payment/item/{itemSku}";
			var requestData = PurchaseParamsGenerator.GeneratePurchaseParamsRequest(purchaseParams);
			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => CreateOrder(itemSku, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorGroup.BuyItemErrors);
		}

		/// <summary>
		/// Creates an order with a specified item, returns unique identifier of the created order and the Pay Station token for the purchase of the specified product by virtual currency. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/purchase-for-vc/).</remarks>
		/// <param name="itemSku">Item SKU to purchase.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, <c>currency</c>, and <c>quantity</c>.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void CreateOrderByVirtualCurrency(string itemSku, string priceSku, Action<OrderId> onSuccess, Action<Error> onError, PurchaseParams purchaseParams = null, string platform = null, Dictionary<string, string> customHeaders = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/payment/item/{itemSku}/virtual/{priceSku}")
				.AddPlatform(platform)
				.Build();

			var requestData = new PurchaseParamsRequest {
				sandbox = XsollaSettings.IsSandbox,
				custom_parameters = purchaseParams?.custom_parameters
			};

			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => CreateOrderByVirtualCurrency(itemSku, priceSku, onSuccess, onError, purchaseParams, platform, customHeaders)),
				ErrorGroup.BuyItemErrors);
		}

		/// <summary>
		/// Create order with specified free item. The created order will get a `done` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="onSuccess">Called after the order was successfully created.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		public static void CreateOrderWithFreeItem(string itemSku, Action<OrderId> onSuccess, Action<Error> onError, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var url = $"{BaseUrl}/free/item/{itemSku}";
			var requestData = PurchaseParamsGenerator.GeneratePurchaseParamsRequest(purchaseParams);
			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest<OrderId, PurchaseParamsRequest>(
				SdkType.Store,
				url,
				requestData,
				headers,
				purchaseData => onSuccess?.Invoke(purchaseData),
				error => TokenAutoRefresher.Check(error, onError, () => CreateOrderWithFreeItem(itemSku, onSuccess, onError, purchaseParams, customHeaders)),
				ErrorGroup.BuyItemErrors);
		}

		/// <summary>
		/// Launches purchase process for a specified item. This method encapsulates methods for creating an order, opening a payment UI, and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/one-click-purchase/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onOrderCreated">Called after the order is created.</param>
		/// <param name="onBrowseClosed">Called after the browser is closed. The event is tracked only when the payment UI is opened in the built-in browser. External browser events can't be tracked.</param>
		/// <param name="purchaseParams">Purchase and payment UI parameters, such as <c>locale</c>, <c>currency</c>, etc.</param>
		/// <param name="customHeaders">Custom web request headers</param>
		/// <param name="platformSpecificAppearance">Additional settings of payment UI appearance for different platforms.</param>
		public static void Purchase(string itemSku, Action<OrderStatus> onSuccess, Action<Error> onError, Action<OrderData> onOrderCreated = null, Action<BrowserCloseInfo> onBrowseClosed = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null, PlatformSpecificAppearance platformSpecificAppearance = null)
		{
			CreateOrder(
				itemSku,
				orderData =>
				{
					onOrderCreated?.Invoke(orderData);

					XsollaWebBrowser.OpenPurchaseUI(
						orderData.token,
						false,
						onBrowseClosed,
						platformSpecificAppearance);

					OrderTrackingService.AddOrderForTracking(orderData.order_id,
						true, () =>
						{
							if (XsollaWebBrowser.InAppBrowser?.IsOpened ?? false)
								XsollaWebBrowser.Close();

							OrderStatusService.GetOrderStatus(orderData.order_id, onSuccess, onError);
						}, onError);
				},
				onError,
				purchaseParams,
				customHeaders);
		}

		/// <summary>
		/// Launch purchase process for a specified item by virtual currency. This method encapsulates methods for creating an order and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/purchase-for-vc/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="priceSku">Virtual currency SKU.</param>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onOrderCreated">Called after the order is created.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="platform">Publishing platform the user plays on.<br/>
		///     Can be `xsolla` (default), `playstation_network`, `xbox_live`, `pc_standalone`, `nintendo_shop`, `google_play`, `app_store_ios`, `android_standalone`, `ios_standalone`, `android_other`, `ios_other`, or `pc_other`.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void PurchaseForVirtualCurrency(string itemSku, string priceSku, Action<OrderStatus> onSuccess, Action<Error> onError, Action<OrderId> onOrderCreated = null, PurchaseParams purchaseParams = null, string platform = null, Dictionary<string, string> customHeaders = null)
		{
			CreateOrderByVirtualCurrency(
				itemSku,
				priceSku,
				orderId =>
				{
					onOrderCreated?.Invoke(orderId);

					OrderTrackingService.AddOrderForTracking(
						orderId.order_id,
						false, () => OrderStatusService.GetOrderStatus(orderId.order_id, onSuccess, onError), onError);
				},
				onError,
				purchaseParams,
				platform,
				customHeaders);
		}

		/// <summary>
		/// Launches purchase process for a specified free item. This method encapsulates methods for creating an order and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="itemSku">Desired free item SKU.</param>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onOrderCreated">Called after the order is created.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void PurchaseFreeItem(string itemSku, Action<OrderStatus> onSuccess, Action<Error> onError, Action<OrderId> onOrderCreated = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			CreateOrderWithFreeItem(
				itemSku,
				orderId =>
				{
					onOrderCreated?.Invoke(orderId);

					OrderTrackingService.AddOrderForTracking(
						orderId.order_id,
						false, () => OrderStatusService.GetOrderStatus(orderId.order_id, onSuccess, onError), onError);
				},
				onError,
				purchaseParams,
				customHeaders);
		}
	}
}
