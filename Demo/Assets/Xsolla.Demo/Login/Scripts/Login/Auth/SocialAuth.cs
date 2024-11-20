using System;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class SocialAuth : LoginAuthorization
	{
		public override void TryAuth(object[] args, Action onSuccess, Action<Error> onError)
		{
			if (!TryExtractProvider(args, out var socialProvider))
			{
				onError?.Invoke(new Error(errorMessage: $"{GetType().Name}. Auth failed. Can't extract provider"));
				return;
			}

			XsollaAuth.AuthViaSocialNetwork(
				socialProvider,
				onSuccess,
				onError,
				() => onError?.Invoke(null));
		}

		private static bool TryExtractProvider(object[] args, out SocialProvider provider)
		{
			provider = default;

			if (args == null)
			{
				XDebug.LogError("AndroidSocialAuth.TryExtractProvider: 'object[] args' was null");
				return false;
			}

			if (args.Length != 1)
			{
				XDebug.LogError($"AndroidSocialAuth.TryExtractProvider: args.Length expected 1, was {args.Length}");
				return false;
			}

			try
			{
				provider = (SocialProvider) args[0];
			}
			catch (Exception ex)
			{
				XDebug.LogError($"AndroidSocialAuth.TryExtractProvider: Error during argument extraction: {ex.Message}");
				return false;
			}

			return true;
		}
	}
}