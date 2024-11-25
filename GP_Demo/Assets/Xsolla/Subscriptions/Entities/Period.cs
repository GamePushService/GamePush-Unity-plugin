using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class Period
	{
		public int value;
		public string unit;

		public PeriodUnit Unit
		{
			get
			{
				switch (unit)
				{
					case "day": return PeriodUnit.Day;
					case "month": return PeriodUnit.Month;
					case "lifetime": return PeriodUnit.LifeTime;
					default: return PeriodUnit.Unknown;
				}
			}
		}
	}
}