using System;

namespace Xsolla.Auth
{
	[Serializable]
	internal class StartAuthByEmailRequest
	{
		public string email;
		public string link_url;
		public bool? send_link;
	}
}