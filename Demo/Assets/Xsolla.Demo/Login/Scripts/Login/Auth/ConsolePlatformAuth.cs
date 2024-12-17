using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class ConsolePlatformAuth : LoginAuthorization
	{
		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
			if (!DemoSettings.UseConsoleAuth)
			{
				onError?.Invoke(null);
				return;
			}

			XsollaAuth.SignInConsoleAccount(
				DemoSettings.UsernameFromConsolePlatform,
				DemoSettings.Platform.GetString(),
				onSuccess,
				onError);
		}
	}
}