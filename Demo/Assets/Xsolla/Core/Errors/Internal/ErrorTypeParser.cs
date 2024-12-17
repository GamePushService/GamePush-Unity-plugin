using System;

namespace Xsolla.Core
{
	internal static class ErrorTypeParser
	{
		public static bool TryGetSpecificType(string code, ErrorGroup errorGroup, out ErrorType errorType)
		{
			if (errorGroup == ErrorGroup.CommonErrors)
				return TryGetCommonType(code, out errorType);

			errorType = ErrorType.Undefined;

			var typeParser = GetErrorTypeParser(errorGroup);
			if (typeParser != null)
				errorType = typeParser(code);

			return errorType != ErrorType.Undefined;
		}

		public static bool TryGetCommonType(string code, out ErrorType errorType)
		{
			errorType = CommonErrors(code);
			return errorType != ErrorType.Undefined;
		}

		private static Func<string, ErrorType> GetErrorTypeParser(ErrorGroup errorGroup)
		{
			switch (errorGroup)
			{
				case ErrorGroup.TokenErrors: return TokenErrors;
				case ErrorGroup.LoginErrors: return LoginErrors;
				case ErrorGroup.RegistrationErrors: return RegistrationErrors;
				case ErrorGroup.ResetPasswordErrors: return ResetPasswordErrors;
				case ErrorGroup.ItemsListErrors: return ItemsListErrors;
				case ErrorGroup.ConsumeItemErrors: return ConsumeItemErrors;
				case ErrorGroup.BuyItemErrors: return BuyItemErrors;
				case ErrorGroup.CartErrors: return CartErrors;
				case ErrorGroup.BuyCartErrors: return BuyCartErrors;
				case ErrorGroup.OrderStatusErrors: return OrderStatusErrors;
				case ErrorGroup.CouponErrors: return CouponErrors;
			}

			return null;
		}

		private static ErrorType TokenErrors(string code)
		{
			switch (code)
			{
				case "422": return ErrorType.TokenVerificationException;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType LoginErrors(string code)
		{
			switch (code)
			{
				case "003-007": return ErrorType.UserIsNotActivated;
				case "010-007": return ErrorType.CaptchaRequiredException;
				case "003-039": return ErrorType.InvalidAuthorizationCode;
				case "003-049": return ErrorType.ExceededAuthorizationCodeAttempts;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType RegistrationErrors(string code)
		{
			switch (code)
			{
				case "010-003": return ErrorType.RegistrationNotAllowedException;
				case "003-003": return ErrorType.UsernameIsTaken;
				case "003-004": return ErrorType.EmailIsTaken;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType ResetPasswordErrors(string code)
		{
			switch (code)
			{
				case "003-007":
				case "003-024": return ErrorType.PasswordResetNotAllowedForProject;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType ItemsListErrors(string code)
		{
			switch (code)
			{
				case "401": return ErrorType.InvalidToken;
				case "422": return ErrorType.InvalidData;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType ConsumeItemErrors(string code)
		{
			switch (code)
			{
				case "401": return ErrorType.InvalidToken;
				case "422": return ErrorType.InvalidData;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType BuyItemErrors(string code)
		{
			switch (code)
			{
				case "401": return ErrorType.InvalidToken;
				case "404": return ErrorType.ProductDoesNotExist;
				case "422": return ErrorType.PayStationServiceException;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType CartErrors(string code)
		{
			switch (code)
			{
				case "401":
				case "403": return ErrorType.InvalidToken;
				case "404": return ErrorType.UserNotFound;
				case "422": return ErrorType.InvalidData;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType BuyCartErrors(string code)
		{
			switch (code)
			{
				case "422": return ErrorType.CartNotFound;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType OrderStatusErrors(string code)
		{
			switch (code)
			{
				case "401": return ErrorType.InvalidToken;
				case "404": return ErrorType.OrderNotFound;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType CouponErrors(string code)
		{
			switch (code)
			{
				case "401": return ErrorType.InvalidToken;
				case "403": return ErrorType.AuthorizationHeaderNotSent;
				case "404": return ErrorType.InvalidCoupon;
				case "422": return ErrorType.InvalidData;
			}

			return ErrorType.Undefined;
		}

		private static ErrorType CommonErrors(string code)
		{
			switch (code)
			{
				case "403": return ErrorType.InvalidToken;
				case "010-023": return ErrorType.InvalidToken;
				case "010-017": return ErrorType.InvalidToken;
				case "405": return ErrorType.MethodIsNotAllowed;
				case "0": return ErrorType.InvalidProjectSettings;
				case "003-061": return ErrorType.InvalidProjectSettings;
				case "003-001": return ErrorType.InvalidLoginOrPassword;
				case "010-011": return ErrorType.MultipleLoginUrlsException;
				case "010-012": return ErrorType.SubmittedLoginUrlNotFoundException;
			}

			return ErrorType.Undefined;
		}
	}
}