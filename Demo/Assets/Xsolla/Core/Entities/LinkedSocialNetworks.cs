using System;
using System.Collections.Generic;

namespace Xsolla.UserAccount
{
	[Serializable]
	public class LinkedSocialNetworks
	{
		public List<LinkedSocialNetwork> items;
	}
	
	[Serializable]
	public class LinkedSocialNetwork
	{
		public string full_name;
		public string nickname;
		public string picture;
		public string provider;
		public string social_id;
	}
}