using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class UserSubscriptionModel : ItemModel
	{
		public enum SubscriptionStatusType
		{
			None,
			Active,
			Expired
		}
	
		public override bool IsVirtualCurrency() => false;
		public override bool IsSubscription() => true;
		public override bool IsBundle() => false;

		public SubscriptionStatusType Status { get; set; }
		public DateTime? Expired { get; set; }
	}
}