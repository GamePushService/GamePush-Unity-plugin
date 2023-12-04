using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Variables : MonoBehaviour
    {
        public static event UnityAction<List<VariablesData>> OnFetchSuccess;
        public static event UnityAction OnFetchError;

        private static event Action<List<VariablesData>> _onSuccess;
        private static event Action _onError;

        [DllImport("__Internal")]
        private static extern void GP_Variables_Fetch();
        public static void Fetch(Action<List<VariablesData>> onFetchSuccess = null, Action onFetchError = null)
        {
            _onSuccess = onFetchSuccess;
            _onError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Variables_Fetch();
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("VARIABLES: ", "FETCH");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Variables_Has(string key);
        public static bool Has(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_Has(key) == "true";
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("VARIABLES: HAS: ", key + " -> TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern int GP_Variables_GetNumberInt(string key);
        public static int GetInt(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_GetNumberInt(key);
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("VARIABLES: GET INT: ", key + " -> 0");
            return 0;
#endif
        }

        [DllImport("__Internal")]
        private static extern float GP_Variables_GetFloat(string key);
        public static float GetFloat(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_GetFloat(key);
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("VARIABLES: GET FLOAT: ", key + " -> 0f");
            return 0f;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Variables_GetString(string key);
        public static string GetString(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_GetString(key);
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("VARIABLES: GET STRING: ", key + " -> NULL");
            return null;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Variables_GetBool(string key);
        public static bool GetBool(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_GetBool(key) == "true";
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("VARIABLES: GET BOOL: ", key + " -> TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Variables_GetImage(string key);
        public static string GetImage(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_GetImage(key);
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("VARIABLES: GET IMAGE: ", key + " -> URL");
            return "URL";
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Variables_GetFile(string key);
        public static string GetFile(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_GetFile(key);
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("VARIABLES: GET FILE: ", key + " -> URL");
            return "URL";
#endif
        }

        private void CallVariablesFetchSuccess(string data)
        {
            var variablesData = GP_JSON.GetList<VariablesData>(data);
            _onSuccess?.Invoke(variablesData);
            OnFetchSuccess?.Invoke(variablesData);
        }
        private void CallVariablesFetchError() { _onError?.Invoke(); OnFetchError?.Invoke(); }

    }

    [System.Serializable]
    public class VariablesData
    {
        public string key;
        public string type;
        public string value;
    }
}