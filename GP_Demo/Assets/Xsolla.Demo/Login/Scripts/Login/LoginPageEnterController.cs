using System;
using System.Linq;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class LoginPageEnterController : LoginPageController
	{
		public static string LastUsername { get; set; }

		public bool IsAuthInProgress
		{
			get => IsInProgress;
			private set
			{
				XDebug.Log(value
					? "LoginPageEnterController: Authentication process started"
					: "LoginPageEnterController: Authentication process ended");

				IsInProgress = value;
			}
		}

		private void TryAuthBy<T>(object[] args, Action onSuccess, Action<Error> onError) where T : LoginAuthorization
		{
			var auth = gameObject.AddComponent<T>();
			XDebug.Log($"Trying auth via: {auth.GetType().Name}");

			void handleSuccess()
			{
				Destroy(auth);
				onSuccess?.Invoke();
			}

			void handleError(Error error)
			{
				Destroy(auth);
				onError?.Invoke(error);
			}

			auth.TryAuth(args, handleSuccess, handleError);
		}

		private void SuperComplete()
		{
			ValidateToken(
				() =>
				{
					XDebug.Log($"Successful auth with access token = {XsollaToken.AccessToken}");
					MainMenuNicknameChecker.ResetFlag();
					IsAuthInProgress = false;
					OnSuccess?.Invoke();
				},
				ProcessError);
		}

		private static void ValidateToken(Action onSuccess, Action<Error> onError)
		{
			void handleError(Error error)
			{
				if (error.ErrorType == ErrorType.InvalidToken)
				{
					XDebug.Log("SavedTokenAuth: Trying to refresh OAuth token");
					XsollaAuth.RefreshToken(
						onSuccess,
						refreshError =>
						{
							XDebug.LogError(error.errorMessage);
							onError?.Invoke(null);
						});
				}
				else
				{
					onError?.Invoke(null);
				}
			}

			XsollaAuth.GetUserInfo(_ => { onSuccess?.Invoke(); }, handleError);
		}

		private void ProcessError(Error error)
		{
			IsAuthInProgress = false;

			if (error != null)
				OnError?.Invoke(error);

			//Do nothing, it means that chosen auth was not completed, but ended without any errors (example: cancelled SocialAuth)
		}

		public void RunLoginProxyAction(Action<LoginPageEnterController, object> action, object arg = null)
		{
			action.Invoke(this, arg);
		}
	}
}