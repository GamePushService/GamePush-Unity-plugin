using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using Xsolla.Auth;
using Xsolla.Core;
using Xsolla.UserAccount;

namespace Xsolla.Demo
{
	public class UserFriends : MonoSingleton<UserFriends>
	{
		public event Action UserFriendsUpdatedEvent;
		public event Action BlockedUsersUpdatedEvent;
		public event Action PendingUsersUpdatedEvent;
		public event Action RequestedUsersUpdatedEvent;
		public event Action SocialFriendsUpdatedEvent;
		public event Action AllUsersUpdatedEvent;
		public bool IsUpdated { get; private set; }

		public List<FriendModel> Friends { get; private set; }
		public List<FriendModel> Blocked { get; private set; }
		public List<FriendModel> Pending { get; private set; }
		public List<FriendModel> Requested { get; private set; }
		public List<FriendModel> SocialFriends { get; private set; }

		private Coroutine _refreshCoroutine;

		public override void Init()
		{
			base.Init();
			IsUpdated = false;
			Friends = new List<FriendModel>();
			Blocked = new List<FriendModel>();
			Pending = new List<FriendModel>();
			Requested = new List<FriendModel>();
			SocialFriends = new List<FriendModel>();
		}

		public FriendModel GetUserById(string userId)
		{
			Func<FriendModel, bool> predicate = user => user.Id.Equals(userId);
			var result = Friends.FirstOrDefault(predicate);

			if (result == null)
				result = Pending.FirstOrDefault(predicate);
			if (result == null)
				result = Blocked.FirstOrDefault(predicate);
			if (result == null)
				result = Requested.FirstOrDefault(predicate);

			return result;
		}

		public void UpdateFriends([CanBeNull] Action onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			IsUpdated = false;
			StartCoroutine(UpdateFriendsCoroutine(onSuccess, onError));
		}

		private void ValidateToken(Action onSuccess, Action<Error> onError)
		{
			void handleError(Error error)
			{
				if (error.ErrorType == ErrorType.InvalidToken)
				{
					XDebug.Log("SavedTokenAuth: Trying to refresh OAuth token");
					XsollaAuth.RefreshToken(
						onSuccess,
						refreshError =>
						{
							XDebug.LogError(error.errorMessage);
							onError?.Invoke(null);
						});
				}
				else
				{
					onError?.Invoke(null);
				}
			}

			XsollaAuth.GetUserInfo(_ => { onSuccess?.Invoke(); }, handleError);
		}

		private IEnumerator UpdateFriendsCoroutine(Action onSuccess, Action<Error> onError)
		{
			bool? isTokenValid = null;
			bool socialBusy = true;
			bool friendsBusy = true;
			bool blockedBusy = true;
			bool pendingBusy = true;
			bool requestedBusy = true;

			ValidateToken(
				onSuccess: () => isTokenValid = true,
				onError: _ => isTokenValid = false
			);

			yield return new WaitWhile(() => isTokenValid == null);

			if (isTokenValid == true)
			{
				// UpdateUserSocialFriends(() => socialBusy = false, onError);
				UpdateUserFriends(() => friendsBusy = false, onError);
				UpdateBlockedUsers(() => blockedBusy = false, onError);
				UpdatePendingUsers(() => pendingBusy = false, onError);
				UpdateRequestedUsers(() => requestedBusy = false, onError);
				yield return new WaitWhile(() => friendsBusy || blockedBusy || pendingBusy || requestedBusy);

				UpdateSocialFriends(onError: onError);
				IsUpdated = true;
				onSuccess?.Invoke();
				AllUsersUpdatedEvent?.Invoke();
			}
			else
			{
				onError?.Invoke(new Error(ErrorType.InvalidToken, errorMessage: "Your token is not valid"));
			}
		}

		private void UpdateUserSocialFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			FriendsLogic.Instance.ForceUpdateFriendsFromSocialNetworks(onSuccess, onError);
		}

		private void UpdateUserFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			FriendsLogic.Instance.GetUserFriends(friends =>
			{
				Friends = friends;
				UserFriendsUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}

		private void UpdateBlockedUsers(Action onSuccess = null, Action<Error> onError = null)
		{
			FriendsLogic.Instance.GetBlockedUsers(users =>
			{
				Blocked = users;
				BlockedUsersUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}

		private void UpdatePendingUsers(Action onSuccess = null, Action<Error> onError = null)
		{
			FriendsLogic.Instance.GetPendingUsers(users =>
			{
				Pending = users;
				PendingUsersUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}

		private void UpdateRequestedUsers(Action onSuccess = null, Action<Error> onError = null)
		{
			FriendsLogic.Instance.GetRequestedUsers(users =>
			{
				Requested = users;
				RequestedUsersUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}

		private void UpdateSocialFriends(Action onSuccess = null, Action<Error> onError = null)
		{
			FriendsLogic.Instance.GetFriendsFromSocialNetworks(users =>
			{
				SocialFriends = users;
				SocialFriendsUpdatedEvent?.Invoke();
				onSuccess?.Invoke();
			}, onError);
		}

		private void RaiseOnError(string message, Action<Error> onError = null)
		{
			XDebug.LogError(message);
			var error = new Error(ErrorType.IncorrectFriendState, errorMessage: message);
			onError?.Invoke(error);
		}

		private void RemoveUserFromMemory(FriendModel user)
		{
			if (Friends.Contains(user))
			{
				Friends.Remove(user);
				UserFriendsUpdatedEvent?.Invoke();
			}

			if (Pending.Contains(user))
			{
				Pending.Remove(user);
				PendingUsersUpdatedEvent?.Invoke();
			}

			if (Requested.Contains(user))
			{
				Requested.Remove(user);
				RequestedUsersUpdatedEvent?.Invoke();
			}

			if (Blocked.Contains(user))
			{
				Blocked.Remove(user);
				BlockedUsersUpdatedEvent?.Invoke();
			}
		}

		public void BlockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (Blocked.Contains(user))
				RaiseOnError($"Can not block user with this nickname = {user.Nickname}. They are already blocked.", onError);
			else
			{
				FriendsLogic.Instance.BlockUser(user, blockedUser =>
				{
					RemoveUserFromMemory(user);
					UpdateBlockedUsers(onError: onError);
					onSuccess?.Invoke(blockedUser);
				}, onError);
			}
		}

		public void UnblockUser(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Blocked.Contains(user))
				RaiseOnError($"Can not unblock user with this nickname = {user.Nickname}. This user is not in the list of blocked friends.", onError);
			else
				FriendsLogic.Instance.UnblockUser(user, u =>
				{
					RemoveUserFromMemory(user);
					onSuccess?.Invoke(u);
				}, onError);
		}

		public void AddFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (Friends.Contains(user) || Blocked.Contains(user) || Pending.Contains(user) || Requested.Contains(user))
				RaiseOnError($"Can not add friend with this nickname = {user.Nickname}. This friend is not in the 'initial' state.", onError);
			else
				FriendsLogic.Instance.SendFriendshipInvite(user, u =>
				{
					RemoveUserFromMemory(user);
					UpdateRequestedUsers(onError: onError);
					onSuccess?.Invoke(u);
				}, onError);
		}

		public void RemoveFriend(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Friends.Contains(user))
				RaiseOnError($"Can not remove friend with this nickname = {user.Nickname}. This user is not in the friend list.", onError);
			else
			{
				FriendsLogic.Instance.RemoveFriend(user, u =>
				{
					RemoveUserFromMemory(user);
					onSuccess?.Invoke(u);
				}, onError);
			}
		}

		public void AcceptFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Pending.Contains(user))
				RaiseOnError($"Can not accept friendship from the user = {user.Nickname}. They are not a pending user.", onError);
			else
				FriendsLogic.Instance.AcceptFriendship(user, u =>
				{
					RemoveUserFromMemory(user);
					UpdateUserFriends();
					onSuccess?.Invoke(u);
				}, onError);
		}

		public void DeclineFriendship(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Pending.Contains(user))
				RaiseOnError($"Can not accept friendship from the user = {user.Nickname}. They are not a pending user.", onError);
			else
				FriendsLogic.Instance.DeclineFriendship(user, u =>
				{
					RemoveUserFromMemory(user);
					onSuccess?.Invoke(u);
				}, onError);
		}

		public void CancelFriendshipRequest(FriendModel user, [CanBeNull] Action<FriendModel> onSuccess = null, [CanBeNull] Action<Error> onError = null)
		{
			if (!Requested.Contains(user))
				RaiseOnError($"Can not cancel friendship request to = {user.Nickname}. This user doesn't have a friend request.", onError);
			else
				FriendsLogic.Instance.CancelFriendshipRequest(user, u =>
				{
					RemoveUserFromMemory(user);
					onSuccess?.Invoke(u);
				}, onError);
		}

		public void SearchUsersByNickname(string nickname, Action<List<FriendModel>> onSuccess = null, Action<Error> onError = null)
		{
			nickname = Uri.EscapeDataString(nickname);

			XsollaUserAccount.SearchUsers(
				nickname,
				0,
				20,
				users =>
				{
					onSuccess?.Invoke(users.users.Where(u => !u.is_me).Select(u =>
					{
						var result = new FriendModel {
							Id = u.user_id,
							AvatarUrl = u.avatar,
							Nickname = u.nickname,
							Tag = u.tag
						};
						var user = UserFriends.Instance.GetUserById(result.Id);
						result.Status = user?.Status ?? UserOnlineStatus.Unknown;
						result.Relationship = user?.Relationship ?? UserRelationship.Unknown;
						return result;
					}).ToList());
				},
				onError);
		}
	}
}