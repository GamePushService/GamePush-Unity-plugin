using UnityEngine;
using GamePush;
using Examples.Console;

namespace Examples.Fullscreen
{
    public class Fullscreen : MonoBehaviour
    {
        private void OnEnable() => GP_Fullscreen.OnFullscreenChange += OnChange;
        private void OnDisable() => GP_Fullscreen.OnFullscreenChange += OnChange;


        public void Open() => GP_Fullscreen.Open(OnOpen);
        public void Close() => GP_Fullscreen.Close(OnClose);
        public void Toggle() => GP_Fullscreen.Toggle();

        public void IsFullscreenEnabled() => ConsoleUI.Instance.Log($"FULLSCREEN: {GP_Fullscreen.IsEnabled()}");

        private void OnOpen() => ConsoleUI.Instance.Log("FULLSCREEN: ON OPEN");
        private void OnClose() => ConsoleUI.Instance.Log("FULLSCREEN: ON CLOSE");
        private void OnChange() => ConsoleUI.Instance.Log("FULLSCREEN: ON CHANGE");
    }
}