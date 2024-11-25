using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class PasswordlessEmailAuth : LoginAuthorization
	{
		private string _currentEmail;
		private string _currentOperationId;
		private ICodeRequester _currentRequester;

		public override void TryAuth(object[] args, Action onSuccess, Action<Error> onError)
		{
			if (!TryExtractArgs(args, out var requester, out var email))
			{
				XDebug.LogError("PasswordlessEmailAuth.TryAuth: Could not extract arguments");
				onError?.Invoke(new Error(errorMessage: "Passwordless auth failed"));
				return;
			}

			if (!IsEmailValid(email))
			{
				var error = new Error(ErrorType.InvalidData, errorMessage: $"Email not valid '{email}'");
				requester?.RaiseOnError(error);
				onError?.Invoke(error);
				return;
			}

			_currentEmail = email;
			_currentRequester = requester;

			XsollaAuth.StartAuthByEmail(
				email,
				data => OnStartAuth(data.operation_id, onSuccess),
				error =>
				{
					requester?.RaiseOnError(error);
					onError?.Invoke(error);
				});
		}

		private static bool TryExtractArgs(object[] args, out ICodeRequester requester, out string email)
		{
			requester = null;
			email = null;

			if (args.Length >= 2 && args[0] is ICodeRequester && args[1] is string)
			{
				requester = (ICodeRequester) args[0];
				email = (string) args[1];
				return true;
			}

			XDebug.LogError("PasswordlessEmailAuth: Could not extract arguments");
			return false;
		}

		private static bool IsEmailValid(string email)
		{
			return email.Length > 1 && email.Length < 255;
		}

		private void OnStartAuth(string operationId, Action onSuccess)
		{
			_currentOperationId = operationId;
			_currentRequester.RequestCode(code => OnCodeReceived(code, onSuccess));
		}

		private void OnCodeReceived(string code, Action onSuccess)
		{
			XsollaAuth.CompleteAuthByEmail(
				_currentEmail,
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