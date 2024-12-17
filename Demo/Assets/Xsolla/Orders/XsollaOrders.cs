using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.Orders
{
	public static class XsollaOrders
	{
		private static string BaseUrl => $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}";

		/// <summary>
		/// Opens Pay Station in the browser with a retrieved Pay Station token.
		/// </summary>
		/// More about the use cases:
		/// - [Cart purchase](https://developers.xsolla.com/sdk/unity/item-purchase/cart-purchase/)
		/// - [Purchase in one click](https://developers.xsolla.com/sdk/unity/item-purchase/one-click-purchase/)
		/// - [Ordering free items](https://developers.xsolla.com/sdk/unity/promo/free-items/#sdk_free_items_order_item_via_cart)
		/// <param name="paymentToken">Pay Station token for the purchase.</param>
		/// <param name="forcePlatformBrowser">Whether to force platform browser usage ignoring plug-in settings.</param>
		/// <param name="onBrowserClosed">Called after the browser was closed.</param>
		/// <seealso cref="XsollaWebBrowser"/>
		public static void OpenPurchaseUI(string paymentToken, bool forcePlatformBrowser = false, Action<BrowserCloseInfo> onBrowserClosed = null)
		{
			XsollaWebBrowser.OpenPurchaseUI(
				paymentToken,
				forcePlatformBrowser,
				onBrowserClosed);
		}

		/// <summary>
		/// Returns status of the specified order.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/item-purchase/track-order/).</remarks>
		/// <param name="orderId">Unique order identifier.</param>
		/// <param name="onSuccess">Called after server response.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="XsollaCatalog.CreateOrderByVirtualCurrency"/>
		public static void CheckOrderStatus(int orderId, Action<OrderStatus> onSuccess, Action<Error> onError)
		{
			OrderStatusService.GetOrderStatus(orderId, onSuccess, onError);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per [ISO 4217](https://en.wikipedia.org/wiki/ISO_4217) (USD by default).</param>
		/// <param name="description">Purchase description. Used to describe the purchase if there are no specific items.</param>
		/// <param name="onSuccess">Called after the successful item purchase.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Interface language. <br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		public static void CreatePaymentToken(
			float amount,
			string currency,
			string description,
			Action<PaymentToken> onSuccess,
			Action<Error> onError,
			string locale = null,
			string externalID = null,
			int? paymentMethod = null,
			object customParameters = null)
		{
			var url = $"{BaseUrl}/payment";
			var checkout = new CreatePaymentTokenRequest.Purchase.Checkout(amount, currency);
			var purchaseDescription = new CreatePaymentTokenRequest.Purchase.Description(description);
			var purchase = new CreatePaymentTokenRequest.Purchase(checkout, purchaseDescription);
			var settings = GeneratePaymentTokenSettings(currency, locale, externalID, paymentMethod);
			var requestData = new CreatePaymentTokenRequest(purchase, settings, customParameters);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				PurchaseParamsGenerator.GeneratePaymentHeaders(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => CreatePaymentToken(amount, currency, description, onSuccess, onError, locale, externalID, paymentMethod, customParameters)),
				ErrorGroup.BuyItemErrors);
		}

		/// <summary>
		/// Creates a new payment token.
		/// </summary>
		/// <param name="amount">The total amount to be paid by the user.</param>
		/// <param name="currency">Default purchase currency. Three-letter code per [ISO 4217](https://en.wikipedia.org/wiki/ISO_4217) (USD by deafault).</param>
		/// <param name="items">Used to describe a purchase if it includes a list of specific items.</param>
		/// <param name="onSuccess">Called after the successful item purchase.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Interface language. <br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).</param>
		/// <param name="externalID">Transaction's external ID.</param>
		/// <param name="paymentMethod">Payment method ID.</param>
		/// <param name="customParameters">Your custom parameters represented as a valid JSON set of key-value pairs.</param>
		public static void CreatePaymentToken(
			float amount,
			string currency,
			PaymentTokenItem[] items,
			Action<PaymentToken> onSuccess,
			Action<Error> onError,
			string locale = null,
			string externalID = null,
			int? paymentMethod = null,
			object customParameters = null)
		{
			var url = $"{BaseUrl}/payment";
			var checkout = new CreatePaymentTokenRequest.Purchase.Checkout(amount, currency);
			var purchaseItems = new List<CreatePaymentTokenRequest.Purchase.Item>(items.Length);

			foreach (var item in items)
			{
				var price = new CreatePaymentTokenRequest.Purchase.Item.Price(item.Amount, item.AmountBeforeDiscount);
				var purchaseItem = new CreatePaymentTokenRequest.Purchase.Item(item.Name, price, item.ImageUrl, item.Description, item.Quantity, item.IsBonus);
				purchaseItems.Add(purchaseItem);
			}

			var purchase = new CreatePaymentTokenRequest.Purchase(checkout, purchaseItems.ToArray());
			var settings = GeneratePaymentTokenSettings(currency, locale, externalID, paymentMethod);
			var requestData = new CreatePaymentTokenRequest(purchase, settings, customParameters);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Store,
				url,
				requestData,
				PurchaseParamsGenerator.GeneratePaymentHeaders(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => CreatePaymentToken(amount, currency, items, onSuccess, onError, locale, externalID, paymentMethod, customParameters)),
				ErrorGroup.BuyItemErrors);
		}

		private static CreatePaymentTokenRequest.Settings GeneratePaymentTokenSettings(string currency, string locale, string externalID, int? paymentMethod)
		{
			var baseSettings = new PurchaseParamsRequest.Settings {
				ui = PayStationUISettings.GenerateSettings(),
				redirect_policy = RedirectPolicySettings.GeneratePolicy()
			};

			baseSettings.return_url = baseSettings.redirect_policy?.return_url;

			var settings = new CreatePaymentTokenRequest.Settings {
				return_url = baseSettings.return_url,
				ui = baseSettings.ui,
				redirect_policy = baseSettings.redirect_policy,
				currency = currency,
				locale = locale,
				sandbox = XsollaSettings.IsSandbox,
				external_id = externalID,
				payment_method = paymentMethod
			};

			return settings;
		}
	}
}