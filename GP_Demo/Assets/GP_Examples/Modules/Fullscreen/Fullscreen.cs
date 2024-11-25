using UnityEngine;

using GamePush;

namespace Examples.Fullscreen
{
    public class Fullscreen : MonoBehaviour
    {
        private void OnEnable() => GP_Fullscreen.OnFullscreenChange += OnChange;
        private void OnDisable() => GP_Fullscreen.OnFullscreenChange += OnChange;


        public void Open() => GP_Fullscreen.Open(OnOpen);
        public void Close() => GP_Fullscreen.Close(OnClose);
        public void Toggle() => GP_Fullscreen.Toggle();


        private void OnOpen() => Debug.Log("FULLSCREEN: ON OPEN");
        private void OnClose() => Debug.Log("FULLSCREEN: ON CLOSE");
        private void OnChange() => Debug.Log("FULLSCREEN: ON CHANGE");
    }
}