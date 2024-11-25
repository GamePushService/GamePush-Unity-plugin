namespace Xsolla.Demo.Popup
{
	public partial class PopupFactory : MonoSingleton<PopupFactory>
	{
		public IBundlePreviewPopup CreateBundlePreview() =>
			CreateDefaultPopup(BundlePreviewPopupPrefab, canvas)?.GetComponent<BundlePreviewPopup>();
	}
}
