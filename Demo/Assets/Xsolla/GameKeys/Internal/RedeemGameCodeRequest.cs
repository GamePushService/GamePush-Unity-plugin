using System;

namespace Xsolla.GameKeys
{
	[Serializable]
	internal class RedeemGameCodeRequest
	{
		public string code;
		public bool sandbox;
	}
}