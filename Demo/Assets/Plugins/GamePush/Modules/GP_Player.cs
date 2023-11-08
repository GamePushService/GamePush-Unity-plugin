using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

using GP_Utilities;
using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Player : MonoBehaviour
    {
        public static event UnityAction OnConnect;
        public static event UnityAction OnPlayerChange;
        public static event UnityAction OnSyncComplete;
        public static event UnityAction OnSyncError;
        public static event UnityAction OnLoadComplete;
        public static event UnityAction OnLoadError;
        public static event UnityAction OnLoginComplete;
        public static event UnityAction OnLoginError;
        public static event UnityAction<List<PlayerFetchFieldsData>> OnPlayerFetchFieldsComplete;
        public static event UnityAction OnPlayerFetchFieldsError;

        private static event Action<List<PlayerFetchFieldsData>> _onFetchFields;


        [DllImport("__Internal")]
        private static extern int GP_Player_GetID();
        public static int GetID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetID();
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET ID: ", "0");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET SCORE: ", "0f");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET NAME: ", "UNKNOWN");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET AVATAR URL: ", "URL");
            return "URL";
#endif
        }
        public async static void GetAvatar(Image image)
        {
            string avatar = GP_Player_GetAvatar();
            if (avatar == null || avatar == "") return;
            await GP_Utility.DownloadImageAsync(avatar, image);
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldName(string key);
        public static string GetFieldName(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldName(key);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET FIELD NAME: ", key);
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET FIELD VARIANT NAME: ", "KEY: " + key + " VALUE: " + value);
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET FIELD VARIANT AT: ", "KEY: " + key + " INDEX: " + index);
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET FIELD VARIANT INDEX: ", "KEY: " + key + " VALUE: " + value);
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET NAME: ", name);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetAvatar(string src);
        public static void SetAvatar(string src)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetAvatar(src);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET AVATAR: ", src);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetScore(float score);
        public static void SetScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET SCORE: ", "" + score);
#endif
        }
        public static void SetScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetScore(score);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET SCORE: ", "" + score);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_AddScore(float score);
        public static void AddScore(float score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ADD SCORE: ", "" + score);
#endif
        }
        public static void AddScore(int score)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_AddScore(score);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ADD SCORE: ", "" + score);
#endif
        }



        [DllImport("__Internal")]
        private static extern int GP_Player_GetNumberInt(string key);
        public static int GetInt(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberInt(key);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET INT: ", "KEY: " + key + " -> 0");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET FLOAT: ", "KEY: " + key + " -> 0");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET STRING: ", "KEY: " + key);
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: GET BOOL: ", "KEY: " + key + " -> TRUE");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET: ", "KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET: ", "KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Bool(key, value.ToString());
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET: ", "KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Set(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Set_Number(key, value);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET: ", "KEY: " + key + " VALUE: " + value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetFlag(string key, bool value);
        public static void SetFlag(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetFlag(key, value);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: SET FLAG: ", "KEY: " + key + " VALUE: " + value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Add(string key, string value);
        public static void Add(string key, float value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ADD: ", "KEY: " + key + " VALUE: " + value);
#endif
        }
        public static void Add(string key, int value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Add(key, value.ToString());
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ADD: ", "KEY: " + key + " VALUE: " + value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Toggle(string key);
        public static void Toggle(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Toggle(key);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: TOGGLE: ", "KEY: " + key);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Reset();
        public static void ResetPlayer()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Reset();
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ", "RESET");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Remove();
        public static void Remove()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Remove();
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ", "REMOVE");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Sync(bool forceOverride = false);
        public static void Sync(bool forceOverride = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Sync(forceOverride);
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ", "SYNC");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Load();
        public static void Load()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Load();
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ", "LOAD");
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Login();
        public static void Login()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Login();
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ", "LOGIN");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: ", "FETCH FIELDS");
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_Has(string key);
        public static bool Has(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_Has(key) == "true";
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: HAS: ", "KEY: " + key + " -> TRUE");
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
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: IS LOGGED IN: ", "TRUE");
            return true;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_HasAnyCredentials();
        public static bool HasAnyCredentials()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_HasAnyCredentials() == "true";
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: HAS ANY CREDENTIALS: ", "TRUE");
            return true;
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_IsStub();
        public static bool IsStub()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_IsStub() == "true";
#else
            if (GP_ConsoleController.Instance.PlayerConsoleLogs)
                Console.Log("PLAYER: IS STUB: ", "TRUE");
            return true;
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

        private void CallPlayerFetchFieldsComplete(string data)
        {
            OnPlayerFetchFieldsComplete?.Invoke(GP_JSON.GetList<PlayerFetchFieldsData>(data));
            _onFetchFields?.Invoke(GP_JSON.GetList<PlayerFetchFieldsData>(data));
        }
        private void CallPlayerFetchFieldsError() => OnPlayerFetchFieldsError?.Invoke();
    }

    [System.Serializable]
    public class PlayerFetchFieldsData
    {
        public string name;
        public string key;
        public string type;
        public string defaultValue; // string | bool | number
        public bool important;
        public Variants[] variants;
    }

    [System.Serializable]
    public class Variants
    {
        public string value; // string | number
        public string name;
    }
}