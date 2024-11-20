using System;
using System.Linq;
using System.Text;

namespace Xsolla.Core
{
	internal class SteamSessionTicket
	{
		private static byte[] RefreshTicket()
		{
			return Initialized()
				? GetTicketData()
				: Array.Empty<byte>();
		}

		private static bool Initialized()
		{
			bool result;
			try
			{
				result = SteamManager.Initialized;
			}
			catch (Exception e)
			{
				XDebug.LogError("Steam initialization error. " + e.Message);
				result = false;
			}

			return result;
		}

		private static byte[] GetTicketData()
		{
			var ticket = new byte[1024];
			try
			{
#if UNITY_STANDALONE
				Steamworks.SteamUser.GetAuthSessionTicket(ticket, 1024, out var length);
				Array.Resize(ref ticket, (int) length);
#else
				ticket = new byte[0];
#endif
			}
			catch (Exception e)
			{
				XDebug.LogError("Get steam session ticket exception: " + e.Message);
				ticket = Array.Empty<byte>();
			}

			return ticket;
		}

		private static string ConvertTicket(byte[] ticket)
		{
			var stringBuilder = new StringBuilder();
			ticket.ToList().ForEach(b => stringBuilder.AppendFormat("{0:x2}", b));
			return stringBuilder.ToString();
		}

		public override string ToString()
		{
			var ticketData = RefreshTicket();
			return ConvertTicket(ticketData);
		}
	}
}