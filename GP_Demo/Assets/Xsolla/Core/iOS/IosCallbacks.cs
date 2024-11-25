#if UNITY_IOS
using System;
using AOT;

namespace Xsolla.Core
{
	internal static class IosCallbacks
	{
		public delegate void ActionVoidCallbackDelegate(IntPtr actionPtr);

		public delegate void ActionStringCallbackDelegate(IntPtr actionPtr, string value);

		public delegate void ActionBoolCallbackDelegate(IntPtr actionPtr, bool value);

		[MonoPInvokeCallback(typeof(ActionVoidCallbackDelegate))]
		public static void ActionVoidCallback(IntPtr actionPtr)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action>();
				action?.Invoke();
			}
		}

		[MonoPInvokeCallback(typeof(ActionStringCallbackDelegate))]
		public static void ActionStringCallback(IntPtr actionPtr, string value)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<string>>();
				action?.Invoke(value);
			}
		}

		[MonoPInvokeCallback(typeof(ActionBoolCallbackDelegate))]
		public static void ActionBoolCallback(IntPtr actionPtr, bool value)
		{
			if (actionPtr != IntPtr.Zero)
			{
				var action = actionPtr.Cast<Action<bool>>();
				action?.Invoke(value);
			}
		}
	}
}
#endif