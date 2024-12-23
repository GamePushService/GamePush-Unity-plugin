using System;
using System.Security.Cryptography;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
using Newtonsoft.Json;
using GamePush.Data;

namespace GamePush.Core
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
        public static Tuple<string, object> SingQuery(object query)
            => GetProjectHash(CoreSDK.projectId, query, CoreSDK.projectToken);

        private static Tuple<string, object> GetProjectHash(int id, object query, string token)
        {
            var sortedQuery = query;
            if (query != null)
            {
                string jsonQuery = JsonConvert.SerializeObject(query);
                //Debug.Log(jsonQuery);
                sortedQuery = SortKeys(jsonQuery);
            }
            
            Dictionary<string, object> data = new Dictionary<string, object>();
            data.Add("projectId", id);
            data.Add("query", sortedQuery);
            data.Add("token", token);

            string json = JsonConvert.SerializeObject(data);
            //Debug.Log(json);

            var sortedObj = SortKeys(json);
            var hash = CalculateSHA256(sortedObj.ToString(Formatting.None));

            return new Tuple<string, object>(hash, sortedQuery);
        }

        static Newtonsoft.Json.Linq.JObject SortKeys(string json)
        {
            var obj = JsonConvert.DeserializeObject<Newtonsoft.Json.Linq.JObject>(json);
            var sortedObj = new Newtonsoft.Json.Linq.JObject(obj.Properties().OrderBy(p => p.Name));
            return sortedObj;
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

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
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

