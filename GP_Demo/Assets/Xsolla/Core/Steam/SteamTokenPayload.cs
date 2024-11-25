using System;

namespace Xsolla.Core
{
	[Serializable]
	internal class SteamTokenPayload
	{
		public string id;
		public bool is_cross_auth;
	}
}