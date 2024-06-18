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
        private string _token;

        #region PlayerInit

        public PlayerModule()
        {
            playerDataFields = new Dictionary<string, PlayerField>();
        }

        public void Init(List<PlayerField> playerFields)
        {
            SetDataFields(playerFields);
            foreach (string key in defaultState.Keys)
            {
                playerState.TryAdd(key, defaultState[key]);
            }

            //foreach (string key in playerState.Keys)
            //{
            //    Debug.Log(key + " " + playerState[key].ToString());
            //}
        }

        public void SetPlayerData(JObject playerData)
        {
            //Debug.Log(playerData.ToString());

            JObject statsObject = (JObject)playerData["stats"];
            playerStats = statsObject.ToObject<PlayerStats>();

            JObject stateObject = (JObject)playerData["state"];
            playerState = stateObject.ToObject<Dictionary<string, object>>();

            //foreach (string key in playerState.Keys.ToList())
            //{
            //    Debug.Log($"{key} , {playerState[key]}");
            //}


            SetStartTime(playerData["sessionStart"].ToString());

            if (playerData.TryGetValue("token", out JToken token) && token.ToString() != "")
            {
                _token = token.ToString();
            }


            //Debug.Log($"TOKEN {_token}");

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

        public async void PlayerSync(bool forceOverride = false) => await Sync(forceOverride);
        public async void PlayerLoad() => await Load();

        public async Task Sync(bool forceOverride = false)
        {
            SyncPlayerInput playerInput = new SyncPlayerInput();
            playerInput.playerState = GetPlayerState();

            playerInput.isFirstRequest = _isFirstRequest;
            playerInput.Override = forceOverride;

            JObject resultObject = await DataFetcher.SyncPlayer(playerInput, _isFirstRequest);
            SetPlayerData(resultObject);

            OnSyncComplete?.Invoke();
        }

        public async Task Load()
        {
            GetPlayerInput playerInput = new GetPlayerInput();
            GetPlayerStateFromPrefs();

            playerInput.isFirstRequest = _isFirstRequest;

            JObject resultObject = await DataFetcher.GetPlayer(playerInput, _isFirstRequest);
            SetPlayerData(resultObject);

            OnLoadComplete?.Invoke();
        }

        public async void FetchFields(Action<List<PlayerFetchFieldsData>> onFetchFields = null)
        {
            List<PlayerField> playerFetchFields = await DataFetcher.FetchPlayerFields(false);

            SetDataFields(playerFetchFields);

            List<PlayerFetchFieldsData> fetchFields = new List<PlayerFetchFieldsData>();

            foreach (PlayerField playerField in dataFields)
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
            fetchField.isPublic = playerField.@public;
            fetchField.variants = playerField.variants.ToArray();

            return fetchField;
        }

        public void Ping() => DataFetcher.Ping(_token);

        #endregion

        #region PlayerState

        private static string ID_STATE_KEY = "id";
        private static string ACTIVE_STATE_KEY = "active";
        private static string REMOVED_STATE_KEY = "removed";
        private static string TEST_STATE_KEY = "test";
        private static string SCORE_STATE_KEY = "score";
        private static string NAME_STATE_KEY = "name";
        private static string AVATAR_STATE_KEY = "avatar";

        private static List<string> IGNORE_FOR_STAB
            = new List<string>(){
                NAME_STATE_KEY,
                AVATAR_STATE_KEY
            };

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

        private JObject GetPlayerState()
        {
            string json = JsonConvert.SerializeObject(playerState);
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            var sortedObj = new JObject(obj.Properties().OrderBy(p => p.Name));

            return sortedObj;
        }

        private void GetPlayerStateFromPrefs()
        {
            Debug.Log("Get state from prefs");

            foreach (string key in playerState.Keys.ToList())
            {
                if (PlayerPrefs.HasKey(SAVE_MODIFICATOR + key))
                    playerState[key] = PlayerPrefs.GetString(SAVE_MODIFICATOR + key);
            }

            if (PlayerPrefs.HasKey(SAVE_MODIFICATOR + SECRETCODE_STATE_KEY))
            {
                //Debug.Log("Secret code from prefs: " + PlayerPrefs.GetString(SAVE_MODIFICATOR + SECRETCODE_STATE_KEY));

                playerState.TryAdd(SECRETCODE_STATE_KEY, "");
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

        private static string SECRETCODE_SAVE_KEY = SAVE_MODIFICATOR + SECRETCODE_STATE_KEY;

        public static string GetPlayerSavedDataCode() => DataHolder.GetSavedSecretCode();

        public void SetPlayerDataCode(string code) => DataHolder.SetSecretCode(code);

        #endregion

        #region DataFields

        protected List<PlayerField> dataFields;
        private Dictionary<string, PlayerField> playerDataFields;
        private Dictionary<string, object> defaultState;
        private Dictionary<string, string> typeState = new Dictionary<string, string>
        {
            { ID_STATE_KEY, "service" },
            { ACTIVE_STATE_KEY, "service" },
            { REMOVED_STATE_KEY, "service" },
            { TEST_STATE_KEY, "service" },
            { SCORE_STATE_KEY, "stats" },
            { NAME_STATE_KEY, "data" },
            { AVATAR_STATE_KEY, "data" }
        };

        public void SetDataFields(List<PlayerField> playerFields)
        {
            dataFields = playerFields;

            playerDataFields = new Dictionary<string, PlayerField>();
            defaultState = new Dictionary<string, object>();

            foreach (PlayerField field in dataFields)
            {
                playerDataFields.Add(field.key, field);
                typeState.TryAdd(field.key, field.type);

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

            //Debug.Log("All types");
            //foreach (string key in typeState.Keys)
            //{
            //    Debug.Log(key + " " + typeState[key].ToString());
            //}
        }

        #endregion

        #region StateChange

        private void SetStateValue(string key, object value)
        {
            //Check type of state
            if (typeState[key] == "service" || typeState[key] == "accounts") return;
            //Check change of state
            if (playerState[key] == value) return;

            if (playerDataFields[key].variants.Count > 0)
            {
                var variantQuery =
                    from variant in playerDataFields[key].variants
                    where variant.value == value.ToString()
                    select variant;

                if (variantQuery.Count() == 0) return;
            }

            playerState[key] = value;
            OnPlayerChange?.Invoke();
        }

        public void Reset()
        {
            foreach (string key in defaultState.Keys)
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

            DataHolder.ResetSecretCode();

            Reset();
        }


        private T StateConverter<T>(string type, object value = null)
        {
            switch (type)
            {
                case "service":
                case "accounts":
                    return default(T);

                default:

                    return Helpers.ConvertValue<T>(value);
            }
        }

        #endregion

        #region Getters


        public T GetValue<T>(string key)
        {
            if (key != "")
                if (playerState.TryGetValue(key, out object value))
                {
                    typeState.TryGetValue(key, out string valueType);
                    return StateConverter<T>(valueType, value);
                }

            return default(T);
        }

        public int GetID()
        {
            playerState.TryGetValue(ID_STATE_KEY, out object value);
            return Helpers.ConvertValue<int>(value);
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

            foreach (PlayerFieldVariant variant in field.variants)
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
            return (int)_playTimeToday;
        }

        public int GetPlaytimeAll()
        {
            return (int)_playTimeAll;
        }

        #endregion

        #region Setters

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
            SetScore(GetScore() + score);
        }

        public void AddScore(float score)
        {
            SetScore(GetScore() + score);
        }

        public void Add(string key, int value)
        {
            int oldValue = Get<int>(key);
            Set(key, oldValue + value);
        }

        public void Add(string key, float value)
        {
            float oldValue = Get<float>(key);
            Set(key, oldValue + value);
        }
        #endregion

        #region IsFuncs

        public bool HasField(string key)
        {
            return typeState.TryGetValue(key, out string type);
        }

        public bool Has(string key)
        {
            if (typeState.TryGetValue(key, out string type))
            {
                switch (type)
                {
                    case "stats":
                        return Helpers.ConvertValue<double>(playerState[key]) != 0;
                    case "data":
                        return Helpers.ConvertValue<string>(playerState[key]) != "";
                    case "flag":
                        return Helpers.ConvertValue<bool>(playerState[key]) != false;
                    case "service":
                        return false;
                    case "accounts":
                        return false;
                }
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
            foreach (string key in defaultState.Keys)
            {
                Debug.Log($"{key}, {playerState[key]}, {defaultState[key]}");
                if (key == NAME_STATE_KEY || key == AVATAR_STATE_KEY) continue;

                if (playerState[key].ToString() != defaultState[key].ToString())
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region TimeSpan

        private double _playTimeAll;
        private double _playTimeToday;

        public void AddPlayTime(float time)
        {
            _playTimeAll += time;
            _playTimeToday += time;
        }

        private void SetStartTime(string sessionStart)
        {
            Debug.Log($"ServerTime {CoreSDK.GetServerTime()}");
            Debug.Log($"sessionStart {sessionStart}");
            TimeSpan timeFromStart =
                sessionStart != "" ?
                (CoreSDK.GetServerTime() - DateTime.Parse(sessionStart)) :
                TimeSpan.Zero;

            Debug.Log($"timeFromStart {timeFromStart.TotalSeconds}");
            _playTimeAll = playerStats.playtimeAll + timeFromStart.TotalSeconds;
            _playTimeToday = playerStats.playtimeToday + timeFromStart.TotalSeconds;
        }

        #endregion

        public void Login()
        {
            if (_token != null)
                OnLoginError?.Invoke();
            else
                OnLoginComplete?.Invoke();

            //OnLoginComplete?.Invoke();
        }

    }
}
