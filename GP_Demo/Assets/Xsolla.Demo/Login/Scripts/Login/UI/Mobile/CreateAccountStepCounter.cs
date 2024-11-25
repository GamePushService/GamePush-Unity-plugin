using UnityEngine;
using UnityEngine.UI;

public class CreateAccountStepCounter : MonoBehaviour
{
	[SerializeField] private Text currentStep;
	[SerializeField] private Text totalSteps;

	public void SetCurrentStep(int step)
	{
		currentStep.text = $"{step + 1} / ";
	}
	
	public void SetTotalSteps(int stepsCount)
	{
		totalSteps.text = $"{stepsCount} steps";
	}
}