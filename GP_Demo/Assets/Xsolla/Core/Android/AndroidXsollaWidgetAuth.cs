#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Xsolla.Core
{
	public class AndroidXsollaWidgetAuth
	{
		public void Perform(Action onSuccess, Action<Error> onError, Action onCancel, string locale)
		{
			try
			{
				var androidHelper = new AndroidHelper();

				var authCallback = new AndroidAuthCallback(
					androidHelper,
					() => androidHelper.MainThreadExecutor.Enqueue(() => onSuccess?.Invoke()),
					error => androidHelper.MainThreadExecutor.Enqueue(() => onError(error)),
					() => androidHelper.MainThreadExecutor.Enqueue(onCancel));

				var proxyActivity = new AndroidJavaObject($"{Application.identifier}.androidProxies.XsollaWidgetAuthProxyActivity");
				proxyActivity.CallStatic(
					"perform",
					androidHelper.CurrentActivity,
					authCallback,
					locale);
			}
			catch (Exception e)
			{
				throw new AggregateException($"AndroidSocialAuth: {e.Message}", e);
			}
		}
	}
}
#endif