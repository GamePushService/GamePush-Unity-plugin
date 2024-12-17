namespace Xsolla.UserAccount
{
	/// <summary>
	/// Type of the action.
	/// </summary>
	public enum FriendAction
	{
		/// <summary>
		/// To send a friend request
		/// </summary>
		SendInviteRequest,
		/// <summary>
		/// To cancel the friend request that was sent
		/// </summary>
		CancelRequest,
		/// <summary>
		/// To confirm the friend request
		/// </summary>
		AcceptInvite,
		/// <summary>
		/// To cancel the friend request that was received
		/// </summary>
		DenyInvite,
		/// <summary>
		/// To delete the user from the friend list
		/// </summary>
		RemoveFriend,
		/// <summary>
		/// To block the user
		/// </summary>
		BlockFriend,
		/// <summary>
		/// To unblock the user
		/// </summary>
		UnblockFriend
	}
}