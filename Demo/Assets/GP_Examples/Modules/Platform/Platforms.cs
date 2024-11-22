using UnityEngine;
using UnityEngine.UI;
using Examples.Console;
using GamePush;

namespace Examples.Platforms
{
    public class Platforms : MonoBehaviour
    {
        [SerializeField] private Button _definePlatform;

        private void OnEnable()
        {
            _definePlatform.onClick.AddListener(DefinePlatform);
        }
        private void OnDisable()
        {
            _definePlatform.onClick.RemoveListener(DefinePlatform);
        }

        public string Type() => GP_Platform.Type().ToString();
        public string Tag() => GP_Platform.Tag();

        public void DefinePlatform()
        {
            ConsoleUI.Instance.Log(Type());
            ConsoleUI.Instance.Log(Tag());
            ConsoleUI.Instance.Log("Has Integrated Auth: " + GP_Platform.HasIntegratedAuth().ToString());
            ConsoleUI.Instance.Log("Is External Links Allowed: " + GP_Platform.IsExternalLinksAllowed().ToString());
            ConsoleUI.Instance.Log("Is Logout Available: " + GP_Platform.IsLogoutAvailable().ToString());
            ConsoleUI.Instance.Log("Is SecretCode Auth Available: " + GP_Platform.IsSecretCodeAuthAvailable().ToString());
            ConsoleUI.Instance.Log("Is Supports Cloud Saves: " + GP_Platform.IsSupportsCloudSaves().ToString());
        }

        public void HasIntegratedAuth() => GP_Platform.HasIntegratedAuth();
        public void IsExternalLinksAllowed() => GP_Platform.IsExternalLinksAllowed();

        public void IsPluginReady()
        {
            ConsoleUI.Instance.Log("Plugin ready: " + GP_Init.isReady);
        }
    }
}