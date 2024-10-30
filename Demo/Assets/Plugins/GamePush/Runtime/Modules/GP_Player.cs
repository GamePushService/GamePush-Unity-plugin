using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using GamePush.Data;
using GamePush.Tools;

namespace GamePush
{
    public class GP_Player : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Player);

        #region Actions
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

        public static event UnityAction<List<PlayerFetchFieldsData>> OnPlayerFetchFieldsComplete;
        public static event UnityAction OnPlayerFetchFieldsError;

        private static event Action<List<PlayerFetchFieldsData>> _onFetchFields;

        public static event UnityAction<PlayerFetchFieldsData> OnFieldMaximum;
        public static event UnityAction<PlayerFetchFieldsData> OnFieldMinimum;
        public static event UnityAction<PlayerFetchFieldsData> OnFieldIncrement;
        #endregion

        private void FieldMaximum(PlayerFetchFieldsData data) => OnFieldMaximum?.Invoke(data);
        private void FieldMinimum(PlayerFetchFieldsData data) => OnFieldMinimum?.Invoke(data);
        private void FieldIncrement(PlayerFetchFieldsData data) => OnFieldIncrement?.Invoke(data);

        private void OnEnable()
        {
            CoreSDK.player.OnPlayerChange += CallPlayerChange;

            CoreSDK.player.OnSyncComplete += CallPlayerSyncComplete;
            CoreSDK.player.OnSyncError += CallPlayerSyncError;

            CoreSDK.player.OnLoadComplete += CallPlayerLoadComplete;
            CoreSDK.player.OnLoadError += CallPlayerLoadError;

            CoreSDK.player.OnLoginComplete += CallPlayerLoginComplete;
            CoreSDK.player.OnLoginError += CallPlayerLoginError;

            CoreSDK.player.OnLogoutComplete += CallPlayerLoginComplete;
            CoreSDK.player.OnLogoutError += CallPlayerLoginError;

            CoreSDK.player.OnFieldMaximum += FieldMaximum;
            CoreSDK.player.OnFieldMinimum += FieldMinimum;
            CoreSDK.player.OnFieldIncrement += FieldIncrement;
        }

        private void OnDisable()
        {
            CoreSDK.player.OnPlayerChange -= CallPlayerChange;

            CoreSDK.player.OnSyncComplete -= CallPlayerSyncComplete;
            CoreSDK.player.OnSyncError -= CallPlayerSyncError;

            CoreSDK.player.OnLoadComplete -= CallPlayerLoadComplete;
            CoreSDK.player.OnLoadError -= CallPlayerLoadError;

            CoreSDK.player.OnLoginComplete -= CallPlayerLoginComplete;
            CoreSDK.player.OnLoginError -= CallPlayerLoginError;

            CoreSDK.player.OnLogoutComplete -= CallPlayerLoginComplete;
            CoreSDK.player.OnLogoutError -= CallPlayerLoginError;

            CoreSDK.player.OnFieldMaximum -= FieldMaximum;
            CoreSDK.player.OnFieldMinimum -= FieldMinimum;
            CoreSDK.player.OnFieldIncrement -= FieldIncrement;
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        #region DllImport
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
        #endregion
#endif

        #region Methods
        public static int GetID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetID();
#else

            return CoreSDK.player.GetID();
#endif
        }

        public static float GetScore()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetScore();
#else
            return CoreSDK.player.GetScore();
#endif
        }

        public static string GetName()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetName();
#else
            return CoreSDK.player.GetName();
#endif
        }

        public static string GetAvatarUrl()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetAvatar();
#else
            return CoreSDK.player.GetAvatarUrl();
#endif
        }

#if UNITY_WEBGL
        public async static void GetAvatar(Image image)
        {
            string avatar = GP_Player_GetAvatar();
            if (avatar == null || avatar == "") return;
            await UtilityImage.DownloadImageAsync(avatar, image);
        }
#endif

        public static string GetFieldName(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldName(key);
#else
            return CoreSDK.player.GetFieldName(key);
#endif
        }

        public static string GetFieldVariantName(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantName(key, value);
#else
            return CoreSDK.player.GetFieldVariantName(key, value);
#endif
        }

        public static string GetFieldVariantAt(string key, int index)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantAt(key, index.ToString());
#else
            return CoreSDK.player.GetFieldVariantAt(key, index);
#endif
        }

        public static string GetFieldVariantIndex(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantIndex(key, value);
#else
            return CoreSDK.player.GetFieldVariantIndex(key, value);
#endif
        }

        public static void SetName(string name)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetName(name);
#else
            CoreSDK.player.SetName(name);
#endif
        }

        public static void SetAvatar(string src)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetAvatar(src);
#else
            CoreSDK.player.SetAvatar(src);
#endif
        }

        public static void SetScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else
            CoreSDK.player.SetScore(score);
#endif
        }

        public static void SetScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else
            CoreSDK.player.SetScore(score);
#endif
        }

        public static void AddScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else
            CoreSDK.player.AddScore(score);
#endif
        }

        public static void AddScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else
            CoreSDK.player.AddScore(score);
#endif
        }

        public static T Get<T>(string key)
        {
            return CoreSDK.player.Get<T>(key);
        }

        public static int GetInt(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberInt(key);
#else
            return CoreSDK.player.Get<int>(key);
#endif
        }

        public static float GetFloat(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberFloat(key);
#else
            return CoreSDK.player.Get<float>(key);
#endif
        }

        public static float GetMaxValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetMaxValue(key);
#else
            
            return CoreSDK.player.GetMaxValue(key);
#endif
        }

        public static float GetMinValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetMinValue(key);
#else

            return CoreSDK.player.GetMinValue(key);
#endif
        }

        public static float GetSecondsLeft(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberInt($"{key}:secondsLeft");
#else
            return CoreSDK.player.GetSecondsLeft(key);
#endif
        }

        public static float GetSecondsLeftTotal(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberInt($"{key}:secondsLeftTotal");
#else
            return CoreSDK.player.GetSecondsLeftTotal(key);
#endif
        }

        public static string GetString(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetString(key);
#else

            return CoreSDK.player.Get<string>(key);
#endif
        }

        public static bool GetBool(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetBool(key) == "true";
#else

            return CoreSDK.player.Get<bool>(key);
#endif
        }

        public static void Set(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_String(key, value);
#else

            CoreSDK.player.Set(key, value);
#endif
        }
        public static void Set(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else
            CoreSDK.player.Set(key, value);
#endif
        }
        public static void Set(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Bool(key, value.ToString());
#else

            CoreSDK.player.Set(key, value);
#endif
        }
        public static void Set(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else

            CoreSDK.player.Set(key, value);
#endif
        }

        public static void SetFlag(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetFlag(key, value);
#else
            CoreSDK.player.Set(key, value);
#endif
        }

        public static void Add(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else

            CoreSDK.player.Add(key, value);
#endif
        }
        public static void Add(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else
            CoreSDK.player.Add(key, value);
#endif
        }

        public static void Toggle(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Toggle(key);
#else
            CoreSDK.player.Toggle(key);
#endif
        }

        public static void ResetPlayer()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Reset();
#else

            CoreSDK.player.Reset();
#endif
        }

        public static void Remove()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Remove();
#else
           
            CoreSDK.player.Remove();
#endif
        }

        public static void Sync(SyncStorageType storage = SyncStorageType.preffered, bool forceOverride = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Sync(forceOverride: forceOverride, storage: storage.ToString());
#else
            
            CoreSDK.player.PlayerSync(storage, forceOverride);
#endif
        }

        public static void Sync(bool forceOverride)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Sync(forceOverride: forceOverride);
#else
            
            CoreSDK.player.PlayerSync(forceOverride);
#endif
        }

        public static bool EnableAutoSync(int interval = 10, SyncStorageType storage = SyncStorageType.cloud)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_EnableAutoSync(interval, storage.ToString());
            return true;
#else
            return CoreSDK.player.EnableAutoSync(interval, storage);
#endif
        }

        public static bool DisableAutoSync(SyncStorageType storage = SyncStorageType.cloud)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_DisableAutoSync(storage.ToString());
            return true;
#else
            return CoreSDK.player.DisableAutoSync(storage);
#endif
        }

        public static void Load()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Load();
#else
            CoreSDK.player.PlayerLoad();
#endif
        }

        public static void Login()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Login();
#else

            CoreSDK.player.Login();
#endif
        }

        public static void Logout()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Logout();
#else

            CoreSDK.player.Logout();
#endif
        }

        public static void FetchFields(Action<List<PlayerFetchFieldsData>> onFetchFields = null)
        {
            _onFetchFields = onFetchFields;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_FetchFields();
#else

            ConsoleLog("FETCH FIELDS");
            CoreSDK.player.FetchFields(onFetchFields);
#endif
        }

        public static bool Has(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_Has(key) == "true";
#else

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

            ConsoleLog("IS STUB: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsStub;
#endif
        }

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
            OnPlayerFetchFieldsComplete?.Invoke(UtilityJSON.GetList<PlayerFetchFieldsData>(data));
            _onFetchFields?.Invoke(UtilityJSON.GetList<PlayerFetchFieldsData>(data));
        }
        private void CallPlayerFetchFieldsError() => OnPlayerFetchFieldsError?.Invoke();

        private void CallPlayerFieldReachMaximum(string field) =>
            OnFieldMaximum?.Invoke(UtilityJSON.Get<PlayerFetchFieldsData>(field));
        private void CallPlayerFieldReachMinimum(string field) =>
            OnFieldMinimum?.Invoke(UtilityJSON.Get<PlayerFetchFieldsData>(field));
        private void CallPlayerFieldIncrement(string field) =>
            OnFieldIncrement?.Invoke(UtilityJSON.Get<PlayerFetchFieldsData>(field));

#endregion
    }
}