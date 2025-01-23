using System;
using System.Collections.Generic;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// User's social friends entity.
	/// </summary>
	[Serializable]
	public class UserSocialFriends
	{
		/// <summary>
		/// Maximum number of friends that are returned at a time.
		/// </summary>
		public int limit;

		/// <summary>
		/// Number of the elements from which the list is generated.
		/// </summary>
		public int offset;

		/// <summary>
		/// Total number of friends that you can get.
		/// </summary>
		public int total_count;

		/// <summary>
		/// List of data from social friends accounts.
		/// </summary>
		public List<UserSocialFriend> data;

		/// <summary>
		/// Name of a social provider.
		/// </summary>
		public string platform;

		/// <summary>
		/// Shows whether the social friends are from your game.
		/// </summary>
		public bool with_xl_uid;
	}

	/// <summary>
	/// User's social friend entity.
	/// </summary>
	[Serializable]
	public class UserSocialFriend
	{
		/// <summary>
		/// Friend’s name from a social provider.
		/// </summary>
		public string name;

		/// <summary>
		/// Name of a social provider.
		/// </summary>
		public string platform;

		/// <summary>
		/// User ID from a social provider.
		/// </summary>
		public string user_id;

		/// <summary>
		/// Friend's avatar from a social provider.
		/// </summary>
		public string avatar;

		/// <summary>
		/// User tag without "#" at the beginning. Can have no unique value and can be used in the Search users by nickname call.
		/// </summary>
		public string tag;

		/// <summary>
		/// The Xsolla Login user ID. You can find it in Publisher Account > your Login project > Users > Username/ID.
		/// </summary>
		public string xl_uid;
	}
}