using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.Demo.Popup;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public class CartMenuController : MonoBehaviour
	{
		[SerializeField] private WidgetProvider ItemPrefabProvider = new WidgetProvider();
		[SerializeField] private GameObject itemsHeader = default;
		[SerializeField] private ItemContainer itemsContainer = default;
		[SerializeField] private CartControls cartControls = default;
		[SerializeField] private GameObject emptyCartMessage = default;
		[SerializeField] private SimpleButton goToStoreButton = default;

		private GameObject ItemPrefab => ItemPrefabProvider.GetValue();

		private readonly List<GameObject> _items = new List<GameObject>();

		protected virtual void Start()
		{
			if (ItemPrefab == null || itemsContainer == null || cartControls == null)
			{
				var message = "Cart prefab is broken. Some fields is null.";
				XDebug.LogError(message);
				PopupFactory.Instance.CreateError()
					.SetMessage(message)
					.SetCallback(() => DemoController.Instance.SetState(MenuState.Main));
				return;
			}

			UserCart.Instance.ClearCartEvent += Refresh;
			UserCart.Instance.UpdateItemEvent += OnUpdateItemEvent;
			UserCart.Instance.RemoveItemEvent += OnRemoveItemEvent;

			goToStoreButton.onClick = () => DemoController.Instance.SetPreviousState();

			cartControls.OnClearCart = UserCart.Instance.Clear;
			cartControls.OnBuyCart = OnBuyCart;
			Refresh();
		}

		private void OnDestroy()
		{
			UserCart.Instance.ClearCartEvent -= Refresh;
			UserCart.Instance.UpdateItemEvent -= OnUpdateItemEvent;
			UserCart.Instance.RemoveItemEvent -= OnRemoveItemEvent;
		}

		private void OnUpdateItemEvent(UserCartItem item, int deltaCount) => Refresh();
		private void OnRemoveItemEvent(UserCartItem item) => Refresh();

		private void Refresh()
		{
			ClearCartItems();
			PutItemsToContainer(UserCart.Instance.GetItems());
			InitPrices();
		}

		private void ClearCartItems()
		{
			_items.ForEach(Destroy);
			_items.Clear();
		}

		private void InitPrices()
		{
			var totalPrice = UserCart.Instance.CalculateFullPrice();
			var discount = UserCart.Instance.CalculateCartDiscount();
			if (discount >= 0.01F)
			{
				totalPrice -= discount;
				cartControls.Initialize(totalPrice, discount);
			}
			else
				cartControls.Initialize(totalPrice);
		}

		private void OnBuyCart()
		{
			UserCart.Instance.Purchase(
				onSuccess: DemoController.Instance.SetPreviousState);
		}

		private void PutItemsToContainer(List<UserCartItem> items)
		{
			ShowEmptyMessage(!items.Any());
			ShowCartContent(items.Any());

			itemsContainer.Clear();
			items.ForEach(i =>
			{
				var go = itemsContainer.AddItem(ItemPrefab);
				go.GetComponent<CartItemUI>().Initialize(i);
				_items.Add(go);
			});
		}

		private void ShowEmptyMessage(bool showEmptyMessage)
		{
			emptyCartMessage.SetActive(showEmptyMessage);
		}

		private void ShowCartContent(bool showCartContent)
		{
			itemsHeader.SetActive(showCartContent);
			itemsContainer.gameObject.SetActive(showCartContent);
			cartControls.gameObject.SetActive(showCartContent);
		}
	}
}
