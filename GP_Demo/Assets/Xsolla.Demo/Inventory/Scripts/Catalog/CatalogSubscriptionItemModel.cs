using System;

namespace Xsolla.Demo
{
	public class CatalogSubscriptionItemModel : CatalogItemModel
	{
		public override bool IsVirtualCurrency() => false;
		public override bool IsSubscription() => true;
		public override bool IsBundle() => false;
	
		public TimeSpan ExpirationPeriod { get; set; }
		public string ExpirationPeriodText { get; set; }
	}
}