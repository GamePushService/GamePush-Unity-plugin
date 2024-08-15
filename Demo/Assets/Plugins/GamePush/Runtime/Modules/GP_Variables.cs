using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.Utilities;

namespace GamePush
{
    public class GP_Variables : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Variables);

        public static event UnityAction<List<VariablesData>> OnFetchSuccess;
        public static event UnityAction OnFetchError;

        private static event Action<List<VariablesData>> _onSuccess;
        private static event Action _onError;

        public static event UnityAction<Dictionary<string, string>> OnPlatformFetchSuccess;
        public static event UnityAction<string> OnPlatformFetchError;

        private static event Action<Dictionary<string, string>> _onPlatformSuccess;
        private static event Action<string> _onPlatformError;

        [DllImport("__Internal")]
        private static extern void GP_Variables_Fetch();
        public static void Fetch(Action<List<VariablesData>> onFetchSuccess = null, Action onFetchError = null)
        {
            _onSuccess = onFetchSuccess;
            _onError = onFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Variables_Fetch();
#else

            ConsoleLog("FETCH");
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Variables_Has(string key);
        public static bool Has(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_Has(key) == "true";
#else

            ConsoleLog("HAS: " + key + " -> TRUE");
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

            ConsoleLog("GET INT: " + key + " -> 0");
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

            ConsoleLog("GET FLOAT: " + key + " -> 0f");
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

            ConsoleLog("GET STRING: " + key + " -> NULL");
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

            ConsoleLog("GET BOOL: " + key + " -> TRUE");
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

            ConsoleLog("GET IMAGE: " + key + " -> URL");
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

            ConsoleLog("GET FILE: " + key + " -> URL");
            return "URL";
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Variables_IsPlatformVariablesAvailable();
        public static bool IsPlatformVariablesAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Variables_IsPlatformVariablesAvailable() == "true";
#else

            Console.Log("Platform Variables: " + "Is Available");
            return true;
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

            Console.Log("Platform Variables: " + "Fetch");
#endif
        }

        public static void FetchPlatformVariables(Action<Dictionary<string, string>> onPlatformFetchSuccess = null, Action<string> onPlatformFetchError = null)
        {
            _onPlatformSuccess = onPlatformFetchSuccess;
            _onPlatformError = onPlatformFetchError;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Variables_FetchPlatformVariables();
#else

            Console.Log("Platform Variables: " + "FETCH");
#endif
        }

        private static string CreateFetchOption(Dictionary<string, string> dict)
        {
            string options = "";

            foreach (string key in dict.Keys)
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

            string clientParams = UtilityJSON.DictionaryToJson(dict);

            return clientParams;
        }



        private void CallVariablesFetchSuccess(string data)
        {
            var variablesData = UtilityJSON.GetList<VariablesData>(data);
            _onSuccess?.Invoke(variablesData);
            OnFetchSuccess?.Invoke(variablesData);
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


    [System.Serializable]
    public class VariablesData
    {
        public string key;
        public string type;
        public string value;
    }
}