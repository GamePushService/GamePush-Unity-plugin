using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core.Editor.AutoFillSettings
{
	public class AutoFillRequester : IDisposable
	{
		private const string PUBLISHER_ROOT_PROJECT = "40db2ea4-5d42-11e6-a3ff-005056a0e04a";
		private const string TOKEN_REQUEST_URL = "https://login.xsolla.com/api/login?projectId="+PUBLISHER_ROOT_PROJECT;
		private const string CONIRMATION_CODE_URL_TEMPLATE = "https://login.xsolla.com/api/otp/validate?challenge_id={0}&code={1}";
		private const string MERCHANT_GET_URL = "https://api.xsolla.com/merchant/users/init";
		private const string PROJECT_GET_URL = "https://api.xsolla.com/merchant/v3/merchants/{0}/projects";
		private const string LOGIN_GET_URL = "https://login.xsolla.com/api/projects?publisher_id={0}&publisher_project_id={1}";
		private const string OAUTH_GET_URL = "https://login.xsolla.com/api/oauth2/clients?project_id={0}";

		private bool _disposed = false;

		public void RequestToken(string username, string password, Action<string> onToken, Action<string> onConfirmationNeeded, Action<Error> onError)
		{
#if UNITY_2022_2_OR_NEWER
			var webRequest = UnityWebRequest.PostWwwForm(TOKEN_REQUEST_URL, UnityWebRequest.kHttpVerbPOST);
#else
			var webRequest = UnityWebRequest.Post(TOKEN_REQUEST_URL, UnityWebRequest.kHttpVerbPOST);
#endif
			var requestBody = new TokenRequestBody(username, password);
			var requestHeaders = WebRequestHelper.Instance.AppendAnalyticHeaders(SdkType.SettingsFillTool, WebRequestHeader.JsonContentTypeHeader());

			Action<string> onRequestSuccess = response =>
			{
				if (_disposed)
					return;
				else if (ParseTokenResponse(response, out string loginUrl, out string _, checkFor: ParseParameter.token))
					EnrichToken(loginUrl, onToken, onError);
				else if (ParseTokenResponse(response, out string _, out string challengeID, checkFor: ParseParameter.challenge_id))
					onConfirmationNeeded?.Invoke(challengeID);
				else
				{
					XDebug.LogError($"ConfirmByCode: Could not find neither token nor challengeID in '{response}'");
					onError?.Invoke(Error.UnknownError);
				}
			};

			Action<Error> onRequestError = error =>
			{
				if (!_disposed)
					onError?.Invoke(error);
			};

			EditorWebRequestHelper.Request(request: webRequest, requestBody: requestBody, requestHeaders: requestHeaders,
				onSuccess: onRequestSuccess, onError: onRequestError);
		}

		public void ConfirmByCode(string challengeID, string code, Action<string> onToken, Action<Error> onError)
		{
			var url = string.Format(CONIRMATION_CODE_URL_TEMPLATE,challengeID,code);
			var webRequest = UnityWebRequest.Get(url);

			Action<string> onRequestSuccess = response =>
			{
				if (_disposed)
					return;
				else if (ParseTokenResponse(response, out string loginUrl, out string _, checkFor: ParseParameter.token))
					EnrichToken(loginUrl,onToken,onError);
				else
				{
					XDebug.LogError($"ConfirmByCode: Could not find token in '{response}'");
					onError?.Invoke(Error.UnknownError);
				}
			};

			Action<Error> onRequestError = error =>
			{
				if (!_disposed)
					onError?.Invoke(error);
			};

			EditorWebRequestHelper.Request(request: webRequest, onSuccess: onRequestSuccess, onError: onRequestError);
		}

		private bool ParseTokenResponse(string response, out string loginUrl, out string value, ParseParameter checkFor)
		{
			var container = ParseUtils.FromJson<TokenResponse>(response);

			loginUrl = container?.login_url ?? container?.url;
			if (loginUrl == null) {
				XDebug.LogError($"Could not parse response. Response: '{response}'");
				value = null;
				return false;
			}

			var success = ParseUtils.TryGetValueFromUrl(loginUrl, checkFor, out value);
			return success;
		}

		private void EnrichToken(string loginUrl, Action<string> onToken, Action<Error> onError)
		{
			var webRequest = UnityWebRequest.Get(loginUrl);

			//Stop at an exact redirect step to catch token
			webRequest.redirectLimit = 1;

			Action<UnityWebRequest> onRequestDone = request =>
			{
				if (_disposed)
					return;

				if (ParseUtils.TryGetValueFromUrl(request.url, ParseParameter.token, out string enrichedToken))
					onToken?.Invoke(enrichedToken);
				else
					onError?.Invoke(new Error(ErrorType.InvalidToken, errorMessage:"Could not get enriched token"));
			};

			EditorWebRequestHelper.RequestRaw(webRequest, onRequestDone);
		}

		public void RequestMerchantIDs(string token, Action<IntIdContainer[]> onSuccess, Action<Error> onError)
		{
			var webRequest = UnityWebRequest.Get(MERCHANT_GET_URL);
			var requestHeaders = new List<WebRequestHeader>(){WebRequestHeader.AuthHeader(token)};

			Action<string> onRequestSuccess = response =>
			{
				if (_disposed) {return;}
				var container = ParseUtils.FromJson<MerchantIDsResponse>(response);
				onSuccess?.Invoke(container.merchants);
			};

			Action<Error> onRequestError = error =>
			{
				if (!_disposed)
					onError?.Invoke(error);
			};

			EditorWebRequestHelper.Request(request: webRequest, requestHeaders: requestHeaders,
				onSuccess: onRequestSuccess, onError: onRequestError);
		}

		public void RequestProjectIDs(string token, int merchantID, Action<IntIdContainer[]> onSuccess, Action<Error> onError)
		{
			var webRequest = UnityWebRequest.Get(string.Format(PROJECT_GET_URL,merchantID));
			var requestHeaders = new List<WebRequestHeader>(){WebRequestHeader.AuthHeader(token)};

			Action<string> onRequestSuccess = response =>
			{
				if (_disposed) {return;}
				var container = ParseUtils.FromJson<IntIdContainer[]>(response);
				onSuccess?.Invoke(container);
			};

			Action<Error> onRequestError = error =>
			{
				if (!_disposed)
					onError?.Invoke(error);
			};

			EditorWebRequestHelper.Request(request: webRequest, requestHeaders: requestHeaders,
				onSuccess: onRequestSuccess, onError: onRequestError);
		}

		public void RequestLoginIDs(string token, int merchantID, int projectID, Action<StringIdContainer[]> onSuccess, Action<Error> onError)
		{
			var webRequest = UnityWebRequest.Get(string.Format(LOGIN_GET_URL,merchantID,projectID));
			var requestHeaders = new List<WebRequestHeader>(){WebRequestHeader.AuthHeader(token)};

			Action<string> onRequestSuccess = response =>
			{
				if (_disposed) {return;}
				var container = ParseUtils.FromJson<StringIdContainer[]>(response);
				onSuccess?.Invoke(container);
			};

			Action<Error> onRequestError = error =>
			{
				if (!_disposed)
					onError?.Invoke(error);
			};

			EditorWebRequestHelper.Request(request: webRequest, requestHeaders: requestHeaders,
				onSuccess: onRequestSuccess, onError: onRequestError);
		}

		public void RequestOAuthIDs(string token, string loginID, Action<OAuthContainer[]> onSuccess, Action<Error> onError)
		{
			var webRequest = UnityWebRequest.Get(string.Format(OAUTH_GET_URL,loginID));
			var requestHeaders = new List<WebRequestHeader>(){new WebRequestHeader("x-server-authorization",$"Bearer {token}")};

			Action<string> onRequestSuccess = response =>
			{
				if (_disposed) {return;}
				var container = ParseUtils.FromJson<OAuthIDsResponse>(response);
				onSuccess?.Invoke(container.clients);
			};

			Action<Error> onRequestError = error =>
			{
				if (!_disposed)
					onError?.Invoke(error);
			};

			EditorWebRequestHelper.Request(request: webRequest, requestHeaders: requestHeaders,
				onSuccess: onRequestSuccess, onError: onRequestError);
		}

		public void Dispose()
		{
			_disposed = true;
			EditorWebRequestHelper.CancelAllRequests();
		}

		[Serializable]
		public class TokenRequestBody
		{
			public readonly string username;
			public readonly string password;

			public TokenRequestBody(string username, string password)
			{
				this.username = username;
				this.password = password;
			}
		}

		[Serializable]
		public class TokenResponse
		{
			public string login_url;
			public string url;
		}

		[Serializable]
		public class MerchantIDsResponse
		{
			public IntIdContainer[] merchants;
		}

		[Serializable]
		public class OAuthIDsResponse
		{
			public OAuthContainer[] clients;
		}
	}
}