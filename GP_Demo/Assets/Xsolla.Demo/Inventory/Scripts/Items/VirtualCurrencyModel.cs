using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class VirtualCurrencyModel : ItemModel
	{
		public override bool IsVirtualCurrency() => true;
		public override bool IsSubscription() => false;
		public override bool IsBundle() => false;
	}
}