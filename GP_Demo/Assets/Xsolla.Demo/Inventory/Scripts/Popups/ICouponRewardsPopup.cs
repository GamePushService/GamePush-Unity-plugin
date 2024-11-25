using System.Collections.Generic;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	public interface ICouponRewardsPopup
	{
		ICouponRewardsPopup SetItems(List<CouponRedeemedItemModel> items);
		ISuccessPopup GetPopupCore();
	}
}