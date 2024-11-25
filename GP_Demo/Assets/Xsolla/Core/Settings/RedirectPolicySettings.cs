using System;
using UnityEngine;

namespace Xsolla.Core
{
	[Serializable]
	public class RedirectPolicySettings
	{
		public bool IsFoldout;

		public bool UseSettingsFromPublisherAccount = true;

		public string ReturnUrl;

		public RedirectConditionsType RedirectConditions;

		public int Delay;

		public StatusForManualRedirectionType StatusForManualRedirection;

		public string RedirectButtonCaption;

		public static RedirectPolicy GeneratePolicy()
		{
#if UNITY_ANDROID
			return !XsollaSettings.AndroidRedirectPolicySettings.UseSettingsFromPublisherAccount
				? XsollaSettings.AndroidRedirectPolicySettings.CreatePolicy()
				: CreateDefaultPaymentsPolicy();
#elif UNITY_WEBGL
			return !XsollaSettings.WebglRedirectPolicySettings.UseSettingsFromPublisherAccount 
				? XsollaSettings.WebglRedirectPolicySettings.CreatePolicy() 
				: null;
#elif UNITY_IOS
			return !XsollaSettings.IosRedirectPolicySettings.UseSettingsFromPublisherAccount 
				? XsollaSettings.IosRedirectPolicySettings.CreatePolicy() 
				: CreateDefaultPaymentsPolicy();
#else
			return !XsollaSettings.DesktopRedirectPolicySettings.UseSettingsFromPublisherAccount
				? XsollaSettings.DesktopRedirectPolicySettings.CreatePolicy()
				: null;
#endif
		}

		private RedirectPolicy CreatePolicy()
		{
			return new RedirectPolicy {
				return_url = ReturnUrl,
				redirect_conditions = ConvertToString(RedirectConditions),
				delay = Delay,
				status_for_manual_redirection = ConvertToString(StatusForManualRedirection),
				redirect_button_caption = RedirectButtonCaption
			};
		}

		private static RedirectPolicy CreateDefaultPaymentsPolicy()
		{
			return new RedirectPolicy {
				return_url = $"app://xpayment.{Application.identifier}",
				redirect_conditions = RedirectConditionsType.Successful.ToString().ToLowerInvariant(),
				delay = 0,
				status_for_manual_redirection = StatusForManualRedirectionType.None.ToString().ToLowerInvariant(),
				redirect_button_caption = "Return to game"
			};
		}

		private string ConvertToString(RedirectConditionsType conditions)
		{
			switch (conditions)
			{
				case RedirectConditionsType.None: return "none";
				case RedirectConditionsType.Successful: return "successful";
				case RedirectConditionsType.SuccessfulOrCanceled: return "successful_or_canceled";
				case RedirectConditionsType.Any: return "any";
				default: return "none";
			}
		}

		private string ConvertToString(StatusForManualRedirectionType status)
		{
			switch (status)
			{
				case StatusForManualRedirectionType.None: return "none";
				case StatusForManualRedirectionType.Vc: return "vc";
				case StatusForManualRedirectionType.Successful: return "successful";
				case StatusForManualRedirectionType.SuccessfulOrCanceled: return "successful_or_canceled";
				case StatusForManualRedirectionType.Any: return "any";
				default: return "none";
			}
		}

		public enum RedirectConditionsType
		{
			None,
			Successful,
			SuccessfulOrCanceled,
			Any
		}

		public enum StatusForManualRedirectionType
		{
			None,
			Vc,
			Successful,
			SuccessfulOrCanceled,
			Any
		}
	}
}