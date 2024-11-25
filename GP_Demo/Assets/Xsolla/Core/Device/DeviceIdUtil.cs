using UnityEngine;

namespace Xsolla.Core
{
	public static class DeviceIdUtil
	{
		/// <summary>
		/// Returns a device ID for user authentication in the format required by the Xsolla API.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		public static string GetDeviceId()
		{
#if UNITY_ANDROID
			var contentResolver = new AndroidHelper().CurrentActivity.Call<AndroidJavaObject>("getContentResolver");
			var secureSettings = new AndroidJavaClass("android.provider.Settings$Secure");
			return secureSettings.CallStatic<string>("getString", contentResolver, "android_id");
#elif UNITY_IOS
			return SystemInfo.deviceUniqueIdentifier;
#else
			throw new System.Exception($"Device id is not supported on this platform: {Application.platform}");
#endif
		}

		public static string GetDeviceName()
		{
			return SystemInfo.deviceName;
		}

		public static string GetDeviceModel()
		{
			return SystemInfo.deviceModel;
		}
	}
}