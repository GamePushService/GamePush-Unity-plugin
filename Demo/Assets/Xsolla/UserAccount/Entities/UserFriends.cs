using System;
using System.Collections.Generic;
using Xsolla.Core;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's friends entity.
	/// </summary>
	[Serializable]
	public class UserFriends
	{
		/// <summary>
		/// Friends details.
		/// </summary>
		public List<UserFriend> relationships;
		/// <summary>
		/// URL of the request for using this call for the next time.
		/// </summary>
		public string next_url;
		/// <summary>
		/// Value of the after parameter that should be passed while requesting this call for the next time.
		/// </summary>
		public string next_after;
	}

	/// <summary>
	/// User's friend entity.
	/// </summary>
	[Serializable]
	public class UserFriend
	{
		/// <summary>
		/// Type of the requested user. Can be:
		///none if there is no action initiated for the user
		///friend if the user is in the friend list of the one with the specified JWT
		///friend_requested if the friend request is sent to the user by the one with the specified JWT
		///blocked if the user is blocked by the one with the specified JWT
		/// </summary>
		public string status_outgoing;
		/// <summary>
		/// Type of the user. Can be:
		///none if there is no action initiated for the user
		///friend if the user is in the friend list of the requested one
		///friend_requested if the friend request is sent to the user by the requested one
		///blocked if the user is blocked by the requested one
		/// </summary>
		public string status_incoming;
		/// <summary>
		/// User details.
		/// </summary>
		public UserInfo user;
		/// <summary>
		/// Time passed since the latest action of adding a friend to the friend list or banning them in seconds.
		/// </summary>
		public float updated;

		public bool IsOnline()
		{
			return !string.IsNullOrEmpty(user.presence) && user.presence.Equals("online");
		}
	}
}