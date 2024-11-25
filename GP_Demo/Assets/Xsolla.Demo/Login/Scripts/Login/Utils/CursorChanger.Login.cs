using UnityEngine;

namespace Xsolla.Demo
{
	public partial class CursorChanger : MonoBehaviour
	{
		partial void AdditionalAwakeActionsLogin()
		{
			BlurSelectedUser.OnActivated += ChangeToButtonHoverCursor;
			BlurSelectedUser.OnDeactivated += ChangeBackToDefault;
		}

		partial void AdditionalDestroyActionsLogin()
		{
			BlurSelectedUser.OnActivated -= ChangeToButtonHoverCursor;
			BlurSelectedUser.OnDeactivated -= ChangeBackToDefault;
		}
	}
}