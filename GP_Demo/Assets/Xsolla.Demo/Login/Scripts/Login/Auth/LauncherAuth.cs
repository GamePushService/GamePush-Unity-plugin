using System;
using System.Linq;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class LauncherAuth : LoginAuthorization
	{
		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
			XsollaAuth.AuthViaXsollaLauncher(
				onSuccess,
				error => onError?.Invoke(null));
		}
	}
}