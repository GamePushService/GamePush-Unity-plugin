using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class SubscriptionCharge
	{
		public float amount;
		public float? amount_with_promotion;
		public string currency;
	}
}