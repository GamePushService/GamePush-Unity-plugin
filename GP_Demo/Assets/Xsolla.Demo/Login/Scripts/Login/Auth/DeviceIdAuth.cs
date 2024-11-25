using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class DeviceIdAuth : LoginAuthorization
	{
		public override void TryAuth(object[] _, Action onSuccess, Action<Error> onError)
		{
#if !(UNITY_ANDROID || UNITY_IOS)
			onError?.Invoke(new Error(ErrorType.MethodIsNotAllowed, errorMessage: "Device ID auth is not supported for this platform"));
			return;
#else
			XsollaAuth.AuthViaDeviceID(onSuccess, onError);
#endif
		}
	}
}