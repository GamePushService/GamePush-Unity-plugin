using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Data;
using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Variables : MonoBehaviour
    {
        public static event UnityAction<List<GameVariable>> OnFetchSuccess;
        public static event UnityAction OnFetchError;

        private static event Action<List<GameVariable>> _onSuccess;
        private static event Action _onError;

        public static event UnityAction<Dictionary<string, string>> OnPlatformFetchSuccess;
        public static event UnityAction<string> OnPlatformFetchError;

        private static event Action<Dictionary<string, string>> _onPlatformSuccess;
        private static event Action<string> _onPlatformError;

        [DllImport("__Internal")]
        private static extern void GP_Variables_Fetch();
        public static void Fetch(Action<List<GameVariable>> onFetchSuccess = null, Action onFetchError = null)
        {
            _onSuccess = onFetchSuccess;
            _onError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Variables_Fetch();
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("VARIABLES: ", "FETCH");

            CoreSDK.variables.Fetch(onFetchSuccess, onFetchError);
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


            return CoreSDK.variables.Has(key);
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

            return CoreSDK.variables.Get<int>(key);
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

            return CoreSDK.variables.Get<float>(key);
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

            return CoreSDK.variables.Get<string>(key);
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

            return CoreSDK.variables.Get<bool>(key);
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

            return CoreSDK.variables.Get<string>(key);
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

            return CoreSDK.variables.Get<string>(key);
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Variables_IsPlatformVariablesAvailable();
        public static bool IsPlatformVariablesAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_IsPlatformVariablesAvailable() == "true";
#else
            if (GP_ConsoleController.Instance.VariablesConsoleLogs)
                Console.Log("Platform Variables: ", "Is Available");

            return CoreSDK.variables.IsPlatformVariablesAvailable();
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Variables_FetchPlatformVariables(string options = null);

        public static void FetchPlatformVariables(Dictionary<string, string> optionsDict, Action<Dictionary<string, string>> onPlatformFetchSuccess = null, Action<string> onPlatformFetchError = null)
        {
            _onPlatformSuccess = onPlatformFetchSuccess;
            _onPlatformError = onPlatformFetchError;

            string options = CreateFetchOption(optionsDict);

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Variables_FetchPlatformVariables(options);
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Platform Variables: ", "Fetch" );
#endif
        }

        public static void FetchPlatformVariables(Action<Dictionary<string, string>> onPlatformFetchSuccess = null, Action<string> onPlatformFetchError = null)
        {
            _onPlatformSuccess = onPlatformFetchSuccess;
            _onPlatformError = onPlatformFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Variables_FetchPlatformVariables();
#else
            if (GP_ConsoleController.Instance.SystemConsoleLogs)
                Console.Log("Platform Variables: ", "FETCH");
#endif
        }

        private static string CreateFetchOption(Dictionary<string, string> dict)
        {
            string options = "";

            foreach(string key in dict.Keys)
            {
                string value = dict[key];

                options += key + ": " + value + ",";
            }

            options = options.Remove(options.Length - 1);

            return options;
        }

        private static string CreateClientParams(string options)
        {
            string[] pairs = options.Split(",");

            Dictionary<string, object> dict = new Dictionary<string, object>();

            foreach (string pair in pairs)
            {
                Debug.Log(pair);
                string[] keyValue = pair.Split(':');

                string key = keyValue[0].Trim().Trim('"');
                Debug.Log(key);
                object value = keyValue[1].Trim().Trim('"');
                Debug.Log(value);

                dict.Add(key, value);
            }

            string clientParams = GP_JSON.DictionaryToJson(dict);

            return clientParams;
        }

        private void CallVariablesFetchSuccess(string data)
        {
            var gameVariable = GP_JSON.GetList<GameVariable>(data);
            _onSuccess?.Invoke(gameVariable);
            OnFetchSuccess?.Invoke(gameVariable);
        }
        private void CallVariablesFetchError()
        {
            _onError?.Invoke();
            OnFetchError?.Invoke();
        }

        private void CallOnFetchPlatformVariables(string variables)
        {
            Dictionary<string, string> dictionary = MapToDictionary(variables);

            _onPlatformSuccess?.Invoke(dictionary);
            OnPlatformFetchSuccess?.Invoke(dictionary);
        }

        private void CallOnFetchPlatformVariablesError(string error)
        {
            _onPlatformError?.Invoke(error);
            OnPlatformFetchError?.Invoke(error);
        }

        public static Dictionary<string, string> MapToDictionary(string map)
        {
            Dictionary<string, string> dictionary = new Dictionary<string, string>();

            string trimmedString = map.TrimStart('{').TrimEnd('}');
            string[] pairs = trimmedString.Split(",");


            foreach (string pair in pairs)
            {
                string[] keyValue = pair.Split(':');

                string key = keyValue[0].Trim().Trim('"');
                string value = keyValue[1].Trim().Trim('"');

                dictionary.Add(key, value);
            }

            return dictionary;
        }

    }
    [System.Serializable]
    public class PlatformFetchVariables
    {
        public string clientParams;
    }
}