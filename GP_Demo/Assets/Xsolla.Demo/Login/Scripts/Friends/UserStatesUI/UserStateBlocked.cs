namespace Xsolla.Demo
{
	public class UserStateBlocked : BaseUserStateUI
	{
		protected override void InitUserButtons(FriendButtonsUI buttons)
		{
			EnableUnblockUserButton();
		}

		protected override void InitUserActionsButton(FriendActionsButton actionsButton)
		{
			actionsButton.gameObject.SetActive(false);
		}
	}
}
