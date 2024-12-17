using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class PlanPromotion
	{
		public float? promotion_charge_amount;
		public int? promotion_remaining_charges;
	}
}