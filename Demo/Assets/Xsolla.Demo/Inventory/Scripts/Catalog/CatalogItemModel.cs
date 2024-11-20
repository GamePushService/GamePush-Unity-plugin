using System.Collections.Generic;

namespace Xsolla.Demo
{
	public abstract class CatalogItemModel : ItemModel
	{
		public KeyValuePair<string, float>? RealPrice { get; set; }
		public KeyValuePair<string, float>? RealPriceWithoutDiscount { get; set; }
		public KeyValuePair<string, int>? VirtualPrice { get; set; }
		public KeyValuePair<string, int>? VirtualPriceWithoutDiscount { get; set; }

		public KeyValuePair<string, float>? Price => VirtualPrice.HasValue
			? new KeyValuePair<string, float>(VirtualPrice.Value.Key, VirtualPrice.Value.Value)
			: RealPrice;

		public override bool IsVirtualCurrency() => false;
	}
}