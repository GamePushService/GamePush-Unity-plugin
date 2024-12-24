using System;
using System.Collections;
using System.Collections.Generic;
using GamePush.Data;
using GamePush.Tools;

namespace GamePush.Core
{
    public class GameVariablesModule
    {
        protected List<GameVariableConfigData> variablesData;
        protected Dictionary<string, string> keyTypeData;
        protected Dictionary<string, object> keyValueData;

        protected List<FetchGameVariable> fetchGameVariables;

        public event Action<List<FetchGameVariable>> OnFetchSuccess;
        public event Action OnFetchError;

        public event Action<Dictionary<string, string>> OnPlatformFetchSuccess;
        public event Action<string> OnPlatformFetchError;

        public List<FetchGameVariable> FetchData()
        {
            if (fetchGameVariables != null)
                return fetchGameVariables;

            Logger.Log("fetchGameVariables null");

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

        public GameVariablesModule()
        {
            keyTypeData = new Dictionary<string, string>();
            keyValueData = new Dictionary<string, object>();
        }

        public void Init(List<GameVariableConfigData> gameVariables) => SetVariablesData(gameVariables);

        public void SetVariablesData(List<GameVariableConfigData> gameVariables)
        {
            variablesData = gameVariables;

            foreach (GameVariableConfigData variable in variablesData)
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
               
            }
        }

        public void SetVariablesData(List<FetchGameVariable> gameVariables)
        {
            variablesData = new List<GameVariableConfigData>();

            foreach (FetchGameVariable variable in gameVariables)
            {
                variablesData.Add(new GameVariableConfigData(variable.key, variable.value.ToString(), variable.type));
                keyTypeData.TryAdd(variable.key, variable.type);
                switch (variable.type)
                {
                    case "flag":
                        bool value = (bool)variable.value;
                        keyValueData.TryAdd(variable.key, value);
                        break;
                    case "stats":
                        if (int.TryParse(variable.value.ToString(), out int intValue))
                            keyValueData.TryAdd(variable.key, intValue);
                        else if (float.TryParse(variable.value.ToString(), out float floatValue))
                            keyValueData.TryAdd(variable.key, floatValue);
                        else
                            keyValueData.TryAdd(variable.key, variable.value);
                        break;
                    default:
                        keyValueData.TryAdd(variable.key, variable.value.ToString());
                        break;
                }
            }

            //Logger.Log("keyTypeData count:" + keyTypeData.Count);
        }

        public bool Has(string key) => keyValueData.ContainsKey(key);
        
        public T Get<T>(string key)
        {
            if(keyValueData.TryGetValue(key, out object value))
            {
                return Helpers.ConvertValue<T>(value);
            }
            
            return default(T);
        }

        public bool IsPlatformVariablesAvailable()
        {
            return false;
        }

        public async void Fetch()
        {
            if (variablesData != null && variablesData.Count > 0)
            {
                OnFetchSuccess(FetchData());
                return;
            }

            fetchGameVariables = await DataFetcher.FetchPlayerProjectVariables(false);
            if (fetchGameVariables != null)
            {
                SetVariablesData(fetchGameVariables);
                OnFetchSuccess(FetchData());
            }
            else
                OnFetchError();
        }

        public void FetchPlatformVariables(string optionsDict = null)
        {
            Logger.Error("Can't fetch platform variables");
            OnPlatformFetchError?.Invoke("Platform doesn't have variables");
        }

    }
}
