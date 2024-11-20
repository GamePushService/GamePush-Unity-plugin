using UnityEngine;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public class MainMenuController : BaseMenuController
	{
		[SerializeField] private SimpleButton storeButton = default;
		[SerializeField] private SimpleButton inventoryButton = default;
		[SerializeField] private SimpleButton webStoreButton = default;
		[SerializeField] private SimpleButton profileButton = default;
		[SerializeField] private SimpleButton friendsButton = default;
		[SerializeField] private SimpleButton publisherAccountButton = default;
		[SerializeField] private SimpleButton documentationButton = default;
		[SerializeField] private SimpleButton feedbackButton = default;
		[SerializeField] private SimpleButton logoutButton = default;
		[SerializeField] private SimpleButton tutorialButton = default;

		private void Start()
		{
			if (DemoMarker.IsStoreDemo)
				InitStoreDemo();
			else if (DemoMarker.IsInventoryDemo)
				InitInventoryDemo();
			else/*if (DemoMarker.IsLoginDemo)*/
				InitLoginDemo();
		}

		private void InitLoginDemo()
		{
			InitLoginButtons();
			InitCommonButtons();
		}

		private void InitStoreDemo()
		{
			if (DemoMarker.IsInventoryPartAvailable)
			{
				if (!UserInventory.IsExist)
					UserInventory.Instance.Init();

				if (!UserCatalog.IsExist)
					UserCatalog.Instance.Init();
			}
			
			AttachButtonCallback(storeButton,
				() => SetMenuState(MenuState.Store, () => UserCatalog.Instance.IsUpdated && UserInventory.Instance.IsUpdated));

			AttachUrlToButton(webStoreButton, DemoController.Instance.GetWebStoreUrl());

			AttachButtonCallback(inventoryButton, () =>
			{
				if (UserInventory.Instance.IsUpdated)
				{
					UserInventory.Instance.Refresh(() => SetMenuState(MenuState.Inventory), StoreDemoPopup.ShowError);
					PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => UserInventory.Instance.IsUpdated);
				}
				else
					SetMenuState(MenuState.Inventory, () => UserInventory.Instance.IsUpdated);
			});

			InitLoginButtons();
			InitCommonButtons();
		}

		private void InitInventoryDemo()
		{
			AttachButtonCallback(inventoryButton, () =>
			{
				if (UserInventory.Instance.IsUpdated)
				{
					UserInventory.Instance.Refresh(() => SetMenuState(MenuState.Inventory), StoreDemoPopup.ShowError);
					PopupFactory.Instance.CreateWaiting().SetCloseCondition(() => UserInventory.Instance.IsUpdated);
				}
				else
					SetMenuState(MenuState.Inventory, () => UserInventory.Instance.IsUpdated);
			});

			AttachUrlToButton(webStoreButton, DemoController.Instance.GetWebStoreUrl());

			AttachButtonCallback(tutorialButton, () =>
			{
				if (DemoController.Instance.IsTutorialAvailable)
					DemoController.Instance.ShowTutorial(false);
			});
			
			InitCommonButtons();
		}

		private void InitLoginButtons()
		{
			AttachButtonCallback(profileButton,
				() => SetMenuState(MenuState.Profile, () => UserCatalog.Instance.IsUpdated));

			AttachButtonCallback(friendsButton, HandleFriendsButton);
		}

		private void HandleFriendsButton()
		{
			UserFriends.Instance.UpdateFriends(
			onSuccess: () =>
			{
				SetMenuState(MenuState.Friends, () => UserFriends.Instance.IsUpdated);
			},
			onError: error =>
			{
				StoreDemoPopup.ShowError(error);
				DemoController.Instance.SetState(MenuState.Authorization);
			});
		}

		private void InitCommonButtons()
		{
			AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));
			AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
			AttachUrlToButton(feedbackButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.FeedbackUrl));

			AttachButtonCallback(logoutButton,
				() => SetMenuState(MenuState.Authorization, () => !WebRequestHelper.Instance.IsBusy()));
		}
	}
}
