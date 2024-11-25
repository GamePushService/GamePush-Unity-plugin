using UnityEngine;

namespace Xsolla.Demo
{
	public class StorePageCommonButtonsController : BaseMenuController
	{
		[SerializeField] private SimpleButton publisherAccountButton = default;
		[SerializeField] private SimpleButton documentationButton = default;
		[SerializeField] private SimpleButton testCardsButton = default;
		[SerializeField] private SimpleButton backButton = default;
		[SerializeField] private SimpleButton tutorialButton = default;

		private void Start()
		{
			AttachUrlToButton(publisherAccountButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.PublisherUrl));
			AttachUrlToButton(documentationButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.DocumentationUrl));
			AttachUrlToButton(testCardsButton, DemoController.Instance.UrlContainer.GetUrl(UrlType.TestCardsUrl));
			AttachButtonCallback(backButton, () => DemoController.Instance.SetPreviousState());

			if (DemoController.Instance.IsTutorialAvailable)
			{
				tutorialButton.gameObject.SetActive(true);
				AttachButtonCallback(tutorialButton, () => DemoController.Instance.ShowTutorial(false));
			}
		}
	}
}
