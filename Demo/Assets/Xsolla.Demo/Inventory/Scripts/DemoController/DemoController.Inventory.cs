using UnityEngine;
using Xsolla.Core;

namespace Xsolla.Demo
{
	public partial class DemoController : MonoSingleton<DemoController>
	{
		private TutorialManager _tutorialManager = default;

		partial void InitTutorial()
		{
			if (DemoMarker.IsInventoryDemo)
				_tutorialManager = GetComponent<TutorialManager>();

			if (_tutorialManager != null)
				IsTutorialAvailable = true;
		}

		partial void AutoStartTutorial()
		{
			if (IsTutorialAvailable)
			{
				if (!_tutorialManager.IsTutorialCompleted())
					_tutorialManager.ShowTutorial();
			}
		}

		partial void ManualStartTutorial(bool showWelcomeMessage)
		{
			_tutorialManager.ShowTutorial(showWelcomeMessage);
		}

		partial void UpdateInventory()
		{
			if (!DemoMarker.IsStorePartAvailable)
			{
				UserCatalog.Instance.UpdateItems(
					onSuccess: () => UserInventory.Instance.Refresh(onError: StoreDemoPopup.ShowError),
					onError: error =>
					{
						XDebug.LogError($"InventorySDK init failure: {error}");
						StoreDemoPopup.ShowError(error);
					});
			}
		}

		partial void DestroyInventory()
		{
			if (UserInventory.IsExist)
				Destroy(UserInventory.Instance.gameObject);
		}
	}
}
