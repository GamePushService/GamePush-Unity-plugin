using System;

namespace Xsolla.Demo.Popup
{
	public interface IWaitingPopup
	{
		IWaitingPopup SetCloseCondition(Func<bool> condition);
		IWaitingPopup SetCloseHandler(Action handler);
	}
}