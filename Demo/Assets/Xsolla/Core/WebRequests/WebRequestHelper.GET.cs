using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		public void GetRequest<T>(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action<T> onComplete, Action<Error> onError, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeaders);
			StartCoroutine(PerformGet(sdkType, url, onComplete, onError, headers, errorsToCheck));
		}

		public void GetRequest<T>(SdkType sdkType, string url, WebRequestHeader requestHeader, Action<T> onComplete, Action<Error> onError, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PerformGet(sdkType, url, onComplete, onError, headers, errorsToCheck));
		}

		public void GetRequest<T>(SdkType sdkType, string url, Action<T> onComplete, Action<Error> onError, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			var headers = GetAnalyticHeaders(sdkType);
			StartCoroutine(PerformGet<T>(sdkType, url, onComplete, onError, headers, errorsToCheck));
		}

		public void GetRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete, Action<Error> onError, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PerformGet(sdkType, url, onComplete, onError, headers, errorsToCheck));
		}

		private IEnumerator PerformGet<T>(SdkType sdkType, string url, Action<T> onComplete, Action<Error> onError, List<WebRequestHeader> requestHeaders, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = UnityWebRequest.Get(url);
			AttachHeadersToRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformGet(SdkType sdkType, string url, Action onComplete, Action<Error> onError, List<WebRequestHeader> requestHeaders, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = UnityWebRequest.Get(url);
			AttachHeadersToRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
	}
}