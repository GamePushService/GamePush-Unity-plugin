using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;
using System;
using System.Text.RegularExpressions;

namespace GamePush.Tools
{
    public class UtilityImage
    {
        public async static Task<bool> DownloadImageAsync(string url, Image image)
        {
            if (url == "" || url == null)
                return false;

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
                    return false;

                Sprite sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height), new Vector2(0.5f, 0.5f), 20f);

                image.sprite = sprite;
                return true;
            }
            else
            {
                Debug.Log("Download Image : Failed");
                return false;
            }
        }

        private const int MaxSize = 2048;

        private static readonly Regex RegexPattern = new Regex(
            @"cdn\.(eponesh|gamepush|spellsync)\.com\/static(\/([\d\-]+\.)([\d\-]+.)|)\/(.*)",
            RegexOptions.Compiled);

        private static readonly Regex SizeRegex = new Regex(@"-(\d+)x(\d+)\.\w+$", RegexOptions.Compiled);

        public static (int, int) GetOriginalSize(string url)
        {
            var matches = SizeRegex.Match(url);
            if (!matches.Success)
            {
                return (0, 0);
            }

            int width = int.TryParse(matches.Groups[1].Value, out var w) ? w : 0;
            int height = int.TryParse(matches.Groups[2].Value, out var h) ? h : 0;

            return (width, height);
        }

        public static string ResizeImage(string url, int? width = null, int? height = null, bool crop = false)
        {
            var matches = Regex.Match(url, @"your-regex-pattern"); // Укажите нужный шаблон regex
            if (!matches.Success)
            {
                return url;
            }

            var (originalWidth, originalHeight) = GetOriginalSize(url);

            if (originalWidth != 0 && width.HasValue && width > originalWidth)
            {
                width = originalWidth;
            }

            if (originalWidth != 0 && height.HasValue && height > originalHeight)
            {
                height = originalHeight;
            }

            if (width.HasValue && width > MaxSize)
            {
                width = MaxSize;
            }

            if (height.HasValue && height > MaxSize)
            {
                height = MaxSize;
            }

            var domain = matches.Groups[1].Value;
            var path = matches.Groups[5].Value;

            return $"https://cdn.{domain}.com/static/{(width.HasValue ? width.ToString() : "-")}x{(height.HasValue ? height.ToString() : "-")}{(crop ? "crop" : "")}/{path}";
        }

        public static Color GetColorByHEX(string hexColor)
        {
            if (ColorUtility.TryParseHtmlString("#" + hexColor.TrimStart('#'), out Color color))
                return color;
            return Color.white;
        }
    }

    public class UtilityJSON
    {
        public static string ToJson(object data)
        {
            return JsonUtility.ToJson(data);
        }

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

    public class TaskQueue
    {
        private readonly Queue<Func<Task>> taskQueue = new Queue<Func<Task>>();
        private bool isProcessing = false;

        public void Enqueue(Func<Task> task)
        {
            taskQueue.Enqueue(task);
            if (!isProcessing)
            {
                _ = ProcessQueue();
            }
        }

        private async Task ProcessQueue()
        {
            isProcessing = true;
            while (taskQueue.Count > 0)
            {
                var task = taskQueue.Dequeue();
                try
                {
                    await task();
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Ошибка в задаче: {ex}");
                }
            }
            isProcessing = false;
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
