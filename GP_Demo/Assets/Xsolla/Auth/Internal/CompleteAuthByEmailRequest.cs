using System;

namespace Xsolla.Auth
{
	[Serializable]
	internal class CompleteAuthByEmailRequest
	{
		public string email;
		public string code;
		public string operation_id;
	}
}