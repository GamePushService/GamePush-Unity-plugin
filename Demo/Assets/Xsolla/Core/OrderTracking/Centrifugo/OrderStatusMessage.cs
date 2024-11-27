using System;

namespace Xsolla.Core.Centrifugo
{
	[Serializable]
	internal class OrderStatusMessage
	{
		public OrderStatusPush push;
	}

	[Serializable]
	internal class OrderStatusPush
	{
		public OrderStatusPub pub;
		public string channel;
	}

	[Serializable]
	internal class OrderStatusPub
	{
		public OrderStatusData data;
		public int offset;
	}

	[Serializable]
	internal class OrderStatusData
	{
		public int order_id;
		public string status;
	}
}