#if UNITY_ANDROID
using System;
using UnityEngine;

namespace Xsolla.Core
{
	internal class AndroidBrowserCallback : AndroidJavaProxy
	{
		public Action<bool> OnBrowserClosed { get; set; }

		public AndroidBrowserCallback() : base("com.xsolla.android.payments.callback.BrowserCallback") { }

		public void onBrowserClosed(bool isManually)
		{
			OnBrowserClosed?.Invoke(isManually);
		}
	}
}
#endif