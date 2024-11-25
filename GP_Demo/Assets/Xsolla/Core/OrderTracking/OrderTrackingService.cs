using System;
using System.Collections.Generic;

namespace Xsolla.Core
{
	public static class OrderTrackingService
	{
		private static readonly Dictionary<int, OrderTracker> Trackers = new Dictionary<int, OrderTracker>();

		/// <summary>
		/// Starts status tracking for the specified order. The tracking mechanism varies based on the build platform.
		/// </summary>
		/// <param name="orderId">Order ID.</param>
		/// <param name="isUserInvolvedToPayment">Whether to use platform-specific methods for tracking, such as Web Sockets or Pay Station callbacks.</param>
		/// <param name="onSuccess">Callback, triggered when the order status is changed to `done`</param>
		/// <param name="onError">Callback, triggered when an error occurs during the order tracking.</param>
		public static void AddOrderForTracking(int orderId, bool isUserInvolvedToPayment, Action onSuccess, Action<Error> onError)
		{
			var tracker = CreateTracker(orderId, isUserInvolvedToPayment, onSuccess, onError);
			if (tracker != null)
				StartTracker(tracker);
		}

		private static OrderTracker CreateTracker(int orderId, bool isUserInvolvedToPayment, Action onSuccess, Action<Error> onError)
		{
			if (Trackers.ContainsKey(orderId))
				return null;

			var trackingData = new OrderTrackingData(orderId, onSuccess, onError);

			if (!isUserInvolvedToPayment)
				return new OrderTrackerByShortPolling(trackingData);

#if UNITY_WEBGL
			return !UnityEngine.Application.isEditor && XsollaSettings.InAppBrowserEnabled
				? new OrderTrackerByPaystationCallbacks(trackingData)
				: new OrderTrackerByShortPolling(trackingData) as OrderTracker;
#else
			return new OrderTrackerByCentrifugo(trackingData);
#endif
		}

		/// <summary>
		/// Stops status tracking for all orders.
		/// </summary>
		public static void RemoveAllOrdersFromTracking()
		{
			foreach (var tracker in Trackers.Values)
			{
				RemoveOrderFromTracking(tracker.TrackingData.orderId);
			}
		}

		/// <summary>
		/// Stops status tracking for specified order.
		/// </summary>
		/// <param name="orderId">Order ID.</param>
		public static void RemoveOrderFromTracking(int orderId)
		{
			if (!Trackers.TryGetValue(orderId, out var tracker))
				return;

			tracker.Stop();
			Trackers.Remove(orderId);
		}

		public static void ReplaceTracker(OrderTracker oldTracker, OrderTracker newTracker)
		{
			RemoveOrderFromTracking(oldTracker.TrackingData.orderId);
			StartTracker(newTracker);
		}

		private static void StartTracker(OrderTracker tracker)
		{
			XDebug.Log($"Order tracker started: {tracker.GetType().Name}");
			Trackers.Add(tracker.TrackingData.orderId, tracker);
			tracker.Start();
		}
	}
}
