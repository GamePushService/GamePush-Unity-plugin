using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Custom : MonoBehaviour
    {

        public static event UnityAction<string> OnCustomAsyncReturn;
        public static event UnityAction<string> OnCustomAsyncError;

        public static event Action<string> _onCustomAsyncReturn;
        public static event Action<string> _onCustomAsyncError;

        
        [DllImport("__Internal")]
        private static extern void GP_CustomCall(string name, string args);
        public static void Call(string name, string args = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
             GP_CustomCall(name, args);
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("CUSTOM Call: ", "Test");
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_CustomGetValue(string path);
        public static string Value(string path)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_CustomGetValue(path);
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("CUSTOM value: ", "Test");
            return null;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_CustomReturn(string name, string args);
        public static string Return(string name, string args = null)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string result = GP_CustomReturn(name, args);
            return result;
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("CUSTOM Return: ", "Test");
            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_CustomAsyncReturn(string name, string args);
        public static void AsyncReturn(string name, string args = null, Action<string> onCustomAsyncReturn = null, Action<string> onCustomAsyncError = null)
        {
            _onCustomAsyncReturn = onCustomAsyncReturn;
            _onCustomAsyncError = onCustomAsyncError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_CustomAsyncReturn(name, args);
#else
            if (GP_ConsoleController.Instance.AppConsoleLogs)
                Console.Log("CUSTOM Async Return: ", "Test");
#endif
        }

        private void CallCustomAsyncReturn(string result)
        {
            OnCustomAsyncReturn?.Invoke(result);
            _onCustomAsyncReturn?.Invoke(result);
        }

        private void CallCustomAsyncError(string error)
        {
            OnCustomAsyncError?.Invoke(error);
            _onCustomAsyncError?.Invoke(error);
        }



    }

}
