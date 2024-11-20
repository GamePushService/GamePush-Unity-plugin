using System;
using System.Collections.Generic;
using UnityEngine;

namespace Xsolla.Demo
{
	public partial class ItemsTabControl : MonoBehaviour
	{
		private enum TabNames
		{
			Store,
			Inventory
		}

		private Dictionary<TabNames, Action> tabSelectHandler;
		int tabIndex = 0;

		private void InitHotKeys()
		{
			tabSelectHandler = new Dictionary<TabNames, Action>()
			{
				{
					TabNames.Store, () =>
					{
						storeButton.Select();
						InternalActivateStoreTab();
					}
				},
				{
					TabNames.Inventory, () =>
					{
						inventoryButton.Select();
						InternalActivateInventoryTab();
					}
				}
			};

			StoreTabsHotkey hotkey = gameObject.GetComponent<StoreTabsHotkey>();
			hotkey.TabKeyPressedEvent += SelectNextTab;
			hotkey.LeftArrowKeyPressedEvent += SelectPreviousTab;
			hotkey.RightArrowKeyPressedEvent += SelectNextTab;
		}

		private void SelectNextTab()
		{
			tabIndex = (tabIndex + 1) % Enum.GetNames(typeof(TabNames)).Length;
			tabSelectHandler[(TabNames) Enum.Parse(typeof(TabNames), tabIndex.ToString())].Invoke();
		}

		private void SelectPreviousTab()
		{
			if (tabIndex == 0)
			{
				tabIndex = Enum.GetNames(typeof(TabNames)).Length;
			}

			tabIndex--;
			tabSelectHandler[(TabNames) Enum.Parse(typeof(TabNames), tabIndex.ToString())].Invoke();
		}
	}
}