using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Core
{
    [System.Serializable]
    public class PlayerModule
    {
        protected List<PlayerField> data;

        protected Dictionary<string, string> keyTypeData;
        protected Dictionary<string, object> keyValueData;

        public Action OnPlayerChange;

        public PlayerModule()
        {
            
        }

        public PlayerModule(List<PlayerField> playerFields)
        {
            data = playerFields;
        }

        public void SetData(List<PlayerField> playerFields)
        {
            data = playerFields;
            keyTypeData = new Dictionary<string, string>();
            keyValueData = new Dictionary<string, object>();

            foreach (PlayerField field in data)
            {
                
                //Debug.Log(variable.key + " " + variable.type + " " + variable.value);
            }
        }

        public int GetID()
        {
            return 0;
        }

        public float GetScore()
        {
            return 0f;
        }

        public string GetName()
        {
            return "UNKNOWN";
        }

        public string GetAvatarUrl()
        {
            return "URL";
        }

        public string GetFieldName(string key)
        {
            return null;
        }

        public string GetFieldVariantName(string key, string value)
        {
            return null;
        }

        public string GetFieldVariantAt(string key, int index)
        {
            return null;
        }

        public string GetFieldVariantIndex(string key, string value)
        {
            return null;
        }

        public void SetName(string name)
        {

        }

        public void SetAvatar(string src)
        {

        }

        public void SetScore(int score)
        {

        }

        public void SetScore(float score)
        {

        }

        public void AddScore(int score)
        {

        }

        public void AddScore(float score)
        {

        }


        public T Get<T>(string key)
        {
            
            return default(T);
        }

        public void Set<T>(string key, T value)
        {

        }

        public void Add<T>(string key, T value)
        {

        }

        public void Toggle(string key)
        {

        }

        public void Reset()
        {

        }

        public void Remove()
        {

        }

        public void Sync(bool forceOverride = false)
        {

        }

        public void Load()
        {

        }

        public void Login()
        {

        }

        public void FetchFields(Action<List<PlayerFetchFieldsData>> onFetchFields = null)
        {

        }

        public bool Has(string key)
        {
            return true;
        }

        public bool IsLoggedIn()
        {
            return false;
        }

        public bool HasAnyCredentials()
        {
            return false;
        }

        public bool IsStub()
        {
            return true;
        }

        public int GetActiveDays()
        {
            return 0;
        }

        public int GetActiveDaysConsecutive()
        {
            return 0;
        }

        public int GetPlaytimeToday()
        {
            return 0;
        }

        public int GetPlaytimeAll()
        {
            return 0;
        }
    }
}
