using UnityEngine;

namespace Xsolla.Demo
{
	public class VirtualCurrencySubMenu : MonoBehaviour
	{
		[SerializeField] private SimpleButton buyCurrencyButton = default;
		[SerializeField] private GameObject virtualCurrencyBalance = default;

		private void Start()
		{
			if (!DemoController.Instance.IsStateAvailable(MenuState.BuyCurrency))
			{
				buyCurrencyButton.gameObject.SetActive(false);

				var balancePosition = virtualCurrencyBalance.GetComponent<RectTransform>().anchoredPosition;
				balancePosition.x = 0;
				virtualCurrencyBalance.GetComponent<RectTransform>().anchoredPosition = balancePosition;
			}
			else
			{
				BaseMenuController.AttachButtonCallback(buyCurrencyButton,
					() => BaseMenuController.SetMenuState(MenuState.BuyCurrency, () => UserCatalog.Instance.IsUpdated));
			}
		}
	}
}