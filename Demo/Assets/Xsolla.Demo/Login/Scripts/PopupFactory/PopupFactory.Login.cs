namespace Xsolla.Demo.Popup
{
	public partial class PopupFactory : MonoSingleton<PopupFactory>
	{
		public INicknamePopup CreateNickname() =>
			CreateDefaultPopup(NicknamePopupPrefab, canvas)?.GetComponent<NicknamePopup>();
	}
}
