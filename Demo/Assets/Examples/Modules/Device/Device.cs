using UnityEngine;
using UnityEngine.UI;
using Examples.Console;
using GamePush;

namespace Examples.Device
{
    public class Device : MonoBehaviour
    {
        [SerializeField] private Button _isMobileButton;
        [SerializeField] private Button _isPortraitButton;

        private void OnEnable()
        {
            _isMobileButton.onClick.AddListener(IsMobile);
            _isPortraitButton.onClick.AddListener(IsPortrait);

            GP_Device.OnChangeOrientation += OrientationChanged;
        }
        private void OnDisable()
        {
            _isMobileButton.onClick.RemoveListener(IsMobile);
            _isPortraitButton.onClick.RemoveListener(IsPortrait);

            GP_Device.OnChangeOrientation -= OrientationChanged;
        }


        void OrientationChanged()
        {
            ConsoleUI.Instance.Log("Device orientation changed");
        }

        public void IsMobile()
        {
            bool isMobile = GP_Device.IsMobile();
            ConsoleUI.Instance.Log("Is Mobile: " + isMobile);
        }

        public void IsPortrait()
        {
           bool isPortrait = GP_Device.IsPortrait();
           ConsoleUI.Instance.Log("Is Portrait: " + isPortrait);
        }
    }
}