using System.Collections.Generic;
using UnityEngine;
using Xsolla.Demo;

public class LoginPageCreateControllerMobile : MonoBehaviour
{
	[SerializeField] SimpleButton loginButton = default;

	[SerializeField] CreateAccountStepCounter stepsCounter = default;

	[SerializeField] List<CreateAccountStepWidget> createAccountSteps = new List<CreateAccountStepWidget>();

	private int currentStep = 0;

	public static bool IsLoginNavigationTriggered { get; private set; }

	void Awake()
	{
		LoginPageCreateController.DropLastCredentials();

		IsLoginNavigationTriggered = false;

		loginButton.onClick += () =>
		{
			IsLoginNavigationTriggered = true;
			DemoController.Instance.SetPreviousState();
		};

		if (stepsCounter != null)
		{
			stepsCounter.SetCurrentStep(currentStep);
			stepsCounter.SetTotalSteps(createAccountSteps.Count);

			foreach (var step in createAccountSteps)
			{
				step.onBackButtonClick += ShowPreviousStep;
				step.onNextButtonClick += () => { ShowNextStep(); };
			}
		}
	}

	private void ShowNextStep()
	{
		if (currentStep < createAccountSteps.Count - 1)
		{
			createAccountSteps[currentStep].gameObject.SetActive(false);
			currentStep++;
			createAccountSteps[currentStep].gameObject.SetActive(true);
		}

		stepsCounter.SetCurrentStep(currentStep);
	}

	private void ShowPreviousStep()
	{
		if (currentStep > 0)
		{
			createAccountSteps[currentStep].gameObject.SetActive(false);
			currentStep--;
			createAccountSteps[currentStep].gameObject.SetActive(true);
		}

		stepsCounter.SetCurrentStep(currentStep);
	}
}