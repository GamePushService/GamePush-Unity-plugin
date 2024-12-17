using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper : MonoBehaviour
	{
		private static WebRequestHelper _instance;

		public static WebRequestHelper Instance
		{
			get
			{
				if (_instance == null)
					_instance = new GameObject("WebRequestHelper").AddComponent<WebRequestHelper>();

				return _instance;
			}
		}

		private readonly List<UnityWebRequest> Requests = new List<UnityWebRequest>();

		public bool IsBusy()
		{
			return Requests.Count > 0;
		}

		private void Awake()
		{
			DontDestroyOnLoad(gameObject);
		}

		private void OnDestroy()
		{
			Requests.ForEach(r => r.Dispose());
			Requests.Clear();

			StopAllCoroutines();
		}

		private IEnumerator InternalPerformWebRequest(UnityWebRequest webRequest, Action onComplete)
		{
			webRequest.disposeDownloadHandlerOnDispose = true;
			webRequest.disposeUploadHandlerOnDispose = true;
			webRequest.disposeCertificateHandlerOnDispose = true;
			Requests.Add(webRequest);

			yield return StartCoroutine(SendWebRequest(webRequest));
			onComplete?.Invoke();

			Requests.Remove(webRequest);
		}

		private static IEnumerator SendWebRequest(UnityWebRequest webRequest)
		{
#if UNITY_2018_1_OR_NEWER
			yield return webRequest.SendWebRequest();
#else
			yield return webRequest.Send();
#endif
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			yield return InternalPerformWebRequest(
				webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<int> onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			yield return InternalPerformWebRequest(
				webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<UnityWebRequest> onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			yield return InternalPerformWebRequest(
				webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<string> onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			yield return InternalPerformWebRequest(
				webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformWebRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, ErrorGroup errorsToCheck) where T : class
		{
			yield return InternalPerformWebRequest(
				webRequest,
				() => ProcessRequest(webRequest, onComplete, onError, errorsToCheck));
		}

		private IEnumerator PerformWebRequest(UnityWebRequest webRequest, Action<Texture2D> onComplete, Action<Error> onError)
		{
			yield return InternalPerformWebRequest(
				webRequest,
				() => ProcessRequest(webRequest, onComplete, onError));
		}

		public static void AttachHeadersToRequest(UnityWebRequest webRequest, List<WebRequestHeader> requestHeaders)
		{
			if (requestHeaders == null)
				return;

			foreach (var header in requestHeaders.Where(header => header != null))
			{
				webRequest.SetRequestHeader(header.Name, header.Value);
			}
		}

		public static void AttachBodyToRequest(UnityWebRequest webRequest, object data)
		{
			if (data == null)
				return;

			var json = ParseUtils.ToJson(data).Replace('\n', ' ');
			var body = new UTF8Encoding().GetBytes(json);
			webRequest.uploadHandler = new UploadHandlerRaw(body);
		}
	}
}