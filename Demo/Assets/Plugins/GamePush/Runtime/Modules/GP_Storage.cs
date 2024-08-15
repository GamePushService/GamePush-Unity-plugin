using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Utilities;

namespace GamePush
{
    public enum SaveStorageType { local, platform };


    public class GP_Storage : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Storage);

        public static event UnityAction<StorageField> OnGetValue;
        public static event UnityAction<StorageField> OnSetValue;
        public static event UnityAction<StorageField> OnGetGlobalValue;
        public static event UnityAction<StorageField> OnSetGlobalValue;

        private static event Action<object> _onGetValue;
        private static event Action<StorageField> _onSetValue;
        private static event Action<object> _onGetGlobalValue;
        private static event Action<StorageField> _onSetGlobalValue;


        [DllImport("__Internal")]
        private static extern void GP_StorageSetType(string key);
        public static void SetStorage(SaveStorageType storage)
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_StorageSetType(storage.ToString());
#else
            ConsoleLog("Set storage: " + storage.ToString());
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_StorageGet(string key);
        public static void Get(string key, Action<object> onGetValue)
        {
            _onGetValue = onGetValue;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_StorageGet(key);
#else
            ConsoleLog("GET: KEY: " + key);
            ConsoleLog("GET: VALUE: " + GetPref<object>(key).ToString());
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_StorageSetString(string key, string value);
        [DllImport("__Internal")]
        private static extern string GP_StorageSetNumber(string key, float value);
        [DllImport("__Internal")]
        private static extern string GP_StorageSetBool(string key, bool value);

        public static void Set(string key, object value, Action<StorageField> onSetValue = null)
        {
            _onSetValue = onSetValue;
#if !UNITY_EDITOR && UNITY_WEBGL
            StorageSetValue(key, value);
#else

            ConsoleLog($"SET: KEY: {key}, VALUE: {value}");
            SetPref(key, value);
#endif
        }

        private static void StorageSetValue(string key, object value)
        {
            if (value.GetType() == typeof(int))
            {
                GP_StorageSetNumber(key, (int)value);
                return;
            }
            if (value.GetType() == typeof(float))
            {
                GP_StorageSetNumber(key, (float)value);
                return;
            }
            if (value.GetType() == typeof(bool))
            {
                GP_StorageSetBool(key, (bool)value);
                return;
            }
            if (value.GetType() == typeof(string))
            {
                GP_StorageSetString(key, (string)value);
                return;
            }
            GP_StorageSetString(key, (string)value);

        }


        [DllImport("__Internal")]
        private static extern string GP_StorageGetGlobal(string key);
        public static void GetGlobal(string key, Action<object> onGetGlobalValue)
        {
            _onGetGlobalValue = onGetGlobalValue;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_StorageGetGlobal(key);
#else
            ConsoleLog("GET GLOBAL: KEY: " + key);
            ConsoleLog("GET GLOBAL: VALUE: " + GetPref<object>(key).ToString());
            //CallOnStorageGet(GetPref<object>(key));
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_StorageSetGlobalString(string key, string value);
        [DllImport("__Internal")]
        private static extern string GP_StorageSetGlobalNumber(string key, float value);
        [DllImport("__Internal")]
        private static extern string GP_StorageSetGlobalBool(string key, bool value);

        public static void SetGlobal(string key, object value, Action<StorageField> onSetGlobalValue = null)
        {
            _onSetGlobalValue = onSetGlobalValue;
#if !UNITY_EDITOR && UNITY_WEBGL
            StorageSetGlobalValue(key, value);
#else
            ConsoleLog($"SET GLOBAL: KEY: {key}, VALUE: {value}");
            SetPref(key, value);
#endif
        }

        private static void StorageSetGlobalValue(string key, object value)
        {
            if (value.GetType() == typeof(int))
            {
                GP_StorageSetGlobalNumber(key, (int)value);
                return;
            }
            if (value.GetType() == typeof(float))
            {
                GP_StorageSetGlobalNumber(key, (float)value);
                return;
            }
            if (value.GetType() == typeof(bool))
            {
                GP_StorageSetGlobalBool(key, (bool)value);
                return;
            }
            if (value.GetType() == typeof(string))
            {
                GP_StorageSetGlobalString(key, (string)value);
                return;
            }
            GP_StorageSetGlobalString(key, (string)value);

        }

        private static T GetPref<T>(string key)
        {
            if (PlayerPrefs.HasKey(key))
            {
                object value = null;
                if (typeof(T) == typeof(int))
                    value = PlayerPrefs.GetInt(key);
                if (typeof(T) == typeof(float))
                    value = PlayerPrefs.GetFloat(key);
                if (typeof(T) == typeof(string))
                    value = PlayerPrefs.GetString(key);
                if (typeof(T) == typeof(object))
                    value = PlayerPrefs.GetString(key);

                return UtilityType.ConvertValue<T>(value);
            }
            else throw new PlayerPrefsException("No such key");
        }


        private static void SetPref<T>(string key, T value)
        {
            if (typeof(T) == typeof(int))
                PlayerPrefs.SetInt(key, UtilityType.ConvertValue<int>(value));
            if (typeof(T) == typeof(float))
                PlayerPrefs.SetFloat(key, UtilityType.ConvertValue<float>(value));
            if (typeof(T) == typeof(string))
                PlayerPrefs.SetString(key, UtilityType.ConvertValue<string>(value));
            if (typeof(T) == typeof(object))
                PlayerPrefs.SetString(key, UtilityType.ConvertValue<string>(value.ToString()));

            PlayerPrefs.Save();
        }

        private void CallOnStorageGetValue(string result)
        {
            StorageField field = JsonUtility.FromJson<StorageField>(result);
            OnGetValue?.Invoke(field);

            string value = field.value;
            if (int.TryParse(value, out int i))
                _onGetValue?.Invoke(i);
            else if (float.TryParse(value, out float f))
                _onGetValue?.Invoke(f);
            else if (bool.TryParse(value, out bool b))
                _onGetValue?.Invoke(b);
            else
                _onGetValue?.Invoke(value);
        }

        private void CallOnStorageSetValue(string result)
        {
            StorageField field = JsonUtility.FromJson<StorageField>(result);
            _onSetValue?.Invoke(field);
            OnSetValue?.Invoke(field);
        }

        private void CallOnStorageGetGlobal(string result)
        {
            StorageField field = JsonUtility.FromJson<StorageField>(result);
            OnGetGlobalValue?.Invoke(field);

            string value = field.value;
            if (int.TryParse(value, out int i))
                _onGetGlobalValue?.Invoke(i);
            else if (float.TryParse(value, out float f))
                _onGetGlobalValue?.Invoke(f);
            else if (bool.TryParse(value, out bool b))
                _onGetGlobalValue?.Invoke(b);
            else
                _onGetGlobalValue?.Invoke(value);
        }

        private void CallOnStorageSetGlobal(string result)
        {
            StorageField field = JsonUtility.FromJson<StorageField>(result);
            _onSetGlobalValue?.Invoke(field);
            OnSetGlobalValue?.Invoke(field);
        }
    }

    [System.Serializable]
    public class StorageField
    {
        public string key;
        public string value;
    }
}
