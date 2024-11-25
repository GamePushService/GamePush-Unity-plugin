using System;

namespace Xsolla.Cart
{
	[Serializable]
	internal class RedeemPromocodeRequest
	{
		public string coupon_code;
		public Cart cart;

		public class Cart
		{
			public string id;
		}
	}
}