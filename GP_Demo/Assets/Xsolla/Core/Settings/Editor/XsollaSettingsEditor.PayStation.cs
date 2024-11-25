using UnityEditor;
using UnityEngine;

namespace Xsolla.Core
{
	public partial class XsollaSettingsEditor
	{
		private const string SANDBOX_LABEL = "Test Payments Mode";
		private const string SANDBOX_TOOLTIP = "If you already accepted a real payment, test payments are available only to users " +
			"who are specified in Publisher Account in the \"Company settings -> Users\" section.";

		private const string PAYSTATION_UI_GROUP_LABEL = "Pay Station Theme Id";
		private const string REDIRECT_POLICY_GROUP_LABEL = "Redirect Policy";
		private const string DESKTOP_GROUP_LABEL = "Desktop";
		private const string WEBGL_GROUP_LABEL = "WebGL";
		private const string ANDROID_GROUP_LABEL = "Android";
		private const string IOS_GROUP_LABEL = "iOS";

		private const string PAYSTATION_THEME_TOOLTIP = "To use default theme ID, enter \"63295a9a2e47fab76f7708e1\" (light) or \"63295aab2e47fab76f7708e3\" (dark) values." +
			"Or enter the ID of a custom theme you have configured in Publisher Account to use it.";

		private const string OVERRIDE_REDIRECT_POLICY_LABEL = "Use settings from Publisher Account ";
		private const string OVERRIDE_REDIRECT_POLICY_TOOLTIP = "If the option is enabled, SDK uses redirect settings " +
			"specified in Publisher Account in the \"Pay Station -> Settings -> Redirect policy\" section.";

		private const string REDIRECT_URL_LABEL = "Redirect URL";
		private const string REDIRECT_URL_TOOLTIP = "URL to redirect the user to after payment.";

		private const string REDIRECT_CONDITIONS_LABEL = "Redirect Conditions";
		private const string REDIRECT_CONDITIONS_TOOLTIP = "Payment status when user is automatically redirected to the return URL.\n" +
			"\"None\" — do not redirect.\n" +
			"\"Successful payment\" — redirect when a payment is successful.\n" +
			"\"Successful or canceled payment\" — redirect when a payment is successful or canceled.\n" +
			"\"Any payment\" — redirect for any payment status.";

		private const string REDIRECT_TIMEOUT_LABEL = "Redirect Timeout";
		private const string REDIRECT_TIMEOUT_TOOLTIP = "Redirect timeout in seconds.";

		private const string MANUAL_REDIRECTION_LABEL = "Status for Manual Redirection";
		private const string MANUAL_REDIRECTION_TOOLTIP = "Payment status when the redirect button appears.\n" +
			"\"None\" — do not redirect.\n" +
			"\"Purchase using virtual currency\" — redirect when purchase is made using virtual currency.\n" +
			"\"Successful payment\" — redirect when a payment is successful.\n" +
			"\"Successful or canceled payment\" — redirect when a payment is successful or canceled.\n" +
			"\"Any payment\" — redirect for any payment status.";

		private const string REDIRECT_BUTTON_LABEL = "Redirect Button Caption";
		private const string REDIRECT_BUTTON_TOOLTIP = "Caption of the button that will redirect the user to the return URL.";

		private readonly string[] RedirectConditionsOptions = {
			"None",
			"Successful payment",
			"Successful or canceled payment",
			"Any payment"
		};

		private readonly string[] StatusForManualRedirectionOptions = {
			"None",
			"Purchase using virtual currency",
			"Successful payment",
			"Successful or canceled payment",
			"Any payment"
		};

		private bool PayStationSettings()
		{
			EditorGUILayout.Space();
			EditorGUILayout.BeginVertical(GroupAreaStyle);
			EditorGUILayout.LabelField("Pay Station", GroupHeaderStyle);

			var changed = false;

			var sandbox = EditorGUILayout.Toggle(new GUIContent(SANDBOX_LABEL, SANDBOX_TOOLTIP), XsollaSettings.IsSandbox);
			if (sandbox != XsollaSettings.IsSandbox)
			{
				XsollaSettings.IsSandbox = sandbox;
				changed = true;
			}

			EditorGUI.indentLevel++;

			var payStationFoldout = EditorGUILayout.Foldout(XsollaSettings.PayStationGroupFoldout, PAYSTATION_UI_GROUP_LABEL);
			if (payStationFoldout != XsollaSettings.PayStationGroupFoldout)
			{
				XsollaSettings.PayStationGroupFoldout = payStationFoldout;
				XsollaSettings.DesktopPayStationUISettings.isFoldout = false;
				XsollaSettings.WebglPayStationUISettings.isFoldout = false;
				XsollaSettings.AndroidPayStationUISettings.isFoldout = false;
				XsollaSettings.IosPayStationUISettings.isFoldout = false;
			}

			if (payStationFoldout)
			{
				changed = DrawPayStationUISettings(XsollaSettings.DesktopPayStationUISettings, DESKTOP_GROUP_LABEL)
					|| DrawPayStationUISettings(XsollaSettings.WebglPayStationUISettings, WEBGL_GROUP_LABEL)
					|| DrawPayStationUISettings(XsollaSettings.AndroidPayStationUISettings, ANDROID_GROUP_LABEL)
					|| DrawPayStationUISettings(XsollaSettings.IosPayStationUISettings, IOS_GROUP_LABEL);
			}

			var redirectPolicyFoldout = EditorGUILayout.Foldout(XsollaSettings.RedirectPolicyGroupFoldout, REDIRECT_POLICY_GROUP_LABEL);
			if (redirectPolicyFoldout != XsollaSettings.RedirectPolicyGroupFoldout)
			{
				XsollaSettings.RedirectPolicyGroupFoldout = redirectPolicyFoldout;
				XsollaSettings.DesktopRedirectPolicySettings.IsFoldout = false;
				XsollaSettings.WebglRedirectPolicySettings.IsFoldout = false;
				XsollaSettings.AndroidRedirectPolicySettings.IsFoldout = false;
				XsollaSettings.IosRedirectPolicySettings.IsFoldout = false;
			}

			if (redirectPolicyFoldout)
			{
				changed = DrawRedirectPolicySettings(XsollaSettings.DesktopRedirectPolicySettings, DESKTOP_GROUP_LABEL)
					|| DrawRedirectPolicySettings(XsollaSettings.WebglRedirectPolicySettings, WEBGL_GROUP_LABEL)
					|| DrawRedirectPolicySettings(XsollaSettings.AndroidRedirectPolicySettings, ANDROID_GROUP_LABEL)
					|| DrawRedirectPolicySettings(XsollaSettings.IosRedirectPolicySettings, IOS_GROUP_LABEL);
			}

			EditorGUI.indentLevel--;

			EditorGUILayout.EndVertical();
			return changed;
		}

		private bool DrawPayStationUISettings(PayStationUISettings settings, string title)
		{
			var paystationThemeId = EditorGUILayout.TextField(new GUIContent(title, PAYSTATION_THEME_TOOLTIP), settings.paystationThemeId);
			if (settings.paystationThemeId != paystationThemeId)
			{
				settings.paystationThemeId = paystationThemeId;
				return true;
			}

			return false;
		}

		private bool DrawRedirectPolicySettings(RedirectPolicySettings settings, string title)
		{
			var changed = false;
			EditorGUI.indentLevel++;

			settings.IsFoldout = EditorGUILayout.Foldout(settings.IsFoldout, title);
			if (settings.IsFoldout)
			{
				var useSettingsFromPublisherAccount = EditorGUILayout.Toggle(new GUIContent(OVERRIDE_REDIRECT_POLICY_LABEL, OVERRIDE_REDIRECT_POLICY_TOOLTIP), settings.UseSettingsFromPublisherAccount);
				if (settings.UseSettingsFromPublisherAccount != useSettingsFromPublisherAccount)
				{
					settings.UseSettingsFromPublisherAccount = useSettingsFromPublisherAccount;
					changed = true;
				}

				GUI.enabled = !settings.UseSettingsFromPublisherAccount;

				var returnUrl = EditorGUILayout.TextField(new GUIContent(REDIRECT_URL_LABEL, REDIRECT_URL_TOOLTIP), settings.ReturnUrl);
				if (settings.ReturnUrl != returnUrl)
				{
					settings.ReturnUrl = returnUrl;
					changed = true;
				}

				var redirectConditions = (RedirectPolicySettings.RedirectConditionsType) EditorGUILayout.Popup(new GUIContent(REDIRECT_CONDITIONS_LABEL, REDIRECT_CONDITIONS_TOOLTIP), (int) settings.RedirectConditions, RedirectConditionsOptions);
				if (settings.RedirectConditions != redirectConditions)
				{
					settings.RedirectConditions = redirectConditions;
					changed = true;
				}

				var delay = EditorGUILayout.IntField(new GUIContent(REDIRECT_TIMEOUT_LABEL, REDIRECT_TIMEOUT_TOOLTIP), settings.Delay);
				if (settings.Delay != delay)
				{
					settings.Delay = delay;
					changed = true;
				}

				var statusForManualRedirection = (RedirectPolicySettings.StatusForManualRedirectionType) EditorGUILayout.Popup(new GUIContent(MANUAL_REDIRECTION_LABEL, MANUAL_REDIRECTION_TOOLTIP), (int) settings.StatusForManualRedirection, StatusForManualRedirectionOptions);
				if (settings.StatusForManualRedirection != statusForManualRedirection)
				{
					settings.StatusForManualRedirection = statusForManualRedirection;
					changed = true;
				}

				var redirectButtonCaption = EditorGUILayout.TextField(new GUIContent(REDIRECT_BUTTON_LABEL, REDIRECT_BUTTON_TOOLTIP), settings.RedirectButtonCaption);
				if (settings.RedirectButtonCaption != redirectButtonCaption)
				{
					settings.RedirectButtonCaption = redirectButtonCaption;
					changed = true;
				}

				GUI.enabled = true;
			}

			EditorGUI.indentLevel--;
			return changed;
		}
	}
}