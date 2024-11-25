namespace Xsolla.Demo
{
	public class UserStateSocialNonXsolla : BaseUserStateUI
	{
		protected override void InitUserButtons(FriendButtonsUI buttons)
		{
			//Do nothing
		}

		protected override void InitUserActionsButton(FriendActionsButton actionsButton)
		{
			actionsButton.gameObject.SetActive(false);
		}
	}
}
