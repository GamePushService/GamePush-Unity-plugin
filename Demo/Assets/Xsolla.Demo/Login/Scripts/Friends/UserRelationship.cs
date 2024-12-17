namespace Xsolla.Demo
{
	public enum UserRelationship
	{
		/// <summary>
		/// Unknown relationship.
		/// </summary>
		Unknown,
		/// <summary>
		/// User is my friend already.
		/// </summary>
		Friend,
		/// <summary>
		/// Blocked user.
		/// </summary>
		Blocked,
		/// <summary>
		/// User send me friendship request and waiting my reaction.
		/// </summary>
		Pending,
		/// <summary>
		/// Friendship request sent to user.
		/// </summary>
		Requested,
		/// <summary>
		/// Friend from social network without Xsolla account.
		/// </summary>
		SocialNonXsolla
	}
}
