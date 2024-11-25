using System;
using System.Collections.Generic;

namespace Xsolla.Demo
{
	public class UserCartModel
	{
		private readonly List<UserCartItem> _items;

		public UserCartModel()
		{
			_items = new List<UserCartItem>();
		}

		private Predicate<UserCartItem> SearchPredicate(CatalogItemModel item) => cartItem => cartItem.Sku.Equals(item.Sku);

		private UserCartItem FindCartItemBy(CatalogItemModel catalogItemModel) => GetCartItems().Find(SearchPredicate(catalogItemModel));

		public UserCartItem GetItem(CatalogItemModel item)
		{
			return FindCartItemBy(item);
		}

		private bool Exist(CatalogItemModel item)
		{
			return _items.Exists(SearchPredicate(item));
		}

		public UserCartItem AddItem(CatalogItemModel item)
		{
			if (!Exist(item))
			{
				_items.Add(new UserCartItem(item));
			}
			return GetItem(item);
		}

		public bool RemoveItem(CatalogItemModel item)
		{
			if(!Exist(item)) return false;
			UserCartItem cartItem = GetItem(item);
			_items.Remove(cartItem);
			return true;
		}

		public UserCartItem IncreaseCountOf(CatalogItemModel item)
		{
			if (!Exist(item)) return null;
			UserCartItem cartItem = GetItem(item);
			cartItem.Quantity++;
			return cartItem;
		}

		public UserCartItem DecreaseCountOf(CatalogItemModel item)
		{
			if (!Exist(item)) return null;

			UserCartItem cartItem = GetItem(item);
			cartItem.Quantity--;
			if (cartItem.Quantity == 0)
			{
				RemoveItem(cartItem.Item);
			}
			return cartItem;
		}

		public List<UserCartItem> GetCartItems()
		{
			return _items;
		}

		public void Clear()
		{
			_items.Clear();
		}

		public bool IsEmpty()
		{
			return GetCartItems().Count == 0;
		}
	}
}
