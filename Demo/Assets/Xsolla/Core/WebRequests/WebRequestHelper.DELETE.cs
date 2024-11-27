using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		public void DeleteRequest(SdkType sdkType, string url, WebRequestHeader requestHeader, Action onComplete, Action<Error> onError, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			var requestHeaders = AppendAnalyticHeaders(sdkType, requestHeader);
			StartCoroutine(PerformDelete(sdkType, url, requestHeaders, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformDelete(SdkType sdkType, string url, List<WebRequestHeader> requestHeaders, Action onComplete, Action<Error> onError, ErrorGroup errorsToCheck = ErrorGroup.CommonErrors)
		{
			url = AppendAnalyticsToUrl(sdkType, url);

			var webRequest = UnityWebRequest.Delete(url);
			webRequest.downloadHandler = new DownloadHandlerBuffer();
			AttachHeadersToRequest(webRequest, requestHeaders);

			yield return StartCoroutine(PerformWebRequest(webRequest, onComplete, onError, errorsToCheck));
		}
	}
}