using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;

namespace GamePush.Utilities
{
    public class UtilityImage
    {
        public async static Task<bool> DownloadImageAsync(string url, Image image)
        {
            if (url == "" || url == null) return false;

            var request = UnityWebRequestTexture.GetTexture(url);
            AsyncOperation operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.isDone)
            {
                Texture2D _texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;

                if (_texture2D == null)
                {
                    Debug.Log("Download Image : Incorrect texture");
                    return false;
                }

                Sprite sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height), new Vector2(0.5f, 0.5f), 20f);

                image.sprite = sprite;
                return true;
            }
            else
            {
                Debug.Log("Download Image : Failed");
            }
            return false;
        }

    }

    public class UtilityJSON
    {
        public static T[] GetArray<T>(string json)
        {
            string newJson = "{\"data\":" + json + "}";
            WrapperArray<T> w = UnityEngine.JsonUtility.FromJson<WrapperArray<T>>(newJson);
            return w.data;
        }

        public static List<T> GetList<T>(string json)
        {
            string newJson = "{\"data\":" + json + "}";
            WrapperList<T> w = UnityEngine.JsonUtility.FromJson<WrapperList<T>>(newJson);
            return w.data;
        }

        public static T Get<T>(string json)
        {
            string newJson = "{\"data\":" + json + "}";
            Wrapper<T> w = UnityEngine.JsonUtility.FromJson<Wrapper<T>>(newJson);
            return w.data;
        }

        [System.Serializable]
        class Wrapper<T>
        {
            public T data;
        }

        [System.Serializable]
        class WrapperArray<T>
        {
            public T[] data;
        }

        [System.Serializable]
        class WrapperList<T>
        {
            public List<T> data;
        }

        public static string DictionaryToJson(Dictionary<string, object> dict)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.Append("{");

            bool first = true;
            foreach (var kvp in dict)
            {
                if (!first)
                    sb.Append(",");
                else
                    first = false;

                sb.Append($"\"{EscapeString(kvp.Key)}\":{ToJsonValue(kvp.Value)}");
            }

            sb.Append("}");
            return sb.ToString();
        }

        static string ToJsonValue(object value)
        {
            if (value == null)
                return "null";
            if (value is int || value is long || value is float || value is double || value is decimal)
                return value.ToString();
            if (value is string)
                return $"\"{EscapeString((string)value)}\"";
            if (value is bool)
                return (bool)value ? "true" : "false";
            if (value is Dictionary<string, object>)
                return DictionaryToJson((Dictionary<string, object>)value);
            if (value is IEnumerable<object>)
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("[");
                bool first = true;
                foreach (var item in (IEnumerable<object>)value)
                {
                    if (!first)
                        sb.Append(",");
                    else
                        first = false;

                    sb.Append(ToJsonValue(item));
                }
                sb.Append("]");
                return sb.ToString();
            }

            Debug.LogError("Unsupported type");
            return null;
        }

        static string EscapeString(string str)
        {
            return str.Replace("\"", "\\\"");
        }
    }

    public class UtilityType
    {
        public static T ConvertValue<T>(object value)
        {
            try
            {
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch
            {
                return default(T);
            }
        }
    }
    

    [System.Serializable]
    public class PlayersIdList
    {
        public List<int> idsList;
    }

    [System.Serializable]
    public class PlayersIdArray
    {
        public int[] idsArray;
    }
}
