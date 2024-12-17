using System;
using System.Text;

namespace Xsolla.Core
{
	[Serializable]
	public class Error
	{
		public string statusCode;
		public string errorCode;
		public string errorMessage;

		public ErrorType ErrorType { get; set; }

		public Error(ErrorType errorType = ErrorType.UnknownError, string statusCode = "", string errorCode = "", string errorMessage = "")
		{
			ErrorType = errorType;
			this.statusCode = statusCode;
			this.errorCode = errorCode;
			this.errorMessage = errorMessage;
		}

		public static Error UnknownError => new Error();

		public override string ToString()
		{
			var builder = new StringBuilder($"Error: {ErrorType}.");

			if (!string.IsNullOrEmpty(statusCode))
				builder.Append($" Status code: {statusCode}.");

			if (!string.IsNullOrEmpty(errorCode))
				builder.Append($" Error code: {errorCode}.");

			if (!string.IsNullOrEmpty(errorMessage))
				builder.Append($" Message: {errorMessage}.");

			return builder.ToString();
		}
	}
}