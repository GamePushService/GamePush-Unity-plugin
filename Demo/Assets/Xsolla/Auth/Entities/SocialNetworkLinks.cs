using System;
using System.Collections.Generic;

namespace Xsolla.Auth
{
	[Serializable]
	public class SocialNetworkLinks
	{
		public List<SocialNetworkLink> items;
	}

	[Serializable]
	public class SocialNetworkLink
	{
		// Link for authentication via the social network.
		public string auth_url;

		// Name of the social network.
		public string provider;
	}
}