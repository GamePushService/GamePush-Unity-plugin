using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Cart
{
	public static class XsollaCart
	{
		private static string BaseUrl => $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}";

		/// <summary>
		/// 	Returns a list of items from the cart with the specified ID or from the cart of the current user. For each item, complete data is returned.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="onSuccess">Called after local cache of cart items was successfully updated.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="locale">Defines localization of item's text fields.<br/> The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="currency">The currency in which prices are displayed. Three-letter currency code per [ISO 4217](https://en.wikipedia.org/wiki/ISO_4217) (USD by default).</param>
		public static void GetCartItems(Action<Cart> onSuccess, Action<Error> onError, string cartId = null, string locale = null, string currency = null)
		{
			var url = string.IsNullOrEmpty(cartId)
				? BaseUrl + "/cart"
				: BaseUrl + $"/cart/{cartId}";

			url = new UrlBuilder(url)
				.AddLocale(locale)
				.AddCurrency(currency)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetCartItems(onSuccess, onError, cartId, locale, currency)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Fills the cart with the specified ID or the cart of the current user with items. If there is already an item with the same SKU in the cart, the existing item position will be replaced by the passed value.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="items">Item for filling the cart. If there is already an item with the same SKU in the cart, the existing item position will be replaced by the passed value.</param>
		/// <param name="onSuccess">Called after cart is successfully filled.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		public static void FillCart(List<CartFillItem> items, Action onSuccess, Action<Error> onError, string cartId = null)
		{
			var url = string.IsNullOrWhiteSpace(cartId)
				? $"{BaseUrl}/cart/fill"
				: $"{BaseUrl}/cart/{cartId}/fill";

			var requestData = new CartFillRequest {
				items = items
			};

			WebRequestHelper.Instance.PutRequest(
				SdkType.Store,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => FillCart(items, onSuccess, onError, cartId)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Updates the quantity of a previously added item in the cart with the specified ID or in the current user cart. If there is no item with the specified SKU in the cart, it will be added.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="itemSku">SKU of item for purchase.</param>
		/// <param name="quantity">Quantity of purchased items.</param>
		/// <param name="onSuccess">Called after successfully adding a new item to the cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		public static void UpdateItemInCart(string itemSku, int quantity, Action onSuccess, Action<Error> onError, string cartId = null)
		{
			var url = string.IsNullOrWhiteSpace(cartId)
				? $"{BaseUrl}/cart/item/{itemSku}"
				: $"{BaseUrl}/cart/{cartId}/item/{itemSku}";

			var requestData = new QuantityRequest {
				quantity = quantity
			};

			WebRequestHelper.Instance.PutRequest(
				SdkType.Store,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UpdateItemInCart(itemSku, quantity, onSuccess, onError, cartId)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Removes the item from the cart with the specified ID or from the cart of the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="itemSku">Item SKU to delete.</param>
		/// <param name="onSuccess">Called after successfully removing an item from the cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		public static void RemoveItemFromCart(string itemSku, Action onSuccess, Action<Error> onError, string cartId = null)
		{
			var url = string.IsNullOrWhiteSpace(cartId)
				? $"{BaseUrl}/cart/item/{itemSku}"
				: $"{BaseUrl}/cart/{cartId}/item/{itemSku}";

			WebRequestHelper.Instance.DeleteRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RemoveItemFromCart(itemSku, onSuccess, onError, cartId)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Removes all items from the cart with the specified ID or from the cart of the current user.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="onSuccess">Called after successful cart clearing.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		public static void ClearCart(Action onSuccess, Action<Error> onError, string cartId = null)
		{
			var url = string.IsNullOrWhiteSpace(cartId)
				? $"{BaseUrl}/cart/clear"
				: $"{BaseUrl}/cart/{cartId}/clear";

			WebRequestHelper.Instance.PutRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => ClearCart(onSuccess, onError, cartId)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Redeems a promo code. After activating the promo code, the user gets free items and/or the price of the cart is reduced.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/promo-codes/).</remarks>
		/// <param name="promocode">Unique code of promocode. Contains letters and numbers.</param>
		/// <param name="cartId">Unique cart identifier. The current user cart will be updated if empty.</param>
		/// <param name="onSuccess">Called after successful promocode redemption.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void RedeemPromocode(string promocode, string cartId, Action<Cart> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/promocode/redeem";

			var requestData = new RedeemPromocodeRequest {
				coupon_code = promocode,
				cart = string.IsNullOrEmpty(cartId) ? null : new RedeemPromocodeRequest.Cart {id = cartId}
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RedeemPromocode(promocode, cartId, onSuccess, onError)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Returns a list of items that can be credited to the user when the promo code is activated. Allows users to choose from several available items.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/promo-codes/#sdk_promo_codes).</remarks>
		/// <param name="promocode">Unique code of promocode. Contains letters and numbers.</param>
		/// <param name="onSuccess">Called after successfully receiving promocode rewards.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetPromocodeReward(string promocode, Action<PromocodeReward> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/promocode/code/{promocode}/rewards";

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetPromocodeReward(promocode, onSuccess, onError)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Removes a promo code from a cart. After the promo code is removed, the total price of all items in the cart will be recalculated without bonuses and discounts provided by a promo code.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/promo-codes/).</remarks>
		/// <param name="cartId">Cart ID. The current user cart will be updated if empty.</param>
		/// <param name="onSuccess">Called after the promo code  has been successful removed from cart.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void RemovePromocodeFromCart(string cartId, Action<RemovePromocodeFromCartResult> onSuccess, Action<Error> onError = null)
		{
			var url = $"{BaseUrl}/promocode/remove";

			var requestData = new RemovePromocodeFromCartRequest {
				id = cartId
			};

			WebRequestHelper.Instance.PutRequest(
				SdkType.Store,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RemovePromocodeFromCart(cartId, onSuccess, onError)),
				ErrorGroup.CartErrors);
		}

		/// <summary>
		/// Creates an order with items from the cart with the specified ID. Returns the payment token and order ID. The created order will get a `new` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="onSuccess">Called after the order was successfully created.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, <c>currency</c>, and <c>quantity</c>.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void CreateOrder(Action<OrderData> onSuccess, Action<Error> onError, string cartId = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var url = string.IsNullOrWhiteSpace(cartId)
				? $"{BaseUrl}/payment/cart"
				: $"{BaseUrl}/payment/cart/{cartId}";

			var requestData = PurchaseParamsGenerator.GeneratePurchaseParamsRequest(purchaseParams);
			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => CreateOrder(onSuccess, onError, cartId, purchaseParams, customHeaders)),
				ErrorGroup.BuyCartErrors);
		}

		/// <summary>
		/// Create order with particular free cart. The created order will get a `done` order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, <c>currency</c>, and <c>quantity</c>.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void CreateOrderWithFreeCart(Action<OrderId> onSuccess, Action<Error> onError, string cartId = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			var url = string.IsNullOrWhiteSpace(cartId)
				? $"{BaseUrl}/free/cart"
				: $"{BaseUrl}/free/cart/{cartId}";

			var requestData = PurchaseParamsGenerator.GeneratePurchaseParamsRequest(purchaseParams);
			var headers = PurchaseParamsGenerator.GeneratePaymentHeaders(customHeaders);

			WebRequestHelper.Instance.PostRequest<OrderId, PurchaseParamsRequest>(
				SdkType.Store,
				url,
				requestData,
				headers,
				purchaseData => onSuccess?.Invoke(purchaseData),
				error => TokenAutoRefresher.Check(error, onError, () => CreateOrderWithFreeCart(onSuccess, onError, cartId, purchaseParams, customHeaders)),
				ErrorGroup.BuyCartErrors);
		}

		/// <summary>
		/// 	Launches purchase process for the cart with the specified ID or for the cart of the current user. This method encapsulates methods for creating an order, opening a payment UI, and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/).</remarks>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="onBrowseClosed">Called after the browser is closed. The event is tracked only when the payment UI is opened in the built-in browser. External browser events can't be tracked.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void Purchase(Action<OrderStatus> onSuccess, Action<Error> onError, string cartId = null, Action<BrowserCloseInfo> onBrowseClosed = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			CreateOrder(
				orderData =>
				{
					XsollaWebBrowser.OpenPurchaseUI(
						orderData.token,
						false,
						onBrowseClosed);

					OrderTrackingService.AddOrderForTracking(orderData.order_id,
						true, () =>
						{
							if (XsollaWebBrowser.InAppBrowser?.IsOpened ?? false)
								XsollaWebBrowser.Close();

							OrderStatusService.GetOrderStatus(orderData.order_id, onSuccess, onError);
						}, onError);
				},
				onError,
				cartId,
				purchaseParams,
				customHeaders
			);
		}

		/// <summary>
		/// Launches purchase process for the free cart with the specified ID or for the free cart of the current user. This method encapsulates methods for creating an order and tracking the order status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/promo/free-items/).</remarks>
		/// <param name="onSuccess">Called after the order transitions to the 'done' status.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="cartId">Unique cart identifier.</param>
		/// <param name="purchaseParams">Purchase parameters such as <c>country</c>, <c>locale</c>, and <c>currency</c>.</param>
		/// <param name="customHeaders">Custom HTTP request headers.</param>
		public static void PurchaseFreeCart(Action<OrderStatus> onSuccess, Action<Error> onError, string cartId = null, PurchaseParams purchaseParams = null, Dictionary<string, string> customHeaders = null)
		{
			CreateOrderWithFreeCart(
				orderId =>
				{
					OrderTrackingService.AddOrderForTracking(
						orderId.order_id,
						false, () => OrderStatusService.GetOrderStatus(orderId.order_id, onSuccess, onError), onError);
				},
				onError,
				cartId,
				purchaseParams,
				customHeaders
			);
		}
	}
}
