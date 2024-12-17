#if UNITY_IOS
using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class IosUtils
	{
		[DllImport("__Internal")]
		private static extern void _configureLoginAnalytics(string gameEngine, string gameEngineVersion);

		[DllImport("__Internal")]
		private static extern void _configurePaymentsAnalytics(string gameEngine, string gameEngineVersion);

		[DllImport("__Internal")]
		private static extern void _setPaystationVersion(int paystationVersion);

		private static bool AnalyticsConfigured { get; set; }

		public static T Cast<T>(this IntPtr instancePtr)
		{
			var instanceHandle = GCHandle.FromIntPtr(instancePtr);
			if (instanceHandle.Target is T target)
				return target;

			throw new InvalidCastException($"Failed to cast IntPtr to {typeof(T).FullName}");
		}

		public static IntPtr GetPointer(this object obj)
		{
			return obj == null ? IntPtr.Zero : GCHandle.ToIntPtr(GCHandle.Alloc(obj));
		}

		public static void ConfigureAnalytics()
		{
			if (!AnalyticsConfigured)
			{
				var gameEngine = "unity";
				var gameEngineVersion = Application.unityVersion;

				_configureLoginAnalytics(gameEngine, gameEngineVersion);
				_configurePaymentsAnalytics(gameEngine, gameEngineVersion);

				AnalyticsConfigured = true;
			}
		}

		public static void SetPaystationVersion(int paystationVersion)
		{
			_setPaystationVersion(paystationVersion);
		}
	}
}
#endif