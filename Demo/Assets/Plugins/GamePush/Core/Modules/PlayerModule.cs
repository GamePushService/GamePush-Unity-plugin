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

        public event Action OnLogoutComplete;
        public event Action OnLogoutError;

        public event Action<PlayerFetchFieldsData> OnFieldMaximum;
        public event Action<PlayerFetchFieldsData> OnFieldMinimum;
        public event Action<PlayerFetchFieldsData> OnFieldIncrement;

        private bool _isFirstRequest = true;
        private string _secretCode;
        private string _token;

        private DateTime playerUpdateTime;

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

        private async Task Sync(SyncStorageType storage = SyncStorageType.preffered, bool forceOverride = false)
        {
            if(storage == SyncStorageType.preffered)
                storage = CoreSDK.platform.prefferedSyncType;

            Logger.Log($"Sync {storage}");
            switch (storage)
            {
                case SyncStorageType.cloud:
                    await CloudSync(forceOverride);
                    break;
                case SyncStorageType.local:
                case SyncStorageType.platform:
                    LocalSync();
                    break;
            }
        }

        private void LocalSync()
        {
            SavePlayerStateToPrefs();
        }

        private async Task CloudSync(bool forceOverride = false)
        {
            SyncPlayerInput playerInput = new SyncPlayerInput();
            playerInput.playerState = GetPlayerState();

            playerInput.isFirstRequest = _isFirstRequest;
            playerInput.@override = forceOverride;

            JObject resultObject = await DataFetcher.SyncPlayer(playerInput, _isFirstRequest);

            if(resultObject == null)
            {
                OnSyncError?.Invoke();
                return;
            }

            SetPlayerData(resultObject);
            OnSyncComplete?.Invoke();
        }

        private async Task Sync(bool forceOverride)
            => await Sync(forceOverride: forceOverride);

        public async void PlayerSync(bool forceOverride)
            => await Sync(forceOverride);
        public async void PlayerSync(SyncStorageType storage = SyncStorageType.preffered, bool forceOverride = false)
            => await Sync(storage, forceOverride);

        public async void PlayerLoad() => await Load();

        private async Task Load()
        {
            GetPlayerInput playerInput = new GetPlayerInput();
            GetPlayerStateFromPrefs();

            playerInput.isFirstRequest = _isFirstRequest;

            JObject resultObject = await DataFetcher.GetPlayer(playerInput, _isFirstRequest);

            if (resultObject == null)
            {
                OnLoadError?.Invoke();
                return;
            }

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

        public void EnableAutoSync(int interval = 10, SyncStorageType storage = SyncStorageType.preffered, bool isOverride = false)
        {
            if(autoSyncList.TryGetValue(storage, out AutoSyncData autoSyncData))
            {
                if (autoSyncData.isEnable)
                {
                    Logger.Warn($"AutoSync for {storage} storage already enabled. Call DisableAutoSync() before re-enabling.");
                    return;
                }
                else
                {
                    autoSyncData.isEnable = true;
                    autoSyncData.interval = interval;
                    return;
                }
            }

            string lastSyncTime = (CoreSDK.GetServerTime().AddSeconds(-interval)).ToString();

            AutoSyncData data = new AutoSyncData(true, storage, interval, isOverride, lastSyncTime);
            autoSyncList.Add(storage, data);
            AutoSync();
        }

        public void DisableAutoSync(SyncStorageType storage = SyncStorageType.preffered)
        {
            if (autoSyncList.TryGetValue(storage, out AutoSyncData autoSyncData))
            {
                if (autoSyncData.isEnable)
                {
                    Logger.Warn($"AutoSync for {storage} storage already disabled");
                    return;
                }
                else
                {
                    autoSyncData.isEnable = false;
                    return;
                }
                
            }
            else
            {
                AutoSyncData data = new AutoSyncData(false, storage);
                autoSyncList.Add(storage, data);
                Logger.Warn($"AutoSync for {storage} storage already disabled");
            }
        }

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
        private Dictionary<string, PlayerField> limitsFields;

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
            limitsFields = new Dictionary<string, PlayerField>();
            defaultState = new Dictionary<string, object>();

            foreach (PlayerField field in dataFields)
            {
                //PrintPlayerField(field);

                playerDataFields.Add(field.key, field);
                typeState.TryAdd(field.key, field.type);

                if(field.limits != null)
                {
                    limitsFields.Add(field.key, field);
                }

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

            playerUpdateTime = CoreSDK.GetServerTime();

            playerState[key] = value;

            if (playerDataFields[key].limits != null)
            {
                if(int.TryParse(value.ToString(), out int intValue))
                {
                    playerState[key] = intValue;
                }
                
                Logger.Log("Set limit value", key);
                LimitStateUpdate(playerDataFields[key]);
            }

            OnPlayerChange?.Invoke();
        }

        private void LimitStateUpdate(PlayerField field)
        {
            float max = Get<float>($"{field.key}:max");
            float min = Get<float>($"{field.key}:min");
            float incrementValue = Get<float>($"{field.key}:incrementValue");

            float currentValue = Get<float>(field.key);
            string timestampKey = $"{field.key}:timestamp";

            // Обработка достижения максимума
            if (currentValue >= max)
            {
                if (!field.limits.couldGoOverLimit)
                {
                    playerState[field.key] = max.ToString();
                }

                Logger.Log("Max value", field.key);
                OnFieldMaximum?.Invoke(FieldToFetchField(field));

                if (field.intervalIncrement != null && incrementValue > 0)
                {
                    timestampKey = "";
                }
            }

            // Обработка достижения минимума
            else if (currentValue <= min)
            {
                playerState[field.key] = min.ToString();

                Logger.Log("Min value", field.key);
                OnFieldMinimum?.Invoke(FieldToFetchField(field));

                if (field.intervalIncrement != null && incrementValue > 0)
                {
                    timestampKey = "";
                }
            }

            bool hasTimestamp = playerState.TryGetValue(timestampKey, out object timestamp);
            if (hasTimestamp) hasTimestamp = !string.IsNullOrEmpty(timestamp.ToString());

            Logger.Log("hasTimestamp", hasTimestamp.ToString());

            // Проверка на необходимость обновления таймстампа
            if (field.intervalIncrement != null &&
                !hasTimestamp &&
                ((incrementValue > 0 && currentValue < max) || (incrementValue < 0 && currentValue > min)))
            {

                string newTimestamp = CoreSDK.GetServerTime().ToString();
                //Logger.Log("New timestamp", newTimestamp);

                if (!playerState.TryAdd($"{field.key}:timestamp", newTimestamp))
                    playerState[$"{field.key}:timestamp"] = newTimestamp;

                Logger.Log($"{field.key}:timestamp", Get<string>($"{field.key}:timestamp"));
            }
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

        public float GetMaxValue(string key)
        {
            if (playerDataFields.TryGetValue(key, out PlayerField field))
            {
                if (playerState.TryGetValue($"{key}:max", out object value))
                    return Get<float>($"{key}:max");
                else
                {
                    string err = $"maxValue not exists on field \"{key}\"";
                    Logger.Error("GET MAX VALUE", err);
                    throw new Exception(err);
                }

            }
            else
            {
                string err = $"field \"{key}\" does not exists";
                Logger.Error("GET MAX VALUE", err);
                throw new Exception(err);
            }
        }

        public float GetMinValue(string key)
        {
            if (playerDataFields.TryGetValue(key, out PlayerField field))
            {
                if (playerState.TryGetValue($"{key}:min", out object value))
                    return Get<float>($"{key}:min");
                else
                {
                    string err = $"minValue not exists on field \"{key}\"";
                    Logger.Error("GET MIN VALUE", err);
                    throw new Exception(err);
                }

            }
            else
            {
                string err = $"field \"{key}\" does not exists";
                Logger.Error("GET MIN VALUE", err);
                throw new Exception(err);
            }
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
            Debug.Log(oldValue);
            Debug.Log(oldValue + value);
            int newValue = oldValue + value;
            SetStateValue(key, newValue);
        }

        public void Add(string key, float value)
        {
            float oldValue = Get<float>(key);
            Debug.Log(oldValue);
            Debug.Log(oldValue + value);
            float newValue = oldValue + value;
            SetStateValue(key, newValue);
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

        #region Account

        public void Login()
        {
            if (_token != null)
                OnLoginError?.Invoke();
            else
                OnLoginComplete?.Invoke();

        }

        public void Logout()
        {
            if (_token != null)
                OnLogoutError?.Invoke();
            else
                OnLogoutComplete?.Invoke();

        }

        #endregion

        #region PlayTime

        private double _playTimeAll;
        private double _playTimeToday;

        public void AddPlayTime(float time)
        {
            _playTimeAll += time;
            _playTimeToday += time;
        }

        private void SetStartTime(string sessionStart)
        {
            Debug.Log($"ServerTime string {CoreSDK.GetConfig().serverTime}");
            //Debug.Log($"ServerTime {CoreSDK.GetServerTime()}");
            //Debug.Log($"SessionStart {sessionStart}");

            DateTime session;
            TimeSpan timeFromStart = TimeSpan.Zero;

            if (DateTime.TryParse(sessionStart, out session))
            {
                DateTime tempSession = session.ToUniversalTime();
                DateTime tempServer = CoreSDK.GetServerTime().ToUniversalTime().ToLocalTime();

                //Debug.Log($"tempSession {tempSession}");
                //Debug.Log($"tempServer {tempServer}");

                timeFromStart = tempServer - tempSession;
            }

            Debug.Log($"timeFromStart {timeFromStart.TotalSeconds}");
            _playTimeAll = playerStats.playtimeAll + timeFromStart.TotalSeconds;
            _playTimeToday = playerStats.playtimeToday + timeFromStart.TotalSeconds;
        }

        #endregion

        #region TickMethods

        private Dictionary<SyncStorageType, AutoSyncData> autoSyncList = new Dictionary<SyncStorageType, AutoSyncData>();
        private DateTime localLastSyncTime;

        public void AutoSync()
        {
            foreach (SyncStorageType storage in autoSyncList.Keys)
            {
                if (autoSyncList[storage].isEnable)
                {
                    //Logger.Log("Autosync", $"{storage} isEnable");
                    int interval = autoSyncList[storage].interval;
                    
                    DateTime currentTime = CoreSDK.GetServerTime();
                    DateTime lastSyncTime;
                    if(DateTime.TryParse(autoSyncList[storage].lastSync, out lastSyncTime))
                    {
                        //Logger.Log("lastSyncTime", lastSyncTime.ToString());
                        if ((currentTime - lastSyncTime).TotalSeconds >= interval)
                        {
                            bool isNeedToSync = playerUpdateTime > localLastSyncTime;
                            if (isNeedToSync)
                            {
                                localLastSyncTime = DateTime.Now;
                                Logger.Log("Autosync", $"{storage} storage");
                                PlayerSync(storage, autoSyncList[storage].@override);
                            }
                        }
                    }
                    else
                    {
                        Logger.Error("Cant parse last sync time");
                    }
                    
                }
            }
        }

        public void IncrementFields()
        {
            foreach (string key in limitsFields.Keys)
            {
                if (typeState[key] == "service" || typeState[key] == "accounts")
                    continue;

                if (limitsFields[key].intervalIncrement != null)
                {
                    IncrementField(playerDataFields[key]);
                }
            }
        }

        private void IncrementField(PlayerField field)
        {
            float oldValue = Get<float>(field.key);
            float max = Get<float>($"{field.key}:max");
            float min = Get<float>($"{field.key}:min");
            int incrementInterval = Get<int>($"{field.key}:incrementInterval");
            float incrementValue = Get<float>($"{field.key}:incrementValue");

            DateTime now = CoreSDK.GetServerTime();
            DateTime timestamp;

            //Logger.Log(field.key, Get<string>($"{field.key}:timestamp"));

            if (Get<string>($"{field.key}:timestamp") == "")
                timestamp = now;
            else
                DateTime.TryParse(Get<string>($"{field.key}:timestamp"), out timestamp);

            //Logger.Log(field.key, timestamp.ToString());
            TimeSpan elapsed = now - timestamp;
            //Logger.Log(field.key, elapsed.ToString());

            int elapsedInterval = (int)elapsed.TotalSeconds;

            //Logger.Log(elapsedInterval.ToString(), incrementInterval.ToString());

            int increments = Mathf.FloorToInt(elapsedInterval / incrementInterval);

            if (increments > 0 && ((incrementValue > 0 && oldValue < max) || (incrementValue < 0 && oldValue > min)))
            {
                float newValue = oldValue + increments * incrementValue;

                newValue = Mathf.Min(Math.Max(newValue, min), max);

                OnFieldIncrement?.Invoke(FieldToFetchField(field));

                DateTime newTimestamp = timestamp.AddSeconds(incrementInterval * increments);
                playerState[$"{field.key}:timestamp"] = newTimestamp.ToString();

                Logger.Warn(field.key, newValue.ToString());
                Set(field.key, newValue);
            }
        }
        #endregion


        private void PrintPlayerField(PlayerField field)
        {
            string log = "";
            log += $"\nField key: {field.key}";
            log += $"\nField name: {field.name}";
            log += $"\nField type: {field.type}";
            log += $"\nField important: {field.important}";
            log += $"\nField public: {field.@public}";
            log += $"\nDefault value: {field.@default}";

            if (field.intervalIncrement != null)
            {
                log += $"\n\n Interval: {field.intervalIncrement.interval}";
                log += $"\n Increment: {field.intervalIncrement.increment}";
            }

            if (field.limits != null)
            {
                log += $"\n\n Min value: {field.limits.min}";
                log += $"\n Max value: {field.limits.max}";
                log += $"\n Could Go Over Limit: {field.limits.couldGoOverLimit}";
            }

            foreach (PlayerFieldVariant variant in field.variants)
            {
                log += $"\n\n variant:";
                log += $"\n  name: {variant.name}";
                log += $"\n  value: {variant.value}";
            }
            Debug.Log(log);
        }

    }
}
