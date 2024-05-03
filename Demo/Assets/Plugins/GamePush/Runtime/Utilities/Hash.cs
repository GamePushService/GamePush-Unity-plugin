using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using GP_Utilities;
using Newtonsoft.Json;

namespace GamePush
{
    [System.Serializable]
    public class ReadyData
    {
        public string projectId;
        public object query;
        public string token;
    }

    public static class Hash
    {
        public static string GetNullHash() => GetQueryHash(null);

        public static string GetQueryHash(object query) => GetProjectHash(CoreSDK.projectId, query, CoreSDK.token);

        public static string GetProjectHash(int id, object query, string token)
        {
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("projectId", id);
            data.Add("query", query);
            data.Add("token", token);

            string json = JsonConvert.SerializeObject(data);
            //string json = GP_JSON.DictionaryToJson(data);
            Debug.Log(json);

            // Сортируем ключи перед сериализацией
            var sortedJson = SortKeys(json);
            //var sortedJson = JsonSorter.SortKeys(json); 
            // Создаем хеш
            var hash = CalculateSHA256(sortedJson);

            return hash;
        }

        static string SortKeys(string json)
        {
            var obj = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
            var sortedObj = new Newtonsoft.Json.Linq.JObject(obj.Properties().OrderBy(p => p.Name));
            return sortedObj.ToString(Newtonsoft.Json.Formatting.None);

        }

        static string CalculateSHA256(string input)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(input);
                var hashBytes = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
            }
        }


        public static string GetHashSha256(string text)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            string hashString = string.Empty;
            foreach (byte x in hash)
            {
                hashString += String.Format("{0:x2}", x);
            }

            return hashString;
        }

        public static Dictionary<string, object> Sort(object src)
        {
            if (src is IDictionary srcDict)
            {
                var sortedDict = new Dictionary<string, object>();

                foreach (var key in srcDict.Keys.Cast<string>().OrderBy(x => x))
                {
                    sortedDict.Add(key, Sort(srcDict[key]));
                }

                return sortedDict;
            }

            if (src is IEnumerable srcEnumerable)
            {
                var sortedList = new List<object>();

                foreach (var item in srcEnumerable)
                {
                    sortedList.Add(Sort(item));
                }

                //return sortedList.ToDictionary((key, index) => index.ToString(), value => value);
            }

            return src as Dictionary<string, object>;
        }
    }
}

