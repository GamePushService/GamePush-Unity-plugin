using System;

namespace Xsolla.Auth
{
	[Serializable]
	internal class AuthViaDeviceIdRequest
	{
		public string device;
		public string device_id;
	}
}