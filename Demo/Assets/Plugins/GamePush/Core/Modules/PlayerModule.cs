using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;

namespace GamePush.Core
{
    [System.Serializable]
    public class PlayerModule
    {
        public event Action OnPlayerChange;
        public event Action OnSyncComplete;
        public event Action OnSyncError;
        public event Action OnLoadComplete;
        public event Action OnLoadError;

        public event Action OnLoginComplete;
        public event Action OnLoginError;

        private bool _isFirstRequest = true;
        private string _secretCode;

        #region PlayerModuleInit

        public PlayerModule()
        {
            playerDataFields = new Dictionary<string, object>();
        }

        public void SetPlayerData(JObject playerData)
        {
            Debug.Log(playerData.ToString());

            JObject statsObject = (JObject)playerData["stats"];
            playerStats = statsObject.ToObject<PlayerStats>();

            JObject stateObject = (JObject)playerData["state"];
            playerState = stateObject.ToObject<Dictionary<string, object>>();

            foreach (string key in playerState.Keys.ToList())
            {
                Debug.Log($"{key} , {playerState[key]}");
            }

            if (playerData["sessionStart"].ToString() != "")
                SetStartTime(playerData["sessionStart"].ToString());
            else
                SetStartTime(CoreSDK.GetServerTime());

            Debug.Log($"ID {GetID()}");

            _isFirstRequest = false;

            SetPlayerDataCode(Get<string>(SECRETCODE_STATE_KEY));

            SavePlayerStateToPrefs();
        }
        #endregion

        #region Fetchers

        public async Task FetchPlayerConfig()
        {
            SetPlayerDataCode(GetPlayerSavedDataCode());

            if (_secretCode == "")
            {
                await Sync();
            }
            else
            {
                await Load();

            }
        }

        public async Task Sync(bool forceOverride = false)
        {
            SyncPlayerInput playerInput = new SyncPlayerInput();
            playerInput.playerState = GetPlayerState();

            playerInput.isFirstRequest = _isFirstRequest;
            playerInput.Override = forceOverride;

            //TODO get return
            await DataFetcher.SyncPlayer(playerInput, _isFirstRequest);

            OnSyncComplete?.Invoke();
        }

        public async Task Load()
        {
            GetPlayerInput playerInput = new GetPlayerInput();
            GetPlayerStateFromPrefs();

            playerInput.isFirstRequest = _isFirstRequest;

            //TODO get return
            await DataFetcher.GetPlayer(playerInput, _isFirstRequest);
            OnLoadComplete?.Invoke();
        }

        public async void FetchFields(Action<List<PlayerFetchFieldsData>> onFetchFields = null)
        {
            List<PlayerField> playerFetchFields = await DataFetcher.FetchPlayerFields(false);

            SetDataFields(playerFetchFields);

            List<PlayerFetchFieldsData> fetchFields = new List<PlayerFetchFieldsData>();

            foreach(PlayerField playerField in dataFields)
            {
                fetchFields.Add(FieldToFetchField(playerField));
            }

            onFetchFields?.Invoke(fetchFields);
        }

        private PlayerFetchFieldsData FieldToFetchField(PlayerField playerField)
        {
            PlayerFetchFieldsData fetchField = new PlayerFetchFieldsData();
            fetchField.name = playerField.name;
            fetchField.key = playerField.key;
            fetchField.type = playerField.type;
            fetchField.defaultValue = playerField.@default;
            fetchField.important = playerField.important;
            fetchField.variants = playerField.variants.ToArray();

            return fetchField;
        }

        #endregion

        #region PlayerState

        private static string ID_STATE_KEY = "id";
        private static string ACTIVE_STATE_KEY = "active";
        private static string REMOVED_STATE_KEY = "removed";
        private static string TEST_STATE_KEY = "test";
        private static string SCORE_STATE_KEY = "score";
        private static string NAME_STATE_KEY = "name";
        private static string AVATAR_STATE_KEY = "avatat";

        private static string SECRETCODE_STATE_KEY = "secretCode";
        private static string SAVE_MODIFICATOR = "xSaveState_";

        private Dictionary<string, object> playerState = new Dictionary<string, object>
        {
            { ID_STATE_KEY, 0 },
            { ACTIVE_STATE_KEY, true },
            { REMOVED_STATE_KEY, false },
            { TEST_STATE_KEY, false },
            { SCORE_STATE_KEY, 0 },
            { NAME_STATE_KEY, "" },
            { AVATAR_STATE_KEY, "" }
        };

        private Newtonsoft.Json.Linq.JObject GetPlayerState()
        {
            string json = JsonConvert.SerializeObject(playerState);
            var obj = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
            var sortedObj = new Newtonsoft.Json.Linq.JObject(obj.Properties().OrderBy(p => p.Name));
            return sortedObj;
        }

        private void GetPlayerStateFromPrefs()
        {
            Debug.Log("Get state from prefs");

            foreach (string key in playerState.Keys.ToList())
            {
                if(PlayerPrefs.HasKey(SAVE_MODIFICATOR + key))
                    playerState[key] = PlayerPrefs.GetString(SAVE_MODIFICATOR + key);
            }

            if (PlayerPrefs.HasKey(SAVE_MODIFICATOR + SECRETCODE_STATE_KEY))
            {
                Debug.Log("Secret code from prefs: " + PlayerPrefs.GetString(SAVE_MODIFICATOR + SECRETCODE_STATE_KEY));
                playerState.Add(SECRETCODE_STATE_KEY, "");
                playerState[SECRETCODE_STATE_KEY] = PlayerPrefs.GetString(SAVE_MODIFICATOR + SECRETCODE_STATE_KEY);
            }
                
        }

        private void SavePlayerStateToPrefs()
        {
            Debug.Log("Save state to prefs");

            foreach (string key in playerState.Keys.ToList())
            {
                CheckNilState(key);

                PlayerPrefs.SetString(SAVE_MODIFICATOR + key, playerState[key].ToString());
            }
        }

        private void CheckNilState(string key)
        {
            if (playerState[key].ToString() == "<nil>")
                playerState[key] = "";
        }

        #endregion

        #region PlayerStats
        private PlayerStats playerStats;

        public void SetPlayerStats()
        {

        }

        #endregion

        #region SecretCode

        private string SECRETCODE_SAVE_KEY = SAVE_MODIFICATOR + SECRETCODE_STATE_KEY;

        public string GetPlayerLocalDataCode() => _secretCode;

        public string GetPlayerSavedDataCode()
        {
            if (PlayerPrefs.HasKey(SECRETCODE_SAVE_KEY))
            {
                return PlayerPrefs.GetString(SECRETCODE_SAVE_KEY);
            }

            return "";
        }

        public void SetPlayerDataCode(string code)
        {
            PlayerPrefs.SetString(SECRETCODE_SAVE_KEY, code);
            _secretCode = code;
        }
        #endregion

        #region DataFields

        protected List<PlayerField> dataFields;
        private Dictionary<string, object> playerDataFields;
        private Dictionary<string, object> defaultState;

        public void SetDataFields(List<PlayerField> playerFields)
        {
            dataFields = playerFields;

            playerDataFields = new Dictionary<string, object>();
            defaultState = new Dictionary<string, object>();

            foreach (PlayerField field in dataFields)
            {
                playerDataFields.Add(field.key, field);
                
                switch (field.type)
                {
                    case "flag":
                        bool value = field.@default == "True";
                        defaultState.Add(field.key, value);
                        break;
                    case "stats":
                        if (int.TryParse(field.@default, out int intValue))
                            defaultState.Add(field.key, intValue);
                        else if (float.TryParse(field.@default, out float floatValue))
                            defaultState.Add(field.key, floatValue);
                        else
                            defaultState.Add(field.key, field.@default);
                        break;

                    case "data":
                        defaultState.Add(field.key, field.@default);
                        break;

                    default:
                        break;
                }

                //Debug.Log(field.@default + " " + field.@default.GetType());
            }

            foreach(string key in defaultState.Keys)
            {
                //Debug.Log(key + " " + defaultState[key].ToString() + " " + defaultState[key].GetType());
            }
        }

        #endregion

        #region TimeSpan

        private DateTime _startSessionTime;
        private float _playTime;
        private float _timeSpan;

        public void AddPlayTime(float time) {
            _playTime += time;
            _timeSpan += time;
        }

        private int GetPlayTime()
        { 
            return (int)_playTime;
        }

        private int GetTimeSpan()
        {
            return (int)_timeSpan;
        }

        private void SetStartTime(string sessionStart)
        {
            _startSessionTime = DateTime.Parse(sessionStart);
            _timeSpan = 0;
        }

        #endregion

        #region Getters

        public T GetValue<T>(string key)
        {
            if(playerState.TryGetValue(key, out object value))
            {
                return Helpers.ConvertValue<T>(value);
            }

            return default(T);
        }

        public int GetID()
        {
            return GetValue<int>(ID_STATE_KEY);
        }

        public float GetScore()
        {
            return GetValue<float>(SCORE_STATE_KEY);
        }

        public string GetName()
        {
            return GetValue<string>(NAME_STATE_KEY);
        }

        public string GetAvatarUrl()
        {
            return GetValue<string>(AVATAR_STATE_KEY);
        }

        public string GetFieldName(string key)
        {
            PlayerField field = (PlayerField)playerDataFields[key];
            return field.name;
        }

        public string GetFieldVariantName(string key, string value)
        {
            PlayerField field = (PlayerField)playerDataFields[key];

            foreach(PlayerFieldVariant variant in field.variants)
            {
                if (variant.value == value)
                    return variant.name;
            }
            return null;
        }

        public string GetFieldVariantAt(string key, int index)
        {
            PlayerField field = (PlayerField)playerDataFields[key];

            return field.variants[index].value;
        }

        public string GetFieldVariantIndex(string key, string value)
        {
            PlayerField field = (PlayerField)playerDataFields[key];

            foreach (PlayerFieldVariant variant in field.variants)
            {
                if (variant.value == value)
                    return field.variants.IndexOf(variant).ToString();
            }
            return null;
        }

        public T Get<T>(string key)
        {
            return GetValue<T>(key);
        }

        public int GetActiveDays()
        {
            return playerStats.activeDays;
        }

        public int GetActiveDaysConsecutive()
        {
            return playerStats.activeDaysConsecutive;
        }

        public int GetPlaytimeToday()
        {
            return playerStats.playtimeToday + GetTimeSpan();
        }

        public int GetPlaytimeAll()
        {
            return playerStats.playtimeAll + GetTimeSpan();
        }

        #endregion

        #region Setters

                private void SetStateValue<T>(string key, T value)
                {
                    playerState[key] = value;
                    OnPlayerChange?.Invoke();
                }

                public void SetName(string name)
                {
                    SetStateValue(NAME_STATE_KEY, name);
                }

                public void SetAvatar(string src)
                {
                    SetStateValue(AVATAR_STATE_KEY, src);
                }

                public void SetScore(int score)
                {
                    SetStateValue(SCORE_STATE_KEY, score);
                }

                public void SetScore(float score)
                {
                    SetStateValue(SCORE_STATE_KEY, score);
                }

                public void Set<T>(string key, T value)
                {
                    SetStateValue(key, value);
                }

                public void Toggle(string key)
                {
                    SetStateValue(key, !GetValue<bool>(key));
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

        #region IsFuncs

        public bool HasField(string key)
        {
            return playerState.TryGetValue(key, out object value);
        }

        public bool Has(string key)
        {
            if (playerState.TryGetValue(key, out object value))
            {
//TODO Helpers.ConvertValue

                string t = value.ToString();
                return !(t == "" || t == "0" || t == "False");
            }

            return false;
        }

        public bool IsLoggedIn()
        {
            return false;
        }

        public bool HasAnyCredentials()
        {
            return Get<string>("credentials") != "";
        }

        public bool IsStub()
        {
            foreach(string key in defaultState.Keys)
            {
                Debug.Log($"{key}, {playerState[key]}, {defaultState[key]}");
                if (key == "name" || key == "avatar") continue;

                if (playerState[key].ToString() != defaultState[key].ToString())
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        public void Reset()
        {
            foreach(string key in defaultState.Keys)
            {
                playerState[key] = defaultState[key];
            }

            //acceptedRewards = [];
            //givenRewards = [];
            //claimedTriggers = [];
            //claimedSchedulersDays = [];
        }

        public void Remove()
        {
            playerState[ID_STATE_KEY] = 0;
            playerState[SECRETCODE_STATE_KEY] = "";
            _secretCode = "";

            Reset();
        }

        
        public void Login()
        {
            OnLoginError?.Invoke();
        }

    }
}
