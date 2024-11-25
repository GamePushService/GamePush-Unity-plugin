using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core.Editor.AutoFillSettings
{
	public class EditorWebRequestHelper : MonoBehaviour
	{
		private static readonly List<RequestContainer> _requestQueue = new List<RequestContainer>();

		public static void Request(UnityWebRequest request,
			object requestBody = null, List<WebRequestHeader> requestHeaders = null,
			Action<string> onSuccess = null, Action<Error> onError = null)
		{
			if (requestBody != null)
				WebRequestHelper.AttachBodyToRequest(request,requestBody);

			if (requestHeaders != null)
				WebRequestHelper.AttachHeadersToRequest(request,requestHeaders);

			var container = new RegularContainer(request, onSuccess, onError);
			StartAndEnqueue(container);
		}

		public static void RequestRaw(UnityWebRequest request, Action<UnityWebRequest> doneCallback)
		{
			StartAndEnqueue (new RawContainer(request, doneCallback));
		}

		private static void StartAndEnqueue(RequestContainer containedRequest)
		{
			containedRequest.request.disposeCertificateHandlerOnDispose = true;
			containedRequest.request.disposeDownloadHandlerOnDispose = true;
			containedRequest.request.disposeUploadHandlerOnDispose = true;
			containedRequest.request.SendWebRequest();

			_requestQueue.Add(containedRequest);
			if (_requestQueue.Count <= 1)
				EditorApplication.update += OnEditorUpdate;
		}

		public static bool CancelAllRequests()
		{
			if (_requestQueue.Count == 0)
				return false;

			EditorApplication.update -= OnEditorUpdate;

			for (int i = 0; i < _requestQueue.Count; i++) {
				_requestQueue[i]?.request?.Abort();
				DisposeRequest(_requestQueue[i]);
			}

			_requestQueue.Clear();
			return true;
		}

		private static void OnEditorUpdate()
		{
			var index = 0;
			while (index < _requestQueue.Count)
			{
				var cur = _requestQueue[index];
				if (!cur.request.isDone) {
					index++;
					continue;
				}

				if (cur is RegularContainer regularContainer)
				{
					if (WebRequestHelper.CheckNoErrors(cur.request, ErrorGroup.CommonErrors, out Error error))
						regularContainer.successCallback?.Invoke(cur.request.downloadHandler.text);
					else
						regularContainer.errorCallback?.Invoke(error);
				}
				else if (cur is RawContainer rawContainer)
				{
					rawContainer.doneCallback?.Invoke(rawContainer.request);
				}

				_requestQueue.RemoveAt(index);
				DisposeRequest(cur);
			}

			if (_requestQueue.Count == 0)
				EditorApplication.update -= OnEditorUpdate;
		}

		private static void DisposeRequest(RequestContainer container)
		{
			container.request.Dispose();
			container.request = null;

			if (container is RegularContainer regularContainer)
			{
				regularContainer.successCallback = null;
				regularContainer.errorCallback = null;
			}
			else if (container is RawContainer rawContainer)
			{
				rawContainer.doneCallback = null;
			}

			container = null;
		}

		private abstract class RequestContainer
		{
			public UnityWebRequest request;

			protected RequestContainer(UnityWebRequest request)
			{
				this.request = request;
			}
		}

		private class RegularContainer : RequestContainer
		{
			public Action<string> successCallback;
			public Action<Error> errorCallback;
			public RegularContainer(UnityWebRequest request, Action<string> successCallback, Action<Error> errorCallback) : base (request)
			{
				this.successCallback = successCallback;
				this.errorCallback = errorCallback;
			}
		}

		private class RawContainer : RequestContainer
		{
			public Action<UnityWebRequest> doneCallback;

			public RawContainer(UnityWebRequest request, Action<UnityWebRequest> doneCallback) : base (request)
			{
				this.doneCallback = doneCallback;
			}
		}
	}
}