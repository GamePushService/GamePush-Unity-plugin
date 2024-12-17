using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Xsolla.Core
{
	public static class SteamUtils
	{
		/// <summary>
		/// This ticket you can change for Login JWT and use it in Xsolla Store
		/// </summary>
		public static string GetSteamSessionTicket()
		{
			var ticket = new SteamSessionTicket().ToString();
			if (!string.IsNullOrEmpty(ticket))
				XDebug.Log($"Requested steam session ticket: {ticket}");
			else
				XDebug.LogError("Requested steam session ticket is null. Please check your Steam settings.");

			return ticket;
		}

		public static Dictionary<string, string> GetAdditionalCustomHeaders()
		{
			return new Dictionary<string, string> {{"x-steam-userid", GetSteamUserId()}};
		}

		private static string GetSteamUserId()
		{
			var encodedToken = XsollaToken.AccessToken;
			if (string.IsNullOrEmpty(encodedToken))
				throw new Exception("Token is empty");

			var tokenParts = encodedToken.Split('.');
			if (tokenParts.Length < 3)
				throw new Exception("Token must contain header, payload and signature. " +
				                    $"Your token parts count was '{tokenParts.Length}'. " +
				                    $"Your token: {encodedToken}");

			var payload = ParsePayload(tokenParts[1]);
			if (!payload.is_cross_auth)
				throw new Exception("Token must not be cross auth. " +
				                    $"Your token: {encodedToken}");

			var steamUserUrl = payload.id;
			if (string.IsNullOrEmpty(steamUserUrl))
				throw new Exception("Token must have 'id' parameter. " +
				                    $"Your token: {encodedToken}");

			return steamUserUrl.Split('/').Last();
		}

		private static SteamTokenPayload ParsePayload(string encodedPayload)
		{
			//Fix FromBase64String conversion
			encodedPayload = encodedPayload
				.Replace('-', '+')
				.Replace('_', '/');

			var padding = encodedPayload.Length % 4;
			if (padding != 0)
			{
				var paddingToAdd = 4 - padding;
				encodedPayload += new string('=', paddingToAdd);
			}

			var bytes = Convert.FromBase64String(encodedPayload);
			var json = Encoding.UTF8.GetString(bytes);
			return ParseUtils.FromJson<SteamTokenPayload>(json);
		}
	}
}