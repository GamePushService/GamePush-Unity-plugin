using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		public void PatchRequest<T, D>(SdkType sdkType, string url, D jsonObject, WebRequestHeader requestHeader, Action<T> onComplete, Action<Error> onError, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			var headers = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PerformPatch(sdkType, url, jsonObject, onComplete, onError, headers, errorsToCheck));
		}

		private IEnumerator PerformPatch<T, D>(SdkType sdkType, string url, D jsonObject, Action<T> onComplete, Action<Error> onError, List<WebRequestHeader> requestHeaders = null, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors) where T : class
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = new UnityWebRequest(url, "PATCH");
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			AttachBodyToRequest(webRequest, jsonObject);
			AttachHeadersToPatchRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private static void AttachHeadersToPatchRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders, bool withContentType = true)
		{
			if (withContentType)
			{
				if (requestHeaders != null)
					requestHeaders.Add(WebRequestHeader.JsonContentTypeHeader());
				else
					requestHeaders = new List<WebRequestHeader> {WebRequestHeader.JsonContentTypeHeader()};
			}

			AttachHeadersToRequest(webRequest, requestHeaders);
		}
	}
}