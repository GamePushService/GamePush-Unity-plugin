namespace Xsolla.UserAccount
{
	internal static class DataClassExtensions
	{
		public static string ToApiParameter(this FriendsSearchType provider)
		{
			switch (provider)
			{
				case FriendsSearchType.Added: return "friends";
				case FriendsSearchType.Requested: return "friend_requested";
				case FriendsSearchType.Pending: return "friend_requested_by";
				case FriendsSearchType.Blocked: return "blocked";
				case FriendsSearchType.BlocksMe: return "blocked_by";
				default: return string.Empty;
			}
		}

		public static string ToApiParameter(this FriendAction provider)
		{
			switch (provider)
			{
				case FriendAction.SendInviteRequest: return "friend_request_add";
				case FriendAction.CancelRequest: return "friend_request_cancel";
				case FriendAction.AcceptInvite: return "friend_request_approve";
				case FriendAction.DenyInvite: return "friend_request_deny";
				case FriendAction.RemoveFriend: return "friend_remove";
				case FriendAction.BlockFriend: return "block";
				case FriendAction.UnblockFriend: return "unblock";
				default: return string.Empty;
			}
		}

		public static string ToApiParameter(this FriendsSearchSort provider)
		{
			switch (provider)
			{
				case FriendsSearchSort.ByNickname: return "by_nickname";
				case FriendsSearchSort.ByUpdated: return "by_updated";
				default: return string.Empty;
			}
		}

		public static string ToApiParameter(this FriendsSearchOrder provider)
		{
			switch (provider)
			{
				case FriendsSearchOrder.Asc: return "asc";
				case FriendsSearchOrder.Desc: return "desc";
				default: return string.Empty;
			}
		}
	}
}