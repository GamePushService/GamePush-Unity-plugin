using Xsolla.Core.Centrifugo;

namespace Xsolla.Core
{
	internal class OrderTrackerByCentrifugo : OrderTracker
	{
		public OrderTrackerByCentrifugo(OrderTrackingData trackingData) : base(trackingData) { }

		public override void Start()
		{
			CentrifugoService.AddTracker(this);
			CentrifugoService.OrderStatusUpdate += OnOrderStatusUpdate;
			CentrifugoService.Error += OnError;
			CentrifugoService.Close += OnClose;
		}

		public override void Stop()
		{
			CentrifugoService.OrderStatusUpdate -= OnOrderStatusUpdate;
			CentrifugoService.Error -= OnError;
			CentrifugoService.Close -= OnClose;
			CentrifugoService.RemoveTracker(this);
		}

		private void OnOrderStatusUpdate(OrderStatusData data)
		{
			if (data.order_id != TrackingData.orderId)
				return;

			XDebug.Log($"Status for an order '{TrackingData.orderId}' is updated: {data.status}");

			HandleOrderStatus(
				data.status,
				() => {
					TrackingData.successCallback?.Invoke();
					RemoveSelfFromTracking();
				},
				RemoveSelfFromTracking);
		}

		private void OnError()
		{
			ReplaceSelfByShortPolling();
		}

		private void OnClose()
		{
			ReplaceSelfByShortPolling();
		}

		private void ReplaceSelfByShortPolling()
		{
			XDebug.Log($"Tracker for an order '{TrackingData.orderId}' is replaced with short polling");
			var shortPollingTracker = new OrderTrackerByShortPolling(TrackingData);
			OrderTrackingService.ReplaceTracker(this, shortPollingTracker);
		}
	}
}