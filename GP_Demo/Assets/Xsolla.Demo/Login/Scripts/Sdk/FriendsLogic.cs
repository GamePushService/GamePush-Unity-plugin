using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Demo
{
	public class FriendsLogic : MonoSingleton<FriendsLogic>
	{
		/// <summary>
		/// Maximum friends count to display.
		/// </summary>
		private const int MAX_FRIENDS_COUNT = 50;
		/// <summary>
		/// Default friend name value if the name isn't provided.
		/// </summary>
		private const string DEFAULT_NAME_VALUE = "User without name";

		private const FriendsSearchOrder FRIENDS_SORT_ORDER = FriendsSearchOrder.Asc;
		private const FriendsSearchSort FRIENDS_SORT_TYPE = FriendsSearchSort.ByNickname;

		public event Action UpdateUserFriendsEvent;
		public event Action UpdateUserSocialFriendsEvent;

		public void GetUserFriends(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			GetUsersByType(FriendsSearchType.Added, UserRelationship.Friend, onSuccess, onError);
		}

		public void GetBlockedUsers(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			GetUsersByType(FriendsSearchType.Blocked, UserRelationship.Blocked, onSuccess, onError);
		}

		public void GetPendingUsers(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			GetUsersByType(FriendsSearchType.Pending, UserRelationship.Pending, onSuccess, onError);
		}

		public void GetRequestedUsers(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			GetUsersByType(FriendsSearchType.Requested, UserRelationship.Requested, onSuccess, onError);
		}

		public void BlockUser(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke(user);
				UpdateUserFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserFriends(FriendAction.BlockFriend, user.Id, successCallback, onError);
		}

		public void UnblockUser(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke(user);
				UpdateUserFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserFriends(FriendAction.UnblockFriend, user.Id, successCallback, onError);
		}

		public void SendFriendshipInvite(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke(user);
				UpdateUserFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserFriends(FriendAction.SendInviteRequest, user.Id, successCallback, onError);
		}

		public void RemoveFriend(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke(user);
				UpdateUserFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserFriends(FriendAction.RemoveFriend, user.Id, successCallback, onError);
		}

		public void AcceptFriendship(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke(user);
				UpdateUserFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserFriends(FriendAction.AcceptInvite, user.Id, successCallback, onError);
		}

		public void DeclineFriendship(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke(user);
				UpdateUserFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserFriends(FriendAction.DenyInvite, user.Id, successCallback, onError);
		}

		public void CancelFriendshipRequest(FriendModel user, Action<FriendModel> onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke(user);
				UpdateUserFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserFriends(FriendAction.CancelRequest, user.Id, successCallback, onError);
		}

		public void ForceUpdateFriendsFromSocialNetworks(Action onSuccess = null, Action<Error> onError = null)
		{
			Action successCallback = () =>
			{
				onSuccess?.Invoke();
				UpdateUserSocialFriendsEvent?.Invoke();
			};

			XsollaUserAccount.UpdateUserSocialFriends(successCallback, onError);
		}

		public void GetFriendsFromSocialNetworks(Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			XsollaUserAccount.GetUserSocialFriends(onSuccess: friends => StartCoroutine(ConvertSocialFriendsToRecommended(friends.data, onSuccess, onError)),
				onError: onError);
		}

		private IEnumerator ConvertSocialFriendsToRecommended(List<UserSocialFriend> socialFriends, Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			var recommendedFriends = new List<FriendModel>(socialFriends.Count);

			foreach (var socialFriend in socialFriends)
			{
				var recommendedFriend = ConvertFriendEntity(socialFriend);

				if (recommendedFriend.Relationship == UserRelationship.SocialNonXsolla) //Social friend without linked Xsolla account
				{
					//Proceed
				}
				else if (recommendedFriend.Relationship != UserRelationship.Unknown) //User is already added/blocked/requested
				{
					continue; //Skip this user, they should not appear as recommended
				}
				else //Social friend with linked Xsolla account, replace nickname and avatar with ones from Xsolla
				{
					bool? isUserinfoObtained = null;

					XsollaUserAccount.GetPublicInfo(
						recommendedFriend.Id,
						info =>
						{
							recommendedFriend.Nickname = info.nickname;
							recommendedFriend.AvatarUrl = info.avatar;
							isUserinfoObtained = true;
						},
						error =>
						{
							onError?.Invoke(error);
							isUserinfoObtained = false;
						});

					yield return new WaitWhile(() => isUserinfoObtained == null);

					if (isUserinfoObtained == false)
					{
						XDebug.LogError($"Could not get user information. UserID: {recommendedFriend.Id}");
						yield break;
					}
				}

				recommendedFriends.Add(recommendedFriend);

				//Avatar preload
				if (!string.IsNullOrEmpty(recommendedFriend.AvatarUrl))
					ImageLoader.LoadSprite(recommendedFriend.AvatarUrl, null);
			}

			onSuccess?.Invoke(recommendedFriends);
		}

		private void GetUsersByType(FriendsSearchType searchType, UserRelationship relationship,
			Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			XsollaUserAccount.GetUserFriends(
				searchType,
				friends =>
				{
					onSuccess?.Invoke(friends.relationships.Select(f =>
					{
						var result = ConvertFriendEntity(f, relationship);
						// this method used at this place for fastest image loading
						if (!string.IsNullOrEmpty(result.AvatarUrl))
							ImageLoader.LoadSprite(result.AvatarUrl, null);
						return result;
					}).ToList());
				}, 
				onError, 
				FRIENDS_SORT_TYPE, 
				FRIENDS_SORT_ORDER, 
				MAX_FRIENDS_COUNT);
		}

		private FriendModel ConvertFriendEntity(UserSocialFriend friend)
		{
			var result = new FriendModel {
				Id = friend.xl_uid,
				Nickname = friend.name,
				AvatarUrl = friend.avatar
			};

			if (!string.IsNullOrEmpty(friend.xl_uid)) //Xsolla ID not null - this is a registered Xsolla user with linked social account
			{
				var existingFriend = UserFriends.Instance.GetUserById(friend.xl_uid);
				result.Status = existingFriend?.Status ?? UserOnlineStatus.Unknown;
				result.Relationship = existingFriend?.Relationship ?? UserRelationship.Unknown;
			}
			else
			{
				result.Status = UserOnlineStatus.Unknown;
				result.Relationship = UserRelationship.SocialNonXsolla;
			}

			if (Enum.TryParse(friend.platform, ignoreCase: true, out SocialProvider provider))
				result.SocialProvider = provider;

			return result;
		}

		private FriendModel ConvertFriendEntity(UserFriend friend, UserRelationship relationship)
		{
			return new FriendModel {
				Id = friend.user.id,
				Nickname = GetUserNickname(friend),
				Tag = friend.user.tag,
				AvatarUrl = friend.user.picture,
				Status = friend.IsOnline() ? UserOnlineStatus.Online : UserOnlineStatus.Offline,
				Relationship = relationship
			};
		}

		private string GetUserNickname(UserFriend friend)
		{
			var userInfo = friend.user;

			if (!string.IsNullOrEmpty(userInfo.nickname))
				return userInfo.nickname;
			if (!string.IsNullOrEmpty(userInfo.name))
				return userInfo.name;
			if (!string.IsNullOrEmpty(userInfo.first_name))
				return userInfo.first_name;
			if (!string.IsNullOrEmpty(userInfo.last_name))
				return userInfo.last_name;
			if (!string.IsNullOrEmpty(userInfo.email))
				return userInfo.email;
			else
				return DEFAULT_NAME_VALUE;
		}
	}
}