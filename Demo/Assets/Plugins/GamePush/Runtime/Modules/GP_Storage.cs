using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Utilities;

namespace GamePush
{
    public class GP_Storage : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Storage);

        public static event UnityAction<object> OnGetValue;
        public static event UnityAction<object> OnSetValue;
        public static event UnityAction<object> OnGetGlobalValue;
        public static event UnityAction<object> OnSetGlobalValue;

        private static event Action<object> _onGetValue;
        private static event Action<object> _onSetValue;
        private static event Action<object> _onGetGlobalValue;
        private static event Action<object> _onSetGlobalValue;

        [DllImport("__Internal")]
        private static extern string GP_StorageGet(string key);

        public static T Get<T>(string key)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Player_GetString(key);
#else
            ConsoleLog("GET: KEY: " + key);
            return GetPref<T>(key);
#endif
        }


        private static T GetPref<T>(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                object value = null;
                if (typeof(T) == typeof(string))
                    value = PlayerPrefs.GetString(key);
                if (typeof(T) == typeof(int))
                    value = PlayerPrefs.GetInt(key);
                if (typeof(T) == typeof(float))
                    value = PlayerPrefs.GetFloat(key);

                return UtilityType.ConvertValue<T>(value);
            }
            else throw new PlayerPrefsException("No such key");
        }
        

        private static void SetPref<T>(string key, T value)
        {
            if (typeof(T) == typeof(string))
                PlayerPrefs.SetString(key, UtilityType.ConvertValue<string>(value));
            if (typeof(T) == typeof(int))
                PlayerPrefs.SetInt(key, UtilityType.ConvertValue<int>(value));
            if (typeof(T) == typeof(float))
                PlayerPrefs.SetFloat(key, UtilityType.ConvertValue<float>(value));

            PlayerPrefs.Save();
        }

        private void CallOnStorageGet(object value)
        {
            OnGetValue?.Invoke(value);
            _onGetValue?.Invoke(value);
        }

        private void CallOnStorageSet(object value)
        {
            OnSetValue?.Invoke(value);
            _onSetValue?.Invoke(value);
        }

        private void CallOnStorageGetGlobal(object value)
        {
            OnGetGlobalValue?.Invoke(value);
            _onGetGlobalValue?.Invoke(value);
        }

        private void CallOnStorageSetGlobal(object value)
        {
            OnSetGlobalValue?.Invoke(value);
            _onSetGlobalValue?.Invoke(value);
        }

    }
}
