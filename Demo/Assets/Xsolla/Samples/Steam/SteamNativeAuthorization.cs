using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;

namespace Xsolla.Samples.Steam
{
	public class SteamNativeAuthorization : MonoBehaviour
	{
		private void Start()
		{
			// Get the steam session ticket from `SteamUtils` class
			var steamSessionTicket = SteamUtils.GetSteamSessionTicket();

			// Start silent authentication
			// Pass `steam` as `providerName` parameter
			// Pass your `Steam App ID` as `appId` parameter. We use `480` as an example
			// Pass `steamSessionTicket` variable as the `sessionTicket` parameter
			// Pass callback functions for success and error cases
			XsollaAuth.SilentAuth("steam", "480", steamSessionTicket, OnSuccess, OnError);
		}

		private void OnSuccess()
		{
			Debug.Log("Authorization successful");
			// Add actions taken in case of success
		}

		private void OnError(Error error)
		{
			Debug.LogError($"Authorization failed. Error: {error.errorMessage}");
			// Add actions taken in case of error
		}
	}
}
