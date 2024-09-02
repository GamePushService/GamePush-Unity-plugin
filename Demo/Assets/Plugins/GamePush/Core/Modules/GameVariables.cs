using System;
using System.Collections;
using System.Collections.Generic;
using GamePush.Data;
using GamePush.Tools;

namespace GamePush.Core
{
    [System.Serializable]
    public class GameVariables
    {
        protected List<GameVariable> variablesData;
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

        public void SetVariablesData(List<GameVariable> gameVariables)
        {
            variablesData = gameVariables;

            foreach (GameVariable variable in variablesData)
            {
                keyTypeData.TryAdd(variable.key, variable.type);
                switch (variable.type)
                {
                    case "flag":
                        bool value = variable.value == "true";
                        keyValueData.TryAdd(variable.key, value);
                        break;
                    case "stats":
                        if(int.TryParse(variable.value, out int intValue))
                            keyValueData.TryAdd(variable.key, intValue);
                        else if (float.TryParse(variable.value, out float floatValue))
                            keyValueData.TryAdd(variable.key, floatValue);
                        else
                            keyValueData.TryAdd(variable.key, variable.value);
                        break;
                    default:
                        keyValueData.TryAdd(variable.key, variable.value);
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
                Helpers.ConvertValue<T>(value);
            }
            
            return default(T);
        }

        public bool IsPlatformVariablesAvailable()
        {
            return false;
        }

        public void Fetch(Action<List<FetchGameVariable>> onFetchSuccess = null, Action onFetchError = null)
        {
            if (variablesData != null)
                onFetchSuccess(FetchData());
            else
                onFetchError();
        }

        public void FetchPlatformVariables(string optionsDict = null, Action<Dictionary<string, string>> onPlatformFetchSuccess = null, Action<string> onPlatformFetchError = null)
        {
            Logger.Error("Can't fetch platform variables");
            onPlatformFetchError?.Invoke("Platform doesn't have variables");
        }

    }
}
