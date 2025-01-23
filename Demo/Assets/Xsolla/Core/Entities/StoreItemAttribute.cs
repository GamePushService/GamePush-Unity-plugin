using System;

namespace Xsolla.Core
{
	[Serializable]
	public class StoreItemAttribute
	{
		public string external_id;
		public string name;
		public ValuePair[] values;

		[Serializable]
		public class ValuePair
		{
			public string external_id;
			public string value;
		}
	}
}