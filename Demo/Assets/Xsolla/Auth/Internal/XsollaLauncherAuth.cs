using System;

namespace Xsolla.Core
{
	internal class XsollaLauncherAuth
	{
		private const string LAUNCHER_TOKEN_KEY = "xsolla-login-token";

		public void Perform(Action onSuccess, Action<Error> onError)
		{
			var launcherToken = GetLauncherToken();
			if (string.IsNullOrEmpty(launcherToken))
			{
				onError?.Invoke(new Error(ErrorType.Undefined, "Can't get launcher token from command line arguments."));
			}
			else
			{
				XsollaToken.Create(launcherToken);
				onSuccess?.Invoke();
			}
		}

		private static string GetLauncherToken()
		{
			var commandLineArgs = Environment.GetCommandLineArgs();
			var tokenValueIndex = -1;

			for (var i = 0; i < commandLineArgs.Length; i++)
			{
				if (commandLineArgs[i].Contains(LAUNCHER_TOKEN_KEY))
				{
					tokenValueIndex = i + 1;
					break;
				}
			}

			if (tokenValueIndex == -1 || tokenValueIndex >= commandLineArgs.Length)
				return null;

			return commandLineArgs[tokenValueIndex];
		}
	}
}