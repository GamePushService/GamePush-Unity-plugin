using System;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Demo.Popup;
using Xsolla.UserAccount;

namespace Xsolla.Demo
{
	public class MainMenuNicknameChecker : MonoBehaviour
	{
		private static bool _isFirstLaunch = true;

		public static void ResetFlag()
		{
			_isFirstLaunch = true;
		}

		private void Start()
		{
			if (_isFirstLaunch)
			{
				_isFirstLaunch = false;
				CheckNicknamePresence();
			}
		}

		private void CheckNicknamePresence()
		{
			XsollaAuth.GetUserInfo(
				info =>
				{
					if (string.IsNullOrEmpty(info.nickname))
					{
						var isUserEmailRegistration = !string.IsNullOrEmpty(info.email) && !string.IsNullOrEmpty(info.username);

						if (isUserEmailRegistration)
							SetNickname(info.username);
						else if (DemoSettings.RequestNicknameOnAuth)
							RequestNickname();
					}
				},
				error => XDebug.LogError("Could not get user info"));
		}

		private void RequestNickname()
		{
			PopupFactory.Instance.CreateNickname().SetCallback(SetNickname).SetCancelCallback(() => { });
		}

		private void SetNickname(string newNickname)
		{
			var isNicknameUpdateInProgress = true;
			ShowWaiting(() => isNicknameUpdateInProgress);

			var updateInfo = new UserInfoUpdate {
				nickname = newNickname
			};

			XsollaUserAccount.UpdateUserInfo(
				updateInfo,
				_ => isNicknameUpdateInProgress = false,
				error =>
				{
					XDebug.LogError("Could not update user info");
					isNicknameUpdateInProgress = false;
					StoreDemoPopup.ShowError(error);
				});
		}

		private void ShowWaiting(Func<bool> waitWhile)
		{
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => !waitWhile.Invoke());
		}
	}
}