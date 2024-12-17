using System.Collections.Generic;

namespace Xsolla.Core
{
	/// <summary>
	/// Purchase parameters.
	/// </summary>
	public class PurchaseParams
	{
		/// <summary>
		/// Default purchase currency.
		/// </summary>
		public string currency;

		/// <summary>
		/// Interface language.
		/// </summary>
		public string locale;

		/// <summary>
		/// Quantity of purchased items.
		/// </summary>
		public int? quantity;

		/// <summary>
		/// Transaction external id.
		/// </summary>
		public string external_id;

		/// <summary>
		/// Project specific parameters.
		/// </summary>
		public Dictionary<string, object> custom_parameters;

		public ShippingData shipping_data;
		public Dictionary<string, object> shipping_method;

		/// <summary>
		/// Payment method ID.
		/// </summary>
		public int? payment_method;

		/// <summary>
		/// Whether to show the ← icon in Pay Station so the user can close the payment UI at any stage of the purchase.
		/// </summary>
		public bool? close_button;

		/// <summary>
		/// Defines the icon of the **Close** button in the payment UI. Can be `arrow` or `cross`. `arrow` by default.
		/// </summary>
		public string close_button_icon;

		/// <summary>
		/// Defines the way the Google Pay payment method is displayed. If true, the button for quick payment via Google Pay is displayed at the top of the payment UI. If `true`, Google Pay is displayed in the list of payment methods according to the PayRank algorithm. `false` by default.
		/// </summary>
		public bool? google_pay_quick_payment_button;
	}
}