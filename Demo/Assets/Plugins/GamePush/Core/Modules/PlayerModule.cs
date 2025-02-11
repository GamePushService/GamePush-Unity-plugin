using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
#if XSOLLA_SERVICE
using GamePush.Xsolla;
# endif

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
        public event Action<string> OnLoginError;

        public event Action OnLogoutComplete;
        public event Action OnLogoutError;

        public event Action<PlayerFetchFieldsData> OnFieldMaximum;
        public event Action<PlayerFetchFieldsData> OnFieldMinimum;
        public event Action<PlayerFetchFieldsData> OnFieldIncrement;

        private bool _isFirstRequest = false;
        private string _token;

        private DateTime _playerUpdateTime;

        private Dictionary<SyncStorageType, AutoSyncData> autoSyncList = new Dictionary<SyncStorageType, AutoSyncData>();
        private Dictionary<SyncStorageType, DateTime> lastSyncTimeList = new Dictionary<SyncStorageType, DateTime>();

        private string _lastSyncResult = "";
        private bool _isPublicFieldsDirty = false;

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
                _playerState.TryAdd(key, defaultState[key]);
            }

            SyncTimeListInit();

            _isFirstRequest = true;
        }

        private void SyncTimeListInit()
        {
            lastSyncTimeList = new Dictionary<SyncStorageType, DateTime>();

            DateTime now = CoreSDK.GetServerTime();
            lastSyncTimeList.Add(SyncStorageType.preffered, now);
            lastSyncTimeList.Add(SyncStorageType.cloud, now);
            lastSyncTimeList.Add(SyncStorageType.local, now);
            lastSyncTimeList.Add(SyncStorageType.platform, now);

            _playerUpdateTime = now;
        }
        #endregion

        #region Fetchers

        public async Task FetchPlayerConfig()
        {
            SetPlayerDataCode(GetPlayerSavedDataCode());

            if (DataHolder.GetSecretCode() == "")
            {
                await Sync(SyncStorageType.cloud);
            }
            else
            {
                await Load();
            }
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

        #region Sync/Load

        

        public async void PlayerSync(bool forceOverride) 
            => await Sync(forceOverride);

        // ReSharper disable Unity.PerformanceAnalysis
        public async void PlayerSync(SyncStorageType storage = SyncStorageType.preffered, bool forceOverride = false)
            => await Sync(storage, forceOverride);
        
        private async Task Sync(bool forceOverride)
            => await Sync(forceOverride: forceOverride);

        private async Task Sync(SyncStorageType storage = SyncStorageType.preffered, bool forceOverride = false)
        {
            if (autoSyncList.TryGetValue(storage, out var autoSyncData))
                autoSyncData.lastSync = CoreSDK.GetServerTime().ToString();

            if (storage == SyncStorageType.preffered)
                storage = CoreSDK.Platform.PrefferedSyncType;

            //Logger.Log($"Sync storage", $"{storage}");
            var isCloudSave = storage == SyncStorageType.cloud;
            var isNeedSyncPublicFields = CoreSDK.Platform.AlwaysSyncPublicFields && _isPublicFieldsDirty;
            var isNeedToSyncWithServer =
                 isNeedSyncPublicFields ||
                 isCloudSave ||
                _isFirstRequest;

            if (isNeedToSyncWithServer)
            {
                var secondState = _playerState;
                var isNeedToSyncOnlyPublicFields = !isCloudSave && isNeedSyncPublicFields;

                if (isNeedToSyncOnlyPublicFields)
                {
                    foreach(var key in playerDataFields.Keys)
                    {
                        if (!playerDataFields[key].@public)
                            secondState.Remove(key);
                    }
                }

                var playerStateData = GetJObjectPlayerState(secondState);
                await CloudSync(playerStateData, storage, forceOverride);
            }
            else if(!isCloudSave)
            {
                LocalSync(storage);
            }

            _isFirstRequest = false;
            _isPublicFieldsDirty = false;

            OnSyncComplete?.Invoke();
        }

        private void LocalSync(SyncStorageType storageType)
        {
            Dictionary<string, object> secondState = _playerState;
            PlayerStats secondStats = _playerStats;

            if (_playerUpdateTime > lastSyncTimeList[storageType])
            {
                SavePlayerStateToPrefs();
                SavePlayerStatsToPrefs();
            }
            else
            {
                secondState = GetPlayerStateFromPrefs();
                secondStats = GetPlayerStatsFromPrefs();
            }

            var lastResult = new JObject();
            if (_lastSyncResult != "")
            {
                var obj = JsonConvert.DeserializeObject<JObject>(_lastSyncResult);
                lastResult = new JObject(obj.Properties().OrderBy(p => p.Name));
            }

            lastResult["state"] = GetJObjectPlayerState(secondState);
            lastResult["stats"] = GetJObjectPlayerStats(secondStats);
            lastResult["sessionStart"] = CoreSDK.GetServerTime();

            HandleSync(lastResult, SyncStorageType.local);

            lastSyncTimeList[storageType] = CoreSDK.GetServerTime();
        }

        private async Task CloudSync(JObject playerState, SyncStorageType storage, bool forceOverride = false)
        {
            var playerInput = new SyncPlayerInput
            {
                playerState = playerState,
                isFirstRequest = _isFirstRequest,
                @override = forceOverride
            };

            var resultObject = await DataFetcher.SyncPlayer(playerInput, _isFirstRequest);

            if (resultObject == null)
            {
                OnSyncError?.Invoke();
                return;
            }

            _lastSyncResult = resultObject.ToString();

            HandleSync(resultObject, storage);

            lastSyncTimeList[SyncStorageType.cloud] = CoreSDK.GetServerTime();
        }


        private void HandleSync(JObject playerData, SyncStorageType syncStorage)
        {
            //Logger.Log("Handle Sync", syncStorage);
            //Logger.Log("Sync Data", playerData);
            
            SetPlayerStats(playerData);
            SetDataFromSync(playerData);

            SetStartTime(playerData["sessionStart"].ToString());

            if (playerData.TryGetValue("token", out JToken token) && token.ToString() != "")
            {
                _token = token.ToString();
            }

            var isServerHasNewProgress = false;

            if(DateTime.TryParse(playerData["state"][MODIFIED_AT_KEY].ToString(), out var newModifTime))
            {
                if (_playerState.TryGetValue("modifiedAt", out object oldModifTime))
                {
                    isServerHasNewProgress =
                        (DateTime.Parse(newModifTime.ToString()) - DateTime.Parse(oldModifTime.ToString())).TotalSeconds > 4; 
                }
            }

            string secretCode = GetPlayerSavedDataCode();
            string credentials = Get<string>("credentials");
            int playerID = GetPlayerSavedID();

            bool isNeedToLoadFromServer =
                (!string.IsNullOrEmpty(credentials) && credentials != playerData["state"]["credentials"].ToString()) ||
                (secretCode != "" && secretCode != playerData["state"]["secretCode"].ToString()) ||
                playerID == 0 ||
                syncStorage == SyncStorageType.cloud;

            //Logger.Log("isNeedToLoadFromServer", isNeedToLoadFromServer);
            //Logger.Log("SyncPublicFields", CoreSDK.platform.alwaysSyncPublicFields);

            if (isNeedToLoadFromServer)
            {
                SetPlayerState(playerData);

                if (CoreSDK.Platform.AlwaysSyncPublicFields && syncStorage != SyncStorageType.cloud && !_isFirstRequest)
                {
                    Dictionary<string, object> storageState = GetPlayerStateFromPrefs();

                    foreach (var playerField in dataFields)
                    {
                        if (!playerField.@public)
                        {
                            _playerState[playerField.key] = storageState[playerField.key];
                        }
                    }
                }
            }
            else
            {
                if (CoreSDK.Platform.AlwaysSyncPublicFields)
                {
                    UpdateOnlyPublicFields(playerData);
                }
                else if(isServerHasNewProgress)
                {
                    SetPlayerState(playerData);
                }
            }

            secretCode = playerData["state"]["secretCode"].ToString();
            //Debug.Log("Set secret: " + secretCode);
            SetPlayerDataCode(secretCode);
            SetPlayerSavedID();

            _playerUpdateTime = CoreSDK.GetServerTime();
            SavePlayerStateToPrefs();
        }

        private void SetDataFromSync(JObject playerData)
        {
            //Set uniques
            var uniques = playerData["uniques"].ToObject<List<UniquesData>>();
            CoreSDK.Uniques.SetUniques(uniques);

            //Set achievements
            var achievements = playerData["achievementsList"].ToObject<List<PlayerAchievement>>();
            CoreSDK.Achievements.SetAchievementsList(achievements);
            
            //Set player events
            var playerEvents = playerData["playerEvents"].ToObject<List<PlayerEvent>>();
            CoreSDK.Events.SetPlayerEvents(playerEvents);
        }

        private void UpdateOnlyPublicFields(JObject playerData)
        {
            var stateObject = (JObject)playerData["state"];
            var tempState = stateObject.ToObject<Dictionary<string, object>>();

            foreach (var playerField in dataFields.Where(playerField => playerField.@public))
            {
                _playerState[playerField.key] = tempState[playerField.key];
            }
        }

        public bool EnableAutoSync(int interval = 10, SyncStorageType storage = SyncStorageType.preffered, bool isOverride = false)
        {
            if (autoSyncList.TryGetValue(storage, out AutoSyncData autoSyncData))
            {
                if (autoSyncData.isEnable)
                {
                    Logger.Warn($"AutoSync for {storage} storage already enabled. Call DisableAutoSync() before re-enabling.");
                    return false;
                }
                else
                {
                    autoSyncData.isEnable = true;
                    autoSyncData.interval = interval;
                    autoSyncData.@override = isOverride;
                    Logger.Log($"AutoSync for {storage} storage enabled", $"{interval}");
                    AutoSync();
                    return true;
                }
            }

            var lastSyncTime = (CoreSDK.GetServerTime().AddSeconds(-interval)).ToString();

            var data = new AutoSyncData(true, storage, interval, isOverride, lastSyncTime);
            autoSyncList.Add(storage, data);
            Logger.Log($"AutoSync for {storage} storage enabled, interval: {interval}");
            AutoSync();
            return true;
        }

        public bool DisableAutoSync(SyncStorageType storage = SyncStorageType.preffered)
        {
            if (autoSyncList.TryGetValue(storage, out AutoSyncData autoSyncData))
            {
                if (!autoSyncData.isEnable)
                {
                    Logger.Warn($"AutoSync for {storage} storage already disabled");
                    return false;
                }
                else
                {
                    autoSyncData.isEnable = false;
                    Logger.Log($"AutoSync for {storage} storage disabled");
                    return true;
                }

            }
            else
            {
                var data = new AutoSyncData(false, storage);
                autoSyncList.Add(storage, data);
                Logger.Warn($"AutoSync for {storage} storage already disabled");
                return false;
            }
        }

        public async void PlayerLoad() => await Load();

        private async Task Load()
        {
            GetPlayerInput playerInput = new GetPlayerInput();
            _playerState = GetPlayerStateFromPrefs();

            playerInput.isFirstRequest = _isFirstRequest;

            JObject resultObject = await DataFetcher.GetPlayer(playerInput, _isFirstRequest);

            if (resultObject == null)
            {
                OnLoadError?.Invoke();
                _isFirstRequest = false;
                return;
            }

            _lastSyncResult = resultObject.ToString();

            HandleSync(resultObject, CoreSDK.Platform.PrefferedSyncType);
            _isFirstRequest = false;

            OnLoadComplete?.Invoke();
        }

        #endregion

        #region PlayerState

        private static string ID_STATE_KEY = "id";
        private static string CREDS_STATE_KEY = "credentials";
        private static string ACTIVE_STATE_KEY = "active";
        private static string REMOVED_STATE_KEY = "removed";
        private static string TEST_STATE_KEY = "test";
        private static string SCORE_STATE_KEY = "score";
        private static string NAME_STATE_KEY = "name";
        private static string AVATAR_STATE_KEY = "avatar";

        private static string MODIFIED_AT_KEY = "modifiedAt";
        private static string PLATFORM_TYPE_KEY = "platformType";
        private static string PROJECT_ID_KEY = "projectId";

        private static List<string> IGNORE_FOR_STAB
            = new List<string>(){
                NAME_STATE_KEY,
                AVATAR_STATE_KEY
            };

        private static string SECRETCODE_STATE_KEY = "secretCode";
        private static string SAVE_STATE_MODIFICATOR = "xSaveState_";
        private static string SAVE_STATS_MODIFICATOR = "xSaveStats_";

        private Dictionary<string, object> _playerState = new Dictionary<string, object>
        {
            { ID_STATE_KEY, 0 },
            { ACTIVE_STATE_KEY, true },
            { REMOVED_STATE_KEY, false },
            { TEST_STATE_KEY, false },
            { SCORE_STATE_KEY, 0 },
            { NAME_STATE_KEY, "" },
            { AVATAR_STATE_KEY, "" }
        };

        private void SetPlayerState(JObject playerData)
        {
            JObject stateObject = (JObject)playerData["state"];
            _playerState = stateObject.ToObject<Dictionary<string, object>>();
        }

        private JObject GetJObjectPlayerState(Dictionary<string, object> playerData)
        {
            if(!playerData.TryAdd(MODIFIED_AT_KEY, CoreSDK.GetServerTime()))
            {
                playerData[MODIFIED_AT_KEY] = CoreSDK.GetServerTime();
            }

            string json = JsonConvert.SerializeObject(playerData);
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            var sortedObj = new JObject(obj.Properties().OrderBy(p => p.Name));

            return sortedObj;
        }

        private Dictionary<string, object> GetPlayerStateFromPrefs()
        {
            //Logger.Log("Get state from prefs");
            Dictionary<string, object> getState = new Dictionary<string, object>();

            foreach (string key in _playerState.Keys.ToList())
            {
                if (typeState[key] == "service") continue;

                if (PlayerPrefs.HasKey(SAVE_STATE_MODIFICATOR + key))
                {
                    getState.Add(key, PlayerPrefs.GetString(SAVE_STATE_MODIFICATOR + key));
                }
                    
            }

            if (PlayerPrefs.HasKey(SAVE_STATE_MODIFICATOR + SECRETCODE_STATE_KEY))
            {
                getState.TryAdd(SECRETCODE_STATE_KEY, "");
                getState[SECRETCODE_STATE_KEY] = PlayerPrefs.GetString(SAVE_STATE_MODIFICATOR + SECRETCODE_STATE_KEY);

            }

            return getState;
        }

        private void SavePlayerStateToPrefs()
        {
            foreach (string key in _playerState.Keys.ToList())
            {
                CheckNilState(key);
                PlayerPrefs.SetString(SAVE_STATE_MODIFICATOR + key, _playerState[key].ToString());
            }
        }

        private void CheckNilState(string key)
        {
            if (_playerState[key].ToString() == "<nil>")
                _playerState[key] = "";
        }

        #endregion

        #region PlayerStats

        private PlayerStats _playerStats;

        private void SetPlayerStats(JObject playerData)
        {
            JObject statsObject = (JObject)playerData["stats"];
            _playerStats = statsObject.ToObject<PlayerStats>();
        }

        private JObject GetJObjectPlayerStats(PlayerStats playerStats)
        {
            string json = JsonConvert.SerializeObject(playerStats);
            var obj = JsonConvert.DeserializeObject<JObject>(json);
            var sortedObj = new JObject(obj.Properties().OrderBy(p => p.Name));

            return sortedObj;
        }

        private PlayerStats GetPlayerStatsFromPrefs()
        {
            PlayerStats playerStats = new PlayerStats();

            if (PlayerPrefs.HasKey(SAVE_STATS_MODIFICATOR + "playtimeAll"))
                playerStats.playtimeAll = PlayerPrefs.GetInt(SAVE_STATS_MODIFICATOR + "playtimeAll");

            if (PlayerPrefs.HasKey(SAVE_STATS_MODIFICATOR + "playtimeToday"))
                playerStats.playtimeToday = PlayerPrefs.GetInt(SAVE_STATS_MODIFICATOR + "playtimeToday");

            if (PlayerPrefs.HasKey(SAVE_STATS_MODIFICATOR + "activeDays"))
                playerStats.activeDays = PlayerPrefs.GetInt(SAVE_STATS_MODIFICATOR + "activeDays");

            if (PlayerPrefs.HasKey(SAVE_STATS_MODIFICATOR + "activeDaysConsecutive"))
                playerStats.activeDaysConsecutive = PlayerPrefs.GetInt(SAVE_STATS_MODIFICATOR + "activeDaysConsecutive");

            return playerStats;
        }

        private void SavePlayerStatsToPrefs()
        {
            PlayerPrefs.SetInt(SAVE_STATS_MODIFICATOR + "playtimeAll", _playerStats.playtimeAll);
            PlayerPrefs.SetInt(SAVE_STATS_MODIFICATOR + "playtimeToday", _playerStats.playtimeToday);
            PlayerPrefs.SetInt(SAVE_STATS_MODIFICATOR + "activeDays", _playerStats.activeDays);
            PlayerPrefs.SetInt(SAVE_STATS_MODIFICATOR + "activeDaysConsecutive", _playerStats.activeDaysConsecutive);
        }

        #endregion

        #region PlayerPurchases

        private Dictionary<string, FetchPlayerPurchase> _playerPurchases;

        public void SetPlayerPurchases()
        {
            //TODO Set purchases from fetch
        }

        public List<FetchPlayerPurchase> GetPlayerPurchases()
        {
            List<FetchPlayerPurchase> purchases = new List<FetchPlayerPurchase>();
            foreach(FetchPlayerPurchase playerPurchase in _playerPurchases.Values)
            {
                purchases.Add(playerPurchase);
            }

            return purchases;
        }

        #endregion

        #region DataHolder

        public string GetPlayerSavedDataCode() => DataHolder.GetSavedSecretCode();
        public void SetPlayerDataCode(string code) => DataHolder.SetSecretCode(code);

        public int GetPlayerSavedID() => DataHolder.GetPlayerID();
        public void SetPlayerSavedID()
        {
            int id = GetID();
            if (id == 0)
            {
                id = DataHolder.GetPlayerID();
                _playerState[ID_STATE_KEY] = id;
            }
            DataHolder.SetPlayerID(id);
        }

        #endregion

        #region DataFields

        protected List<PlayerField> dataFields;
        private Dictionary<string, PlayerField> playerDataFields;
        private Dictionary<string, PlayerField> limitsFields;

        private Dictionary<string, object> defaultState;
        private Dictionary<string, string> typeState = new Dictionary<string, string>
        {
            { ID_STATE_KEY, "service" },
            { CREDS_STATE_KEY, "service" },
            { ACTIVE_STATE_KEY, "service" },
            { REMOVED_STATE_KEY, "service" },
            { TEST_STATE_KEY, "service" },
            { MODIFIED_AT_KEY, "service" },
            { PLATFORM_TYPE_KEY, "service" },
            { SECRETCODE_STATE_KEY, "service" },
            { PROJECT_ID_KEY, "service" },
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

            }
        }

        #endregion

        #region StateChange

        private void SetStateValue(string key, object value)
        {
            //Check type of state
            if (typeState[key] == "service" || typeState[key] == "accounts") return;
            //Check change of state
            if (_playerState[key] == value) return;

            //Check public field
            if (playerDataFields[key].@public)
            {
                _isPublicFieldsDirty = true;
            }
                

            if (playerDataFields[key].variants.Count > 0)
            {
                var variantQuery =
                    from variant in playerDataFields[key].variants
                    where variant.value == value.ToString()
                    select variant;

                if (variantQuery.Count() == 0) return;
            }

            _playerUpdateTime = CoreSDK.GetServerTime();

            _playerState[key] = value;

            if (playerDataFields[key].limits != null)
            {
                if(int.TryParse(value.ToString(), out int intValue))
                {
                    _playerState[key] = intValue;
                }
                
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

            //Processing maximum value
            if (currentValue >= max)
            {
                if (!field.limits.couldGoOverLimit)
                {
                    _playerState[field.key] = max.ToString();
                }

                OnFieldMaximum?.Invoke(FieldToFetchField(field));

                if (field.intervalIncrement != null && incrementValue > 0)
                {
                    timestampKey = "";
                }
            }

            //Processing minimum value
            else if (currentValue <= min)
            {
                _playerState[field.key] = min.ToString();

                OnFieldMinimum?.Invoke(FieldToFetchField(field));

                if (field.intervalIncrement != null && incrementValue > 0)
                {
                    timestampKey = "";
                }
            }

            bool hasTimestamp = _playerState.TryGetValue(timestampKey, out object timestamp);
            if (hasTimestamp) hasTimestamp = !string.IsNullOrEmpty(timestamp.ToString());

            //Check timestamp update
            if (field.intervalIncrement != null &&
                !hasTimestamp &&
                ((incrementValue > 0 && currentValue < max) || (incrementValue < 0 && currentValue > min)))
            {

                string newTimestamp = CoreSDK.GetServerTime().ToString();

                if (!_playerState.TryAdd($"{field.key}:timestamp", newTimestamp))
                    _playerState[$"{field.key}:timestamp"] = newTimestamp;
            }
        }

        public void Reset()
        {
            foreach (string key in defaultState.Keys)
            {
                _playerState[key] = defaultState[key];
            }

            //acceptedRewards = [];
            //givenRewards = [];
            //claimedTriggers = [];
            //claimedSchedulersDays = [];
        }

        public void Remove()
        {
            _playerState[ID_STATE_KEY] = 0;
            _playerState[SECRETCODE_STATE_KEY] = "";

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
                if (_playerState.TryGetValue(key, out object value))
                {
                    typeState.TryGetValue(key, out string valueType);
                    return StateConverter<T>(valueType, value);
                }

            return default(T);
        }

        public int GetID()
        {
            _playerState.TryGetValue(ID_STATE_KEY, out object value);
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
                if (_playerState.TryGetValue($"{key}:max", out object value))
                    return Get<float>($"{key}:max");
                else
                {
                    string err = $"maxValue not exists on field \"{key}\"";
                    Logger.Error("GET MAX VALUE", err);
                    //throw new Exception(err);
                    return Get<float>($"{key}");
                }

            }
            else
            {
                string err = $"field \"{key}\" does not exists";
                Logger.Error("GET MAX VALUE", err);
                //throw new Exception(err);
                return default(float);
            }

        }

        public float GetMinValue(string key)
        {
            if (playerDataFields.TryGetValue(key, out PlayerField field))
            {
                if (_playerState.TryGetValue($"{key}:min", out object value))
                    return Get<float>($"{key}:min");
                else
                {
                    string err = $"minValue not exists on field \"{key}\"";
                    Logger.Error("GET MIN VALUE", err);
                    //throw new Exception(err);
                    return Get<float>($"{key}");
                }

            }
            else
            {
                string err = $"field \"{key}\" does not exists";
                Logger.Error("GET MIN VALUE", err);
                //throw new Exception(err);
                return default(float);
            }

            
        }


        public int GetSecondsLeft(string key)
        {
            if (playerDataFields.TryGetValue(key, out PlayerField field))
            {
                if (field.intervalIncrement == null)
                {
                    string err = $"Increment of field \"{key}\" does not exists";
                    Logger.Error("Get Seconds Left", err);
                    //throw new Exception(err);
                    return 0;
                }

                if (_playerState.TryGetValue($"{key}:incrementValue", out object increment))
                {
                    int.TryParse(increment.ToString(), out int incrementValue);
                    bool isDecrement = incrementValue < 0;

                    float.TryParse(_playerState[key].ToString(), out float current);

                    float boundValue = isDecrement ? Get<float>($"{key}:min") : Get<float>($"{key}:max");

                    if (isDecrement && current <= boundValue) return 0;
                    if (!isDecrement && current >= boundValue) return 0;

                    object timestamp;

                    if (_playerState.TryGetValue($"{key}:timestamp", out timestamp))
                    {
                        if (timestamp.ToString() == "") return 0;
                    }
                    else return 0;

                    if (DateTime.TryParse(timestamp.ToString(), out DateTime prevUpd))
                    {
                        int diff = (int)(CoreSDK.GetServerTime() - prevUpd).TotalSeconds;

                        int.TryParse(_playerState[$"{key}:incrementInterval"].ToString(), out int interval);
                        int left = Mathf.CeilToInt(interval - diff);

                        return left >= 0 ? left : 0;
                    }
                }

                return 0;
            }
            else
            {
                string err = $"field \"{key}\" does not exists";
                Logger.Error("Get Seconds Left", err);
                //throw new Exception(err);
                return 0;
            }
        }

        public int GetSecondsLeftTotal(string key)
        {
            if (playerDataFields.TryGetValue(key, out PlayerField field))
            {
                if (field.intervalIncrement == null)
                {
                    string err = $"Increment of field \"{key}\" does not exists";
                    Logger.Error("Get Seconds Left Total", err);
                    //throw new Exception(err);
                    return 0;
                }

                if (_playerState.TryGetValue($"{key}:incrementValue", out object increment))
                {
                    int.TryParse(increment.ToString(), out int incrementValue);
                    bool isDecrement = incrementValue < 0;

                    float.TryParse(_playerState[key].ToString(), out float current);
                    float boundValue = isDecrement ? Get<float>($"{key}:min") : Get<float>($"{key}:max");

                    if (isDecrement && current <= boundValue) return 0;
                    if (!isDecrement && current >= boundValue) return 0;

                    int.TryParse(_playerState[$"{key}:incrementInterval"].ToString(), out int incrementInterval);

                    // Сколько энергии осталось восполнить (для инкремента) или уменьшить (для декремента)
                    float energyLeft = isDecrement ? current - boundValue : boundValue - current;
                    // Полные циклы (корректируем количество итераций в зависимости от знака incrementValue)
                    int iterationsCount = Mathf.CeilToInt(energyLeft / Math.Abs(incrementValue));
                    // Сколько секунд осталось до полного восстановления/уменьшения
                    int fullSecondsLeft = iterationsCount * incrementInterval;
                    // Один из оставшихся отсчетов уже запущен, вычтем сколько ему осталось (пример выше)
                    int left = fullSecondsLeft - (incrementInterval - GetSecondsLeft(key));

                    return left >= 0 ? left : 0;
                }

                return 0;
            }
            else
            {
                string err = $"field \"{key}\" does not exists";
                Logger.Error("Get Seconds Left Total", err);
                //throw new Exception(err);
                return 0;
            }
        }


        public int GetActiveDays()
        {
            return _playerStats.activeDays;
        }

        public int GetActiveDaysConsecutive()
        {
            return _playerStats.activeDaysConsecutive;
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
            int newValue = oldValue + value;
            SetStateValue(key, newValue);
        }

        public void Add(string key, float value)
        {
            float oldValue = Get<float>(key);
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
                return type switch
                {
                    "stats" => Helpers.ConvertValue<double>(_playerState[key]) != 0,
                    "data" => Helpers.ConvertValue<string>(_playerState[key]) != "",
                    "flag" => Helpers.ConvertValue<bool>(_playerState[key]) != false,
                    "service" => false,
                    "accounts" => false,
                    _ => false
                };
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
                //Logger.Log($"{key}, {_playerState[key]}, {defaultState[key]}");
                if (key == NAME_STATE_KEY || key == AVATAR_STATE_KEY) continue;

                if (_playerState[key].ToString() != defaultState[key].ToString())
                {
                    return false;
                }
            }
            return true;
        }
        #endregion

        #region Account

        public void Login(Action onLoginComplete = null, Action<string> onLoginError = null)
        {
            Logger.Log("Try to login");
            Action combinedComplete = () =>
            {
                onLoginComplete?.Invoke();
                OnLoginComplete?.Invoke();
            };

            Action<string> combinedError = (string error) =>
            {
                onLoginError?.Invoke(error);
                OnLoginError?.Invoke(error);
            };

#if XSOLLA_SERVICE
            AuthService.Login(combinedComplete, combinedError);
#else
            combinedError?.Invoke("Nо auth service for login");
#endif

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
            //Logger.Log($"ServerTime", CoreSDK.GetConfig().serverTime);

            DateTime session;
            TimeSpan timeFromStart = TimeSpan.Zero;

            if (DateTime.TryParse(sessionStart, out session))
            {
                DateTime tempSession = session.ToUniversalTime();
                DateTime tempServer = CoreSDK.GetServerTime().ToUniversalTime().ToLocalTime();

                timeFromStart = tempServer - tempSession;
            }

            //Debug.Log($"timeFromStart {timeFromStart.TotalSeconds}");
            _playTimeAll = _playerStats.playtimeAll + timeFromStart.TotalSeconds;
            _playTimeToday = _playerStats.playtimeToday + timeFromStart.TotalSeconds;
        }

        #endregion

        #region TickMethods

        public void AutoSync()
        {
            foreach (SyncStorageType storage in autoSyncList.Keys)
            {
                //Logger.Log("Autosync", $"{storage} check {autoSyncList[storage].isEnable}");
                if (autoSyncList[storage].isEnable)
                {
                    int interval = autoSyncList[storage].interval;
                    //Logger.Log("interval", interval.ToString());
                    DateTime currentTime = CoreSDK.GetServerTime();
                    DateTime lastSyncTime;
                    if(DateTime.TryParse(autoSyncList[storage].lastSync, out lastSyncTime))
                    {
                        if ((currentTime - lastSyncTime).TotalSeconds >= interval)
                        {
                            bool isNeedToSync = _playerUpdateTime > lastSyncTimeList[storage];
                            //Logger.Log("isNeedToSync", isNeedToSync.ToString());
                            if (isNeedToSync)
                            {
                                lastSyncTimeList[storage] = CoreSDK.GetServerTime();
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

            //Logger.Log(elapsedInterval.ToString(), incrementInterval.ToString())
            int increments = 0;
            if (incrementInterval != 0)
                increments = Mathf.FloorToInt(elapsedInterval / incrementInterval);

            if (increments > 0 && ((incrementValue > 0 && oldValue < max) || (incrementValue < 0 && oldValue > min)))
            {
                float newValue = oldValue + increments * incrementValue;

                newValue = Mathf.Min(Math.Max(newValue, min), max);

                OnFieldIncrement?.Invoke(FieldToFetchField(field));

                DateTime newTimestamp = timestamp.AddSeconds(incrementInterval * increments);
                _playerState[$"{field.key}:timestamp"] = newTimestamp.ToString();

                //Logger.Warn(field.key, newValue.ToString());
                Set(field.key, newValue);
            }
        }
        #endregion

        private void PrintPlayerFields(List<PlayerField> fields)
        {
            foreach (PlayerField field in fields)
                PrintPlayerField(field);
        }

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
            Logger.Log(log);
        }

    }
}
