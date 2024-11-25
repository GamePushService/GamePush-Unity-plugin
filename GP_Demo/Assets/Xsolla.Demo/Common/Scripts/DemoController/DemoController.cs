using UnityEngine;
using Xsolla.Core;
using Xsolla.Demo.Popup;

namespace Xsolla.Demo
{
	public partial class DemoController : MonoSingleton<DemoController>
	{
		[SerializeField] private MenuStateMachine stateMachine = default;
		[SerializeField] private UrlContainer urlContainer = default;
		[SerializeField] private XsollaSettingsValidator settingsValidator = default;

		public UrlContainer UrlContainer => urlContainer;
		public bool IsTutorialAvailable { get; private set; } = false;

		public override void Init()
		{
			base.Init();
			stateMachine.StateChangedEvent += OnStateChanged;
			InitTutorial();
		}

		private void Start()
		{
			var settingsAreValid = settingsValidator.ValidateXsollaSettings();

			if (settingsAreValid)
				stateMachine.SetInitialState();
			else
				stateMachine.SetState(MenuState.LoginSettingsError);
		}

		protected override void OnDestroy()
		{
			DestroyInstances();

			if (PopupFactory.IsExist)
				Destroy(PopupFactory.Instance.gameObject);

			base.OnDestroy();
		}

		public GameObject SetState(MenuState state)
		{
			if (stateMachine != null)
				stateMachine.SetState(state);

			return null;
		}

		public MenuState GetState()
		{
			if (stateMachine != null)
				return stateMachine.CurrentState;

			return MenuState.None;
		}

		public void SetPreviousState()
		{
			if (stateMachine != null)
				stateMachine.SetPreviousState();
		}

		public bool IsStateAvailable(MenuState state)
		{
			return stateMachine.IsStateAvailable(state);
		}

		public string GetWebStoreUrl()
		{
			return $"{DemoSettings.WebStoreUrl}?token={XsollaToken.AccessToken}&remember_me=false";
		}

		public void ShowTutorial(bool showWelcomeMessage) => ManualStartTutorial(showWelcomeMessage);

		private void OnStateChanged(MenuState lastState, MenuState newState)
		{
			if (lastState.IsPostAuthState() && newState.IsAuthState())
			{
				DestroyInstances();
				XsollaToken.DeleteSavedInstance();
			}

			if (lastState.IsAuthState() && newState == MenuState.Main)
			{
				UpdateStore();
				UpdateInventory();
				AutoStartTutorial();
			}
		}

		private void DestroyInstances()
		{
			if (UserFriends.IsExist)
				Destroy(UserFriends.Instance.gameObject);

			if (UserAttributes.IsExist)
				Destroy(UserAttributes.Instance.gameObject);

			DestroyInventory();
			DestroyStore();
		}

		partial void InitTutorial();
		partial void AutoStartTutorial();
		partial void ManualStartTutorial(bool showWelcomeMessage);

		partial void UpdateInventory();
		partial void UpdateStore();

		partial void DestroyInventory();
		partial void DestroyStore();
	}
}