using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		#region PostRequest<T,D>

		public void PostRequest<T, D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
			where D : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T, D>(SdkType sdkType, string url, D jsonObject, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
			where D : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T, D>(SdkType sdkType, string url, D jsonObject, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
			where D : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		#endregion

		#region PostRequest<T>

		public void PostRequest<T>(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject: null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject: null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor<T>(sdkType, url, jsonObject: null, headers, onComplete, onError, errorsToCheck));
		}

		#endregion

		#region PostRequest<D> with jsonObject

		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where D : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where D : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject = null, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where D : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject = null, Action<int> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where D : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<D>(SdkType sdkType, string url, D jsonObject = null, Action<UnityWebRequest> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where D : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor(sdkType, url, jsonObject, headers, onComplete, onError, errorsToCheck));
		}

		#endregion

		#region PostRequest

		public void PostRequest(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PostRequestCor(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest(SdkType sdkType, string url, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor(sdkType, url, null, headers, onComplete, onError, errorsToCheck));
		}

		#endregion

		#region PostRquest<T> with WWWForm

		public void PostRequest<T>(SdkType sdkType, string url, WWWForm data, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PostRequestCor<T>(sdkType, url, data, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, WWWForm data, WebRequestHeader requestHeader, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PostRequestCor<T>(sdkType, url, data, headers, onComplete, onError, errorsToCheck));
		}

		public void PostRequest<T>(SdkType sdkType, string url, WWWForm data, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PostRequestCor<T>(sdkType, url, data, headers, onComplete, onError, errorsToCheck));
		}

		#endregion

		#region PostUploadRequest

		public void PostUploadRequest<T>(SdkType sdkType, string url, string pathToFile, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
			where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PostUploadRequestCor(sdkType, url, pathToFile, headers, onComplete, onError, errorsToCheck));
		}

		public void PostUploadRequest(SdkType sdkType, string url, byte[] fileData, List<WebRequestHeader> requestHeaders, Action<string> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PostUploadRequestCor(sdkType, url, fileData, headers, onComplete, onError, errorsToCheck));
		}

		#endregion

		private IEnumerator PostRequestCor(SdkType sdkType, string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			var webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PostRequestCor(SdkType sdkType, string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action<int> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			var webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PostRequestCor(SdkType sdkType, string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action<UnityWebRequest> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			var webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PostRequestCor<T>(SdkType sdkType, string url, object jsonObject, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			var webRequest = PreparePostWebRequest(url, jsonObject, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PostUploadRequestCor<T>(SdkType sdkType, string url, string pathToFile, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			var webRequest = PreparePostUploadRequest(url, pathToFile, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PostUploadRequestCor(SdkType sdkType, string url, byte[] fileData, List<WebRequestHeader> requestHeaders, Action<string> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);
#if UNITY_2022_2_OR_NEWER
			UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST);
#else
			var webRequest = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
#endif

			//Timeout was set to 10 seconds for the unknown reason. I've changed it to 30 because of one specific request that can take up to 10-30 seconds.
			//Also considered this recommendation from a UnityTech employee:
			//https://forum.unity.com/threads/sendwebrequest-curl-timeout-error.854812/
			webRequest.timeout = 30;

			webRequest.uploadHandler = (UploadHandler) new UploadHandlerRaw(fileData);
			AttachHeadersToPostRequest(webRequest, requestHeaders, false);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PostRequestCor<T>(SdkType sdkType, string url, WWWForm data, List<WebRequestHeader> requestHeaders, Action<T> onComplete = null, Action<Error> onError = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);
			var webRequest = UnityWebRequest.Post(url, data);

			AttachHeadersToPostRequest(webRequest, requestHeaders, false);

			yield return StartCoroutine(PerformWebRequest<T>(webRequest, onComplete, onError, errorsToCheck));
		}

		private UnityWebRequest PreparePostWebRequest(string url, object jsonObject, List<WebRequestHeader> requestHeaders)
		{
#if UNITY_2022_2_OR_NEWER
			UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST);
#else
			var webRequest = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
#endif
			webRequest.timeout = 30;

			AttachBodyToRequest(webRequest, jsonObject);
			AttachHeadersToPostRequest(webRequest, requestHeaders);

			return webRequest;
		}

		private UnityWebRequest PreparePostUploadRequest(string url, string pathToFile, List<WebRequestHeader> requestHeaders)
		{
#if UNITY_2022_2_OR_NEWER
			UnityWebRequest webRequest = UnityWebRequest.PostWwwForm(url, UnityWebRequest.kHttpVerbPOST);
#else
			var webRequest = UnityWebRequest.Post(url, UnityWebRequest.kHttpVerbPOST);
#endif
			webRequest.timeout = 30;

			AttachFileToUploadRequest(webRequest, pathToFile);
			AttachHeadersToPostRequest(webRequest, requestHeaders);

			return webRequest;
		}

		private void AttachFileToUploadRequest(UnityWebRequest webRequest, string pathToFile)
		{
			if (!string.IsNullOrEmpty(pathToFile))
			{
				webRequest.uploadHandler = new UploadHandlerFile(pathToFile);
			}
		}

		private void AttachHeadersToPostRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders, bool withContentType = true)
		{
			if (withContentType)
			{
				if (requestHeaders != null)
					requestHeaders.Add(WebRequestHeader.JsonContentTypeHeader());
				else
					requestHeaders = new List<WebRequestHeader>() {WebRequestHeader.JsonContentTypeHeader()};
			}

			AttachHeadersToRequest(webRequest, requestHeaders);
		}
	}
}