using System;

namespace Xsolla.Auth
{
	[Serializable]
	internal class StartAuthByPhoneNumberRequest
	{
		public string link_url;
		public string phone_number;
		public bool? send_link;
	}
}