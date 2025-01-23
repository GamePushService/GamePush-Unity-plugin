using System;
using Xsolla.Core;

namespace Xsolla.Orders
{
	[Serializable]
	internal class CreatePaymentTokenRequest
	{
		public Settings settings;
		public object custom_parameters;
		public Purchase purchase;

		public CreatePaymentTokenRequest(Purchase purchase, Settings settings, object custom_parameters)
		{
			this.settings = settings;
			this.custom_parameters = custom_parameters;
			this.purchase = purchase;
		}

		[Serializable]
		public class Settings
		{
			public string return_url;
			public PayStationUI ui;
			public RedirectPolicy redirect_policy;
			public string currency;
			public string locale;
			public bool? sandbox;
			public string external_id;
			public int? payment_method;
		}

		[Serializable]
		public class Purchase
		{
			public Item[] items;
			public Description description;

			public Checkout checkout;

			public Purchase(Checkout checkout, Description description)
			{
				items = null;
				this.description = description;
				this.checkout = checkout;
			}

			public Purchase(Checkout checkout, Item[] items)
			{
				this.items = items;
				description = null;
				this.checkout = checkout;
			}

			[Serializable]
			public class Item
			{
				public string name;
				public string image_url;
				public string description;
				public Price price;
				public int? quantity;
				public bool? is_bonus;

				public Item(string name, Price price, string image_url = null, string description = null, int? quantity = null, bool? is_bonus = null)
				{
					this.name = name;
					this.image_url = image_url;
					this.description = description;
					this.price = price;
					this.quantity = quantity;
					this.is_bonus = is_bonus;
				}

				[Serializable]
				public class Price
				{
					public string amount;
					public string amount_before_discount;

					public Price(string amount, string amount_before_discount = null)
					{
						this.amount = amount;
						this.amount_before_discount = amount_before_discount;
					}
				}
			}

			[Serializable]
			public class Description
			{
				public string value;

				public Description(string value)
				{
					this.value = value;
				}
			}

			[Serializable]
			public class Checkout
			{
				public float amount;
				public string currency;

				public Checkout(float amount, string currency)
				{
					this.amount = amount;
					this.currency = currency;
				}
			}
		}
	}
}