using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Native;

namespace GamePush
{
    public class GP_Fullscreen : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Custom);

        public static event UnityAction OnFullscreenOpen;
        public static event UnityAction OnFullscreenClose;
        public static event UnityAction OnFullscreenChange;

        private static event Action _onFullscreenOpen;
        private static event Action _onFullscreenClose;


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Open();
        #endif
        public static void Open(Action onFullscreenOpen = null)
        {
            _onFullscreenOpen = onFullscreenOpen;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Open();
#else
            if (GamePushHost.UseNativeCore)
            {
                SetNativeFullscreen(true);
                return;
            }
            if (GP_Play2Web.Call("FullscreenOpen"))
                return;
            ConsoleLog("OPEN");
            OnFullscreenOpen?.Invoke();
            _onFullscreenOpen?.Invoke();
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Close();
        #endif
        public static void Close(Action onFullscreenClose = null)
        {
            _onFullscreenClose = onFullscreenClose;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Close();
#else
            if (GamePushHost.UseNativeCore)
            {
                SetNativeFullscreen(false);
                return;
            }
            if (GP_Play2Web.Call("FullscreenClose"))
                return;
            ConsoleLog("CLOSE");
            OnFullscreenClose?.Invoke();
            _onFullscreenClose?.Invoke();
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Fullscreen_Toggle();
        #endif
        public static void Toggle()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Fullscreen_Toggle();
#else
            if (GamePushHost.UseNativeCore)
            {
                SetNativeFullscreen(!IsFullscreen());
                return;
            }
            if (GP_Play2Web.Call("FullscreenToggle"))
                return;
            ConsoleLog("TOGGLE");
#endif
        }

        /// <summary>
        /// Android and other handhelds always render fullscreen; on desktop this mirrors
        /// <see cref="Screen.fullScreen"/>.
        /// </summary>
        public static bool IsFullscreen()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return Screen.fullScreen;
#else
            if (GamePushHost.UseNativeCore)
                return IsAlwaysFullscreenPlatform || Screen.fullScreen;
            if (GP_Play2Web.TryGetBool("FullscreenIsFullscreen", out var live))
                return live;
            return Screen.fullScreen;
#endif
        }

        private static bool IsAlwaysFullscreenPlatform =>
            Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer;

        private static void SetNativeFullscreen(bool value)
        {
            if (IsAlwaysFullscreenPlatform)
            {
                // Nothing to switch, but callers still expect their callback to run.
                if (value)
                {
                    OnFullscreenOpen?.Invoke();
                    _onFullscreenOpen?.Invoke();
                }
                else
                {
                    OnFullscreenClose?.Invoke();
                    _onFullscreenClose?.Invoke();
                }
                OnFullscreenChange?.Invoke();
                return;
            }

            if (Screen.fullScreen == value)
            {
                if (value)
                {
                    OnFullscreenOpen?.Invoke();
                    _onFullscreenOpen?.Invoke();
                }
                else
                {
                    OnFullscreenClose?.Invoke();
                    _onFullscreenClose?.Invoke();
                }
                return;
            }

            Screen.fullScreenMode = value ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
            Screen.fullScreen = value;
            GP_FullscreenWatcher.Ensure();
        }

        internal static void NativeFireChanged(bool isFullscreen)
        {
            if (isFullscreen)
            {
                OnFullscreenOpen?.Invoke();
                _onFullscreenOpen?.Invoke();
            }
            else
            {
                OnFullscreenClose?.Invoke();
                _onFullscreenClose?.Invoke();
            }
            OnFullscreenChange?.Invoke();
        }


        private void CallFullscreenOpen() { _onFullscreenOpen?.Invoke(); OnFullscreenOpen?.Invoke(); }
        private void CallFullscreenClose() { _onFullscreenClose?.Invoke(); OnFullscreenClose?.Invoke(); }
        private void CallFullscreenChange() => OnFullscreenChange?.Invoke();
    }
}