using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Xsolla.Demo
{
	public class FriendsMenuHelper : MonoBehaviour
	{
		public const string USER_FRIENDS_GROUP = "MY FRIENDS";
		public const string PENDING_USERS_GROUP = "PENDING";
		public const string REQUESTED_USERS_GROUP = "REQUESTS";
		public const string BLOCKED_USERS_GROUP = "BLOCKED";
		public const string ADD_FRIENDS_GROUP = "ADD FRIENDS";

		private static string _selectedGroup = string.Empty;
	
		public static List<FriendModel> GetUsersByGroup(string group)
		{
			group = group.Split('(')[0].Trim();
			switch (group)
			{
				case USER_FRIENDS_GROUP: return UserFriends.Instance.Friends;
				case PENDING_USERS_GROUP: return UserFriends.Instance.Pending;
				case REQUESTED_USERS_GROUP: return UserFriends.Instance.Requested;
				case BLOCKED_USERS_GROUP: return UserFriends.Instance.Blocked;
				case ADD_FRIENDS_GROUP: return UserFriends.Instance.SocialFriends;
				default: return new List<FriendModel>();
			}
		}

		public static void RefreshGroups(GroupsController groupsController, string selectedGroup = null)
		{
			if(groupsController == null) return;
			groupsController.RemoveAll();

			var friendsGroup = groupsController.AddGroup(USER_FRIENDS_GROUP).GetComponent<FriendGroup>();
			friendsGroup.SetUsersCount(UserFriends.Instance.Friends.Count);
			UserFriends.Instance.UserFriendsUpdatedEvent += () => friendsGroup.SetUsersCount(UserFriends.Instance.Friends.Count);

			var pendingGroup = groupsController.AddGroup(PENDING_USERS_GROUP).GetComponent<FriendGroup>();
			pendingGroup.SetUsersCount(UserFriends.Instance.Pending.Count);
			UserFriends.Instance.PendingUsersUpdatedEvent += () => pendingGroup.SetUsersCount(UserFriends.Instance.Pending.Count);

			var requestedGroup = groupsController.AddGroup(REQUESTED_USERS_GROUP).GetComponent<FriendGroup>();
			requestedGroup.SetUsersCount(UserFriends.Instance.Requested.Count);
			UserFriends.Instance.RequestedUsersUpdatedEvent += () => requestedGroup.SetUsersCount(UserFriends.Instance.Requested.Count);

			var blockedGroup = groupsController.AddGroup(BLOCKED_USERS_GROUP).GetComponent<FriendGroup>();
			blockedGroup.SetUsersCount(UserFriends.Instance.Blocked.Count);
			UserFriends.Instance.BlockedUsersUpdatedEvent += () => blockedGroup.SetUsersCount(UserFriends.Instance.Blocked.Count);

			var addFriendsGroup = groupsController.AddGroup(ADD_FRIENDS_GROUP).GetComponent<FriendGroup>();
			addFriendsGroup.enabled = false;

			if (string.IsNullOrEmpty(selectedGroup) && !string.IsNullOrEmpty(_selectedGroup))
				selectedGroup = _selectedGroup;
			_selectedGroup = selectedGroup;
		
			if(string.IsNullOrEmpty(_selectedGroup))
				groupsController.SelectDefault();
			else
				groupsController.Groups.First(g => g.Name.Equals(_selectedGroup)).Select();
		}
	}
}
