using System;

namespace Xsolla.Demo.Popup
{
	public interface IErrorPopup
	{
		IErrorPopup SetCallback(Action buttonPressed);
		IErrorPopup SetTitle(string titleText);
		IErrorPopup SetMessage(string messageText);
		IErrorPopup SetButtonText(string buttonText);
	}
}