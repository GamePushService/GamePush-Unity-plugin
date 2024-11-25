namespace Xsolla.Demo.Popup
{
	public partial class PopupFactory : MonoSingleton<PopupFactory>
	{
		public ICouponRedeemPopup CreateRedeemCoupon() =>
			CreateDefaultPopup(RedeemCouponPopupPrefab, canvas)?.GetComponent<CouponRedeemPopup>();

		public ICouponRewardsPopup CreateCouponRewards() =>
			CreateDefaultPopup(CouponRewardsPopupPrefab, canvas)?.GetComponent<CouponRewardsPopup>();

		public ITutorialPopup CreateTutorial() =>
			CreateDefaultPopup(TutorialPopupPrefab, canvas)?.GetComponent<TutorialPopup>();
	}
}
