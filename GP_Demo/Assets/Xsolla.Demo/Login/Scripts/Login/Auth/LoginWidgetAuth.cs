using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	internal class LoginWidgetAuth : LoginAuthorization
	{
		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
#if UNITY_WEBGL
			onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: "Xsoll Login widget auth is not supported for this platform"));
			return;
#else
			XsollaAuth.AuthWithXsollaWidget(
				onSuccess,
				error => onError?.Invoke(null),
				() => onError?.Invoke(null));
#endif
		}
	}
}