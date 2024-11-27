using System;

namespace Xsolla.Core
{
	[Serializable]
	internal class LoginError
	{
		public Data error;

		public Error ToError()
		{
			if (error == null)
				return null;

			return new Error {
				statusCode = error.code,
				errorCode = error.code,
				errorMessage = error.description
			};
		}

		[Serializable]
		public class Data
		{
			public string code;
			public string description;
		}
	}
}