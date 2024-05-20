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

        public void DefinePlatform()
        {
            ConsoleUI.Instance.Log(Type());
            ConsoleUI.Instance.Log("Has Integrated Auth: " + GP_Platform.HasIntegratedAuth().ToString());
            ConsoleUI.Instance.Log("Is External Links Allowed: " + GP_Platform.IsExternalLinksAllowed().ToString());
        }

        public void HasIntegratedAuth() => GP_Platform.HasIntegratedAuth();
        public void IsExternalLinksAllowed() => GP_Platform.IsExternalLinksAllowed();
    }
}