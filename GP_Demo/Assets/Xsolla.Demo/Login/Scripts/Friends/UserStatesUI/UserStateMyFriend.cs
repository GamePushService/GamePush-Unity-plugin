namespace Xsolla.Demo
{
	public class UserStateMyFriend : BaseUserStateUI
	{
		protected override void InitUserButtons(FriendButtonsUI buttons)
		{ }

		protected override void InitUserActionsButton(FriendActionsButton actionsButton)
		{
			EnableDeleteUserOption();
			EnableBlockUserOption();
		}
	}
}
