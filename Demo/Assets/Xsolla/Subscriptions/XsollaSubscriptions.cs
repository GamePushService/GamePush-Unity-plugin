using System;
using Xsolla.Core;

namespace Xsolla.Subscriptions
{
	public static class XsollaSubscriptions
	{
		private const string BaseUrl = "https://subscriptions.xsolla.com/api";
		private static string StoreProjectId => XsollaSettings.StoreProjectId;

		/// <summary>
		/// Returns a list of all plans, including plans purchased by the user while promotions are active.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscriptions-purchase/).</remarks>
		/// <param name="onSuccess">Called after public plans have been successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="planId">Array of subscription plan IDs. Plan ID can be found in the URL of the subscription details page in Publisher Account (https://publisher.xsolla.com/{merchant_id}/projects/{project_id}/subscriptions/plans/{plan_id}).</param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		///     By default, it is determined by the user's IP address.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public static void GetSubscriptionPublicPlans(Action<PlanItems> onSuccess, Action<Error> onError, int[] planId = null, string[] planExternalId = null, int limit = 50, int offset = 0, string locale = null, string country = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/public/v1/projects/{StoreProjectId}/user_plans")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddArray("plan_id", planId)
				.AddArray("plan_external_id", planExternalId)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Subscriptions,
				url,
				onSuccess,
				onError);
		}

		/// <summary>
		/// Returns a list of active recurrent subscriptions that have the status `active`, `non renewing`, and `pause`.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="onSuccess">Called after the list pf subscriptions has been successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		///     By default, it is determined by the user's IP address.</param>
		public static void GetSubscriptions(Action<SubscriptionItems> onSuccess, Action<Error> onError, int limit = 50, int offset = 0, string locale = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/v1/projects/{StoreProjectId}/subscriptions")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Subscriptions,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetSubscriptions(onSuccess, onError, limit, offset, locale)));
		}

		/// <summary>
		/// Returns information about a subscription by its ID. Subscription can be have any status.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="subscriptionId">Subscription ID.</param>
		/// <param name="onSuccess">Called after subscription data have been successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale">Language of the UI.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<br/>
		///     By default, it is determined by the user's IP address.</param>
		public static void GetSubscriptionDetails(int subscriptionId, Action<SubscriptionDetails> onSuccess, Action<Error> onError, string locale = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/v1/projects/{StoreProjectId}/subscriptions/{subscriptionId}")
				.AddLocale(locale)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Subscriptions,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetSubscriptionDetails(subscriptionId, onSuccess, onError, locale)));
		}

		/// <summary>
		/// Changes a regular subscription status to `non_renewing` (subscription is automatically canceled after expiration).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="subscriptionId">Subscription ID.</param>
		/// <param name="onSuccess">Called after successful subscription cancelling.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void CancelSubscription(int subscriptionId, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/user/v1/projects/{StoreProjectId}/subscriptions/{subscriptionId}/cancel";

			WebRequestHelper.Instance.PutRequest(
				SdkType.Subscriptions,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => CancelSubscription(subscriptionId, onSuccess, onError)));
		}

		/// <summary>
		/// Returns the URL of the renewal interface for the selected subscription.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="subscriptionId">Subscription ID. </param>
		/// <param name="onSuccess">Called after the URL has been successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="settings">Pay Station UI settings.</param>
		public static void GetSubscriptionRenewalUrl(int subscriptionId, Action<PaymentLink> onSuccess, Action<Error> onError, PaymentSettings settings = null)
		{
			var url = $"{BaseUrl}/user/v1/projects/{StoreProjectId}/subscriptions/{subscriptionId}/renew";

			var requestData = new RenewalSubscriptionRequest {
				settings = CheckAndFillPaymentSettings(settings)
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Subscriptions,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetSubscriptionRenewalUrl(subscriptionId, onSuccess, onError, settings)));
		}

		/// <summary>
		/// Returns Pay Station URL for the subscription purchase.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscriptions-purchase/).</remarks>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="onSuccess">Called after the URL has been successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="settings">Pay Station UI settings.</param>
		/// <param name="country">User's country. Affects the choice of locale and currency. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). By default, it is determined by the user's IP address.</param>
		public static void GetSubscriptionPurchaseUrl(string planExternalId, Action<PaymentLink> onSuccess, Action<Error> onError, PaymentSettings settings = null, string country = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/v1/projects/{StoreProjectId}/subscriptions/buy")
				.AddCountry(country)
				.Build();

			var requestData = new BuySubscriptionRequest {
				plan_external_id = planExternalId,
				settings = CheckAndFillPaymentSettings(settings)
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Subscriptions,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetSubscriptionPurchaseUrl(planExternalId, onSuccess, onError, settings, country)));
		}

		/// <summary>
		/// Returns the URL of the management interface for the selected subscription.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscription-management/).</remarks>
		/// <param name="onSuccess">Called after the URL has been successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="settings">Settings.</param>
		/// <param name="country">User's country. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
		public static void GetSubscriptionManagementUrl(Action<PaymentLink> onSuccess, Action<Error> onError, PaymentSettings settings = null, string country = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/v1/projects/{StoreProjectId}/subscriptions/manage")
				.AddCountry(country)
				.Build();

			var requestData = new ManageSubscriptionRequest {
				settings = CheckAndFillPaymentSettings(settings)
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Subscriptions,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetSubscriptionManagementUrl(onSuccess, onError, settings, country)));
		}

		/// <summary>
		/// Returns a list of plans available to authorized users, including plans purchased by the user while promotions are active.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/subscriptions/subscriptions-purchase/).</remarks>
		/// <param name="onSuccess">Called after a list of plans has been successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="planId">Array of subscription plan IDs. Plan ID can be found in the URL of the subscription details page in Publisher Account (https://publisher.xsolla.com/{merchant_id}/projects/{project_id}/subscriptions/plans/{plan_id}).</param>
		/// <param name="planExternalId">List of subscription plan external IDs (32 characters per ID). Plan external ID can be found in Publisher Account in the **Subscriptions > Subscription plans** section next to the plan name.</param>
		/// <param name="limit">Limit for the number of elements on the page (15 elements are displayed by default).</param>
		/// <param name="offset">Number of elements from which the list is generated (the count starts from 0).</param>
		/// <param name="locale">Language of the UI.<br/>
		///     The following languages are supported: Arabic (`ar`), Bulgarian (`bg`), Czech (`cs`), German (`de`), Spanish (`es`), French (`fr`), Hebrew (`he`), Italian (`it`), Japanese (`ja`), Korean (`ko`), Polish (`pl`), Portuguese (`pt`), Romanian (`ro`), Russian (`ru`), Thai (`th`), Turkish (`tr`), Vietnamese (`vi`), Chinese Simplified (`cn`), Chinese Traditional (`tw`), English (`en`, default).<bt/>
		///     By default, it is determined by the user's IP address.</param>
		/// <param name="country">User's country. Two-letter uppercase country code per [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2). Affects the choice of locale and currency. By default, it is determined by the user's IP address.</param>
		public static void GetSubscriptionPlans(Action<PlanItems> onSuccess, Action<Error> onError, int[] planId = null, string[] planExternalId = null, int limit = 50, int offset = 0, string locale = null, string country = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/user/v1/projects/{StoreProjectId}/plans")
				.AddLimit(limit)
				.AddOffset(offset)
				.AddLocale(locale)
				.AddCountry(country)
				.AddArray("plan_id", planId)
				.AddArray("plan_external_id", planExternalId)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Subscriptions,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetSubscriptionPlans(onSuccess, onError, planId, planExternalId, limit, offset, locale, country)));
		}

		private static PaymentSettings CheckAndFillPaymentSettings(PaymentSettings settings)
		{
			if (settings == null)
				settings = new PaymentSettings();

			if (settings.sandbox == null)
				settings.sandbox = XsollaSettings.IsSandbox;

			var commonUiSettings = PayStationUISettings.GenerateSettings();
			if (commonUiSettings != null)
			{
				if (settings.ui == null)
					settings.ui = new PaymentSettings.UI();

				if (settings.ui.theme == null)
					settings.ui.theme = commonUiSettings.theme;
			}

			var commonRedirectPolicy = RedirectPolicySettings.GeneratePolicy();
			if (commonRedirectPolicy != null)
			{
				if (settings.redirect_policy == null)
					settings.redirect_policy = new PaymentSettings.RedirectPolicy();

				if (settings.return_url == null)
					settings.return_url = commonRedirectPolicy.return_url;

				if (settings.redirect_policy.redirect_conditions == null)
					settings.redirect_policy.redirect_conditions = commonRedirectPolicy.redirect_conditions;

				if (settings.redirect_policy.delay == null)
					settings.redirect_policy.delay = commonRedirectPolicy.delay;

				if (settings.redirect_policy.status_for_manual_redirection == null)
					settings.redirect_policy.status_for_manual_redirection = commonRedirectPolicy.status_for_manual_redirection;

				if (settings.redirect_policy.redirect_button_caption == null)
					settings.redirect_policy.redirect_button_caption = commonRedirectPolicy.redirect_button_caption;
			}

			return settings;
		}
	}
}