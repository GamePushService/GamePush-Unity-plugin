namespace Xsolla.Demo
{
	public class UserStatePending : BaseUserStateUI
	{
		protected override void InitUserButtons(FriendButtonsUI buttons)
		{
			EnableAcceptFriendshipButton(() => StatusLine.EnableRequestAcceptedMessage());
			EnableDeclineFriendshipButton(() =>
			{
				UserButtons.DisableAddFriendButton();
				StatusLine.EnableRequestDeclinedMessage();
			});
		}

		protected override void InitUserActionsButton(FriendActionsButton actionsButton)
		{
			EnableBlockUserOption();
		}
	}
}
