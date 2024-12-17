using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class PurchaseParamsGenerator
	{
		public static PurchaseParamsRequest GeneratePurchaseParamsRequest(PurchaseParams purchaseParams)
		{
			var settings = new PurchaseParamsRequest.Settings {
				ui = PayStationUISettings.GenerateSettings(),
				redirect_policy = RedirectPolicySettings.GeneratePolicy(),
				external_id = purchaseParams?.external_id,
				payment_method = purchaseParams?.payment_method
			};

			ProcessUiCloseButton(settings.ui, purchaseParams);
			ProcessGooglePayQuickButton(settings, purchaseParams);
			ProcessSdkTokenSettings(settings);

			if (settings.redirect_policy != null)
				settings.return_url = settings.redirect_policy.return_url;

			//Fix 'The array value is found, but an object is required' in case of empty values.
			if (settings.ui == null && settings.redirect_policy == null && settings.return_url == null)
				settings = null;

			return new PurchaseParamsRequest {
				sandbox = XsollaSettings.IsSandbox,
				settings = settings,
				custom_parameters = purchaseParams?.custom_parameters,
				currency = purchaseParams?.currency,
				locale = purchaseParams?.locale,
				quantity = purchaseParams?.quantity,
				shipping_data = purchaseParams?.shipping_data,
				shipping_method = purchaseParams?.shipping_method
			};
		}

		private static void ProcessUiCloseButton(PayStationUI settings, PurchaseParams purchaseParams)
		{
			if (purchaseParams == null)
				return;

			if (purchaseParams.close_button == null && string.IsNullOrEmpty(purchaseParams.close_button_icon))
				return;

#if UNITY_ANDROID || UNITY_IOS
			if (settings.mobile == null)
				settings.mobile = new PayStationUI.Mobile();

			if (settings.mobile.header == null)
				settings.mobile.header = new PayStationUI.Mobile.Header();

			settings.mobile.header.close_button = purchaseParams.close_button;
			settings.mobile.header.close_button_icon = purchaseParams.close_button_icon;
#else
			if (settings.desktop == null)
				settings.desktop = new PayStationUI.Desktop();

			if (settings.desktop.header == null)
				settings.desktop.header = new PayStationUI.Desktop.Header();

			settings.desktop.header.close_button = purchaseParams.close_button;
			settings.desktop.header.close_button_icon = purchaseParams.close_button_icon;
#endif
		}

		private static void ProcessGooglePayQuickButton(PurchaseParamsRequest.Settings settings, PurchaseParams purchaseParams)
		{
#if UNITY_ANDROID
			if (purchaseParams == null)
			{
				purchaseParams = new PurchaseParams {
					google_pay_quick_payment_button = true
				};
			}
#endif

			if (purchaseParams?.google_pay_quick_payment_button != null)
				settings.ui.gp_quick_payment_button = purchaseParams.google_pay_quick_payment_button;
		}

		private static void ProcessSdkTokenSettings(PurchaseParamsRequest.Settings settings)
		{
#if UNITY_ANDROID || UNITY_IOS
			var sdkTokenSettings = new SdkTokenSettings();

#if UNITY_ANDROID
			sdkTokenSettings.platform = "android";
#elif UNITY_IOS
			sdkTokenSettings.platform = "ios";
#endif

			if (!XsollaSettings.InAppBrowserEnabled)
				sdkTokenSettings.browser_type = "system";

			settings.sdk = sdkTokenSettings;
#endif
		}

		public static List<WebRequestHeader> GeneratePaymentHeaders(Dictionary<string, string> customHeaders = null)
		{
			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader()
			};

			if (customHeaders == null)
				return headers;

			foreach (var kvp in customHeaders)
			{
				if (!string.IsNullOrEmpty(kvp.Key) && !string.IsNullOrEmpty(kvp.Value))
					headers.Add(new WebRequestHeader(kvp.Key, kvp.Value));
			}

			return headers;
		}
	}
}