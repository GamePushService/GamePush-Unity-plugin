using Xsolla.Core;

namespace Xsolla.Auth
{
	public class DeviceInfo
	{
		// Type of the device. Can be `android` and `ios`.
		public DeviceType DeviceType;

		// For Android, it is an [ANDROID_ID](https://developer.android.com/reference/android/provider/Settings.Secure#ANDROID_ID) constant.<br/>
		// For iOS, it is an [identifierForVendor](https://developer.apple.com/documentation/uikit/uidevice/1620059-identifierforvendor?language=objc) property.
		public string DeviceId;

		// Manufacturer name of the device.
		public string DeviceModel;

		// Model name of the device.
		public string DeviceName;

		public static DeviceInfo Create()
		{
			var deviceInfo = new DeviceInfo {
				DeviceId = DeviceIdUtil.GetDeviceId(),
				DeviceModel = DeviceIdUtil.GetDeviceModel(),
				DeviceName = DeviceIdUtil.GetDeviceName()
			};

#if UNITY_ANDROID
			deviceInfo.DeviceType = Core.DeviceType.Android;
#else
			deviceInfo.DeviceType = Core.DeviceType.iOS;
#endif
			return deviceInfo;
		}
	}

	public static class DeviceInfoExtensions
	{
		public static string GetDeviceType(this DeviceInfo deviceInfo)
		{
			return deviceInfo.DeviceType.ToString().ToLower();
		}

		public static string GetSafeDeviceData(this DeviceInfo deviceInfo)
		{
			var deviceData = $"{deviceInfo.DeviceName}:{deviceInfo.DeviceModel}";
			const int maxDeviceDataLength = 100;
			
			if (deviceData.Length > maxDeviceDataLength)
			{
				XDebug.LogWarning($"Device data is too long. It will be truncated to {maxDeviceDataLength} symbols. Original device data: {deviceData}");
				deviceData = deviceData.Substring(0, maxDeviceDataLength);
			}

			return deviceData;
		}

		public static string GetSafeDeviceId(this DeviceInfo deviceInfo)
		{
			var deviceId = deviceInfo.DeviceId;
			const int minDeviceIdLength = 16;
			const int maxDeviceIdLength = 36;

			if (deviceId.Length < minDeviceIdLength)
			{
				XDebug.LogWarning($"Device ID is too short. It will be padded to {minDeviceIdLength} symbols. Original device ID: {deviceId}");
				deviceId = deviceId.PadLeft(minDeviceIdLength, '0');
			}
			else if (deviceId.Length > maxDeviceIdLength)
			{
				XDebug.LogWarning($"Device ID is too long. It will be truncated to {maxDeviceIdLength} symbols. Original device ID: {deviceId}");
				deviceId = deviceId.Substring(0, maxDeviceIdLength);
			}

			return deviceId;
		}
	}
}