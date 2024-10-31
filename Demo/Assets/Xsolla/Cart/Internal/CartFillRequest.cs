using System;
using System.Collections.Generic;

namespace Xsolla.Cart
{
	[Serializable]
	internal class CartFillRequest
	{
		public List<CartFillItem> items;
	}
}