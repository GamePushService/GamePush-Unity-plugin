using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Auth
{
	public static class XsollaAuth
	{
		private const string BASE_URL = "https://login.xsolla.com/api";

		/// <summary>
		/// Checks if the user is authenticated. Returns `true` if the token exists and the user is authenticated.
		/// </summary>
		public static bool IsUserAuthenticated()
		{
			return XsollaToken.Exists;
		}

		/// <summary>
		/// Creates a new user account in the application and sends a sign-up confirmation email to the specified email address. To complete registration, the user should follow the link from the email. To disable email confirmation, contact your Account Manager.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="username">Username.</param>
		/// <param name="password">User password.</param>
		/// <param name="email">User email address.</param>
		/// <param name="onSuccess">Called after successful user registration. An account confirmation message will be sent to the specified email address if not disabled.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="locale">Defines localization of the email the user receives.<br/>
		///     The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <param name="acceptConsent">Whether the user gave consent to processing of their personal data.</param>
		/// <param name="promoEmailAgreement">User consent to receive the newsletter.</param>
		/// <param name="fields">Parameters used for extended registration form. To use this feature, please contact your Account Manager.</param>
		/// <seealso cref="SignIn"/>
		/// <seealso cref="ResetPassword"/>
		/// <seealso cref="ResendConfirmationLink"/>
		public static void Register(string username, string password, string email, Action<LoginLink> onSuccess, Action<Error> onError, string redirectUri = null, string state = null, string locale = null, bool? acceptConsent = null, bool? promoEmailAgreement = null, List<string> fields = null)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/user")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddState(GetState(state))
				.AddLocale(locale)
				.AddResponseType(GetResponseType())
				.Build();

			var requestData = new RegisterRequest {
				username = username,
				password = password,
				email = email,
				accept_consent = acceptConsent,
				fields = fields,
				promo_email_agreement = promoEmailAgreement.HasValue && promoEmailAgreement.Value ? 1 : 0
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				webRequest =>
				{
					var responseJson = webRequest?.downloadHandler?.text;
					if (string.IsNullOrEmpty(responseJson))
					{
						onSuccess?.Invoke(new LoginLink());
					}
					else
					{
						var loginLink = ParseUtils.FromJson<LoginLink>(responseJson);
						onSuccess?.Invoke(loginLink);
					}
				},
				onError,
				ErrorGroup.RegistrationErrors);
		}

		/// <summary>
		/// Authenticates the user by the username/email and password specified via the authentication interface.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="username">Username or email address.</param>
		/// <param name="password">User password.</param>
		/// <param name="onSuccess">Called after successful user authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <seealso cref="SignInConsoleAccount"/>
		/// <seealso cref="Register"/>
		/// <seealso cref="ResetPassword"/>
		/// <seealso cref="ResendConfirmationLink"/>
		public static void SignIn(string username, string password, Action onSuccess, Action<Error> onError, string redirectUri = null)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/login/token")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddScope(GetScope())
				.Build();

			var requestData = new SignInRequest {
				username = username,
				password = password
			};

			WebRequestHelper.Instance.PostRequest<TokenResponse, SignInRequest>(
				SdkType.Login,
				url,
				requestData,
				response =>
				{
					XsollaToken.Create(response.access_token, response.refresh_token);
					onSuccess?.Invoke();
				},
				onError,
				ErrorGroup.LoginErrors);
		}

		/// <summary>
		/// Starts user authentication and sends an email with a one-time code and a link to the specified email address (if login via magic link is configured for the Login project)
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="email">User email address.</param>
		/// <param name="onSuccess">Called after successful email authentication start.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="locale">Defines localization of the email the user receives.
		///     The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <param name="sendLink">Shows whether a link is sent with the confirmation code in the email or not.</param>
		/// <param name="linkUrl">URL to redirect the user to the status authentication page.</param>
		public static void StartAuthByEmail(string email, Action<OperationId> onSuccess, Action<Error> onError, string redirectUri = null, string state = null, string locale = null, bool? sendLink = null, string linkUrl = null)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/login/email/request")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddState(GetState(state))
				.AddLocale(locale)
				.AddScope(GetScope())
				.AddResponseType(GetResponseType())
				.Build();

			var requestData = new StartAuthByEmailRequest {
				email = email,
				link_url = linkUrl,
				send_link = sendLink
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				onSuccess,
				onError,
				ErrorGroup.LoginErrors);
		}

		/// <summary>
		/// Completes authentication after the user enters a one-time code or follows a link received in an email.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="email">User email address.</param>
		/// <param name="confirmationCode">Confirmation code.</param>
		/// <param name="operationId">Identifier of the confirmation code.</param>
		/// <param name="onSuccess">Called after successful email authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void CompleteAuthByEmail(string email, string confirmationCode, string operationId, Action onSuccess, Action<Error> onError)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/login/email/confirm")
				.AddClientId(XsollaSettings.OAuthClientId)
				.Build();

			var requestData = new CompleteAuthByEmailRequest {
				email = email,
				code = confirmationCode,
				operation_id = operationId
			};

			WebRequestHelper.Instance.PostRequest<LoginLink, CompleteAuthByEmailRequest>(
				SdkType.Login,
				url,
				requestData,
				response => ParseCodeFromUrlAndExchangeToToken(response.login_url, onSuccess, onError),
				onError,
				ErrorGroup.LoginErrors);
		}

		/// <summary>
		/// Starts user authentication and sends an SMS with a one-time code and a link to the specified phone number (if login via magic link is configured for the Login project).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="phoneNumber">User phone number.</param>
		/// <param name="onSuccess">Called after successful phone number authentication start.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="sendLink">Shows whether a link is sent with the confirmation code in the SMS or not.</param>
		/// <param name="linkUrl">URL to redirect the user to the status authentication page.</param>
		public static void StartAuthByPhoneNumber(string phoneNumber, Action<OperationId> onSuccess, Action<Error> onError, string redirectUri = null, string state = null, bool? sendLink = null, string linkUrl = null)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/login/phone/request")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddState(GetState(state))
				.AddScope(GetScope())
				.AddResponseType(GetResponseType())
				.Build();

			var requestData = new StartAuthByPhoneNumberRequest {
				link_url = linkUrl,
				phone_number = phoneNumber,
				send_link = sendLink
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				onSuccess,
				onError,
				ErrorGroup.LoginErrors);
		}

		/// <summary>
		/// Completes authentication after the user enters a one-time code or follows a link received by SMS.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/passwordless-auth/).</remarks>
		/// <param name="phoneNumber">User phone number.</param>
		/// <param name="confirmationCode">Confirmation code.</param>
		/// <param name="operationId">Identifier of the confirmation code.</param>
		/// <param name="onSuccess">Called after successful phone number authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void CompleteAuthByPhoneNumber(string phoneNumber, string confirmationCode, string operationId, Action onSuccess, Action<Error> onError)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/login/phone/confirm")
				.AddClientId(XsollaSettings.OAuthClientId)
				.Build();

			var requestData = new CompleteAuthByPhoneNumberRequest {
				phone_number = phoneNumber,
				code = confirmationCode,
				operation_id = operationId
			};

			WebRequestHelper.Instance.PostRequest<LoginLink, CompleteAuthByPhoneNumberRequest>(
				SdkType.Login,
				url,
				requestData,
				response => ParseCodeFromUrlAndExchangeToToken(response.login_url, onSuccess, onError),
				onError,
				ErrorGroup.LoginErrors);
		}

		/// <summary>
		/// Resets the user’s current password and sends an email to change the password to the email address specified during sign-up.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="email">Email to send the password change verification message to.</param>
		/// <param name="onSuccess">Called after successful user password reset.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="locale">Defines localization of the email the user receives.
		///     The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <seealso cref="Register"/>
		/// <seealso cref="SignIn"/>
		/// <seealso cref="ResendConfirmationLink"/>
		public static void ResetPassword(string email, Action onSuccess, Action<Error> onError, string redirectUri = null, string locale = null)
		{
			var url = new UrlBuilder(BASE_URL + "/password/reset/request")
				.AddProjectId(XsollaSettings.LoginId)
				.AddParam("login_url", RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddLocale(locale)
				.Build();

			var requestData = new ResetPasswordRequest {
				username = email
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				onSuccess,
				onError,
				ErrorGroup.ResetPasswordErrors);
		}

		/// <summary>
		/// Resends a sign-up confirmation email to the specified email address. To complete registration, the user must follow the link from the email.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/classic-auth/).</remarks>
		/// <param name="username">Username or user email address.</param>
		/// <param name="onSuccess">Called after successful sending of the request.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		/// <param name="locale">Defines localization of the email user receives.
		///     The following languages are supported: Arabic (`ar_AE`), Bulgarian (`bg_BG`), Czech (`cz_CZ`), German (`de_DE`), Spanish (`es_ES`), French (`fr_FR`), Hebrew (`he_IL`), Italian (`it_IT`), Japanese (`ja_JP`), Korean (`ko_KR`), Polish (`pl_PL`), Portuguese (`pt_BR`), Romanian (`ro_RO`), Russian (`ru_RU`), Thai (`th_TH`), Turkish (`tr_TR`), Vietnamese (`vi_VN`), Chinese Simplified (`zh_CN`), Chinese Traditional (`zh_TW`), English (`en_XX`, default).
		/// </param>
		/// <seealso cref="Register"/>
		/// <seealso cref="SignIn"/>
		/// <seealso cref="ResetPassword"/>
		public static void ResendConfirmationLink(string username, Action onSuccess, Action<Error> onError, string redirectUri = null, string state = null, string locale = null)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/user/resend_confirmation_link")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddState(GetState(state))
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddLocale(locale)
				.Build();

			var requestData = new ResendConfirmationLinkRequest {
				username = username
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				onSuccess,
				onError);
		}

		/// <summary>
		/// Authenticates the user with the access token using social network credentials.
		/// </summary>
		/// <param name="accessToken">Access token received from a social network.</param>
		/// <param name="accessTokenSecret">Parameter `oauth_token_secret` received from the authorization request. Required for Twitter only.</param>
		/// <param name="openId">Parameter `openid` received from the social network. Required for WeChat only.</param>
		/// <param name="provider">Name of the social network connected to Login in Publisher Account. Can be `facebook`, `google`, `linkedin`, `twitter`, `discord`, `naver`, `baidu`, `wechat`, or `qq_mobile`.</param>
		/// <param name="onSuccess">Called after successful user authentication on the specified platform.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		public static void AuthWithSocialNetworkAccessToken(string accessToken, string accessTokenSecret, string openId, string provider, Action onSuccess, Action<Error> onError, string redirectUri = null, string state = null)
		{
			var url = new UrlBuilder(BASE_URL + $"/oauth2/social/{provider}/login_with_token")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddResponseType(GetResponseType())
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddState(GetState(state))
				.AddScope(GetScope())
				.Build();

			var requestData = new AuthWithSocialNetworkAccessTokenRequest {
				access_token = accessToken,
				access_token_secret = accessTokenSecret,
				openId = openId
			};

			WebRequestHelper.Instance.PostRequest<LoginLink, AuthWithSocialNetworkAccessTokenRequest>(
				SdkType.Login,
				url,
				requestData,
				link => ParseCodeFromUrlAndExchangeToToken(link.login_url, onSuccess, onError),
				onError,
				ErrorGroup.LoginErrors);
		}

		/// <summary>
		/// Authenticates the user by saved token. Returns `true` if the token is loaded successfully and the user is authenticated
		/// </summary>
		public static bool AuthViaSavedToken()
		{
			return XsollaToken.TryLoadInstance();
		}

		/// <summary>
		/// Authenticates the user with Xsolla Login widget.
		/// For standalone builds, the widget opens in the built-in browser that is included with the SDK.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/login-widget/).</remarks>
		/// <param name="onSuccess">Called after successful authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onCancel">Called after browser closing by user.</param>
		/// <param name="locale">Login widget UI language. Supported languages: Arabic (ar_AE), Bulgarian (bg_BG), Czech (cz_CZ), Filipino (fil-PH), English (en_XX), German (de_DE), Spanish (es_ES), French (fr_FR), Hebrew (he_IL), Indonesian (id-ID), Italian (it_IT), Japanese (ja_JP), Khmer (km-KH), Korean (ko_KR), Lao language ( lo-LA), Myanmar (my-MM), NepaliPolish (ne-NP), (pl_PL), Portuguese (pt_BR), Romanian (ro_RO), Russian (ru_RU), Thai (th_TH), Turkish (tr_TR), Vietnamese (vi_VN), Chinese Simplified (zh_CN), Chinese Traditional (zh_TW).</param>
		public static void AuthWithXsollaWidget(Action onSuccess, Action<Error> onError, Action onCancel, string locale = null)
		{
#if UNITY_STANDALONE
			new StandaloneXsollaWidgetAuth().Perform(onSuccess, onError, onCancel, locale);
#elif UNITY_ANDROID
			new AndroidXsollaWidgetAuth().Perform(onSuccess, onError, onCancel, locale);
#elif UNITY_IOS
			new IosXsollaWidgetAuth().Perform(onSuccess, onError, onCancel, locale);
#else
			onError?.Invoke(new Error(ErrorType.NotSupportedOnCurrentPlatform, errorMessage: $"Auth with Xsolla Widget is not supported for this platform: {Application.platform}"));
#endif
		}

		/// <summary>
		/// Authenticates the user via Xsolla Launcher
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-launcher/#unity_sdk_how_to_set_up_auth_via_launcher).</remarks>
		/// <param name="onSuccess">Called after successful authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void AuthViaXsollaLauncher(Action onSuccess, Action<Error> onError)
		{
			new XsollaLauncherAuth().Perform(onSuccess, onError);
		}

		/// <summary>
		/// Logs the user out and deletes the user session according to the value of the sessions parameter (OAuth2.0 only).
		/// </summary>
		/// <param name="onSuccess">Called after successful user logout.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="logoutType">Shows how the user is logged out and how the user session is deleted. Can be `sso` or `all` (default). Leave empty to use the default value.</param>
		public static void Logout(Action onSuccess, Action<Error> onError, LogoutType logoutType = LogoutType.All)
		{
			var url = new UrlBuilder(BASE_URL + "/oauth2/logout")
				.AddParam("sessions", logoutType.ToString().ToLowerInvariant())
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				onError);

			XsollaToken.DeleteSavedInstance();
		}

		/// <summary>
		/// This method is used for authenticating users in Xsolla Login,
		/// who play on the consoles and other platforms
		/// where Xsolla Login isn't used. You must implement it
		/// on your server side.
		/// Integration flow on the server side:
		/// <list type="number">
		///		<item>
		///			<term>Generate server JWT</term>
		///			<description>
		///				<list type="bullet">
		///					<item>
		///						<term>Connect OAuth 2.0 server client.</term>
		///						<description>Follow the [instructions](https://developers.xsolla.com/doc/login/security/connecting-oauth2/#login_features_connecting_oauth2_connecting_client) to connect the client and cope copy the <b>Client ID</b> and <b>Secret key</b>.
		///						</description>
		///					</item>
		///					<item>
		///						<term>Implement method: </term>
		///						<description>
		///							<see href="https://developers.xsolla.com/login-api/oauth-20/generate-user-jwt"/>
		///							with `application/x-www-form-urlencoded` payload parameters:
		///							<list type="bullet">
		///								<item>
		///									<description>client_id=YOUR_CLIENT_ID</description>
		///								</item>
		///								<item>
		///									<description>client_secret=YOUR_CLIENT_SECRET</description>
		///								</item>
		///								<item>
		///									<description>grant_type=client_credentials</description>
		///								</item>
		///							</list>
		///						</description>
		///					</item>
		///				</list>
		///			</description>
		///		</item>
		///		<item>
		///			<term>Implement auth method </term>
		///			<description>
		///				<see href="https://developers.xsolla.com/login-api/jwt/auth-by-custom-id"/>
		///				with:
		///				<list type="bullet">
		///					<item>
		///						<term>Query parameters </term>
		///						<description><c>?publisher_project_id=XsollaSettings.StoreProjectId</c></description>
		///					</item>
		///					<item>
		///						<term>Headers </term>
		///						<description>
		///						`Content-Type: application/json` and `X-SERVER-AUTHORIZATION: YourGeneratedJwt`
		///						</description>
		///					</item>
		///					<item>
		///						<term>[More information about authentication via custom ID](https://developers.xsolla.com/sdk/unity/authentication/auth-via-custom-id/).</term>
		///					</item>
		///				</list>
		///
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <param name="userId">Social platform (XBox, PS4, etc) user unique identifier.</param>
		/// <param name="platform">Platform name (XBox, PS4, etc).</param>
		/// <param name="onSuccess">Called after successful user authentication. Authentication data including the JWT will be received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void SignInConsoleAccount(string userId, string platform, Action onSuccess, Action<Error> onError)
		{
			var withLogoutValue = XsollaSettings.InvalidateExistingSessions ? "1" : "0";
			var url = new UrlBuilder("https://livedemo.xsolla.com/sdk/sdk-shadow-account/auth")
				.AddParam("user_id", userId)
				.AddPlatform(platform)
				.AddParam("with_logout", withLogoutValue)
				.Build();

			WebRequestHelper.Instance.GetRequest<AccessTokenResponse>(
				SdkType.Login,
				url,
				response =>
				{
					XsollaToken.Create(response.token);
					onSuccess?.Invoke();
				},
				onError);
		}

		/// <summary>
		/// Authenticates the user via a particular device ID.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="onSuccess">Called after successful user authentication via the device ID.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="deviceInfo">Information about the device that is used to identify the user. if not specified, the method defines this infotmation automatically.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		public static void AuthViaDeviceID(Action onSuccess, Action<Error> onError, DeviceInfo deviceInfo = null, string redirectUri = null, string state = null)
		{
#if !(UNITY_ANDROID || UNITY_IOS)
			onError?.Invoke(new Error(ErrorType.NotSupportedOnCurrentPlatform, "This method is only available for Android and iOS platforms"));
#else
			if (deviceInfo == null)
				deviceInfo = DeviceInfo.Create();

			var deviceType = deviceInfo.GetDeviceType();
			var url = new UrlBuilder(BASE_URL + $"/oauth2/login/device/{deviceType}")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddResponseType(GetResponseType())
				.AddState(GetState(state))
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddScope(GetScope())
				.Build();

			var requestData = new AuthViaDeviceIdRequest {
				device = deviceInfo.GetSafeDeviceData(),
				device_id = deviceInfo.GetSafeDeviceId()
			};

			WebRequestHelper.Instance.PostRequest<LoginLink, AuthViaDeviceIdRequest>(
				SdkType.Login,
				url,
				requestData,
				response => ParseCodeFromUrlAndExchangeToToken(response.login_url, onSuccess, onError),
				onError,
				ErrorGroup.LoginErrors);
#endif
		}

		/// <summary>
		/// Authenticates a user by exchanging the session ticket from Steam, Xbox, or Epic Games to the JWT.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/silent-auth/).</remarks>
		/// <param name="providerName">Platform on which the session ticket was obtained. Can be `steam`, `xbox`, or `epicgames`.</param>
		/// <param name="appId">Platform application identifier.</param>
		/// <param name="sessionTicket">Session ticket received from the platform.</param>
		/// <param name="onSuccess">Called after successful user authentication with a platform session ticket. Authentication data including a JWT will be received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. Will be `xsollatest` by default. Used only for OAuth2.0 auth.</param>
		/// <param name="code">Code received from the platform.</param>
		public static void SilentAuth(string providerName, string appId, string sessionTicket, Action onSuccess, Action<Error> onError, string redirectUri = null, string state = null, string code = null)
		{
			var url = new UrlBuilder(BASE_URL + $"/oauth2/social/{providerName}/cross_auth")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddResponseType(GetResponseType())
				.AddState(GetState(state))
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddScope(GetScope())
				.AddParam("app_id", appId)
				.AddParam("session_ticket", sessionTicket)
				.AddParam("code", code)
				.AddParam("is_redirect", "false")
				.Build();

			WebRequestHelper.Instance.GetRequest<LoginLink>(
				SdkType.Login,
				url,
				response => ParseCodeFromUrlAndExchangeToToken(response.login_url, onSuccess, onError),
				onError);
		}

		/// <summary>
		/// Returns URL for authentication via the specified social network in a browser.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/social-auth/#sdk_how_to_set_up_web_auth_via_social_networks).</remarks>
		/// <param name="provider">Name of a social network. Provider must be connected to Login in Publisher Account.
		/// Can be `amazon`, `apple`, `baidu`, `battlenet`, `discord`, `facebook`, `github`, `google`, `kakao`, `linkedin`, `mailru`, `microsoft`, `msn`, `naver`, `ok`, `paypal`, `psn`, `qq`, `reddit`, `steam`, `twitch`, `twitter`, `vimeo`, `vk`, `wechat`, `weibo`, `yahoo`, `yandex`, `youtube`, or `xbox`.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="state">Value used for additional user verification on backend. Must be at least 8 symbols long. `xsollatest` by default. Required for OAuth 2.0.</param>
		public static string GetSocialNetworkAuthUrl(SocialProvider provider, string redirectUri = null, string state = null)
		{
			var providerValue = provider.ToApiParameter();
			var url = new UrlBuilder(BASE_URL + $"/oauth2/social/{providerValue}/login_redirect")
				.AddClientId(XsollaSettings.OAuthClientId)
				.AddState(GetState(state))
				.AddResponseType(GetResponseType())
				.AddRedirectUri(RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.AddScope(GetScope())
				.Build();

			return WebRequestHelper.Instance.AppendAnalyticsToUrl(SdkType.Login, url);
		}

		/// <summary>
		/// Returns list of links for social authentication enabled in Publisher Account (<b>your Login project > Authentication > Social login</b> section).
		/// The links are valid for 10 minutes.
		/// You can get the link by this call and add it to your button for authentication via the social network.
		/// </summary>
		/// <param name="onSuccess">Called after list of links for social authentication was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="locale"> Region in the `language code_country code` format, where:
		///     - `language code` — language code in the [ISO 639-1](https://en.wikipedia.org/wiki/List_of_ISO_639-1_codes) format;
		///     - `country code` — country/region code in the [ISO 3166-1 alpha-2](https://en.wikipedia.org/wiki/ISO_3166-1_alpha-2) format.<br/>
		///     The list of the links will be sorted from most to least used social networks, according to the variable value.
		/// </param>
		public static void GetLinksForSocialAuth(Action<SocialNetworkLinks> onSuccess, Action<Error> onError, string locale = null)
		{
			var url = new UrlBuilder(BASE_URL + "/users/me/login_urls")
				.AddLocale(locale)
				.Build();

			WebRequestHelper.Instance.GetRequest<List<SocialNetworkLink>>(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				list =>
				{
					onSuccess?.Invoke(new SocialNetworkLinks {
						items = list
					});
				},
				error => TokenAutoRefresher.Check(error, onError, () => GetLinksForSocialAuth(onSuccess, onError, locale)));
		}

		/// <summary>
		/// Refreshes the token in case it is expired. Works only when OAuth 2.0 is enabled.
		/// </summary>
		/// <param name="onSuccess"> Called after successful token refreshing. Refresh data including the JWT will be received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		public static void RefreshToken(Action onSuccess, Action<Error> onError, string redirectUri = null)
		{
			var refreshToken = XsollaToken.RefreshToken;
			if (string.IsNullOrEmpty(refreshToken))
			{
				onError?.Invoke(new Error(ErrorType.InvalidToken, errorMessage: "Invalid refresh token"));
				return;
			}

			var requestData = new WWWForm();
			requestData.AddField("client_id", XsollaSettings.OAuthClientId);
			requestData.AddField("redirect_uri", RedirectUrlHelper.GetRedirectUrl(redirectUri));
			requestData.AddField("grant_type", "refresh_token");
			requestData.AddField("refresh_token", refreshToken);

			const string url = BASE_URL + "/oauth2/token";
			WebRequestHelper.Instance.PostRequest<TokenResponse>(
				SdkType.Login,
				url,
				requestData,
				response =>
				{
					XsollaToken.Create(response.access_token, response.refresh_token);
					onSuccess?.Invoke();
				},
				error => onError?.Invoke(error));
		}

		/// <summary>
		/// Exchanges the user authentication code to a valid JWT.
		/// </summary>
		/// <param name="code">Access code received from several other OAuth 2.0 requests (example: code from social network authentication).</param>
		/// <param name="onSuccess">Called after successful exchanging. Contains exchanged token.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		public static void ExchangeCodeToToken(string code, Action onSuccess, Action<Error> onError, string redirectUri = null)
		{
			const string url = BASE_URL + "/oauth2/token";

			var requestData = new WWWForm();
			requestData.AddField("client_id", XsollaSettings.OAuthClientId);
			requestData.AddField("redirect_uri", RedirectUrlHelper.GetRedirectUrl(redirectUri));
			requestData.AddField("grant_type", "authorization_code");
			requestData.AddField("code", code);

			WebRequestHelper.Instance.PostRequest<TokenResponse>(
				SdkType.Login,
				url,
				requestData,
				response =>
				{
					XsollaToken.Create(response.access_token, response.refresh_token);
					onSuccess?.Invoke();
				},
				error => onError?.Invoke(error));
		}

		/// <summary>
		/// Returns user details.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="onSuccess">Called after successful user details were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetUserInfo(Action<UserInfo> onSuccess, Action<Error> onError)
		{
			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				BASE_URL + "/users/me",
				WebRequestHeader.AuthHeader(),
				onSuccess,
				onError);
		}

		/// <summary>
		/// Authenticates user via an account in the specified social networks.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/social-auth/).</remarks>
		/// <param name="provider">Name of a social network. Provider must be connected to Login in Publisher Account.
		/// Can be `amazon`, `apple`, `baidu`, `battlenet`, `discord`, `facebook`, `github`, `google`, `kakao`, `linkedin`, `mailru`, `microsoft`, `msn`, `naver`, `ok`, `paypal`, `psn`, `qq`, `reddit`, `steam`, `twitch`, `twitter`, `vimeo`, `vk`, `wechat`, `weibo`, `yahoo`, `yandex`, `youtube`, or `xbox`.</param>
		/// <param name="onSuccess">Called after successful user authentication.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="onCancel">Called in case user closed browser.</param>
		public static void AuthViaSocialNetwork(SocialProvider provider, Action onSuccess, Action<Error> onError, Action onCancel)
		{
#if UNITY_STANDALONE
			new StandaloneSocialAuth().Perform(provider, onSuccess, onError, onCancel);
#elif UNITY_ANDROID
			new AndroidSocialAuth().Perform(provider, onSuccess, onError, onCancel);
#elif UNITY_IOS
			new IosSocialAuth().Perform(provider, onSuccess, onError, onCancel);
#else
			onError?.Invoke(new Error(ErrorType.NotSupportedOnCurrentPlatform, errorMessage: $"Social auth is not supported for this platform: {Application.platform}"));
#endif
		}

		private static void ParseCodeFromUrlAndExchangeToToken(string url, Action onSuccess, Action<Error> onError)
		{
			if (ParseUtils.TryGetValueFromUrl(url, ParseParameter.code, out var parsedCode))
				ExchangeCodeToToken(parsedCode, onSuccess, onError);
			else
				onError?.Invoke(Error.UnknownError);
		}

		private static string GetState(string oauthState)
		{
			return !string.IsNullOrEmpty(oauthState)
				? oauthState
				: "xsollatest";
		}

		private static string GetResponseType()
		{
			return "code";
		}

		private static string GetScope()
		{
			return "offline";
		}
	}
}
