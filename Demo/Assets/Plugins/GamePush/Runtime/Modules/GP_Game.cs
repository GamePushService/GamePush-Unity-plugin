﻿using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Game : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Custom);

        public static event UnityAction OnPause;
        public static event UnityAction OnResume;

        private static event Action _onPause;
        private static event Action _onResume;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_IsPaused();
        [DllImport("__Internal")]
        private static extern void GP_Pause();
        [DllImport("__Internal")]
        private static extern void GP_Resume();
        [DllImport("__Internal")]
        private static extern void GP_GameplayStart();
        [DllImport("__Internal")]
        private static extern void GP_GameplayStop();
        [DllImport("__Internal")]
        private static extern void GP_GameReady();
        [DllImport("__Internal")]
        private static extern void GP_HappyTime();
#endif
        private void OnEnable()
        {
            CoreSDK.game.OnPause += CallOnPause;
            CoreSDK.game.OnResume += CallOnResume;
        }

        private void OnDisable()
        {
            CoreSDK.game.OnPause -= CallOnPause;
            CoreSDK.game.OnResume -= CallOnResume;
        }

        public static bool IsPaused()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsPaused() == "true";
#else
            return CoreSDK.game.IsPaused();
#endif
        }

        public static void Pause(Action onPause = null)
        {
            _onPause = onPause;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Pause();
#else
            CoreSDK.game.GamePause();
#endif
        }

        public static void Resume(Action onResume = null)
        {
            _onResume = onResume;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Resume();
#else
            CoreSDK.game.GameResume();
#endif
        }

        public static void GameplayStart()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStart();
#else
            CoreSDK.game.GameplayStart();
#endif
        }

        public static void GameplayStop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStop();
#else
            CoreSDK.game.GameplayStop();
#endif
        }

        public static void GameReady()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameReady();
#else
            CoreSDK.game.GameReady();
#endif
        }

        public static void HappyTime()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_HappyTime();
#else
            CoreSDK.game.HappyTime();
#endif
        }

        private void CallOnPause()
        {
            OnPause?.Invoke();
            _onPause?.Invoke();
        }
        private void CallOnResume()
        {
            OnResume?.Invoke();
            _onResume?.Invoke();
        }
    }

}