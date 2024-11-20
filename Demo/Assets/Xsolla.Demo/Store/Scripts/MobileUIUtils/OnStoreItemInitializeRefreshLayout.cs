using UnityEngine;

namespace Xsolla.Demo
{
	[RequireComponent(typeof(StoreItemUI))]
	public class OnStoreItemInitializeRefreshLayout : OnStartRefreshLayout
    {
		private void Awake()
		{
			GetComponent<StoreItemUI>().OnInitialized += _ => base.RefreshLayout();
		}
	}
}
