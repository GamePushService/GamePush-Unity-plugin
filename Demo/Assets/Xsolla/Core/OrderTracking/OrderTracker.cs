using System;

namespace Xsolla.Core
{
	public abstract class OrderTracker
	{
		private bool _isCheckInProgress;

		public readonly OrderTrackingData TrackingData;

		public abstract void Start();

		public abstract void Stop();

		protected void RemoveSelfFromTracking()
		{
			OrderTrackingService.RemoveOrderFromTracking(TrackingData.orderId);
		}

		protected void CheckOrderStatus(Action onDone = null, Action onCancel = null, Action<Error> onError = null)
		{
			if (!XsollaToken.Exists)
			{
				XDebug.LogWarning("No Token in order status check. Check cancelled");
				onCancel?.Invoke();
				return;
			}

			if (_isCheckInProgress) // Prevent double check
				return;

			_isCheckInProgress = true;

			OrderStatusService.GetOrderStatus(
				TrackingData.orderId,
				status =>
				{
					_isCheckInProgress = false;
					HandleOrderStatus(status.status, onDone, onCancel);
				},
				error =>
				{
					_isCheckInProgress = false;
					onError?.Invoke(error);
				}
			);
		}

		protected static void HandleOrderStatus(string status, Action onDone, Action onCancel)
		{
			switch (status)
			{
				case "done":
					onDone?.Invoke();
					break;
				case "canceled":
					onCancel?.Invoke();
					break;
			}
		}

		protected OrderTracker(OrderTrackingData trackingData)
		{
			TrackingData = trackingData;
		}
	}
}