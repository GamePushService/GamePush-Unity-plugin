using System;
using System.Collections.Generic;

namespace Xsolla.UserAccount
{
	/// <summary>
	/// Found user entity.
	/// </summary>
	[Serializable]
	public class FoundUsers
	{
		/// <summary>
		/// List of data from social friends accounts.
		/// </summary>
		public List<FoundUser> users;
		/// <summary>
		/// Number of the elements from which the list is generated.
		/// </summary>
		public int offset;
		/// <summary>
		/// Total number of users that you can get.
		/// </summary>
		public int total_count;
	}

	/// <summary>
	/// Found user entity.
	/// </summary>
	[Serializable]
	public class FoundUser
	{
		/// <summary>
		/// The Xsolla Login user ID. You can find it in Publisher Account > your Login project > Users > Username/ID.
		/// </summary>
		public string user_id;
		/// <summary>
		/// User nickname.
		/// </summary>
		public string nickname;
		/// <summary>
		/// Shows whether the user who initiated a search or not.
		/// </summary>
		public bool is_me;
		/// <summary>
		/// Date of user registration in the RFC3339 format.
		/// </summary>
		public string registered;
		/// <summary>
		/// Date of the last user login in the RFC3339 format.
		/// </summary>
		public string last_login;
		/// <summary>
		/// URL of the user avatar.
		/// </summary>
		public string avatar;
		/// <summary>
		/// User tag without "#" at the beginning. Can have no unique value.
		/// </summary>
		public string tag;
	}
}