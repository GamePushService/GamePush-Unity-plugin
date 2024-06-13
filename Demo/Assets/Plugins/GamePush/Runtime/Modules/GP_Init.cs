using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace GamePush
{
    public class GP_Init : MonoBehaviour
    {
        public static bool isReady = false;

        public static Task Ready;
        public static event Action OnReady;
        public static event Action OnError;

        private void Awake()
        {

#if UNITY_EDITOR || !UNITY_WEBGL
            Console.Log("SDK ready");
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
    }
}