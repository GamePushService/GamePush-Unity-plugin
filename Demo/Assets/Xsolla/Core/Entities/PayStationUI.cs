using System;

namespace Xsolla.Core
{
	[Serializable]
	public class PayStationUI
	{
		public string theme;
		public bool is_independent_windows;
		public Desktop desktop;
		public Mobile mobile;
		public bool? gp_quick_payment_button;

		[Serializable]
		public class Desktop
		{
			public Header header;

			[Serializable]
			public class Header
			{
				public bool? is_visible;
				public bool? visible_logo;
				public bool? visible_name;
				public string type;
				public bool? close_button;
				public string close_button_icon;
			}
		}

		[Serializable]
		public class Mobile
		{
			public string mode;
			public Footer footer;
			public Header header;

			[Serializable]
			public class Footer
			{
				public bool? is_visible;
			}

			[Serializable]
			public class Header
			{
				public bool? close_button;
				public string close_button_icon;
			}
		}
	}
}