using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Fullscreen : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Fullscreen);

        public static event UnityAction OnFullscreenOpen;
        public static event UnityAction OnFullscreenClose;
        public static event UnityAction OnFullscreenChange;

        private static event Action _onFullscreenOpen;
        private static event Action _onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Open();
        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Close();
        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Toggle();
#endif

        private void OnEnable()
        {
            CoreSDK.device.OnFullscreenChange += CallFullscreenChange;
            CoreSDK.device.OnFullscreenOpen += CallFullscreenOpen;
            CoreSDK.device.OnFullscreenClose += CallFullscreenClose;
        }

        private void OnDisable()
        {
            CoreSDK.device.OnFullscreenChange -= CallFullscreenChange;
            CoreSDK.device.OnFullscreenOpen -= CallFullscreenOpen;
            CoreSDK.device.OnFullscreenClose -= CallFullscreenClose;
        }

        public static void Open(Action onFullscreenOpen = null)
        {
            _onFullscreenOpen = onFullscreenOpen;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Open();
#else
            CoreSDK.device.OpenFullscreen();
#endif
        }

        public static void Close(Action onFullscreenClose = null)
        {
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Close();
#else
            CoreSDK.device.CloseFullscreen();
#endif
        }

        public static void Toggle()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Toggle();
#else
            CoreSDK.device.ToggleFullscreen();
#endif
        }

        private void CallFullscreenOpen() { _onFullscreenOpen?.Invoke(); OnFullscreenOpen?.Invoke(); }
        private void CallFullscreenClose() { _onFullscreenClose?.Invoke(); OnFullscreenClose?.Invoke(); }
        private void CallFullscreenChange() => OnFullscreenChange?.Invoke();
    }
}