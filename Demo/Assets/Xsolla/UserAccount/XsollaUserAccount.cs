using System;
using System.Collections.Generic;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	public static class XsollaUserAccount
	{
		private static string BaseUrl => "https://login.xsolla.com/api";
		private static string StoreProjectId => XsollaSettings.StoreProjectId;

		/// <summary>
		/// Updates the specified user’s information. Changes are made on the user data storage side.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="info">User information.</param>
		/// <param name="onSuccess">Called after successful user details modification.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void UpdateUserInfo(UserInfoUpdate info, Action<UserInfo> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me";

			WebRequestHelper.Instance.PatchRequest(
				SdkType.Login,
				url,
				info,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UpdateUserInfo(info, onSuccess, onError)));
		}

		/// <summary>
		/// Searches users by the nickname parameter and gets a list of them. Search can be performed instantly when the user starts entering the search parameter.
		/// NOTE: User can search only 1 time per second.
		/// </summary>
		/// <param name="nickname">The search string that may contain: Nickname only, Tag only, Nickname and tag together</param>
		/// <param name="offset">Number of the elements from which the list is generated.</param>
		/// <param name="limit">Maximum number of users that are returned at a time.</param>
		/// <param name="onSuccess">Called after user search is successfully completed.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void SearchUsers(string nickname, int offset, int limit, Action<FoundUsers> onSuccess, Action<Error> onError)
		{
			var url = new UrlBuilder($"{BaseUrl}/users/search/by_nickname")
				.AddParam("nickname", nickname)
				.AddLimit(limit)
				.AddOffset(offset)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => SearchUsers(nickname, offset, limit, onSuccess, onError)));
		}

		/// <summary>
		/// Returns specified user public profile information.
		/// </summary>
		/// <param name="userId">User identifier of public profile information to be received.</param>
		/// <param name="onSuccess">Called after user profile data was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetPublicInfo(string userId, Action<UserPublicInfo> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/{userId}/public";

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetPublicInfo(userId, onSuccess, onError)));
		}

		/// <summary>
		/// Returns user phone number that is used for two-factor authentication.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="onSuccess">Called after user phone number was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="UpdateUserPhoneNumber"/>
		/// <seealso cref="DeleteUserPhoneNumber"/>
		public static void GetUserPhoneNumber(Action<UserPhoneNumber> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/phone";

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetUserPhoneNumber(onSuccess, onError)));
		}

		/// <summary>
		/// Changes the user’s phone number that is used for two-factor authentication. Changes are made on the user data storage side (server-side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="phoneNumber">Updated user phone number according to national conventions.</param>
		/// <param name="onSuccess">Called after user phone number was successfully modified.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="GetUserPhoneNumber"/>
		/// <seealso cref="DeleteUserPhoneNumber"/>
		public static void UpdateUserPhoneNumber(string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/phone";

			var requestData = new UserPhoneNumber {
				phone_number = phoneNumber
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UpdateUserPhoneNumber(phoneNumber, onSuccess, onError)));
		}

		/// <summary>
		/// Deletes the user’s phone number that is used for two-factor authentication. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="phoneNumber">User phone number for removal.</param>
		/// <param name="onSuccess">Called after the user phone number was successfully removed.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="GetUserPhoneNumber"/>
		/// <seealso cref="UpdateUserPhoneNumber"/>
		public static void DeleteUserPhoneNumber(string phoneNumber, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/phone/{phoneNumber}";

			WebRequestHelper.Instance.DeleteRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => DeleteUserPhoneNumber(phoneNumber, onSuccess, onError)));
		}

		/// <summary>
		/// Changes the user’s avatar. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="pictureData">User profile picture in the binary format.</param>
		/// <param name="boundary"></param>
		/// <param name="onSuccess">Called after the user profile picture was successfully modified.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="DeleteUserPicture"/>
		public static void UploadUserPicture(byte[] pictureData, string boundary, Action<string> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/picture";

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.FormDataContentTypeHeader(boundary)
			};

			WebRequestHelper.Instance.PostUploadRequest(
				SdkType.Login,
				url,
				pictureData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UploadUserPicture(pictureData, boundary, onSuccess, onError)));
		}

		/// <summary>
		/// Deletes the user’s avatar. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="onSuccess">Called after user profile picture was successfully removed.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="UploadUserPicture"/>
		public static void DeleteUserPicture(Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/picture";

			WebRequestHelper.Instance.DeleteRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => DeleteUserPicture(onSuccess, onError)));
		}

		/// <summary>
		/// Checks user age for a particular region. The age requirements depend on the region. Service determines the user location by the IP address.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="dateOfBirth">User's birth date in the `YYYY-MM-DD` format.</param>
		/// <param name="onSuccess">Called after successful check of the user age.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void CheckUserAge(string dateOfBirth, Action<CheckUserAgeResult> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/age/check";

			var request = new UserCheckAgeRequest {
				dob = dateOfBirth,
				project_id = XsollaSettings.LoginId
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				request,
				onSuccess,
				onError);
		}

		/// <summary>
		/// Returns the user’s email.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-account/).</remarks>
		/// <param name="onSuccess">Called after user email was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetUserEmail(Action<UserEmail> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/email";

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetUserEmail(onSuccess, onError)));
		}

		/// <summary>
		/// Creates the code for linking the platform account to the existing main account when the user logs in to the game via a gaming console.
		/// The call is used with [Link accounts by code](https://developers.xsolla.com/login-api/linking-account/linking/link-accounts-by-code/) request.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/account-linking/).</remarks>
		/// <param name="onSuccess">Called after successful linking code creation.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="Xsolla.Auth.XsollaAuth.SignInConsoleAccount"/>
		/// <seealso cref="LinkConsoleAccount"/>
		public static void RequestLinkingCode(Action<LinkingCode> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/account/code";

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RequestLinkingCode(onSuccess, onError)));
		}

		/// <summary>
		/// This method is used for authenticating users in Xsolla Login,
		/// who play on the consoles and other platforms
		/// where Xsolla Login isn't used. You must implement it
		/// on the your server side.
		/// Integration flow on the server side:
		/// <list type="number">
		///		<item>
		///			<term>Generate server JWT</term>
		///			<description>
		///				<list type="bullet">
		///					<item>
		///						<term>Connect OAuth 2.0 server client</term>
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
		///			<term>Implement APIs for account linking </term>
		///			<description>
		///				<see href="https://developers.xsolla.com/login-api/users/link-accounts-by-code"/>
		///				with:
		///				<list type="bullet">
		///					<item>
		///						<term>Headers </term>
		///						<description>
		///						`Content-Type: application/json` and `X-SERVER-AUTHORIZATION: YourGeneratedJwt`
		///						</description>
		///					</item>
		///					<item>
		///					<term>Body </term>
		///					<description>[See documentation](https://developers.xsolla.com/api/login/operation/link-accounts-by-code/).</description>
		///					</item>
		///				</list>
		///			</description>
		///		</item>
		/// </list>
		/// </summary>
		/// <param name="userId">Social platform (XBox, PS4, etc) user unique identifier.</param>
		/// <param name="platform">Platform name (XBox, PS4, etc).</param>
		/// <param name="confirmationCode">Code, taken from unified account.</param>
		/// <param name="onSuccess">Success operation callback.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="Xsolla.Auth.XsollaAuth.SignInConsoleAccount"/>
		/// <seealso cref="RequestLinkingCode"/>
		public static void LinkConsoleAccount(string userId, string platform, string confirmationCode, Action onSuccess, Action<Error> onError)
		{
			var url = new UrlBuilder("https://livedemo.xsolla.com/sdk/sdk-shadow-account/link")
				.AddParam("user_id", userId)
				.AddParam("code", confirmationCode)
				.AddPlatform(platform)
				.Build();

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				onComplete: onSuccess,
				onError: onError);
		}

		/// <summary>
		/// Returns a list of particular user’s attributes with their values and descriptions. Returns only user-editable attributes.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-attributes/).</remarks>
		/// <param name="attributeType">Type of attributes to get. Can be `Readonly` or `Custom`.</param>
		/// <param name="keys">List of attributes’ keys which you want to get. If not specified, the method returns all user’s attributes.</param>
		/// <param name="userId">Identifier of a user whose public attributes should be requested. If not specified, the method returns attributes for the current user.</param>
		/// <param name="onSuccess">Called after user attributes were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="UpdateUserAttributes"/>
		/// <seealso cref="RemoveUserAttributes"/>
		public static void GetUserAttributes(UserAttributeType attributeType, List<string> keys, string userId, Action<UserAttributes> onSuccess, Action<Error> onError)
		{
			string url;
			switch (attributeType)
			{
				case UserAttributeType.CUSTOM:
					url = $"{BaseUrl}/attributes/users/me/get";
					break;
				case UserAttributeType.READONLY:
					url = $"{BaseUrl}/attributes/users/me/get_read_only";
					break;
				default:
					throw new ArgumentOutOfRangeException(nameof(attributeType), attributeType, null);
			}

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			var getAttributesRequestBody = new GetAttributesRequest(keys, StoreProjectId, userId);

			WebRequestHelper.Instance.PostRequest<List<UserAttribute>, GetAttributesRequest>(
				SdkType.Login,
				url,
				getAttributesRequestBody,
				headers,
				response =>
				{
					onSuccess?.Invoke(new UserAttributes {
						items = response
					});
				},
				error => TokenAutoRefresher.Check(error, onError, () => GetUserAttributes(attributeType, keys, userId, onSuccess, onError)));
		}

		/// <summary>
		/// Updates the values of user attributes with the specified IDs. The method can be used to create or remove attributes. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-attributes/).</remarks>
		/// <param name="attributes">List of attributes of the specified game. To add attribute which does not exist, set this attribute to the `key` parameter. To update `value` of the attribute, specify its `key` parameter and set the new `value`. You can change several attributes at a time.</param>
		/// <param name="onSuccess">Called after successful user attributes modification on the server side.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="GetUserAttributes"/>
		/// <seealso cref="RemoveUserAttributes"/>
		public static void UpdateUserAttributes(List<UserAttribute> attributes, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/attributes/users/me/update";

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			var requestData = new ModifyAttributesRequest(attributes, StoreProjectId, null);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UpdateUserAttributes(attributes, onSuccess, onError)));
		}

		/// <summary>
		/// Removes user attributes with the specified IDs. Changes are made on the user data storage side (server side).
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/user-attributes/).</remarks>
		/// <param name="removingKeys">List of attribute keys for removal.</param>
		/// <param name="onSuccess">Called after successful user attributes removal on the server side.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <seealso cref="GetUserAttributes"/>
		/// <seealso cref="UpdateUserAttributes"/>
		public static void RemoveUserAttributes(List<string> removingKeys, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/attributes/users/me/update";

			var headers = new List<WebRequestHeader> {
				WebRequestHeader.AuthHeader(),
				WebRequestHeader.JsonContentTypeHeader()
			};

			var requestData = new ModifyAttributesRequest(null, StoreProjectId, removingKeys);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestData,
				headers,
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => RemoveUserAttributes(removingKeys, onSuccess, onError)));
		}

		/// <summary>
		/// Adds a username, email address, and password, that can be used for authentication, to the current account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="username">Username.</param>
		/// <param name="password">User password.</param>
		/// <param name="email">User email.</param>
		/// <param name="onSuccess">Called after successful email and password linking.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		/// <param name="promoEmailAgreement">Whether the user gave consent to receive the newsletters.</param>
		public static void AddUsernameEmailAuthToAccount(string username, string password, string email, Action<AddUsernameAndEmailResult> onSuccess, Action<Error> onError, string redirectUri = null, int? promoEmailAgreement = null)
		{
			var url = new UrlBuilder($"{BaseUrl}/users/me/link_email_password")
				.AddParam("login_url", RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.Build();

			var requestBody = new AddUsernameAndEmailRequest(username, password, email, promoEmailAgreement);

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestBody,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => AddUsernameEmailAuthToAccount(username, password, email, onSuccess, onError, redirectUri, promoEmailAgreement)),
				ErrorGroup.RegistrationErrors);
		}

		/// <summary>
		/// Gets a list of user's devices.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="onSuccess">Called after users devices data was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetUserDevices(Action<UserDevicesInfo> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/devices";

			WebRequestHelper.Instance.GetRequest<List<UserDeviceInfo>>(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				response =>
				{
					onSuccess?.Invoke(new UserDevicesInfo {
						items = response
					});
				},
				error => TokenAutoRefresher.Check(error, onError, () => GetUserDevices(onSuccess, onError)));
		}

		/// <summary>
		/// Links the specified device to the current user account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="onSuccess">Called after successful linking of the device.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="deviceInfo">Information about the device that is used to identify the user. if not specified, the method defines this infotmation automatically.</param>
		public static void LinkDeviceToAccount(Action onSuccess, Action<Error> onError, DeviceInfo deviceInfo = null)
		{
#if !(UNITY_ANDROID || UNITY_IOS)
			onError?.Invoke(new Error(ErrorType.NotSupportedOnCurrentPlatform, "This method is only available for Android and iOS platforms"));
#else
			if (deviceInfo == null)
				deviceInfo = DeviceInfo.Create();

			var deviceType = deviceInfo.GetDeviceType();
			var url = $"{BaseUrl}/users/me/devices/{deviceType}";

			var requestBody = new LinkDeviceRequest {
				device = deviceInfo.GetSafeDeviceData(),
				device_id = deviceInfo.GetSafeDeviceId()
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				requestBody,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => LinkDeviceToAccount(onSuccess, onError, deviceInfo)));
#endif
		}

		/// <summary>
		/// Unlinks the specified device from the current user account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/authentication/auth-via-device-id/).</remarks>
		/// <param name="deviceId">Platform specific unique device ID.<br/>
		/// For Android, it is an [ANDROID_ID](https://developer.android.com/reference/android/provider/Settings.Secure#ANDROID_ID) constant.<br/>
		/// For iOS, it is an [identifierForVendor](https://developer.apple.com/documentation/uikit/uidevice/1620059-identifierforvendor?language=objc) property.</param>
		/// <param name="onSuccess">Called after successful unlinking of the device.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void UnlinkDeviceFromAccount(int deviceId, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/devices/{deviceId}";

			WebRequestHelper.Instance.DeleteRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UnlinkDeviceFromAccount(deviceId, onSuccess, onError)));
		}

		/// <summary>
		/// Gets a list of user’s friends from a social provider.
		/// </summary>
		/// <param name="onSuccess">Called after user friends data was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="platform">Name of a social network. Provider must be connected to Login in Publisher Account.<br/>
		///     Can be `amazon`, `apple`, `baidu`, `battlenet`, `discord`, `facebook`, `github`, `google`, `kakao`, `linkedin`, `mailru`, `microsoft`, `msn`, `naver`, `ok`, `paypal`, `psn`, `qq`, `reddit`, `steam`, `twitch`, `twitter`, `vimeo`, `vk`, `wechat`, `weibo`, `yahoo`, `yandex`, `youtube`, or `xbox`.</param>
		/// <param name="offset">Number of the elements from which the list is generated. </param>
		/// <param name="limit">Maximum number of friends that are returned at a time.</param>
		/// <param name="withXlUid">Shows whether the social friends are from your game.</param>
		public static void GetUserSocialFriends(Action<UserSocialFriends> onSuccess, Action<Error> onError, SocialProvider platform = SocialProvider.None, int offset = 0, int limit = 500, bool withXlUid = false)
		{
			var withXlUidParam = withXlUid ? "true" : "false";
			var platformValue = platform != SocialProvider.None
				? platform.ToApiParameter()
				: null;

			var url = new UrlBuilder($"{BaseUrl}/users/me/social_friends")
				.AddParam("offset", offset)
				.AddParam("limit", limit)
				.AddParam("with_xl_uid", withXlUidParam)
				.AddPlatform(platformValue)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetUserSocialFriends(onSuccess, onError, platform, offset, limit, withXlUid)));
		}

		/// <summary>
		/// Begins data processing to update a list of user’s friends from a social provider.
		/// Note that there may be a delay in data processing because of the Xsolla Login server or provider server high loads.
		/// </summary>
		/// <param name="onSuccess">Called after user friends were successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="platform">Name of a social network. Provider must be connected to Login in Publisher Account.<br/>
		///     Can be `amazon`, `apple`, `baidu`, `battlenet`, `discord`, `facebook`, `github`, `google`, `kakao`, `linkedin`, `mailru`, `microsoft`, `msn`, `naver`, `ok`, `paypal`, `psn`, `qq`, `reddit`, `steam`, `twitch`, `twitter`, `vimeo`, `vk`, `wechat`, `weibo`, `yahoo`, `yandex`, `youtube`, or `xbox`.<br/>
		///     If you do not specify it, the call gets friends from all social providers.</param>
		public static void UpdateUserSocialFriends(Action onSuccess, Action<Error> onError, SocialProvider platform = SocialProvider.None)
		{
			var platformParam = platform != SocialProvider.None
				? platform.ToApiParameter()
				: null;

			var url = new UrlBuilder($"{BaseUrl}/users/me/social_friends/update")
				.AddPlatform(platformParam)
				.Build();

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UpdateUserSocialFriends(onSuccess, onError, platform)));
		}

		/// <summary>
		/// Gets a list of users added as friends of the authenticated user.
		/// </summary>
		/// <param name="type">Friends type.</param>
		/// <param name="onSuccess">Called after user friends data was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="sortBy">Condition for sorting the users.</param>
		/// <param name="sortOrder">Condition for sorting the list of users.</param>
		/// <param name="offset">Parameter that is used for API pagination.</param>
		/// <param name="limit">Maximum number of users that are returned at a time. Default: 20.</param>
		public static void GetUserFriends(FriendsSearchType type,
			Action<UserFriends> onSuccess,
			Action<Error> onError,
			FriendsSearchSort sortBy = FriendsSearchSort.ByNickname,
			FriendsSearchOrder sortOrder = FriendsSearchOrder.Asc,
			int limit = 20,
			int offset = 0)
		{
			var url = new UrlBuilder($"{BaseUrl}/users/me/relationships")
				.AddParam("type", type.ToApiParameter())
				.AddParam("sort_by", sortBy.ToApiParameter())
				.AddParam("sort_order", sortOrder.ToApiParameter())
				.AddParam("after", offset)
				.AddParam("limit", limit)
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => GetUserFriends(type, onSuccess, onError, sortBy, sortOrder, offset, limit)));
		}

		/// <summary>
		/// Modifies relationships with the specified user.
		/// </summary>
		/// <param name="action">Type of the action.</param>
		/// <param name="user">Identifier of the user to change relationship with.</param>
		/// <param name="onSuccess">Called after successful user friends data modification.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void UpdateUserFriends(FriendAction action, string user, Action onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/relationships";

			var request = new UserFriendUpdateRequest {
				action = action.ToApiParameter(),
				user = user
			};

			WebRequestHelper.Instance.PostRequest(
				SdkType.Login,
				url,
				request,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => UpdateUserFriends(action, user, onSuccess, onError)));
		}

		/// <summary>
		/// Links a social network that can be used for authentication to the current account.
		/// </summary>
		/// <remarks>[More about the use cases](https://developers.xsolla.com/sdk/unity/user-account-and-attributes/account-linking/#sdk_account_linking_additional_account).</remarks>
		/// <param name="providerName">Name of a social network. Provider must be connected to Login in Publisher Account.<br/>
		/// Can be `amazon`, `apple`, `baidu`, `battlenet`, `discord`, `facebook`, `github`, `google`, `instagram`, `kakao`, `linkedin`, `mailru`, `microsoft`, `msn`, `naver`, `ok`, `paradox`, `paypal`, `psn`, `qq`, `reddit`, `steam`, `twitch`, `twitter`, `vimeo`, `vk`, `wechat`, `weibo`, `yahoo`, `yandex`, `youtube`, `xbox`, `playstation`.</param>
		/// <param name="onSuccess">Called after the URL for social authentication was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		/// <param name="redirectUri">URI to redirect the user to after account confirmation, successful authentication, two-factor authentication configuration, or password reset confirmation.
		///     Must be identical to the OAuth 2.0 redirect URIs specified in Publisher Account.
		///     Required if there are several URIs.</param>
		public static void LinkSocialProvider(SocialProvider providerName, Action<LinkSocialProviderLink> onSuccess, Action<Error> onError, string redirectUri = null)
		{
			var providerValue = providerName.ToApiParameter();
			var url = new UrlBuilder($"{BaseUrl}/users/me/social_providers/{providerValue}/login_url")
				.AddParam("login_url", RedirectUrlHelper.GetRedirectUrl(redirectUri))
				.Build();

			WebRequestHelper.Instance.GetRequest(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => LinkSocialProvider(providerName, onSuccess, onError, redirectUri)));
		}

		/// <summary>
		/// Returns the list of social networks linked to the user account.
		/// </summary>
		/// <param name="onSuccess">Called after the list of linked social networks was successfully received.</param>
		/// <param name="onError">Called after the request resulted with an error.</param>
		public static void GetLinkedSocialProviders(Action<LinkedSocialNetworks> onSuccess, Action<Error> onError)
		{
			var url = $"{BaseUrl}/users/me/social_providers";

			WebRequestHelper.Instance.GetRequest<List<LinkedSocialNetwork>>(
				SdkType.Login,
				url,
				WebRequestHeader.AuthHeader(),
				response =>
				{
					onSuccess?.Invoke(new LinkedSocialNetworks {
						items = response
					});
				},
				error => TokenAutoRefresher.Check(error, onError, () => GetLinkedSocialProviders(onSuccess, onError)));
		}
	}
}
