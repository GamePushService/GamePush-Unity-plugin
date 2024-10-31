using System.Collections;
using UnityEngine;

namespace Xsolla.Core
{
	internal class OrderTrackerByShortPolling : OrderTracker
	{
		private Coroutine checkStatusCoroutine;

		public OrderTrackerByShortPolling(OrderTrackingData trackingData) : base(trackingData) { }

		public override void Start()
		{
			checkStatusCoroutine = CoroutinesExecutor.Run(TrackOrderStatus());
		}

		public override void Stop()
		{
			CoroutinesExecutor.Stop(checkStatusCoroutine);
		}

		private IEnumerator TrackOrderStatus()
		{
			var timeLimit = Time.realtimeSinceStartup + Constants.SHORT_POLLING_LIMIT;

			while (true)
			{
				yield return new WaitForSeconds(Constants.SHORT_POLLING_INTERVAL);
				CheckOrderStatus(HandleOrderDone, RemoveSelfFromTracking, HandleError);

				if (Time.realtimeSinceStartup > timeLimit)
				{
					HandleError(new Error(ErrorType.TimeLimitReached, errorMessage: "Polling time limit reached"));
					break;
				}
			}
		}

		private void HandleOrderDone()
		{
			TrackingData?.successCallback?.Invoke();
			RemoveSelfFromTracking();
		}

		private void HandleError(Error error)
		{
			TrackingData?.errorCallback?.Invoke(error);
			RemoveSelfFromTracking();
		}
	}
}