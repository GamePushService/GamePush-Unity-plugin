using UnityEngine;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public class TutorialManager : MonoBehaviour
	{
		[SerializeField] private TutorialInfo _tutorialInfo = default;
		private const string INVENTORY_TUTORIAL_COMPLETED = "xsolla_inventory_tutorial_completion_flag";

#if UNITY_EDITOR
		[ContextMenu("Reset tutorial")]
		public void DropTutorial() => PlayerPrefs.SetInt(INVENTORY_TUTORIAL_COMPLETED, 0);
#endif

		public void ShowTutorial(bool showWelcomeMessage = true)
		{
			var tutorialPopup = PopupFactory.Instance.CreateTutorial();
			tutorialPopup.SetTutorialInfo(_tutorialInfo, showWelcomeMessage);
			tutorialPopup.SetCancelCallback(MarkTutorialAsCompleted);
			tutorialPopup.SetCompletionCallback(MarkTutorialAsCompleted);
		}

		public bool IsTutorialCompleted()
		{
			return PlayerPrefs.GetInt(INVENTORY_TUTORIAL_COMPLETED, 0) > 0;
		}

		void MarkTutorialAsCompleted()
		{
			PlayerPrefs.SetInt(INVENTORY_TUTORIAL_COMPLETED, 1);
		}
	}
}