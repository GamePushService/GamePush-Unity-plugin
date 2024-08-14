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
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Init);

        public static bool isReady = false;

        public static Task Ready;
        public static event Action OnReady;
        public static event Action OnError;

        private void OnEnable()
        {
            OnReady += GRA;
        }

        private void OnDisable()
        {
            OnReady -= GRA;
        }

        private void GRA() => StartCoroutine(GameReadyAutocall());

        private void Start()
        {

#if UNITY_EDITOR || !UNITY_WEBGL
            GP_Logger.SystemLog("SDK ready");
            CallOnSDKReady();
#endif
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

        IEnumerator GameReadyAutocall()
        {
            while (!SplashScreen.isFinished)
            {
                yield return null;
            }
            if (ProjectData.GAMEREADY_AUTOCALL)
            {
                GP_Game.GameReady();
            }
        }

    }
}