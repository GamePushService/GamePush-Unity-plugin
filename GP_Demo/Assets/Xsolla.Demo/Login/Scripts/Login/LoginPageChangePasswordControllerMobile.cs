using UnityEngine;
using Xsolla.Demo;

public class LoginPageChangePasswordControllerMobile : MonoBehaviour
{
	[SerializeField] SimpleButton backButton = default;

	public static bool IsBackNavigationTriggered { get; private set; }

	void Awake()
	{
		IsBackNavigationTriggered = false;

		backButton.onClick += () =>
		{
			IsBackNavigationTriggered = true;
			DemoController.Instance.SetPreviousState();
		};
	}
}