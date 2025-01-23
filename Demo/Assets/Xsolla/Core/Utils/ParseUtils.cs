using System;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace Xsolla.Core
{
	public static class ParseUtils
	{
		private static readonly JsonSerializerSettings serializerSettings;

		static ParseUtils()
		{
			serializerSettings = new JsonSerializerSettings {
				NullValueHandling = NullValueHandling.Ignore
			};
		}

		public static string ToJson<TData>(TData data) where TData : class
		{
			return JsonConvert.SerializeObject(data, serializerSettings);
		}

		public static TData FromJson<TData>(string json) where TData : class
		{
			TData result;

			try
			{
				result = JsonConvert.DeserializeObject<TData>(json, serializerSettings);
			}
			catch (Exception)
			{
				XDebug.LogWarning($"Deserialization failed for {typeof(TData)}");
				result = null;
			}

			return result;
		}

		private static Error ParseError(string json)
		{
			if (json.Contains("statusCode") && json.Contains("errorCode") && json.Contains("errorMessage"))
				return FromJson<Error>(json);

			if (json.Contains("error") && json.Contains("code") && json.Contains("description"))
				return FromJson<LoginError>(json).ToError();

			return null;
		}

		public static bool TryParseError(string json, out Error error)
		{
			if (string.IsNullOrEmpty(json))
			{
				error = null;
				return false;
			}

			try
			{
				error = ParseError(json);
				return error != null;
			}
			catch (Exception ex)
			{
				error = new Error(ErrorType.InvalidData, errorMessage: ex.Message);
				return true;
			}
		}

		public static bool TryGetValueFromUrl(string url, ParseParameter parameter, out string value)
		{
			var parameterName = parameter.ToString();
			var regex = new Regex($"[&?]{parameterName}=[a-zA-Z0-9._+-]+");
			value = regex.Match(url)
				.Value
				.Replace($"{parameterName}=", string.Empty)
				.Replace("&", string.Empty)
				.Replace("?", string.Empty);

			switch (parameter)
			{
				case ParseParameter.error_code:
				case ParseParameter.error_description:
					value = value?.Replace("+", " ");
					break;
				default:
					XDebug.Log($"Trying to find {parameterName} in URL:{url}");
					break;
			}

			return !string.IsNullOrEmpty(value);
		}
	}
}