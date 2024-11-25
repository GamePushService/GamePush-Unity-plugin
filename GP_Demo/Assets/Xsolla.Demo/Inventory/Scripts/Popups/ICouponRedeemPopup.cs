using System;

namespace Xsolla.Demo.Popup
{
	public interface ICouponRedeemPopup
	{
		ICouponRedeemPopup SetRedeemCallback(Action<string> buttonPressed);
		ICouponRedeemPopup SetCancelCallback(Action buttonPressed);
		ICouponRedeemPopup ShowError();
		void Close();
	}
}