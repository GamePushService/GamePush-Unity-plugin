namespace Xsolla.Demo
{
	public class UserStateRequested : BaseUserStateUI
	{
		protected override void InitUserButtons(FriendButtonsUI buttons)
		{
			EnableCancelFriendshipRequestButton();
		}

		protected override void InitUserActionsButton(FriendActionsButton actionsButton)
		{
			EnableBlockUserOption();
		}
	}
}
