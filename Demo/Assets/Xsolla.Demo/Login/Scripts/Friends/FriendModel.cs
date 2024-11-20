using Xsolla.Core;

namespace Xsolla.Demo
{
	public class FriendModel
	{
		/// <summary>
		/// Friend id.
		/// </summary>
		public string Id;
		/// <summary>
		/// Friend nickname.
		/// </summary>
		public string Nickname;
		/// <summary>
		/// Friend tag.
		/// </summary>
		public string Tag;
		/// <summary>
		/// Friend avatar url.
		/// </summary>
		public string AvatarUrl;
		/// <summary>
		/// Friend online status.
		/// </summary>
		public UserOnlineStatus Status;
		/// <summary>
		/// Friend relationship.
		/// </summary>
		public UserRelationship Relationship;
		/// <summary>
		/// Friend from social network
		/// </summary>
		public SocialProvider SocialProvider = SocialProvider.None;

		public override bool Equals(object obj)
		{
			if (!(obj is FriendModel)) return false;
			return string.IsNullOrEmpty(Id) ? base.Equals(obj) : Id.Equals(((FriendModel) obj).Id);
		}

		public override int GetHashCode()
		{
			return string.IsNullOrEmpty(Id) ? base.GetHashCode() : Id.GetHashCode();
		}
	}
}