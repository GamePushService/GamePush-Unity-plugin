namespace Xsolla.Demo
{
	public enum MenuState
	{
		Authorization = 0,
		AuthorizationFailed = 1,
		Registration = 2,
		RegistrationFailed = 3,
		RegistrationSuccess = 4,
		ChangePassword = 5,
		ChangePasswordFailed = 6,
		ChangePasswordSuccess = 7,
		LoginSettingsError = 8,
		Main = 10,
		Store = 11,
		BuyCurrency = 12,
		Cart = 13,
		Inventory = 14,
		Profile = 15,
		Character = 16,
		Friends = 17,
		SocialFriends = 18,
		None = 19
	}

	public static class MenuStateExtension
	{
		public static bool IsAuthState(this MenuState menuState) => (int)menuState < 10;
		public static bool IsPostAuthState(this MenuState menuState) => !menuState.IsAuthState() && menuState != MenuState.None;
	}
}
