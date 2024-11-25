using System;
using System.Collections.Generic;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public class DemoInventory : MonoSingleton<DemoInventory>
	{
		public void ConsumeInventoryItem(InventoryItemModel item, int count = 1, Action<InventoryItemModel> onSuccess = null, Action<Error> onError = null,
			bool isConfirmationRequired = true)
		{
			var onConfirmation = new Action(() =>
			{
				var isFinished = false;
				PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => isFinished);

				InventoryLogic.Instance.ConsumeInventoryItem(item, count,
					onSuccess: consumedItem =>
					{
						isFinished = true;
						onSuccess?.Invoke(consumedItem);
					},
					onError: error =>
					{
						isFinished = true;
						StoreDemoPopup.ShowError(error);
						onError?.Invoke(error);
					});
			});

			if (isConfirmationRequired)
				StoreDemoPopup.ShowConsumeConfirmation(item.Name, (uint) count, onConfirmation, () => onError?.Invoke(null));
			else
				onConfirmation.Invoke();
		}

		public void RedeemCouponCode(string couponCode, Action<List<CouponRedeemedItemModel>> onSuccess, Action<Error> onError)
		{
			var isFinished = false;
			PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => isFinished);

			InventoryLogic.Instance.RedeemCouponCode(couponCode,
				onSuccess: redeemedItems =>
				{
					isFinished = true;
					onSuccess?.Invoke(redeemedItems);
				},
				onError: error =>
				{
					isFinished = true;

					if (error.ErrorType != ErrorType.InvalidCoupon)
						StoreDemoPopup.ShowError(error);

					onError?.Invoke(error);
				});
		}
	}
}