using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

using GP_Utilities;

namespace GP_Utilities
{
    public class GP_Utility
    {
        public async static Task DownloadImageAsync(string url, Image image)
        {
            if (url == "" || url == null) return;

            var request = UnityWebRequestTexture.GetTexture(url);
            AsyncOperation operation = request.SendWebRequest();

            while (!operation.isDone)
            {
                await Task.Yield();
            }

            if (request.isDone)
            {
                Texture2D _texture2D = ((DownloadHandlerTexture)request.downloadHandler).texture;

                Sprite sprite = Sprite.Create(_texture2D, new Rect(0, 0, _texture2D.width, _texture2D.height), new Vector2(0.5f, 0.5f), 20f);

                image.sprite = sprite;
            }
            else
            {
                Debug.Log("Download Image : Failed");
            }
        }

    }


    public class GP_JSON
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


namespace GamePush
{
    public class GP_Data
    {
        private string _data;

        public GP_Data(string data) => _data = data;

        public T Get<T>() => GP_JSON.Get<T>(_data);
        public List<T> GetList<T>() => GP_JSON.GetList<T>(_data);
        public T[] GetArray<T>() => GP_JSON.GetArray<T>(_data);
    }

    public class Console
    {
        public static void Log(string message) => Debug.Log("<color=#04bc04> Game Push: </color> " + message);
        public static void Log(string message, string colorMessage) => Debug.Log("<color=#04bc04> Game Push: </color> " + message + $"<color=#04bc04> {colorMessage} </color>");
    }
}
