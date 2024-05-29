using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;
using Newtonsoft.Json;
using System.Linq;
using System.Text;

namespace GamePush.Core
{
    [System.Serializable]
    public class PlayerModule
    {

        private Dictionary<string, object> playerState = new Dictionary<string, object>
        {
            { "id", 0 },
            { "active", true },
            { "removed", false },
            { "test", false },
            { "score", 0 },
            { "name", "" },
            { "avatar", "" }
        };

        public Newtonsoft.Json.Linq.JObject GetPlayerState()
        {
            string json = JsonConvert.SerializeObject(playerState);
            var obj = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
            var sortedObj = new Newtonsoft.Json.Linq.JObject(obj.Properties().OrderBy(p => p.Name));
            return sortedObj;
        }

        protected List<PlayerField> data;
        protected Dictionary<string, object> keyValueData;

        public Action OnPlayerChange;


        #region SecretCode

        private string SECRET_CODE_KEY = "xPlayerSecretCode";

        public string GetPlayerDataCode()
        {
            if (PlayerPrefs.HasKey(SECRET_CODE_KEY))
            {
                Debug.Log(PlayerPrefs.GetString(SECRET_CODE_KEY));
                return PlayerPrefs.GetString(SECRET_CODE_KEY);
            }

            return null;
        }

        public void SetPlayerDataCode(string code)
        {
            PlayerPrefs.SetString(SECRET_CODE_KEY, code);
        }
        #endregion


        public async Task FetchPlayerConfig()
        {
            SyncPlayerInput playerInput = new SyncPlayerInput();
            if (GetPlayerDataCode() != null)
            {

                playerInput.playerState = GetPlayerState();
                playerInput.isFirstRequest = false;

                await DataFetcher.GetPlayer(playerInput, false);
            }
            else
            {
                playerInput.playerState = GetPlayerState();
                playerInput.isFirstRequest = true;

                await DataFetcher.SyncPlayer(playerInput, false);
            }
        }

        public PlayerModule()
        {
            keyValueData = new Dictionary<string, object>();
        }

        public PlayerModule(List<PlayerField> playerFields)
        {
            SetData(playerFields);
        }

        public void SetData(List<PlayerField> playerFields)
        {
            data = playerFields;
            keyValueData = new Dictionary<string, object>();

            foreach (PlayerField field in data)
            {
                switch (field.type)
                {
                    case "flag":
                        bool value = field.@default == "true";
                        keyValueData.Add(field.key, value);
                        break;
                    case "stats":
                        if (int.TryParse(field.@default, out int intValue))
                            keyValueData.Add(field.key, intValue);
                        else if (float.TryParse(field.@default, out float floatValue))
                            keyValueData.Add(field.key, floatValue);
                        else
                            keyValueData.Add(field.key, field.@default);
                        break;
                    default:
                        keyValueData.Add(field.key, field.@default);
                        break;
                }
            }
        }

        #region Gettters

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

        public T Get<T>(string key)
        {

            return default(T);
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

        #endregion

        #region Setters

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

        public void Set<T>(string key, T value)
        {

        }

        public void Toggle(string key)
        {

        }

        #endregion

        #region Adders

        public void AddScore(int score)
        {

        }

        public void AddScore(float score)
        {

        }


        public void Add<T>(string key, T value)
        {

        }
        #endregion


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


        #region IsFuncs

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
        #endregion


    }
}
