using System;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public partial class LoginPageEnterController
	{
		private static bool _isFirstLaunch = true;
		private int _autoAuthCounter;

		private void Start()
		{
			if (_isFirstLaunch)
			{
				_isFirstLaunch = false;
				IsAuthInProgress = true;
				PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => IsAuthInProgress == false);
				RunAutomaticAuths();
			}
			else
			{
				XsollaToken.DeleteSavedInstance();
			}
		}

		private void RunAutomaticAuths()
		{
			Action<Error> handleErrorOrSkip = error =>
			{
				if (error != null)
				{
					ProcessError(error);
				}
				else
				{
					_autoAuthCounter++;
					RunAutomaticAuths();
				}
			};

			switch (_autoAuthCounter)
			{
				case 0:
					TryAuthBy<LauncherAuth>(null, SuperComplete, handleErrorOrSkip);
					break;
				case 1:
					TryAuthBy<SteamAuth>(null, SuperComplete, handleErrorOrSkip);
					break;
				case 2:
					TryAuthBy<ConsolePlatformAuth>(null, SuperComplete, handleErrorOrSkip);
					break;
				case 3:
					TryAuthBy<SavedTokenAuth>(null, SuperComplete, handleErrorOrSkip);
					break;
				default:
					IsAuthInProgress = false;
					break;
			}
		}
	}
}