namespace Xsolla.Demo
{
	public class UserStateInitial : BaseUserStateUI
	{
		protected override void InitUserButtons(FriendButtonsUI buttons)
		{
			EnableAddFriendButton();
		}

		protected override void InitUserActionsButton(FriendActionsButton actionsButton)
		{
			EnableBlockUserOption();
		}
	}
}
