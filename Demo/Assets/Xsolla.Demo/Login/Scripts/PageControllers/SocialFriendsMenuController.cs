using System;
using System.Collections.Generic;
using UnityEngine;
using Xsolla.Core;
using Xsolla.UIBuilder;

namespace Xsolla.Demo
{
	public class SocialFriendsMenuController : MonoBehaviour
	{
		[SerializeField] private WidgetProvider userPrefabProvider = default;
		[SerializeField] private ItemContainer usersContainer = default;
		[SerializeField] private FriendSystemSocialNetwork[] SocialNetworks = default;
		[SerializeField] private SimpleButton RefreshButton = default;

		private Dictionary<SocialProvider, List<FriendModel>> _socialFriends = new Dictionary<SocialProvider, List<FriendModel>>();

		private void Awake()
		{
			foreach (var network in SocialNetworks)
				network.StateChanged += NetworkOnStateChanged;

			if (RefreshButton)
				RefreshButton.onClick += RefreshSocialNetworks;
		}

		private void NetworkOnStateChanged(SocialProvider socialProvider, FriendSystemSocialNetwork.State state)
		{
			switch (state)
			{
				case FriendSystemSocialNetwork.State.Linked:
					FriendsLogic.Instance.GetFriendsFromSocialNetworks(
						onSuccess: newFriends =>
						{
							_socialFriends[socialProvider] = newFriends;
							RefreshSocialFriendsContainer(_socialFriends);
						},
						onError: StoreDemoPopup.ShowError);
					break;
				case FriendSystemSocialNetwork.State.Unlinked:
					_socialFriends.Remove(socialProvider);
					RefreshSocialFriendsContainer(_socialFriends);
					break;
			}
		}

		private void RefreshSocialFriendsContainer(Dictionary<SocialProvider, List<FriendModel>> socialFriends)
		{
			usersContainer.Clear();
			if (socialFriends.Count < 1) {
				usersContainer.EnableEmptyContainerMessage();
				return;
			} else {
				usersContainer.DisableEmptyContainerMessage();
			}

			var addedUsers = new List<FriendModel>();
			var createdFriendUIs = new List<FriendUI>();

			foreach (var pair in socialFriends)
			{
				//var provider = pair.Key;
				var friends = pair.Value;

				foreach (var friend in friends)
				{
					int index;
					if ((index = addedUsers.IndexOf(friend)) != -1)//This will be true for Xsolla users with several social networks linked
					{
						createdFriendUIs[index].AddSocialFriendshipMark(friend.SocialProvider);
					}
					else
					{
						var friendUiGameObject = usersContainer.AddItem(userPrefabProvider.GetValue());
						var friendUiScript = friendUiGameObject.GetComponent<FriendUI>();
						friendUiScript.Initialize(friend);

						addedUsers.Add(friend);
						createdFriendUIs.Add(friendUiScript);
					}
				}
			}
		}

		private void RefreshSocialNetworks()
		{
			Action onFriendsUpdate = () =>
			{
				foreach (var network in SocialNetworks)
					if (network.gameObject.activeSelf)
						network.RefreshState();
			};

			UserAccountLogic.Instance.PurgeSocialProvidersCache();
			UserFriends.Instance.UpdateFriends(onFriendsUpdate, StoreDemoPopup.ShowError);
		}
	}
}
