using System;
using System.Collections.Generic;

namespace Xsolla.Auth
{
	[Serializable]
	internal class RegisterRequest
	{
		public string username;
		public string password;
		public string email;
		public bool? accept_consent;
		public List<string> fields;
		public int? promo_email_agreement;
	}
}