using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		public void PutRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PutRequestCor(sdkType, url, headers, onComplete, onError, errorsToCheck));
		}

		public void PutRequest<T>(SdkType sdkType, string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PutRequest<T>(SdkType sdkType, string url, T jsonObject, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PutRequest<T, D>(SdkType sdkType, string url, D jsonObject, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where D : class where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PutRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PutRequestCor(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = new UnityWebRequest(url, "PUT") {
				downloadHandler = new DownloadHandlerBuffer()
			};

			AttachHeadersToPutRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PutRequestCor<T>(SdkType sdkType, string url, T jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = new UnityWebRequest(url, "PUT") {
				downloadHandler = new DownloadHandlerBuffer()
			};

			AttachBodyToRequest(webRequest, jsonObject);
			AttachHeadersToPutRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PutRequestCor<T, D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class where D : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = new UnityWebRequest(url, "PUT") {
				downloadHandler = new DownloadHandlerBuffer()
			};

			AttachBodyToRequest(webRequest, jsonObject);
			AttachHeadersToPutRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private void AttachHeadersToPutRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders)
		{
			if (requestHeaders != null)
				requestHeaders.Add(WebRequestHeader.JsonContentTypeHeader());
			else
				requestHeaders = new List<WebRequestHeader>() {WebRequestHeader.JsonContentTypeHeader()};

			AttachHeadersToRequest(webRequest, requestHeaders);
		}
	}
}