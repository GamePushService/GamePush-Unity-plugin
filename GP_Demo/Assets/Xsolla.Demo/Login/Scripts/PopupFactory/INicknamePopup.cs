using System;

namespace Xsolla.Demo.Popup
{
	public interface INicknamePopup
	{
		INicknamePopup SetCallback(Action<string> nicknameCallback);
		INicknamePopup SetCancelCallback(Action cancelCallback);
	}
}