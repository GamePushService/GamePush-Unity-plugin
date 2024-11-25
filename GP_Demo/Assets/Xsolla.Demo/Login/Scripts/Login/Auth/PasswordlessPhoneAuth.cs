using System;
using System.Text.RegularExpressions;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class PasswordlessPhoneAuth : LoginAuthorization
	{
		private string _currentPhone;
		private string _currentOperationId;
		private ICodeRequester _currentRequester;

		public override void TryAuth(object[] args, Action onSuccess, Action<Error> onError)
		{
			if (!TryExtractArgs(args, out var requester, out var phone))
			{
				XDebug.LogError("PasswordlessPhoneAuth.TryAuth: Could not extract arguments");
				onError?.Invoke(new Error(errorMessage: "Passwordless auth failed"));
				return;
			}

			if (!IsPhoneValid(phone))
			{
				var error = new Error(ErrorType.InvalidData, errorMessage: $"Phone not valid '{phone}'");
				requester?.RaiseOnError(error);
				onError?.Invoke(error);
				return;
			}

			_currentPhone = phone;
			_currentRequester = requester;

			XsollaAuth.StartAuthByPhoneNumber(
				phone,
				data => OnStartAuth(data.operation_id, onSuccess),
				error =>
				{
					requester?.RaiseOnError(error);
					onError?.Invoke(error);
				});
		}

		private static bool TryExtractArgs(object[] args, out ICodeRequester requester, out string phone)
		{
			requester = null;
			phone = null;

			if (args.Length >= 2 && args[0] is ICodeRequester && args[1] is string)
			{
				requester = (ICodeRequester) args[0];
				phone = (string) args[1];
				return true;
			}

			XDebug.LogError("PasswordlessPhoneAuth: Could not extract arguments");
			return false;
		}

		private static bool IsPhoneValid(string phone)
		{
			var regex = new Regex("^\\+(\\d){5,25}$");
			var valid = regex.IsMatch(phone);

			if (!valid)
				XDebug.LogError($"Phone not valid: '{phone}'");

			return valid;
		}

		private void OnStartAuth(string operationId, Action onSuccess)
		{
			_currentOperationId = operationId;
			_currentRequester.RequestCode(code => OnCodeReceived(code, onSuccess));
		}

		private void OnCodeReceived(string code, Action onSuccess)
		{
			XsollaAuth.CompleteAuthByPhoneNumber(
				_currentPhone,
				code,
				_currentOperationId,
				onSuccess,
				OnCodeError
			);
		}

		private void OnCodeError(Error error)
		{
			_currentRequester.RaiseOnError(error);
		}
	}
}