using System.Collections.Generic;

namespace Xsolla.Demo
{
	public class CatalogBundleItemModel : CatalogItemModel
	{
		public override bool IsVirtualCurrency() => false;
		public override bool IsSubscription() => false;
		public override bool IsBundle() => true;

		public KeyValuePair<string, float>? ContentRealPrice { get; set; }
		public KeyValuePair<string, float>? ContentRealPriceWithoutDiscount { get; set; }

		public KeyValuePair<string, int>? ContentVirtualPrice { get; set; }
		public KeyValuePair<string, int>? ContentVirtualPriceWithoutDiscount { get; set; }

		public List<BundleContentItem> Content { get; set; }
	}
}