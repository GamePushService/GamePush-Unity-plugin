using System;

namespace Xsolla.Demo.Popup
{
	public interface IConfirmationCodePopup
	{
		IConfirmationCodePopup SetTitle(string titleText);
		IConfirmationCodePopup SetConfirmCallback(Action<string> buttonPressed);
		IConfirmationCodePopup SetConfirmButtonText(string buttonText);
		IConfirmationCodePopup SetCancelCallback(Action buttonPressed);
		IConfirmationCodePopup SetCancelButtonText(string buttonText);
	}
}