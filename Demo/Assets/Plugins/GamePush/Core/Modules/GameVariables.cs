using System;
using System.Collections;
using System.Collections.Generic;
using GamePush.Data;
using UnityEngine;
using GP_Utilities;

namespace GamePush.Core
{
    [System.Serializable]
    public class GameVariables
    {
        protected List<GameVariable> data;
        protected Dictionary<string, string> keyTypeData;
        protected Dictionary<string, object> keyValueData;

        protected List<FetchGameVariable> fetchGameVariables;

        public List<FetchGameVariable> FetchData()
        {
            if (fetchGameVariables != null)
                return fetchGameVariables;

            List<FetchGameVariable> fetchData = new List<FetchGameVariable>();
            foreach(string key in keyTypeData.Keys)
            {
                FetchGameVariable variable = new FetchGameVariable();
                variable.key = key;
                variable.value = keyValueData[key];
                variable.type = keyTypeData[key];

                fetchData.Add(variable);
            }
            fetchGameVariables = fetchData;

            return fetchGameVariables;
        }

        public GameVariables()
        {
            keyTypeData = new Dictionary<string, string>();
            keyValueData = new Dictionary<string, object>();
        }

        public GameVariables(List<GameVariable> gameVariables)
        {
            keyTypeData = new Dictionary<string, string>();
            keyValueData = new Dictionary<string, object>();

            SetData(gameVariables);
        }

        public void SetData(List<GameVariable> gameVariables)
        {
            data = gameVariables;
            foreach (GameVariable variable in data)
            {
                keyTypeData.Add(variable.key, variable.type);
                switch (variable.type)
                {
                    case "flag":
                        bool value = variable.value == "true";
                        keyValueData.Add(variable.key, value);
                        break;
                    case "stats":
                        if(int.TryParse(variable.value, out int intValue))
                            keyValueData.Add(variable.key, intValue);
                        else if (float.TryParse(variable.value, out float floatValue))
                            keyValueData.Add(variable.key, floatValue);
                        else
                            keyValueData.Add(variable.key, variable.value);
                        break;
                    default:
                        keyValueData.Add(variable.key, variable.value);
                        break;
                }
               
                //Debug.Log(variable.key + " " + variable.type + " " + variable.value);
            }
        }

        public bool Has(string key) => keyValueData.ContainsKey(key);

        public T Get<T>(string key)
        {
            if(keyValueData.TryGetValue(key, out object value))
            {
                try
                {
                    return (T)value;
                }
                catch
                {
                    return default(T);
                }
            }
               
            
            return default(T);
        }

        public bool IsPlatformVariablesAvailable()
        {
            return false;
        }

        public void Fetch(Action<List<FetchGameVariable>> onFetchSuccess = null, Action onFetchError = null)
        {
            if (data != null)
                onFetchSuccess(FetchData());
            else
                onFetchError();
        }

        public void FetchPlatformVariables(string optionsDict = null, Action<Dictionary<string, string>> onPlatformFetchSuccess = null, Action<string> onPlatformFetchError = null)
        {
            Console.Error("Can't fetch platform variables");
            onPlatformFetchError?.Invoke("Platform doesn't have variables");
        }

    }
}
