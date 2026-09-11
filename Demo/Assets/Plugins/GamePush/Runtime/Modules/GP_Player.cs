using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using GamePush.Utilities;
using GamePush.Native;

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

        private static event Action<List<PlayerFieldData>> OnFetchFields;

        #endregion

        #region Callbacks

        private void CallPlayerChange() => OnPlayerChange?.Invoke();
        private void CallPlayerConnect() => OnConnect?.Invoke();

        private void CallPlayerSyncComplete() => OnSyncComplete?.Invoke();
        private void CallPlayerSyncError() => OnSyncError?.Invoke();

        private void CallPlayerLoadComplete() => OnLoadComplete?.Invoke();
        private void CallPlayerLoadError() => OnLoadError?.Invoke();

        private void CallPlayerLoginComplete()
        {
            GP_WebGLInput.Restore();
            OnLoginComplete?.Invoke();
        }
        private void CallPlayerLoginError()
        {
            GP_WebGLInput.Restore();
            OnLoginError?.Invoke();
        }

        private void CallPlayerLogoutComplete() => OnLogoutComplete?.Invoke();
        private void CallPlayerLogoutError() => OnLogoutError?.Invoke();

        private void CallPlayerFetchFieldsComplete(string data)
        {
            OnPlayerFetchFieldsComplete?.Invoke(UtilityJSON.GetList<PlayerFieldData>(data));
            OnFetchFields?.Invoke(UtilityJSON.GetList<PlayerFieldData>(data));
        }
        private void CallPlayerFetchFieldsError() => OnPlayerFetchFieldsError?.Invoke();

        private void CallPlayerFieldReachMaximum(string field) =>
            OnFieldMaximum?.Invoke(UtilityJSON.Get<PlayerFieldData>(field));
        private void CallPlayerFieldReachMinimum(string field) =>
            OnFieldMinimum?.Invoke(UtilityJSON.Get<PlayerFieldData>(field));
        private void CallPlayerFieldIncrement(string field) =>
            OnFieldIncrement?.Invoke(UtilityJSON.Get<PlayerFieldData>(field));

        internal static void NotifyNativeSync() => OnSyncComplete?.Invoke();
        internal static void NotifyNativeLogin(bool success)
        {
            if (success) OnLoginComplete?.Invoke();
            else OnLoginError?.Invoke();
        }
        internal static void NotifyNativeLogout(bool success)
        {
            if (success) OnLogoutComplete?.Invoke();
            else OnLogoutError?.Invoke();
        }

        #endregion
        
        #region DLL Imports
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
        [DllImport("__Internal")]
        private static extern int GP_Player_GetID();
        
        [DllImport("__Internal")]
        private static extern float GP_Player_GetScore();
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetName();
        
        [DllImport("__Internal")]
        private static extern string GP_Player_GetAvatar();
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldName(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantName(string key, string value);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantAt(string key, string index);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantIndex(string key, string value);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_SetName(string name);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_SetAvatar(string src);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_SetScore(float score);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_AddScore(float score);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GP_Player_GetNumberInt(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern float GP_Player_GetNumberFloat(string key);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern float GP_Player_GetMaxValue(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern float GP_Player_GetMinValue(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_GetString(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_GetBool(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Set_Number(string key, float value);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Set_Bool(string key, string value);
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Set_String(string key, string value);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_SetFlag(string key, bool value);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Add(string key, string value);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Toggle(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Reset();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Remove();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Sync(bool forceOverride = false, string storage = "preferred");
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_EnableAutoSync(int interval = 10, string storage = "cloud");
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_DisableAutoSync(string storage = "cloud");
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Load();
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Login();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_Logout();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern void GP_Player_FetchFields();
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_Has(string key);
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_IsLoggedIn();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_HasAnyCredentials();
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Player_IsStub();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDays();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDaysConsecutive();
        #endif
        
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeToday();
        #endif

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeAll();
        #endif
#endif
        #endregion
        
        #region Getters

        public static int GetID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetID();
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.Id;
            if (GP_Play2Web.TryGetInt("PlayerGetID", out var live))
                return live;
            int id = GP_Prefs.TryGet<int>("id");
            ConsoleLog("GET ID: " + id);
            return id;
#endif
        }

        public static float GetScore()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetScore();
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.GetFloat("score");
            if (GP_Play2Web.TryGetFloat("PlayerGetScore", out var live))
                return live;
            float score = GP_Prefs.TryGet<float>("score");
            ConsoleLog("GET SCORE: " + score);
            return score;
#endif
        }
        
        public static string GetName()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetName();
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.GetString("name");
            if (GP_Play2Web.TryGet("PlayerGetName", out var live))
                return live;
            string name = GP_Prefs.TryGet<string>("name");
            ConsoleLog("GET NAME: " + name);
            return name;
#endif
        }
        
        public static string GetAvatarUrl()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetAvatar();
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.GetString("avatar");
            if (GP_Play2Web.TryGet("PlayerGetAvatar", out var live))
                return live;
            string avatar = GP_Prefs.TryGet<string>("avatar");
            ConsoleLog("GET AVATAR URL: " + avatar);
            return avatar;
#endif
        }
        public async static void GetAvatar(Image image)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetFieldName(key);
#else

            ConsoleLog("GET FIELD NAME: " + key);
            return null;
#endif
        }
        
        public static int GetInt(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetNumberInt(key);
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.GetInt(key);
            if (GP_Play2Web.TryGet("player:" + key, out var liveInt) && int.TryParse(liveInt, out var parsedInt))
                return parsedInt;
            int value = GP_Prefs.TryGet<int>(key);
            ConsoleLog("GET INT: KEY: " + key + " -> "  + value);
            return value;
#endif
        }

        public static float GetFloat(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetNumberFloat(key);
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.GetFloat(key);
            if (GP_Play2Web.TryGet("player:" + key, out var liveFloat) && float.TryParse(liveFloat, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var parsedFloat))
                return parsedFloat;
            float value = GP_Prefs.TryGet<float>(key);
            ConsoleLog("GET FLOAT: KEY: " + key + " -> " + value);
            return value;
#endif
        }
        
        public static string GetString(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetString(key);
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.GetString(key);
            if (GP_Play2Web.TryGet("player:" + key, out var liveStr))
                return liveStr;
            string value = GP_Prefs.TryGet<string>(key);
            ConsoleLog("GET STRING: KEY: " + key + " -> " + value);
            return value;
#endif
        }

        public static bool GetBool(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetBool(key) == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.GetBool(key);
            if (GP_Play2Web.TryGetBool("player:" + key, out var liveBool))
                return liveBool;
            bool value = GP_Prefs.TryGet<bool>(key);
            ConsoleLog("GET BOOL: KEY: " + key + " -> " + value);
            return value;
#endif
        }
        
        public static float GetMaxValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetMaxValue(key);
#else

            ConsoleLog("GET MAX: KEY: " + key);
            return 100;
#endif
        }
        
        public static float GetMinValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetFieldVariantName(key, value);
#else

            ConsoleLog("GET FIELD VARIANT NAME: KEY: " + key + " VALUE: " + value);
            return null;
#endif
        }
        
        public static string GetFieldVariantAt(string key, int index)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetFieldVariantAt(key, index.ToString());
#else

            ConsoleLog("GET FIELD VARIANT AT: KEY: " + key + " INDEX: " + index);
            return null;
#endif
        }

        public static string GetFieldVariantIndex(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_SetName(name);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Set("name", name);
                return;
            }
            if (GP_Play2Web.Call("PlayerSetName", name))
                return;
            Set("name", name);
            ConsoleLog("SET NAME: " + name);
#endif
        }
        
        public static void SetAvatar(string src)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_SetAvatar(src);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Set("avatar", src);
                return;
            }
            if (GP_Play2Web.Call("PlayerSetAvatar", src))
                return;
            Set("avatar", src);
            ConsoleLog("SET AVATAR: " + src);
#endif
        }

        public static void SetScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_SetScore(score);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Set("score", score.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return;
            }
            if (GP_Play2Web.Call("PlayerSetScore", score))
                return;
            Set("score", score);
            ConsoleLog("SET SCORE: " + score);
#endif
        }
        public static void SetScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_SetScore(score);
#else

            Set("score", (float)score);
            ConsoleLog("SET SCORE: " + score);
#endif
        }
        
        public static void Set(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Set_String(key, value);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Set(key, value);
                return;
            }
            if (GP_Play2Web.Call("PlayerSetString", key, value))
                return;
            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Set_Number(key, value);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Set(key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return;
            }
            if (GP_Play2Web.Call("PlayerSetNumber", key, value))
                return;
            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Set_Bool(key, value.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Set(key, value ? "true" : "false");
                return;
            }
            if (GP_Play2Web.Call("PlayerSetBool", key, value.ToString()))
                return;
            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Set_Number(key, value);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Set(key, value.ToString(System.Globalization.CultureInfo.InvariantCulture));
                return;
            }
            if (GP_Play2Web.Call("PlayerSetNumber", key, value))
                return;
            GP_Prefs.Set(key, value);
            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        
        public static void SetFlag(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_SetFlag(key, value);
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.SetFlag(key, value);
                return;
            }
            GP_Prefs.Set(key, value);
            ConsoleLog("SET FLAG: KEY: " + key + " VALUE: " + value);
#endif
        }
        
        public static void Toggle(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Add(key, value.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Add(key, value);
                return;
            }
            GP_Prefs.Add(key, value);
            ConsoleLog("ADD: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Add(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Add(key, value.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.Add(key, value);
                return;
            }
            GP_Prefs.Add(key, value);
            ConsoleLog("ADD: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void AddScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_AddScore(score);
#else
            Add("score", score);
            ConsoleLog("ADD SCORE: " + score);
#endif
        }
        public static void AddScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Reset();
#else
            GP_Prefs.Reset();
            ConsoleLog("RESET");
#endif
        }

        public static void Remove()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Remove();
#else

            ConsoleLog("REMOVE");
#endif
        }
        
        public static void Login()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_WebGLInput.Release();
            GP_Player_Login();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAuth.Login();
                return;
            }
            if (GP_Play2Web.Call("PlayerLogin"))
                return;
            ConsoleLog("LOGIN");
#endif
        }

        public static void Logout()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Logout();
#else
            if (GamePushHost.UseNativeCore)
            {
                NativeAuth.Logout();
                return;
            }
            if (GP_Play2Web.Call("PlayerLogout"))
                return;
            ConsoleLog("LOGOUT");
#endif
        }

        #endregion

        #region Sync/Load

        public static void Sync(SyncStorageType storage = SyncStorageType.preferred, bool forceOverride = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Sync(forceOverride: forceOverride, storage: storage.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                _ = NativePlayer.Sync(forceOverride);
                return;
            }
            if (GP_Play2Web.Call("PlayerSync", storage.ToString(), forceOverride.ToString()))
                return;
            ConsoleLog($"SYNC: {storage.ToString()}");
#endif
        }

        public static void Sync(bool forceOverride)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Sync(forceOverride: forceOverride);
#else
            if (GamePushHost.UseNativeCore)
            {
                _ = NativePlayer.Sync(forceOverride);
                return;
            }

            ConsoleLog("SYNC");
#endif
        }
        
        public static void Load()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_Load();
#else
            if (GP_Play2Web.Call("PlayerLoad"))
                return;
            ConsoleLog("LOAD");
#endif
        }

        public static void EnableAutoSync(int interval = 10, SyncStorageType storage = SyncStorageType.cloud)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_EnableAutoSync(interval, storage.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.EnableAutoSync(interval);
                return;
            }
            ConsoleLog("AUTO SYNC: ON");
#endif
        }

        public static void DisableAutoSync(SyncStorageType storage = SyncStorageType.cloud)
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_DisableAutoSync(storage.ToString());
#else
            if (GamePushHost.UseNativeCore)
            {
                NativePlayer.DisableAutoSync();
                return;
            }
            ConsoleLog("AUTO SYNC: OFF");
#endif
        }

        #endregion

        #region Playtime Getters

        public static int GetActiveDays()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetActiveDays();
#else

            ConsoleLog("ACTIVE DAYS: 1");
            return 0;
#endif
        }

        public static int GetActiveDaysConsecutive()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetActiveDaysConsecutive();
#else

            ConsoleLog("ACTIVE DAYS CONSECUTIVE: 1");
            return 0;
#endif
        }

        public static int GetPlaytimeToday()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_GetPlaytimeToday();
#else

            ConsoleLog("PLAYTIME TODAY: 0");
            return 0;
#endif
        }
        
        public static int GetPlaytimeAll()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_Has(key) == "true";
#else
            GP_Prefs.HasKey(key);
            ConsoleLog("KEY: " + key + " -> EMPTY");
            return true;
#endif
        }

        public static bool IsLoggedIn()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_IsLoggedIn() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.IsLoggedIn;
            if (GP_Play2Web.TryGetBool("PlayerIsLoggedIn", out var live))
                return live;
            ConsoleLog("IS LOGGED IN: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsLoggedIn;
#endif
        }
        
        public static bool HasAnyCredentials()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_HasAnyCredentials() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return !string.IsNullOrEmpty(NativePlayer.Credentials);
            ConsoleLog("HAS ANY CREDENTIALS: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().HasAnyCredentials;
#endif
        }

        public static bool IsStub()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Player_IsStub() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativePlayer.IsStub();
            bool isStub = GP_Settings.instance.GetFromPlatformSettings().IsStub;
            ConsoleLog("IS STUB: " + isStub);
            return isStub;
#endif
        }

        #endregion
        
        public static void FetchFields(Action<List<PlayerFieldData>> onFetchFields = null)
        {
            OnFetchFields = onFetchFields;

#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            GP_Player_FetchFields();
#else

            ConsoleLog("FETCH FIELDS");
#endif
        }

        
    }

    [System.Serializable]
    public enum SyncStorageType
    {
        preferred,
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