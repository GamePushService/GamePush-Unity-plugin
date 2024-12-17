using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Cart;
using Xsolla.Catalog;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public class StoreLogic : MonoSingleton<StoreLogic>
	{
		private static Dictionary<string, string> GenerateCustomHeaders()
		{
#if UNITY_STANDALONE || UNITY_EDITOR
			if (DemoSettings.UseSteamAuth && DemoSettings.PaymentsFlow == PaymentsFlow.SteamGateway)
				return SteamUtils.GetAdditionalCustomHeaders();
#endif
			return null;
		}

		public static void PurchaseForRealMoney(CatalogItemModel item, Action<CatalogItemModel> onSuccess, Action<Error> onError)
		{
			XsollaCatalog.Purchase(
				item.Sku,
				_ => CompletePurchase(item, () => onSuccess?.Invoke(item)),
				error => OnPurchaseError(error, onError),
				customHeaders: GenerateCustomHeaders());
		}

		public static void PurchaseFreeItem(CatalogItemModel item, Action<CatalogItemModel> onSuccess, Action<Error> onError)
		{
			XsollaCatalog.PurchaseFreeItem(item.Sku,
				_ => CompletePurchase(item, () => onSuccess?.Invoke(item)),
				error => OnPurchaseError(error, onError),
				customHeaders: GenerateCustomHeaders());
		}

		public static void PurchaseForVirtualCurrency(CatalogItemModel item, Action<CatalogItemModel> onSuccess, Action<Error> onError)
		{
			var onConfirmation = new Action(() =>
			{
				var isPurchaseComplete = false;
				PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => isPurchaseComplete);

				XsollaCatalog.PurchaseForVirtualCurrency(
					item.Sku,
					item.VirtualPrice?.Key,
					_ =>
					{
						isPurchaseComplete = true;
						CompletePurchase(item, () => onSuccess?.Invoke(item));
					},
					error =>
					{
						isPurchaseComplete = true;
						OnPurchaseError(error, onError);
					},
					customHeaders: GenerateCustomHeaders());
			});

			StoreDemoPopup.ShowConfirm(onConfirmation);
		}

		public static void PurchaseCart(List<UserCartItem> items, Action<List<UserCartItem>> onSuccess, Action<Error> onError)
		{
			if (!items.Any())
			{
				var error = new Error(errorMessage: "Cart is empty");
				onError?.Invoke(error);
				return;
			}

			XsollaCart.GetCartItems(
				newCart =>
				{
					XsollaCart.ClearCart(
						() =>
						{
							var cartItems = items
								.Select(i => new CartFillItem {
									sku = i.Sku,
									quantity = i.Quantity
								}).ToList();

							XsollaCart.FillCart(
								cartItems,
								() =>
								{
									XsollaCart.Purchase(
										_ => CompletePurchase(null, () => onSuccess?.Invoke(items)),
										error => OnPurchaseError(error, onError),
										newCart.cart_id,
										customHeaders:
										GenerateCustomHeaders()
									);
								},
								error => OnPurchaseError(error, onError),
								newCart.cart_id);
						},
						error => OnPurchaseError(error, onError),
						newCart.cart_id);
				},
				error => OnPurchaseError(error, onError),
				XsollaSettings.StoreProjectId);
		}

		public static void PurchaseFreeCart(List<UserCartItem> items, Action<List<UserCartItem>> onSuccess, Action<Error> onError)
		{
			if (!items.Any())
			{
				var error = new Error(errorMessage: "Cart is empty");
				onError?.Invoke(error);
				return;
			}

			XsollaCart.GetCartItems(
				newCart =>
				{
					XsollaCart.ClearCart(
						() =>
						{
							var cartItems = items
								.Select(i => new CartFillItem {
									sku = i.Sku,
									quantity = i.Quantity
								}).ToList();

							XsollaCart.FillCart(
								cartItems,
								() =>
								{
									XsollaCart.PurchaseFreeCart(
										_ => CompletePurchase(null, () => onSuccess?.Invoke(items)),
										onError,
										newCart.cart_id,
										customHeaders: GenerateCustomHeaders()
									);
								},
								error => OnPurchaseError(error, onError));
						},
						error => OnPurchaseError(error, onError),
						newCart.cart_id);
				},
				error => OnPurchaseError(error, onError),
				XsollaSettings.StoreProjectId);
		}

		private static void OnPurchaseError(Error error, Action<Error> callback)
		{
			StoreDemoPopup.ShowError(error);
			callback?.Invoke(error);
		}

		private static void CompletePurchase(ItemModel item, Action popupCallback)
		{
			UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError);

			Action callback = () =>
			{
				if (item != null)
					StoreDemoPopup.ShowSuccess($"You have purchased '{item.Name}'", popupCallback);
				else
					StoreDemoPopup.ShowSuccess(null, popupCallback);
			};

			var inAppBrowser = XsollaWebBrowser.InAppBrowser;
			if (inAppBrowser != null && inAppBrowser.IsOpened)
				inAppBrowser.AddCloseHandler(callback);
			else
				callback.Invoke();
		}
	}
}