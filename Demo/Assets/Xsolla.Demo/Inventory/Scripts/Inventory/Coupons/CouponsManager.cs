using UnityEngine;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public class CouponsManager : MonoBehaviour
	{
		[SerializeField] private SimpleTextButton redeemCouponButton = default;

		void Start()
		{
			redeemCouponButton.onClick += ShowRedeemCouponPopup;
		}

		private void ShowRedeemCouponPopup()
		{
			var redeemCouponPopup = PopupFactory.Instance.CreateRedeemCoupon();
			redeemCouponPopup.SetRedeemCallback(code =>
			{
				DemoInventory.Instance.RedeemCouponCode(code, redeemedItems =>
				{
					redeemCouponPopup.Close();
					var rewardCouponPopup = PopupFactory.Instance.CreateCouponRewards();
					rewardCouponPopup.SetItems(redeemedItems);
					var popupCore = rewardCouponPopup.GetPopupCore();
					popupCore.SetCallback(() => UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError));
				}, error => redeemCouponPopup.ShowError());
			});
		}
	}
}
