namespace Xsolla.Auth
{
	public enum LogoutType
	{
		// Is used for deleting only the SSO user session.
		Sso,

		// Is used for deleting the SSO user session and invalidating all access and refresh tokens.
		All
	}
}