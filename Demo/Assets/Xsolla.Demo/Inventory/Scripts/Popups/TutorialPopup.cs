using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Core;
using Xsolla.Demo;

namespace Xsolla.Demo.Popup
{
	public class TutorialPopup : MonoBehaviour, ITutorialPopup
	{
		[SerializeField] private SimpleTextButton backButton = default;
		[SerializeField] private SimpleTextButton nextButton = default;
		[SerializeField] private SimpleTextButton startButton = default;
		[SerializeField] private SimpleTextButton cancelButton = default;

		[SerializeField] private SimpleButton closeButton = default;

		[SerializeField] private SimpleButton documentationButton = default;

		[SerializeField] private Text titleText = default;
		[SerializeField] private Text descriptionText = default;

		private TutorialInfo _tutorialInfo;
		private bool _showWelcomeMessage;

		private Action _onTutorialCompleted;

		private int _currentStepIndex;
		private RectTransform _highlightCopy;

		private bool IsTutorialInfoValid => _tutorialInfo != null && _tutorialInfo.tutorialSteps.Count > 0;
		private int FirstStepIndex => _showWelcomeMessage ? 0 : 1;

		private TutorialInfo.TutorialStep CurrentStepInfo => _tutorialInfo.tutorialSteps[_currentStepIndex];

		private void Awake()
		{
			backButton.onClick = MoveToPreviousStep;
			nextButton.onClick = MoveToNextStep;
			startButton.onClick = MoveToNextStep;

			cancelButton.onClick = () => { Destroy(gameObject, 0.001F); };
			closeButton.onClick = () => { Destroy(gameObject, 0.001F); };
		}

		public ITutorialPopup SetTutorialInfo(TutorialInfo info, bool showWelcomeMessage)
		{
			_tutorialInfo = info;
			_showWelcomeMessage = showWelcomeMessage;
			_currentStepIndex = FirstStepIndex;

			if (IsTutorialInfoValid)
			{
				startButton.Text = _tutorialInfo.tutorialSteps[0].nextButtonText;
				cancelButton.Text = _tutorialInfo.tutorialSteps[0].prevButtonText;
			}

			RefreshTutorial();

			return this;
		}

		public ITutorialPopup SetCancelCallback(Action onCancel)
		{
			cancelButton.onClick = () =>
			{
				onCancel?.Invoke();
				Destroy(gameObject, 0.001F);
			};
			return this;
		}

		public ITutorialPopup SetCompletionCallback(Action onComplete)
		{
			_onTutorialCompleted = onComplete;
			return this;
		}

		private void RefreshTutorial()
		{
			titleText.text = CurrentStepInfo.FormattedTitle;
			descriptionText.text = CurrentStepInfo.FormattedDescription;

			nextButton.Text = CurrentStepInfo.nextButtonText;
			backButton.Text = CurrentStepInfo.prevButtonText;

			RefreshBackgroundScreen();

			StartCoroutine(RefreshElementHighlight());

			RefreshButtonsVisibility();
		}

		private void RefreshBackgroundScreen()
		{
			if (CurrentStepInfo.screenToActivate != ActivationScreen.None)
			{
				if (CurrentStepInfo.screenToActivate == ActivationScreen.Inventory)
					DemoController.Instance.SetState(MenuState.Inventory);
				if (CurrentStepInfo.screenToActivate == ActivationScreen.MainMenu)
					DemoController.Instance.SetState(MenuState.Main);

				transform.SetAsLastSibling();
			}
		}

		private IEnumerator RefreshElementHighlight()
		{
			if (_highlightCopy != null)
				Destroy(_highlightCopy.gameObject);

			yield return new WaitForSeconds(0.1f);

			if (!string.IsNullOrEmpty(CurrentStepInfo.highlightElementId))
			{
				var highlightableObjects = GameObject.FindGameObjectsWithTag("Highlight");

				var highlightObject = highlightableObjects.FirstOrDefault(
					obj => obj.activeSelf && obj.GetComponent<HighlightElement>()?.ID == CurrentStepInfo.highlightElementId);

				if (highlightObject == null)
					yield break;

				var highlightObjectParent = (RectTransform) highlightObject.transform.parent;

				_highlightCopy = Instantiate(highlightObjectParent, transform);

				_highlightCopy.position = highlightObjectParent.position;
				_highlightCopy.sizeDelta = highlightObjectParent.sizeDelta;

				_highlightCopy.GetComponentInChildren<HighlightElement>().Highlight();
			}
		}

		private void RefreshButtonsVisibility()
		{
			if (_currentStepIndex == FirstStepIndex)
			{
				if (_showWelcomeMessage)
				{
					SetDefaultButtonsVisibility(false);
					SetWelcomeButtonsVisibility(true);
				}
				else
				{
					// 'Back' button should not be displayed for the first tutorial step in case there is no welcome message
					backButton.gameObject.SetActive(false);
				}
			}
			else
			{
				SetDefaultButtonsVisibility(true);
				SetWelcomeButtonsVisibility(false);
			}

			RefreshDocumentationButton();
		}

		private void RefreshDocumentationButton()
		{
			if (!string.IsNullOrEmpty(CurrentStepInfo.associatedDocumentation))
			{
				documentationButton.gameObject.SetActive(true);
				documentationButton.onClick = () => { XsollaWebBrowser.Open(CurrentStepInfo.associatedDocumentation, true); };
			}
			else
			{
				documentationButton.gameObject.SetActive(false);
				documentationButton.onClick = null;
			}
		}

		private void SetDefaultButtonsVisibility(bool show)
		{
			backButton.gameObject.SetActive(show);
			nextButton.gameObject.SetActive(show);
		}

		private void SetWelcomeButtonsVisibility(bool show)
		{
			startButton.gameObject.SetActive(show);
			cancelButton.gameObject.SetActive(show);
		}

		private void MoveToNextStep()
		{
			if (!IsTutorialInfoValid)
			{
				XDebug.LogError("Tutorial popup info is invalid or missing!");
				Destroy(gameObject, 0.001F);
				return;
			}

			_currentStepIndex++;

			if (_currentStepIndex < _tutorialInfo.tutorialSteps.Count)
			{
				RefreshTutorial();
				return;
			}

			_onTutorialCompleted?.Invoke();
			Destroy(gameObject, 0.001F);
		}

		private void MoveToPreviousStep()
		{
			if (!IsTutorialInfoValid)
			{
				XDebug.LogError("Tutorial popup info is invalid or missing!");
				Destroy(gameObject, 0.001F);
				return;
			}

			_currentStepIndex--;

			RefreshTutorial();
		}
	}
}