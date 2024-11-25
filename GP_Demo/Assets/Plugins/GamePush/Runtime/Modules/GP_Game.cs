using System;
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

        public static bool IsPaused()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_IsPaused() == "true";
#else

            ConsoleLog("IS PAUSED: FALSE");
            return false;
#endif
        }

        public static void Pause(Action onPause = null)
        {
            _onPause = onPause;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Pause();
#else

            ConsoleLog("PAUSE");
            OnPause?.Invoke();
            _onPause?.Invoke();
#endif
        }

        public static void Resume(Action onResume = null)
        {
            _onResume = onResume;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Resume();
#else

            ConsoleLog("RESUME");
            OnResume?.Invoke();
            _onResume?.Invoke();
#endif
        }

        public static void GameplayStart()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStart();
#else

            Console.Log("GAMEPLAY: START");
#endif
        }

        public static void GameplayStop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStop();
#else

            Console.Log("GAMEPLAY: STOP");
#endif
        }

        public static void GameReady()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameReady();
#else

            Console.Log("GAME:", "READY");
#endif
        }

        public static void HappyTime()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_HappyTime();
#else

            Console.Log("GAME:", "HAPPY TIME!!!");
#endif
        }

        private void CallOnPause() => OnPause?.Invoke();
        private void CallOnResume() => OnResume?.Invoke();
    }

}