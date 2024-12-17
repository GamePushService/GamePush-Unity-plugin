using System;

namespace Xsolla.Core.Centrifugo
{
	[Serializable]
	internal class ConnectionMessage
	{
		public ConnectCommand connect;
		public int id;
	}

	[Serializable]
	internal class ConnectCommand
	{
		public ConnectionData data;
	}

	[Serializable]
	internal class ConnectionData
	{
		public string auth;
		public int project_id;
	}
}