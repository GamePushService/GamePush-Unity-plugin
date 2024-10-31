using System;
using System.Collections.Generic;

namespace Xsolla.UserAccount
{
	[Serializable]
	public class UserDevicesInfo
	{
		public List<UserDeviceInfo> items;
	}
	
	[Serializable]
	public class UserDeviceInfo
	{
		public int id;
		public string type;
		public string device;
		public string last_used_at;
	}
}