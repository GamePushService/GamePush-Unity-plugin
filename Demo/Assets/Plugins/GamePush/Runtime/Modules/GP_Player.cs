using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using GamePush.Utilities;

namespace GamePush
{
    public class GP_Player : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Player);

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

        public static event UnityAction<PlayerFetchFieldsData> OnFieldMaximum;
        public static event UnityAction<PlayerFetchFieldsData> OnFieldMinimum;
        public static event UnityAction<PlayerFetchFieldsData> OnFieldIncrement;

        private static event Action<List<PlayerFetchFieldsData>> _onFetchFields;


        [DllImport("__Internal")]
        private static extern int GP_Player_GetID();
        public static int GetID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetID();
#else

            ConsoleLog("GET ID: 0");
            return 0;
#endif
        }


        [DllImport("__Internal")]
        private static extern float GP_Player_GetScore();
        public static float GetScore()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetScore();
#else

            ConsoleLog("GET SCORE: 0f");
            return 0f;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Player_GetName();
        public static string GetName()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetName();
#else

            ConsoleLog("GET NAME: UNKNOWN");
            return "UNKNOWN";
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetAvatar();
        public static string GetAvatarUrl()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetAvatar();
#else

            ConsoleLog("GET AVATAR URL: URL");
            return "URL";
#endif
        }
        public async static void GetAvatar(Image image)
        {
            string avatar = GP_Player_GetAvatar();
            if (avatar == null || avatar == "") return;
            await UtilityImage.DownloadImageAsync(avatar, image);
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldName(string key);
        public static string GetFieldName(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldName(key);
#else

            ConsoleLog("GET FIELD NAME: " + key);
            return null;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantName(string key, string value);
        public static string GetFieldVariantName(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantName(key, value);
#else

            ConsoleLog("GET FIELD VARIANT NAME: KEY: " + key + " VALUE: " + value);
            return null;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantAt(string key, string index);
        public static string GetFieldVariantAt(string key, int index)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantAt(key, index.ToString());
#else

            ConsoleLog("GET FIELD VARIANT AT: KEY: " + key + " INDEX: " + index);
            return null;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantIndex(string key, string value);
        public static string GetFieldVariantIndex(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantIndex(key, value);
#else

            ConsoleLog("GET FIELD VARIANT INDEX: KEY: " + key + " VALUE: " + value);
            return null;
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetName(string name);
        public static void SetName(string name)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetName(name);
#else

            ConsoleLog("SET NAME: " + name);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetAvatar(string src);
        public static void SetAvatar(string src)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetAvatar(src);
#else

            ConsoleLog("SET AVATAR: " + src);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetScore(float score);
        public static void SetScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else

            ConsoleLog("SET SCORE: " + score);
#endif
        }
        public static void SetScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else

            ConsoleLog("SET SCORE: " + score);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_AddScore(float score);
        public static void AddScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else

            ConsoleLog("ADD SCORE: " + score);
#endif
        }
        public static void AddScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else

            ConsoleLog("ADD SCORE: " + score);
#endif
        }



        [DllImport("__Internal")]
        private static extern int GP_Player_GetNumberInt(string key);
        public static int GetInt(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberInt(key);
#else

            ConsoleLog("GET INT: KEY: " + key + " -> 0");
            return 0;
#endif
        }



        [DllImport("__Internal")]
        private static extern float GP_Player_GetNumberFloat(string key);
        public static float GetFloat(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberFloat(key);
#else

            ConsoleLog("GET FLOAT: KEY: " + key + " -> 0");
            return 0;
#endif
        }

        [DllImport("__Internal")]
        private static extern float GP_Player_GetMaxValue(string key);
        public static float GetMaxValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetMaxValue(key);
#else

            ConsoleLog("GET MAX: KEY: " + key);
            return 0;
#endif
        }

        [DllImport("__Internal")]
        private static extern float GP_Player_GetMinValue(string key);
        public static float GetMinValue(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetMinValue(key);
#else

            ConsoleLog("GET MIN: KEY: " + key);
            return 0;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Player_GetString(string key);
        public static string GetString(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetString(key);
#else

            ConsoleLog("GET STRING: KEY: " + key);
            return null;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetBool(string key);
        public static bool GetBool(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetBool(key) == "true";
#else

            ConsoleLog("GET BOOL: KEY: " + key + " -> TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern void GP_Player_Set_Number(string key, float value);
        [DllImport("__Internal")]
        private static extern void GP_Player_Set_Bool(string key, string value);
        [DllImport("__Internal")]
        private static extern void GP_Player_Set_String(string key, string value);

        public static void Set(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_String(key, value);
#else

            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else

            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Bool(key, value.ToString());
#else

            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else

            ConsoleLog("SET: KEY: " + key + " VALUE: " + value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetFlag(string key, bool value);
        public static void SetFlag(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetFlag(key, value);
#else

            ConsoleLog("SET FLAG: KEY: " + key + " VALUE: " + value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Add(string key, string value);
        public static void Add(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else

            ConsoleLog("ADD: KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Add(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else

            ConsoleLog("ADD: KEY: " + key + " VALUE: " + value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Toggle(string key);
        public static void Toggle(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Toggle(key);
#else

            ConsoleLog("TOGGLE: KEY: " + key);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Reset();
        public static void ResetPlayer()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Reset();
#else

            ConsoleLog("RESET");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Remove();
        public static void Remove()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Remove();
#else

            ConsoleLog("REMOVE");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Sync(bool forceOverride = false);
        public static void Sync(bool forceOverride = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Sync(forceOverride);
#else

            ConsoleLog("SYNC");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Load();
        public static void Load()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Load();
#else

            ConsoleLog("LOAD");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Login();
        public static void Login()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Login();
#else

            ConsoleLog("LOGIN");
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_Player_Logout();
        public static void Logout()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Logout();
#else

            ConsoleLog("LOGOUT");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_FetchFields();
        public static void FetchFields(Action<List<PlayerFetchFieldsData>> onFetchFields = null)
        {
            _onFetchFields = onFetchFields;

#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_FetchFields();
#else

            ConsoleLog("FETCH FIELDS");
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_Has(string key);
        public static bool Has(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_Has(key) == "true";
#else

            ConsoleLog("KEY: " + key + " -> EMPTY");
            return true;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_IsLoggedIn();
        public static bool IsLoggedIn()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_IsLoggedIn() == "true";
#else

            ConsoleLog("IS LOGGED IN: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsLoggedIn;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_HasAnyCredentials();
        public static bool HasAnyCredentials()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_HasAnyCredentials() == "true";
#else

            ConsoleLog("HAS ANY CREDENTIALS: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().HasAnyCredentials;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_IsStub();
        public static bool IsStub()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_IsStub() == "true";
#else

            ConsoleLog("IS STUB: TRUE");
            return GP_Settings.instance.GetFromPlatformSettings().IsStub;
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDays();
        public static int GetActiveDays()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetActiveDays();
#else

            ConsoleLog("ACTIVE DAYS: 1");
            return 0;
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDaysConsecutive();
        public static int GetActiveDaysConsecutive()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetActiveDaysConsecutive();
#else

            ConsoleLog("ACTIVE DAYS CONSECUTIVE: 1");
            return 0;
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeToday();
        public static int GetPlaytimeToday()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetPlaytimeToday();
#else

            ConsoleLog("PLAYTIME TODAY: 0");
            return 0;
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeAll();
        public static int GetPlaytimeAll()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetPlaytimeAll();
#else

            ConsoleLog("PLAYTIME ALL: 0");
            return 0;
#endif
        }

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
    }

    [System.Serializable]
    public class PlayerFetchFieldsData
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