#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidSocialAuth
	{
		public void Perform(SocialProvider provider, Action onSuccess, Action<Error> onError, Action onCancel)
		{
			try
			{
				var androidHelper = new AndroidHelper();

				var providerName = provider.ToString().ToUpper();
				var socialNetworkClass = new AndroidJavaClass("com.xsolla.android.login.social.SocialNetwork");
				var socialNetworkObject = socialNetworkClass.GetStatic<AndroidJavaObject>(providerName);

				var authCallback = new AndroidAuthCallback(
					androidHelper,
					() => androidHelper.MainThreadExecutor.Enqueue(() => onSuccess?.Invoke()),
					error => androidHelper.MainThreadExecutor.Enqueue(() => onError(error)),
					() => androidHelper.MainThreadExecutor.Enqueue(onCancel));

				var proxyActivity = new AndroidJavaObject($"{Application.identifier}.androidProxies.SocialAuthProxyActivity");
				proxyActivity.CallStatic(
					"perform",
					androidHelper.CurrentActivity,
					socialNetworkObject,
					authCallback);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSocialAuth: {e.Message}", e);
			}
		}
	}
}
#endif