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

        public List<GameVariable> GetData() => data;

        public void SetData(List<GameVariable> gameVariables)
        {
            data = gameVariables;
            foreach (GameVariable variable in data)
            {
                keyTypeData.Add(variable.key, variable.type);
                keyValueData.Add(variable.key, variable.value);
            }
        }

        public bool Has(string key) => keyValueData.ContainsKey(key);

        public T Get<T>(string key)
        {
            if(keyValueData.TryGetValue(key, out object value))
                return (T)value;
            
            return default(T);
        }

        public bool IsPlatformVariablesAvailable()
        {
            return false;
        }

        public void Fetch(Action<List<GameVariable>> onFetchSuccess = null, Action onFetchError = null)
        {
            if (data != null)
                onFetchSuccess(data);
            else
                onFetchError();
        }
    }
}
