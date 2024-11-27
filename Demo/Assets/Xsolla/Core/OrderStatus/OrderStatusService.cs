using System;

namespace Xsolla.Core
{
	internal static class OrderStatusService
	{
		public static void GetOrderStatus(int orderId, Action<OrderStatus> onSuccess, Action<Error> onError)
		{
			if (OrderStatusCache.TryPerform(orderId, onSuccess))
				return;

			PerformWebRequest(
				orderId,
				status =>
				{
					OrderStatusCache.UpdateStatus(status);
					onSuccess?.Invoke(status);
				},
				onError);
		}

		private static void PerformWebRequest(int orderId, Action<OrderStatus> onSuccess, Action<Error> onError)
		{
			var url = $"https://store.xsolla.com/api/v2/project/{XsollaSettings.StoreProjectId}/order/{orderId}";

			WebRequestHelper.Instance.GetRequest(
				SdkType.Store,
				url,
				WebRequestHeader.AuthHeader(),
				onSuccess,
				error => TokenAutoRefresher.Check(error, onError, () => PerformWebRequest(orderId, onSuccess, onError)),
				ErrorGroup.OrderStatusErrors);
		}
	}
}