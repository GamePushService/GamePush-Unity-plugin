using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class PlanCharge
	{
		public float amount;
		public float? setup_fee;
		public string currency;
	}
}