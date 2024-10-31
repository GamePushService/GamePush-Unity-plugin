using System;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace Xsolla.Core
{
	public partial class WebRequestHelper
	{
		private static void ProcessRequest(UnityWebRequest webRequest, Action onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
				onComplete?.Invoke();
			else
				onError?.Invoke(error);

			webRequest.Dispose();
		}

		private static void ProcessRequest(UnityWebRequest webRequest, Action<int> onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
				onComplete?.Invoke((int) webRequest.responseCode);
			else
				onError?.Invoke(error);

			webRequest.Dispose();
		}

		private static void ProcessRequest(UnityWebRequest webRequest, Action<UnityWebRequest> onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
				onComplete?.Invoke(webRequest);
			else
				onError?.Invoke(error);

			webRequest.Dispose();
		}

		private static void ProcessRequest(UnityWebRequest webRequest, Action<string> onComplete, Action<Error> onError, ErrorGroup errorsToCheck)
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
			{
				var data = webRequest.downloadHandler.text;
				if (data != null)
					onComplete?.Invoke(data);
				else
					onError?.Invoke(Error.UnknownError);
			}
			else
				onError?.Invoke(error);

			webRequest.Dispose();
		}

		private static void ProcessRequest(UnityWebRequest webRequest, Action<Texture2D> onComplete, Action<Error> onError)
		{
			if (CheckNoErrors(webRequest, ErrorGroup.CommonErrors, out var error, false))
			{
				var texture = ((DownloadHandlerTexture) webRequest.downloadHandler).texture;
				if (texture != null)
					onComplete?.Invoke(texture);
				else
					onError?.Invoke(Error.UnknownError);
			}
			else
				onError?.Invoke(error);

			webRequest.Dispose();
		}

		private static void ProcessRequest<T>(UnityWebRequest webRequest, Action<T> onComplete, Action<Error> onError, ErrorGroup errorsToCheck) where T : class
		{
			if (CheckNoErrors(webRequest, errorsToCheck, out var error))
			{
				var rawData = webRequest.downloadHandler.text;
				var data = rawData != null ? ParseUtils.FromJson<T>(rawData) : null;
				if (data != null)
					onComplete?.Invoke(data);
				else
					onError?.Invoke(Error.UnknownError);
			}
			else
				onError?.Invoke(error);

			webRequest.Dispose();
		}

		public static bool CheckNoErrors(UnityWebRequest webRequest, ErrorGroup errorsToCheck, out Error error, bool log = true)
		{
#if UNITY_2020_1_OR_NEWER
			var isNetworkError = webRequest.result == UnityWebRequest.Result.ConnectionError;
			var isHttpError = webRequest.result == UnityWebRequest.Result.ProtocolError;
#else
			var isNetworkError = webRequest.isNetworkError;
			var isHttpError = webRequest.isHttpError;
#endif
			if (isNetworkError)
			{
				error = new Error(ErrorType.NetworkError);
				return false;
			}

			var data = webRequest.downloadHandler.text;

			if (log)
			{
				var logMessage = $"WebRequest: {webRequest.url}";

				var header = webRequest.GetResponseHeader("Authorization");
				if (!string.IsNullOrEmpty(header))
					logMessage += $"\n\nAuthorization: {header}";

				var uploadBytes = webRequest.uploadHandler?.data;
				if (uploadBytes != null)
				{
					var dataJson = Encoding.UTF8.GetString(uploadBytes);
					logMessage += $"\n\nRequest: {dataJson}";
				}

				if (!string.IsNullOrEmpty(data))
				{
					logMessage += $"\n\nResponse: {data}";
				}

				XDebug.Log($"{logMessage}\n");
			}

			if (ParseUtils.TryParseError(data, out error))
			{
				if (ErrorTypeParser.TryGetSpecificType(error.statusCode, errorsToCheck, out var specificErrorType))
					error.ErrorType = specificErrorType;
				else if (ErrorTypeParser.TryGetCommonType(error.statusCode, out var commonErrorType))
					error.ErrorType = commonErrorType;

				return false;
			}

			if (isHttpError)
			{
				error = Error.UnknownError;
				return false;
			}

			error = null;
			return true;
		}
	}
}