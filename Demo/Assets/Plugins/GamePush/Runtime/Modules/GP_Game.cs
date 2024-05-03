using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Game : MonoBehaviour
    {
        public static event UnityAction OnPause;
        public static event UnityAction OnResume;

        private static event Action _onPause;
        private static event Action _onResume;


        [DllImport("__Internal")]
        private static extern string GP_IsPaused();
        public static bool IsPaused()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
           return GP_IsPaused() == "true";
#else
            Console.Log("GAME: IS PAUSED: ", CoreSDK.game.isPaused.ToString());
            return CoreSDK.game.isPaused;
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Pause();
        public static void Pause(Action onPause = null)
        {
            _onPause = onPause;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Pause();
#else
            CoreSDK.game.isPaused = true;
            Console.Log("GAME: ", "PAUSE");
            OnPause?.Invoke();
            _onPause?.Invoke();
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Resume();
        public static void Resume(Action onResume = null)
        {
            _onResume = onResume;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Resume();
#else
            CoreSDK.game.isPaused = false;
            Console.Log("GAME: ", "RESUME");
            OnResume?.Invoke();
            _onResume?.Invoke();
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_GameplayStart();
        public static void GameplayStart()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStart();
#else
            CoreSDK.game.isGameplay = true;
            Console.Log("GAMEPLAY: ", "START");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_GameplayStop();
        public static void GameplayStop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStop();
#else
            CoreSDK.game.isGameplay = false;
            Console.Log("GAMEPLAY: ", "STOP");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_GameReady();
        public static void GameReady()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameReady();
#else
            CoreSDK.game.GameReady();
            Console.Log("GAME:", "READY");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_HappyTime();
        public static void HappyTime()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_HappyTime();
#else
            CoreSDK.game.HappyTime();
            Console.Log("GAME:", "HAPPY TIME!!!");
#endif
        }


        private void CallOnPause() => OnPause?.Invoke();
        private void CallOnResume() => OnResume?.Invoke();
    }

}