using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Fullscreen : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Custom);

        public static event UnityAction OnFullscreenOpen;
        public static event UnityAction OnFullscreenClose;
        public static event UnityAction OnFullscreenChange;

        private static event Action _onFullscreenOpen;
        private static event Action _onFullscreenClose;


        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Open();
        public static void Open(Action onFullscreenOpen = null)
        {
            _onFullscreenOpen = onFullscreenOpen;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Open();
#else

            ConsoleLog("OPEN");
            OnFullscreenOpen?.Invoke();
            _onFullscreenOpen?.Invoke();
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Close();
        public static void Close(Action onFullscreenClose = null)
        {
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Close();
#else

            ConsoleLog("CLOSE");
            OnFullscreenClose?.Invoke();
            _onFullscreenClose?.Invoke();
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Toggle();
        public static void Toggle()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Toggle();
#else

            ConsoleLog("TOGGLE");
#endif
        }


        private void CallFullscreenOpen() { _onFullscreenOpen?.Invoke(); OnFullscreenOpen?.Invoke(); }
        private void CallFullscreenClose() { _onFullscreenClose?.Invoke(); OnFullscreenClose?.Invoke(); }
        private void CallFullscreenChange() => OnFullscreenChange?.Invoke();
    }
}