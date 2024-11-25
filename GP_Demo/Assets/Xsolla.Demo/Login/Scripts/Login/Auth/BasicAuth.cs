using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class BasicAuth : LoginAuthorization
	{
		private const string DEMO_USER_NAME = "XSOLLA";
		private bool _isDemoUser;
		private bool _isJwtInvalidationEnabled;

		public override void TryAuth(object[] args, Action onSuccess, Action<Error> onError)
		{
			if (!TryExtractArgs(args, out var username, out var password))
			{
				onError?.Invoke(new Error(errorMessage: "Basic auth failed. Can't extract username and password"));
				return;
			}

			ChangeJwtInvalidationIfNeeded(username, password);

			XsollaAuth.SignIn(
				username,
				password,
				() =>
				{
					RestoreJwtInvalidationIfNeeded();
					onSuccess?.Invoke();
				},
				error =>
				{
					RestoreJwtInvalidationIfNeeded();
					onError?.Invoke(error);
				});
		}

		private static bool TryExtractArgs(object[] args, out string username, out string password)
		{
			username = default;
			password = default;

			if (args == null)
			{
				XDebug.LogError("BasicAuth.TryExtractArgs: 'object[] args' was null");
				return false;
			}

			if (args.Length != 2)
			{
				XDebug.LogError($"BasicAuth.TryExtractArgs: args.Length expected 2, was {args.Length}");
				return false;
			}

			try
			{
				username = (string) args[0];
				password = (string) args[1];
				LoginPageEnterController.LastUsername = username;
			}
			catch (Exception ex)
			{
				XDebug.LogError($"BasicAuth.TryExtractArgs: Error during argument extraction: {ex.Message}");
				return false;
			}

			return true;
		}

		private void ChangeJwtInvalidationIfNeeded(string username, string password)
		{
			_isDemoUser = username.ToUpper() == DEMO_USER_NAME && password.ToUpper() == DEMO_USER_NAME;
			_isJwtInvalidationEnabled = XsollaSettings.InvalidateExistingSessions;

			if (_isDemoUser && _isJwtInvalidationEnabled)
				XsollaSettings.InvalidateExistingSessions = false;
		}

		private void RestoreJwtInvalidationIfNeeded()
		{
			if (_isDemoUser && _isJwtInvalidationEnabled)
				XsollaSettings.InvalidateExistingSessions = true;
		}
	}
}