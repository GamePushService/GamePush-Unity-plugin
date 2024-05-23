using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Tools;

namespace GamePush
{
    public class GP_Game : MonoBehaviour
    {
        public static event UnityAction OnPause;
        public static event UnityAction OnResume;

        private static event Action _onPause;
        private static event Action _onResume;

        private void OnEnable()
        {
            CoreSDK.OnInit += SubToEvents;
        }

        private void SubToEvents()
        {
            CoreSDK.game.OnPause += CallOnPause;
            CoreSDK.game.OnResume += CallOnResume;
        }

        private void OnDisable()
        {
            CoreSDK.OnInit -= SubToEvents;

            if (CoreSDK.isInit)
            {
                CoreSDK.game.OnPause -= CallOnPause;
                CoreSDK.game.OnResume -= CallOnResume;
            }
            
        }

        [DllImport("__Internal")]
        private static extern string GP_IsPaused();
        public static bool IsPaused()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsPaused() == "true";
#else
            return CoreSDK.game.IsPaused();
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
            CoreSDK.game.GamePause(onPause);
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
            CoreSDK.game.GameResume(onResume);
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_GameplayStart();
        public static void GameplayStart()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStart();
#else
            CoreSDK.game.GameplayStart();
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_GameplayStop();
        public static void GameplayStop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_GameplayStop();
#else
            CoreSDK.game.GameplayStop();
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
#endif
        }

        private void CallOnPause() => OnPause?.Invoke();
        private void CallOnResume() => OnResume?.Invoke();
    }

}