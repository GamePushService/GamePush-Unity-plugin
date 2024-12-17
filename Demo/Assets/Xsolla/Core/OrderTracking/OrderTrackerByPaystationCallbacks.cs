#if UNITY_WEBGL
namespace Xsolla.Core
{
	internal class OrderTrackerByPaystationCallbacks : OrderTracker
	{
		private bool isCancelByUser;

		public OrderTrackerByPaystationCallbacks(OrderTrackingData trackingData) : base(trackingData) { }

		public override void Start()
		{
			XsollaWebCallbacks.AddPaymentStatusUpdateHandler(HandleStatusUpdate);
			XsollaWebCallbacks.AddPaymentCancelHandler(HandlePaymentCancel);
		}

		public override void Stop()
		{
			XsollaWebCallbacks.RemovePaymentStatusUpdateHandler(HandleStatusUpdate);
			XsollaWebCallbacks.RemovePaymentCancelHandler(HandlePaymentCancel);
			XsollaWebBrowser.ClosePaystationWidget(isCancelByUser);
		}

		private void HandleStatusUpdate()
		{
			CheckOrderStatus(
				() =>
				{
					TrackingData?.successCallback?.Invoke();
					RemoveSelfFromTracking();
				},
				RemoveSelfFromTracking,
				error =>
				{
					TrackingData?.errorCallback?.Invoke(error);
					RemoveSelfFromTracking();
				}
			);
		}

		private void HandlePaymentCancel()
		{
			isCancelByUser = true;
			RemoveSelfFromTracking();
		}
	}
}
#endif