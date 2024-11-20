using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SavedTokenAuth : LoginAuthorization
	{
		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
			var success = XsollaAuth.AuthViaSavedToken();
			if (success)
				onSuccess?.Invoke();
			else
				onError?.Invoke(null);
		}
	}
}