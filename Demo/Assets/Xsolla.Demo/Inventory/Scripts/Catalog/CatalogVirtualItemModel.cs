namespace Xsolla.Demo
{
	public class CatalogVirtualItemModel : CatalogItemModel
	{
		public override bool IsVirtualCurrency() => false;
		public override bool IsSubscription() => false;
		public override bool IsBundle() => false;
	}
}