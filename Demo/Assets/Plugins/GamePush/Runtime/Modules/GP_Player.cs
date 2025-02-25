using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using GamePush.Utilities;

namespace GamePush
{
    public class GP_Player : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Player);
        
        public static List<PlayerFieldData> PlayerFields = new List<PlayerFieldData>();
        
        #region  Events
        public static event UnityAction OnConnect;
        public static event UnityAction OnPlayerChange;
        public static event UnityAction OnSyncComplete;
        public static event UnityAction OnSyncError;
        public static event UnityAction OnLoadComplete;
        public static event UnityAction OnLoadError;
        public static event UnityAction OnLoginComplete;
        public static event UnityAction OnLoginError;

        public static event UnityAction OnLogoutComplete;
        public static event UnityAction OnLogoutError;

        public static event UnityAction<List<PlayerFieldData>> OnPlayerFetchFieldsComplete;
        public static event UnityAction OnPlayerFetchFieldsError;

        public static event UnityAction<PlayerFieldData> OnFieldMaximum;
        public static event UnityAction<PlayerFieldData> OnFieldMinimum;
        public static event UnityAction<PlayerFieldData> OnFieldIncrement;

        private static event Action<List<PlayerFieldData>> _onFetchFields;

        #endregion

        #region Callbacks

        private void CallPlayerChange() => OnPlayerChange?.Invoke();
        private void CallPlayerConnect() => OnConnect?.Invoke();

        private void CallPlayerSyncComplete() => OnSyncComplete?.Invoke();
        private void CallPlayerSyncError() => OnSyncError?.Invoke();

        private void CallPlayerLoadComplete() => OnLoadComplete?.Invoke();
        private void CallPlayerLoadError() => OnLoadError?.Invoke();

        private void CallPlayerLoginComplete() => OnLoginComplete?.Invoke();
        private void CallPlayerLoginError() => OnLoginError?.Invoke();

        private void CallPlayerLogoutComplete() => OnLogoutComplete?.Invoke();
        private void CallPlayerLogoutError() => OnLogoutError?.Invoke();

        private void CallPlayerFetchFieldsComplete(string data)
        {
            OnPlayerFetchFieldsComplete?.Invoke(UtilityJSON.GetList<PlayerFieldData>(data));
            _onFetchFields?.Invoke(UtilityJSON.GetList<PlayerFieldData>(data));
        }
        private void CallPlayerFetchFieldsError() => OnPlayerFetchFieldsError?.Invoke();

        private void CallPlayerFieldReachMaximum(string field) =>
            OnFieldMaximum?.Invoke(UtilityJSON.Get<PlayerFieldData>(field));
        private void CallPlayerFieldReachMinimum(string field) =>
            OnFieldMinimum?.Invoke(UtilityJSON.Get<PlayerFieldData>(field));
        private void CallPlayerFieldIncrement(string field) =>
            OnFieldIncrement?.Invoke(UtilityJSON.Get<PlayerFieldData>(field));

        #endregion
        
        #region DLL Imports
#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern int GP_Player_GetID();
        
        [DllImport("__Internal")]
        private static extern float GP_Player_GetScore();
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetName();
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetAvatar();
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldName(string key);
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantName(string key, string value);
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantAt(string key, string index);
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantIndex(string key, string value);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_SetName(string name);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_SetAvatar(string src);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_SetScore(float score);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_AddScore(float score);
        
        [DllImport("__Internal")]
        private static extern int GP_Player_GetNumberInt(string key);
        
        [DllImport("__Internal")]
        private static extern float GP_Player_GetNumberFloat(string key);

        [DllImport("__Internal")]
        private static extern float GP_Player_GetMaxValue(string key);
        
        [DllImport("__Internal")]
        private static extern float GP_Player_GetMinValue(string key);
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetString(string key);
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetBool(string key);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Set_Number(string key, float value);

        [DllImport("__Internal")]
        private static extern void GP_Player_Set_Bool(string key, string value);

        [DllImport("__Internal")]
        private static extern void GP_Player_Set_String(string key, string value);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_SetFlag(string key, bool value);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Add(string key, string value);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Toggle(string key);
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Reset();
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Remove();
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Sync(bool forceOverride = false, string storage = "preferred");

        [DllImport("__Internal")]
        private static extern void GP_Player_EnableAutoSync(int interval = 10, string storage = "cloud");
        
        [DllImport("__Internal")]
        private static extern void GP_Player_DisableAutoSync(string storage = "cloud");
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Load();

        [DllImport("__Internal")]
        private static extern void GP_Player_Login();
        
        [DllImport("__Internal")]
        private static extern void GP_Player_Logout();
        
        [DllImport("__Internal")]
        private static extern void GP_Player_FetchFields();

        [DllImport("__Internal")]
        private static extern string GP_Player_Has(string key);
        
        [DllImport("__Internal")]
        private static extern string GP_Player_IsLoggedIn();
        
        [DllImport("__Internal")]
        private static extern string GP_Player_HasAnyCredentials();

        [DllImport("__Internal")]
        private static extern string GP_Player_IsStub();
        
        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDays();
        
        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDaysConsecutive();
        
        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeToday();

        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeAll();
#endif
        #endregion
        
        private async void Start()
        {
            await GP_Init.Ready;
            FetchFields(fields =>
            {
                PlayerFields = fields;
            });
        }

        #region Getters

        public static int GetID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetID();
#else

            int id = GP_Prefs.TryGet<int>("id");
            ConsoleLog("GET ID: " + id);
            return id;
#endif
        }

        public static float GetScore()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetScore();
#else
            
            float score = GP_Prefs.TryGet<float>("score");
            ConsoleLog("GET SCORE: " + score);
            return score;
#endif
        }
        
        public static string GetName()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetName();
#else
            string name = GP_Prefs.TryGet<string>("name");
            ConsoleLog("GET NAME: " + name);
            return name;
#endif
        }
        
        public static string GetAvatarUrl()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetAvatar();
#else

            string avatar = GP_Prefs.TryGet<string>("avatar");
            ConsoleLog("GET AVATAR URL: " + avatar);
            return avatar;
#endif
        }
        public async static void GetAvatar(Image image)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            string avatar = GP_Player_GetAvatar();
            if (avatar == null || avatar == "") return;
            await UtilityImage.DownloadImageAsync(avatar, image);
#else
            await Task.Delay(1);
            ConsoleLog("GET AVATAR");
            image.sprite = Sprite.Create(
                Texture2D.normalTexture, 
                new Rect(image.rectTransform.position, image.rectTransform.sizeDelta),
                image.rectTransform.pivot);
#endif
        }
        public static string GetFieldName(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldName(key);
#else

            ConsoleLog("GET FIELD NAME: " + key);
            return null;
#endif
        }
        
        public static int GetInt(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberInt(key);
#else

            int value = GP_Prefs.TryGet<int>(key);
            ConsoleLog("GET INT: KEY: " + key + " -> "  + value);
            return value;
#endif
        }

        public static float GetFloat(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberFloat(key);
#else

            float value = GP_Prefs.TryGet<float>(key);
            ConsoleLog("GET FLOAT: KEY: " + key + " -> " + value);
            return value;
#endif
        }
        
        public static string GetString(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetString(key);
#else
            string value = GP_Prefs.TryGet<string>(key);
            ConsoleLog("GET STRING: KEY: " + key + " -> " + value);
            return value;
#endif
        }

        public static bool GetBool(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetBool(key) == "true";
#else

            bool value = GP_Prefs.TryGet<bool>(key);
            ConsoleLog("GET BOOL: KEY: " + key + " -> " + value);
            return value;
#endif
        }
        
        public static float GetMaxValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetMaxValue(key);
#else

            ConsoleLog("GET MAX: KEY: " + key);
            return 100;
#endif
        }
        
        public static float GetMinValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetMinValue(key);
#else

            ConsoleLog("GET MIN: KEY: " + key);
            return 0;
#endif
        }
        
        #endregion
        
        #region Variant Getters
        public static string GetFieldVariantName(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantName(key, value);
#else

            ConsoleLog("GET FIELD VARIANT NAME: KEY: " + key + " VALUE: " + value);
            return null;
#endif
        }
        
        public static string GetFieldVariantAt(string key, int index)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantAt(key, index.ToString());
#else

            ConsoleLog("GET FIELD VARIANT AT: KEY: " + key + " INDEX: " + index);
            return null;
#endif
        }

        public static string GetFieldVariantIndex(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantIndex(key, value);
#else

            ConsoleLog("GET FIELD VARIANT INDEX: KEY: " + key + " VALUE: " + value);
            return null;
#endif
        }

        #endregion

        #region Setters

        public static void SetName(string name)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetName(name);
#else
            Set("name", name);
            ConsoleLog("SET NAME: " + name);
#endif
        }
        
        public static void SetAvatar(string src)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetAvatar(src);
#else
            Set("avatar", src);
            ConsoleLog("SET AVATAR: " + src);
#endif
        }

        public static void SetScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else

            Set("score", score);
            ConsoleLog("SET SCORE: " + score);
#endif
        }
        public static void SetScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else

            Set("score", (float)score);
            ConsoleLog("SET SCORE: " + score);
#endif
        }
        
        public static void Set(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_String(key, value);
#else

            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else
            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Bool(key, value.ToString());
#else
            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else
            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        
        public static void SetFlag(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetFlag(key, value);
#else
            GP_Prefs.Set(key, value);
            ConsoleLog("SET FLAG: KEY: " + key + " VALUE: " + value);
#endif
        }
        
        public static void Toggle(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Toggle(key);
#else
            Set(key, !GetBool(key));
            ConsoleLog("TOGGLE: KEY: " + key);
#endif
        }

        #endregion

        #region Adders

        public static void Add(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else
            GP_Prefs.Add(key, value);
            ConsoleLog("ADD: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Add(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else
            GP_Prefs.Add(key, value);
            ConsoleLog("ADD: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void AddScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else
            Add("score", score);
            ConsoleLog("ADD SCORE: " + score);
#endif
        }
        public static void AddScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else
            Add("score", (float)score);
            ConsoleLog("ADD SCORE: " + score);
#endif
        }

        #endregion

        #region StateChangers

        public static void ResetPlayer()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Reset();
#else
            GP_Prefs.Reset();
            ConsoleLog("RESET");
#endif
        }

        public static void Remove()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Remove();
#else

            ConsoleLog("REMOVE");
#endif
        }
        
        public static void Login()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Login();
#else

            ConsoleLog("LOGIN");
#endif
        }

        public static void Logout()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Logout();
#else

            ConsoleLog("LOGOUT");
#endif
        }

        #endregion

        #region Sync/Load

        public static void Sync(SyncStorageType storage = SyncStorageType.preffered, bool forceOverride = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Sync(forceOverride: forceOverride, storage: storage.ToString());
#else

            ConsoleLog($"SYNC: {storage.ToString()}");
#endif
        }

        public static void Sync(bool forceOverride)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Sync(forceOverride: forceOverride);
#else

            ConsoleLog("SYNC");
#endif
        }
        
        public static void Load()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Load();
#else

            ConsoleLog("LOAD");
#endif
        }

        public static void EnableAutoSync(int interval = 10, SyncStorageType storage = SyncStorageType.cloud)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_EnableAutoSync(interval, storage.ToString());
#else
            ConsoleLog("AUTO SYNC: ON");
#endif
        }

        public static void DisableAutoSync(SyncStorageType storage = SyncStorageType.cloud)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_DisableAutoSync(storage.ToString());
#else
            ConsoleLog("AUTO SYNC: OFF");
#endif
        }

        #endregion

        #region Playtime Getters

        public static int GetActiveDays()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetActiveDays();
#else

            ConsoleLog("ACTIVE DAYS: 1");
            return 0;
#endif
        }

        public static int GetActiveDaysConsecutive()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetActiveDaysConsecutive();
#else

            ConsoleLog("ACTIVE DAYS CONSECUTIVE: 1");
            return 0;
#endif
        }

        public static int GetPlaytimeToday()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetPlaytimeToday();
#else

            ConsoleLog("PLAYTIME TODAY: 0");
            return 0;
#endif
        }
        
        public static int GetPlaytimeAll()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetPlaytimeAll();
#else

            ConsoleLog("PLAYTIME ALL: 0");
            return 0;
#endif
        }

        #endregion
        
        #region IsFuncs

        public static bool Has(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_Has(key) == "true";
#else
            GP_Prefs.HasKey(key);
            ConsoleLog("KEY: " + key + " -> EMPTY");
            return true;
#endif
        }

        public static bool IsLoggedIn()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_IsLoggedIn() == "true";
#else

            ConsoleLog("IS LOGGED IN: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsLoggedIn;
#endif
        }
        
        public static bool HasAnyCredentials()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_HasAnyCredentials() == "true";
#else

            ConsoleLog("HAS ANY CREDENTIALS: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().HasAnyCredentials;
#endif
        }

        public static bool IsStub()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_IsStub() == "true";
#else
            bool isStub = GP_Settings.instance.GetFromPlatformSettings().IsStub;
            ConsoleLog("IS STUB: " + isStub);
            return isStub;
#endif
        }

        #endregion
        
        public static void FetchFields(Action<List<PlayerFieldData>> onFetchFields = null)
        {
            _onFetchFields = onFetchFields;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_FetchFields();
#else

            ConsoleLog("FETCH FIELDS");
#endif
        }

        
    }

    [System.Serializable]
    public enum SyncStorageType
    {
        preffered,
        local,
        platform,
        cloud
    }

    [System.Serializable]
    public class PlayerFieldData
    {
        public string name;
        public string key;
        public string type;
        public string defaultValue; // string | bool | number
        public bool important;
        public bool @public;
        public PlayerFieldIncrement intervalIncrement;
        public PlayerFieldLimits limits;
        public PlayerFieldVariant[] variants;
    }

    [System.Serializable]
    public class PlayerFieldIncrement
    {
        public float interval;
        public float increment;
    }

    [System.Serializable]
    public class PlayerFieldLimits
    {
        public float min;
        public float max;
        public bool couldGoOverLimit;
    }

    [System.Serializable]
    public class PlayerFieldVariant
    {
        public string value; // string | number
        public string name;
    }
}