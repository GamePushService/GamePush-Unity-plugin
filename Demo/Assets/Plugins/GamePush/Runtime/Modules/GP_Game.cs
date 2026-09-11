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


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_IsPaused();
        #endif
        public static bool IsPaused()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_IsPaused() == "true";
#else
            if (GP_Play2Web.TryGetBool("IsPaused", out var live))
                return live;
#if UNITY_EDITOR
            bool paused = GP_AdsStub.Enabled && GP_AdsStub.IsPaused;
            ConsoleLog("IS PAUSED: " + paused);
            return paused;
#else
            ConsoleLog("IS PAUSED: FALSE");
            return false;
#endif
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Pause();
        #endif
        public static void Pause(Action onPause = null)
        {
            _onPause = onPause;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Pause();
#else
            if (GP_Play2Web.Call("Pause"))
                return;
            ConsoleLog("PAUSE");
            OnPause?.Invoke();
            _onPause?.Invoke();
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Resume();
        #endif
        public static void Resume(Action onResume = null)
        {
            _onResume = onResume;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Resume();
#else
            if (GP_Play2Web.Call("Resume"))
                return;
            ConsoleLog("RESUME");
            OnResume?.Invoke();
            _onResume?.Invoke();
#endif
        }


        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_GameplayStart();
        #endif
        public static void GameplayStart()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStart();
#else
            if (GP_Play2Web.Call("GameplayStart"))
                return;
            Console.Log("GAMEPLAY: START");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_GameplayStop();
        #endif
        public static void GameplayStop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStop();
#else
            if (GP_Play2Web.Call("GameplayStop"))
                return;
            Console.Log("GAMEPLAY: STOP");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_GameReady();
        #endif
        public static void GameReady()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameReady();
#else
            if (GP_Play2Web.Call("GameReady"))
                return;
            Console.Log("GAME:", "READY");
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_HappyTime();
        #endif
        public static void HappyTime()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_HappyTime();
#else
            if (GP_Play2Web.Call("HappyTime"))
                return;
            Console.Log("GAME:", "HAPPY TIME!!!");
#endif
        }


        internal static void FirePause() => OnPause?.Invoke();
        internal static void FireResume() => OnResume?.Invoke();

        private void CallOnPause() => FirePause();
        private void CallOnResume() => FireResume();
    }

}