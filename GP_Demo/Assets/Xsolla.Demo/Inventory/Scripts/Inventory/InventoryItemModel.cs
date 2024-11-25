using System;

namespace Xsolla.Demo
{
	[Serializable]
	public class InventoryItemModel : ItemModel
	{
		public override bool IsVirtualCurrency() => false;
		public override bool IsSubscription() => false;
		public override bool IsBundle() => false;

		public string InstanceId;
		public uint? RemainingUses;
	}
}