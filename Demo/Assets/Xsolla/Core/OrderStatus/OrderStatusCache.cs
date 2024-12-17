using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Core
{
	internal static class OrderStatusCache
	{
		private static readonly Dictionary<int, OrderStatus> Items = new Dictionary<int, OrderStatus>();

		public static bool TryPerform(int orderId, Action<OrderStatus> callback)
		{
			if (orderId <= 0)
				throw new ArgumentException("Order id must be positive number", nameof(orderId));

			var status = GetCompleted(orderId);
			if (status == null)
				return false;

			CoroutinesExecutor.Run(PerformSuccess(status, callback));
			return true;
		}

		public static void UpdateStatus(OrderStatus orderStatus)
		{
			if (orderStatus == null)
				throw new ArgumentNullException(nameof(orderStatus));

			if (orderStatus.order_id <= 0)
				throw new ArgumentException("Order id must be positive number", nameof(orderStatus.order_id));

			Items[orderStatus.order_id] = orderStatus;
		}

		private static OrderStatus GetCompleted(int orderId)
		{
			if (!Items.TryGetValue(orderId, out var orderStatus))
				return null;

			return orderStatus.status == "done"
				? orderStatus
				: null;
		}

		private static IEnumerator PerformSuccess(OrderStatus status, Action<OrderStatus> callback)
		{
			yield return new WaitForSeconds(0.1f);
			callback?.Invoke(status);
		}
	}
}