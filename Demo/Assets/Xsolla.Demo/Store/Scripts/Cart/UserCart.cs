using System;
using System.Collections.Generic;
using System.Linq;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class UserCart : MonoSingleton<UserCart>
	{
		public event Action<UserCartItem> AddItemEvent;
		public event Action<UserCartItem> RemoveItemEvent;
		public event Action<UserCartItem, int> UpdateItemEvent;
		public event Action PurchaseCartEvent;
		public event Action ClearCartEvent;

		private UserCartModel _cart;

		public override void Init()
		{
			base.Init();
			_cart = new UserCartModel();
		}

		public bool Contains(string sku)
		{
			return _cart.GetCartItems().Any(i => i.Sku.Equals(sku));
		}

		public void AddItem(CatalogItemModel item)
		{
			UserCartItem cartItem = _cart.AddItem(item);
			if (cartItem != null)
				AddItemEvent?.Invoke(cartItem);
		}

		public void RemoveItem(CatalogItemModel item)
		{
			var cartItem = _cart.GetItem(item);
			if(!_cart.RemoveItem(item)) return;
			if (cartItem.Quantity > 1)
				UpdateItemEvent?.Invoke(cartItem, (-1) * (cartItem.Quantity - 1));
			RemoveItemEvent?.Invoke(cartItem);
		}

		public void IncreaseCountOf(CatalogItemModel item)
		{
			var cartItem = _cart.IncreaseCountOf(item);
			if (cartItem != null)
			{
				UpdateItemEvent?.Invoke(cartItem, 1);
			}
		}

		public void DecreaseCountOf(CatalogItemModel item)
		{
			UserCartItem cartItem = _cart.DecreaseCountOf(item);
			if (cartItem == null) return;
			if (cartItem.Quantity == 0)
				RemoveItemEvent?.Invoke(cartItem);
			else
				UpdateItemEvent?.Invoke(cartItem, -1);
		}

		public List<UserCartItem> GetItems()
		{
			return _cart.GetCartItems();
		}

		public void Clear()
		{
			var items = GetItems();
			_cart.Clear();
			items.ForEach(i => RemoveItemEvent?.Invoke(i));
			ClearCartEvent?.Invoke();
		}

		public bool IsEmpty()
		{
			return _cart.IsEmpty();
		}

		public float CalculateRealPrice()
		{
			if (IsEmpty())
			{
				return 0.0F;
			}
			return CalculateFullPrice() - CalculateCartDiscount();
		}

		public float CalculateFullPrice()
		{
			return GetItems().Sum(i => i.TotalPrice);
		}

		public float CalculateCartDiscount()
		{
			return GetItems().Sum(i => i.TotalDiscount);
		}

		public void Purchase(Action onSuccess = null, Action<Error> onError = null)
		{
			Action<List<UserCartItem>> handleSuccess = list =>
			{
				Clear();
				onSuccess?.Invoke();
				PurchaseCartEvent?.Invoke();
			};

			var items = GetItems();
			if (items.Any(x => x.Item.RealPrice != null))
				StoreLogic.PurchaseCart(items, handleSuccess, onError);
			else
				StoreLogic.PurchaseFreeCart(items, handleSuccess, onError);
		}
	}
}
