using System.Collections.Generic;
using UnityEngine;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	[AddComponentMenu("Scripts/Xsolla.Core/Popup/CouponRewardsPopup")]
	public class CouponRewardsPopup : MonoBehaviour, ICouponRewardsPopup
	{
		[SerializeField] private GameObject itemPrefab = default;
		[SerializeField] private ItemContainer itemsContainer = default;

		public ICouponRewardsPopup SetItems(List<CouponRedeemedItemModel> items)
		{
			items.ForEach(AddCouponRewardItem);
			return this;
		}

		public ISuccessPopup GetPopupCore()
		{
			return this.gameObject.GetComponent<SuccessPopup>();
		}

		private void AddCouponRewardItem(CouponRedeemedItemModel couponRewardItem)
		{
			itemsContainer.AddItem(itemPrefab).GetComponent<CouponRewardItemUI>().Initialize(couponRewardItem);
		}
	}
}
