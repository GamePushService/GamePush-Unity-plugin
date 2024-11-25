using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	internal class BuySubscriptionRequest
	{
		public string plan_external_id;
		public PaymentSettings settings;
	}
}