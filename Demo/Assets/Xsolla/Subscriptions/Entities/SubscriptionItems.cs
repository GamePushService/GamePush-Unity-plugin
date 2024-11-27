using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class SubscriptionItems
	{
		public SubscriptionItem[] items;
		public bool has_more;
	}

	[Serializable]
	public class SubscriptionItem
	{
		public int id;
		public int plan_id;
		public string plan_external_id;
		public string plan_name;
		public string plan_description;
		public DateTime? plan_start_date;
		public DateTime? plan_end_date;
		public int? product_id;
		public string product_external_id;
		public string product_name;
		public string product_description;
		public string status;
		public bool is_in_trial;
		public int? trial_period;
		public DateTime date_create;
		public DateTime? date_next_charge;
		public DateTime? date_last_charge;
		public SubscriptionCharge charge;
		public Period period;

		public SubscriptionStatus Status
		{
			get
			{
				switch (status)
				{
					case "new": return SubscriptionStatus.New;
					case "active": return SubscriptionStatus.Active;
					case "canceled": return SubscriptionStatus.Canceled;
					case "non_renewing": return SubscriptionStatus.NonRenewing;
					case "pause": return SubscriptionStatus.Pause;
					default: return SubscriptionStatus.Unknown;
				}
			}
		}
	}
}