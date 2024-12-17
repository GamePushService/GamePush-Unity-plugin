using System;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public class FriendUI : MonoBehaviour
	{
		[SerializeField] private Image avatarImage = default;
		[SerializeField] private Image statusImage = default;
		[SerializeField] private Text nicknameText = default;
		[SerializeField] private Text tagText = default;
		[SerializeField] private FriendActionsButton actionsButton = default;
		[SerializeField] private FriendButtonsUI userButtons = default;
		[SerializeField] private FriendStatusLineUI userStatusLine = default;
		[SerializeField] private GameObject SocialFriendship = default;
		[SerializeField] private SocialProviderContainer[] SocialFriendshipMarkers = default;

		public FriendModel FriendModel { get; private set; }

		public void Initialize(FriendModel friend)
		{
			if (friend == null) return;
			FriendModel = friend;

			InitAvatar(FriendModel);
			InitStatus(FriendModel);
			InitNickname(FriendModel);
			SetUserRelationship(FriendModel.Relationship);
			SetSocialFriendship(FriendModel.SocialProvider);
		}

		private void InitAvatar(FriendModel friend)
		{
			if (!string.IsNullOrEmpty(friend.AvatarUrl))
			{
				ImageLoader.LoadSprite(friend.AvatarUrl, sprite =>
				{
					if (avatarImage)
					{
						avatarImage.gameObject.SetActive(true);
						avatarImage.sprite = sprite;
					}
				});
			}
			else
				avatarImage.gameObject.SetActive(true);
		}

		private void InitStatus(FriendModel friend)
		{
			if (statusImage != null)
			{
				statusImage.gameObject.SetActive(true);
				switch (friend.Status)
				{
					case UserOnlineStatus.Online:
						statusImage.color = new Color(0.043F, 0.043F, 0, 1.0F);
						break;
					case UserOnlineStatus.Offline:
						statusImage.color = new Color(0.517F, 0.557F, 0.694F, 0.5F);
						break;
					case UserOnlineStatus.Unknown:
						statusImage.color = new Color(0.5F, 0.5F, 0.5F, 0.5F);
						break;
					default:
						statusImage.color = new Color(0.8F, 0F, 0F, 0.3F);
						break;
				}
			}
		}

		private void InitNickname(FriendModel friend)
		{
			var text = string.IsNullOrEmpty(friend.Nickname) ? string.Empty : friend.Nickname;

			if (!string.IsNullOrEmpty(text))
				gameObject.name = text;

			nicknameText.text = text;

			tagText.text = string.IsNullOrEmpty(friend.Tag) ? string.Empty : $"#{friend.Tag}";
			tagText.gameObject.SetActive(!string.IsNullOrEmpty(friend.Tag));
		}

		public void SetUserState(UserState state)
		{
			BaseUserStateUI userState;
			switch (state)
			{
				case UserState.Initial:
				{
					userState = gameObject.AddComponent<UserStateInitial>();
					break;
				}
				case UserState.MyFriend:
				{
					userState = gameObject.AddComponent<UserStateMyFriend>();
					break;
				}
				case UserState.Pending:
				{
					userState = gameObject.AddComponent<UserStatePending>();
					break;
				}
				case UserState.Requested:
				{
					userState = gameObject.AddComponent<UserStateRequested>();
					break;
				}
				case UserState.Blocked:
				{
					userState = gameObject.AddComponent<UserStateBlocked>();
					break;
				}
				case UserState.SocialNonXsolla:
				{
					userState = gameObject.AddComponent<UserStateSocialNonXsolla>();
					break;
				}
				default:
				{
					XDebug.LogWarning($"Set up handle of user state = '{state.ToString()}' in FriendUI.cs");
					return;
				}
			}

			userState.Init(this, userButtons, userStatusLine, actionsButton);
		}

		public void SetUserRelationship(UserRelationship relationship)
		{
			switch (relationship)
			{
				case UserRelationship.Unknown:
				{
					SetUserState(UserState.Initial);
					break;
				}
				case UserRelationship.Friend:
				{
					SetUserState(UserState.MyFriend);
					break;
				}
				case UserRelationship.Pending:
				{
					SetUserState(UserState.Pending);
					break;
				}
				case UserRelationship.Requested:
				{
					SetUserState(UserState.Requested);
					break;
				}
				case UserRelationship.Blocked:
				{
					SetUserState(UserState.Blocked);
					break;
				}
				case UserRelationship.SocialNonXsolla:
				{
					SetUserState(UserState.SocialNonXsolla);
					break;
				}
				default:
				{
					throw new ArgumentOutOfRangeException(nameof(relationship), relationship, null);
				}
			}
		}

		public void AddSocialFriendshipMark(SocialProvider socialProvider)
		{
			SocialFriendship.SetActive(true);

			foreach (var marker in SocialFriendshipMarkers)
			{
				if (marker.SocialProvider == socialProvider)
				{
					marker.gameObject.SetActive(true);
					break;
				}
			}
		}

		private void SetSocialFriendship(SocialProvider provider)
		{
			if (provider != SocialProvider.None)
				AddSocialFriendshipMark(provider);
		}
	}
}