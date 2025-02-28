using System;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush
{
    public static class GP_Prefs
    {
        private static List<string> keys = new List<string>();
        private static string GPKey(string key) => "xGP_" + key;
        public static void Set<T>(string key, T value) where T : IConvertible
        {
            Debug.Log($"Setting {key} to {value}");
            
            switch (value.GetTypeCode())
            {
                case TypeCode.Boolean:
                    PlayerPrefs.SetString(GPKey(key), value.ToString());
                    return;
                case TypeCode.Int32:
                    PlayerPrefs.SetInt(GPKey(key), value.ToInt32(null));
                    return;
                case TypeCode.Single:
                    PlayerPrefs.SetFloat(GPKey(key), value.ToSingle(null));
                    return;
                case TypeCode.String:
                    PlayerPrefs.SetString(GPKey(key), value.ToString());
                    return;
                default:
                    throw new InvalidOperationException($"Unsupported type: {typeof(T)}");
            }
        }
        
        public static void Add<T>(string key, T value) where T : IConvertible
        {
            T oldValue = Get<T>(key);
            var newValue =  value.GetTypeCode() switch
            {
                TypeCode.Int32 => oldValue.ToInt32(null) + value.ToInt32(null),
                TypeCode.Single => oldValue.ToSingle(null) + value.ToSingle(null),
                _ => throw new InvalidOperationException($"Unsupported type: {typeof(T)}")
            };
            Set<T>(key, (T)Convert.ChangeType(newValue, typeof(T)));
        }
        
        public static T Get<T>(string key) where T : IConvertible
        {
            Debug.Log($"TypeCode {typeof(T)}");
            T value = (T)Convert.ChangeType(0, typeof(T));
            
            T result =  value.GetTypeCode() switch
            {
                TypeCode.Boolean => (T)Convert.ChangeType(PlayerPrefs.GetString(GPKey(key)), typeof(T)),
                TypeCode.Int32 => (T)Convert.ChangeType(PlayerPrefs.GetInt(GPKey(key)), typeof(T)),
                TypeCode.Single =>(T)Convert.ChangeType(PlayerPrefs.GetFloat(GPKey(key)), typeof(T)),
                TypeCode.String => (T)Convert.ChangeType(PlayerPrefs.GetString(GPKey(key)), typeof(T)),
                _ => throw new InvalidOperationException($"Unsupported type: {typeof(T)}")
            };
            
            return result;
        }

        public static T TryGet<T>(string key) where T : IConvertible
        {
            if (PlayerPrefs.HasKey(GPKey(key)))
            {
                Debug.Log($"Getting {key}");
                return Get<T>(key);
            }
            else
            {
                Debug.Log($"Add {key}");
                T result = (T)Convert.ChangeType(0, typeof(T));
                
                keys.Add(key);
                Set(key, result);
                return result;
            }
        }

        public static bool HasKey(string key)
        {
            return PlayerPrefs.HasKey(GPKey(key));
        }

        public static void Reset()
        {
            foreach (string key in keys)
            {
                PlayerPrefs.DeleteKey(GPKey(key));
            }
            keys.Clear();
        }
        
    }
}