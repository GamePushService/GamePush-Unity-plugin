using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GamePush.Data;
using UnityEngine.Rendering;

namespace GamePush
{
    public class GP_Init : GP_Module
    {
        public static bool isReady = false;

        public static Task Ready;
        public static event Action OnReady;
        public static event Action OnError;

        private void OnEnable()
        {
            OnReady += Autocalls;
        }

        private void OnDisable()
        {
            OnReady -= Autocalls;
        }

        private void Autocalls() => StartCoroutine(AutocallsCoroutine());
        
        private void Start()
        {

#if UNITY_EDITOR || !UNITY_WEBGL
            GP_Logger.SystemLog("SDK ready");
            CallOnSDKReady();
#endif
        }

        IEnumerator AutocallsCoroutine()
        {
            while (!SplashScreen.isFinished)
            {
                yield return null;
            }

            if (ProjectData.SHOW_STICKY_ON_START)
                GP_Ads.ShowSticky();

            if (ProjectData.GAMEREADY_AUTOCALL)
                GP_Game.GameReady();
            
        }

        private void CallOnSDKReady()
        {
            isReady = true;
            OnReady?.Invoke();
        }

        private void CallOnSDKError()
        {
            OnError?.Invoke();
        }

    }
}