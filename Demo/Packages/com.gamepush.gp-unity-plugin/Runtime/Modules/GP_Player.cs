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


        private void OnEnable()
        {
            CoreSDK.player.OnPlayerChange += PlayerChange;
        }

        private void OnDisable()
        {
            CoreSDK.player.OnPlayerChange -= PlayerChange;
        }

        private void PlayerChange()
        {
            OnPlayerChange?.Invoke();
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetID();
        public static int GetID()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetID();
#else
            return CoreSDK.player.GetID();
#endif
        }


        [DllImport("__Internal")]
        private static extern float GP_Player_GetScore();
        public static float GetScore()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetScore();
#else
            return CoreSDK.player.GetScore();
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Player_GetName();
        public static string GetName()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetName();
#else
            return CoreSDK.player.GetName();
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetAvatar();
        public static string GetAvatarUrl()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetAvatar();
#else
            return CoreSDK.player.GetAvatarUrl();
#endif
        }

        public async static void GetAvatar(Image image)
        {
            string avatar;
#if !UNITY_EDITOR && UNITY_WEBGL
            avatar = GP_Player_GetAvatar();
#else
            avatar = CoreSDK.player.GetAvatarUrl();
#endif
            
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
            return CoreSDK.player.GetFieldName(key);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantName(string key, string value);
        public static string GetFieldVariantName(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantName(key, value);
#else
            return CoreSDK.player.GetFieldVariantName(key, value);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantAt(string key, string index);
        public static string GetFieldVariantAt(string key, int index)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantAt(key, index.ToString());
#else
            return CoreSDK.player.GetFieldVariantAt(key, index);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetFieldVariantIndex(string key, string value);
        public static string GetFieldVariantIndex(string key, string value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetFieldVariantIndex(key, value);
#else
            return CoreSDK.player.GetFieldVariantIndex(key, value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetName(string name);
        public static void SetName(string name)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetName(name);
#else
            CoreSDK.player.SetName(name);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetAvatar(string src);
        public static void SetAvatar(string src)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetAvatar(src);
#else
            CoreSDK.player.SetAvatar(src);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_SetScore(float score);
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



        [DllImport("__Internal")]
        private static extern void GP_Player_AddScore(float score);
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



        [DllImport("__Internal")]
        private static extern int GP_Player_GetNumberInt(string key);
        public static int GetInt(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberInt(key);
#else
            return CoreSDK.player.Get<int>(key);
#endif
        }



        [DllImport("__Internal")]
        private static extern float GP_Player_GetNumberFloat(string key);
        public static float GetFloat(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetNumberFloat(key);
#else
            return CoreSDK.player.Get<float>(key);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetString(string key);
        public static string GetString(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetString(key);
#else
            return CoreSDK.player.Get<string>(key);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_GetBool(string key);
        public static bool GetBool(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetBool(key) == "true";
#else
            return CoreSDK.player.Get<bool>(key);
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



        [DllImport("__Internal")]
        private static extern void GP_Player_SetFlag(string key, bool value);
        public static void SetFlag(string key, bool value)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_SetFlag(key, value);
#else
            CoreSDK.player.Set(key, value);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Add(string key, string value);
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



        [DllImport("__Internal")]
        private static extern void GP_Player_Toggle(string key);
        public static void Toggle(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Toggle(key);
#else
            CoreSDK.player.Toggle(key);
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Reset();
        public static void ResetPlayer()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Reset();
#else
            CoreSDK.player.Reset();
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Remove();
        public static void Remove()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Remove();
#else
            CoreSDK.player.Remove();
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Sync(bool forceOverride = false);
        public static void Sync(bool forceOverride = false)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Sync(forceOverride);
#else
            CoreSDK.player.Sync();
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Load();
        public static void Load()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Load();
#else
            CoreSDK.player.Load();
#endif
        }



        [DllImport("__Internal")]
        private static extern void GP_Player_Login();
        public static void Login()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player_Login();
#else
            CoreSDK.player.Login();
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
            CoreSDK.player.FetchFields(onFetchFields);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_Has(string key);
        public static bool Has(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_Has(key) == "true";
#else
            return CoreSDK.player.Has(key);
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_IsLoggedIn();
        public static bool IsLoggedIn()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_IsLoggedIn() == "true";
#else
            return CoreSDK.player.IsLoggedIn();
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_HasAnyCredentials();
        public static bool HasAnyCredentials()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_HasAnyCredentials() == "true";
#else
            return CoreSDK.player.HasAnyCredentials();
#endif
        }



        [DllImport("__Internal")]
        private static extern string GP_Player_IsStub();
        public static bool IsStub()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_IsStub() == "true";
#else
            return CoreSDK.player.IsStub();
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDays();
        public static int GetActiveDays()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetActiveDays();
#else
            return CoreSDK.player.GetActiveDays();
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetActiveDaysConsecutive();
        public static int GetActiveDaysConsecutive()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetActiveDaysConsecutive();
#else
            return CoreSDK.player.GetActiveDaysConsecutive();
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeToday();
        public static int GetPlaytimeToday()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetPlaytimeToday();
#else
            return CoreSDK.player.GetPlaytimeToday();
#endif
        }

        [DllImport("__Internal")]
        private static extern int GP_Player_GetPlaytimeAll();
        public static int GetPlaytimeAll()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetPlaytimeAll();
#else
            return CoreSDK.player.GetPlaytimeAll();
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
            OnPlayerFetchFieldsComplete?.Invoke(UtilityJSON.GetList<PlayerFetchFieldsData>(data));
            _onFetchFields?.Invoke(UtilityJSON.GetList<PlayerFetchFieldsData>(data));
        }
        private void CallPlayerFetchFieldsError() => OnPlayerFetchFieldsError?.Invoke();
    }

    
}