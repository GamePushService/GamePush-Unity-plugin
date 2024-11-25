using System;

namespace Xsolla.Demo.Popup
{
	public interface IConfirmationPopup
	{
		IConfirmationPopup SetTitle(string titleText);
		IConfirmationPopup SetMessage(string messageText);
		IConfirmationPopup SetConfirmCallback(Action buttonPressed);
		IConfirmationPopup SetConfirmButtonText(string buttonText);
		IConfirmationPopup SetCancelCallback(Action buttonPressed);
		IConfirmationPopup SetCancelButtonText(string buttonText);
	}
}