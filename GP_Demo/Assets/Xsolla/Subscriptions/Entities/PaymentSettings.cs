using System;

namespace Xsolla.Subscriptions
{
	[Serializable]
	public class PaymentSettings
	{
		public bool? sandbox;
		public UI ui;
		public string currency;
		public string locale;
		public string external_id;
		public int? payment_method;
		public string return_url;
		public RedirectPolicy redirect_policy;

		[Serializable]
		public class UI
		{
			public string size;
			public string theme;
			public string version;
			public Desktop desktop;
			public Mobile mobile;
			public string license_url;
			public string mode;
			public UserAccount user_account;

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
				}
			}

			[Serializable]
			public class UserAccount
			{
				public EnableAndOrder history;
				public EnableAndOrder payment_accounts;
				public EnableAndOrder info;
				public EnableAndOrder subscriptions;

				[Serializable]
				public class EnableAndOrder
				{
					public bool enable;
					public int order;
				}
			}
		}

		[Serializable]
		public class RedirectPolicy
		{
			public string redirect_conditions;
			public int? delay;
			public string status_for_manual_redirection;
			public string redirect_button_caption;
		}
	}
}