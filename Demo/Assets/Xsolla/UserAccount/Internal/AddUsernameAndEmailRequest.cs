using System;

namespace Xsolla.UserAccount
{
	[Serializable]
	internal class AddUsernameAndEmailRequest
	{
		public string username;
		public string password;
		public string email;
		public int? promo_email_agreement;

		public AddUsernameAndEmailRequest(string username, string password, string email, int? promo_email_agreement)
		{
			this.username = username;
			this.password = password;
			this.email = email;
			this.promo_email_agreement = promo_email_agreement;
		}
	}
}