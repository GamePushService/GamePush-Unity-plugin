using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Xsolla.Demo;
using Xsolla.UIBuilder;

namespace Xsolla.Core
{
	public class DevMenuPage : MonoBehaviour
	{
		[SerializeField] private SimpleButton BackButton = default;
		[SerializeField] private SimpleButton ResetButton = default;
		[SerializeField] private InputField LoginIdInput = default;
		[SerializeField] private InputField StoreProjectIdInput = default;
		[SerializeField] private InputField OAuthClientIdInput = default;
		[SerializeField] private InputField WebShopUrlInput = default;
		[SerializeField] private Dropdown UiThemeDropdown = default;

		private void OnEnable()
		{
			BackButton.onClick += OnBackButtonClick;
			ResetButton.onClick += OnResetButtonClick;

			LoginIdInput.onEndEdit.AddListener(OnLoginIdInputEndEdit);
			StoreProjectIdInput.onEndEdit.AddListener(OnStoreProjectIdInputEndEdit);
			OAuthClientIdInput.onEndEdit.AddListener(OnOAuthClientIdInputInputEndEdit);
			WebShopUrlInput.onEndEdit.AddListener(OnWebShopUrlEdit);
			UiThemeDropdown.onValueChanged.AddListener(OnUiThemeDropdownChanged);
		}

		private void OnDisable()
		{
			BackButton.onClick -= OnBackButtonClick;
			ResetButton.onClick -= OnResetButtonClick;

			LoginIdInput.onEndEdit.RemoveListener(OnLoginIdInputEndEdit);
			StoreProjectIdInput.onEndEdit.RemoveListener(OnStoreProjectIdInputEndEdit);
			OAuthClientIdInput.onEndEdit.RemoveListener(OnOAuthClientIdInputInputEndEdit);
			WebShopUrlInput.onEndEdit.RemoveListener(OnWebShopUrlEdit);
			UiThemeDropdown.onValueChanged.RemoveListener(OnUiThemeDropdownChanged);
		}

		private void Start()
		{
			RedrawFields();
		}

		private void OnBackButtonClick()
		{
			Destroy(gameObject);
		}

		private void OnResetButtonClick()
		{
			XsollaSettings.LoginId = Constants.DEFAULT_LOGIN_ID;
			XsollaSettings.StoreProjectId = Constants.DEFAULT_PROJECT_ID;
			XsollaSettings.OAuthClientId = Constants.DEFAULT_OAUTH_CLIENT_ID;
			DemoSettings.WebStoreUrl = Constants.DEFAULT_WEB_STORE_URL;
			ThemesLibrary.Current = ThemesLibrary.Themes.FirstOrDefault();

			RedrawFields();
		}

		private void OnLoginIdInputEndEdit(string value)
		{
			XsollaSettings.LoginId = value;
			RedrawFields();
		}

		private void OnStoreProjectIdInputEndEdit(string value)
		{
			XsollaSettings.StoreProjectId = value;
			RedrawFields();
		}

		private void OnOAuthClientIdInputInputEndEdit(string value)
		{
			if (int.TryParse(value, out var oAuthClientId))
				XsollaSettings.OAuthClientId = oAuthClientId;

			RedrawFields();
		}

		private void OnWebShopUrlEdit(string value)
		{
			DemoSettings.WebStoreUrl = value;
			RedrawFields();
		}

		private void OnUiThemeDropdownChanged(int index)
		{
			ThemesLibrary.Current = ThemesLibrary.Themes[index];
			SaveUiTheme();
			RedrawFields();
		}

		private void RedrawFields()
		{
			LoginIdInput.text = XsollaSettings.LoginId;
			StoreProjectIdInput.text = XsollaSettings.StoreProjectId;
			OAuthClientIdInput.text = XsollaSettings.OAuthClientId.ToString();
			WebShopUrlInput.text = DemoSettings.WebStoreUrl;

			var uiThemes = ThemesLibrary.Themes;
			UiThemeDropdown.options = uiThemes.Select(theme => new Dropdown.OptionData(theme.Name)).ToList();

			var uiThemeIndex = uiThemes.IndexOf(ThemesLibrary.Current);
			if (uiThemeIndex < 0)
				uiThemeIndex = 0;
			UiThemeDropdown.SetValueWithoutNotify(uiThemeIndex);

			var oAuthInputParent = OAuthClientIdInput.transform.parent.gameObject;
			oAuthInputParent.SetActive(true);
		}

		private const string UiThemeOverrideKey = "UiThemeOverride";

		private static void SaveUiTheme()
		{
			PlayerPrefs.SetString(UiThemeOverrideKey, ThemesLibrary.Current.Id);
		}

		public static void TryLoadUiTheme()
		{
			if (!PlayerPrefs.HasKey(UiThemeOverrideKey))
				return;

			var id = PlayerPrefs.GetString(UiThemeOverrideKey);
			ThemesLibrary.SetCurrentThemeById(id);
		}
	}
}