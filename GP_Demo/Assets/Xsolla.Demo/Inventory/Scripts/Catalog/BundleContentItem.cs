namespace Xsolla.Demo
{
	public class BundleContentItem
	{
		public CatalogItemModel Item { get; }

		public string Sku => Item.Sku;
		public string Name => Item.Name;
		public string Description => Item.Description;
		public string ImageUrl => Item.ImageUrl;

		public int Quantity { get; }

		public BundleContentItem(CatalogItemModel storeItem, int quantity)
		{
			Item = storeItem;
			Quantity = quantity;
		}
	}
}