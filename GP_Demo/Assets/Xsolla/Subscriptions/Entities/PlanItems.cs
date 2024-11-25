using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class PlanItems
	{
		public PlanItem[] items;
		public bool has_more;
	}

	[Serializable]
	public class PlanItem
	{
		public int plan_id;
		public string plan_external_id;
		public string plan_group_id;
		public string plan_type;
		public string plan_name;
		public string plan_description;
		public DateTime? plan_start_date;
		public DateTime? plan_end_date;
		public int? trial_period;
		public Period period;
		public PlanCharge charge;
		public PlanPromotion promotion;

		public PlanType PlanType
		{
			get
			{
				switch (plan_type)
				{
					case "all": return PlanType.All;
					default: return PlanType.Unknown;
				}
			}
		}
	}
}