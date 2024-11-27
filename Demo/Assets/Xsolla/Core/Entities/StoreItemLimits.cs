using System;

namespace Xsolla.Core
{
	[Serializable]
	public class StoreItemLimits
	{
		public PerUser per_user;

			[Serializable]
			public class PerUser
			{
				public int available;
				public int total;
				public RecurrentSchedule recurrent_schedule;

				[Serializable]
				public class RecurrentSchedule
				{
					public string interval_type;
					public int reset_next_date;
				}
				
			}
	}
}