using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleGraphQL;
using UnityEngine;
using UnityEngine.Serialization;
using GamePush.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine.Networking;
using System.Collections;

namespace GamePush.Core
{
    public class DataFetcher
    {
        private static string _apiURL = "https://api.gamepush.com/gs/api";
        private static string _configName = "GP_GraphQL";
        private static string _fetchConfigQueryName = "FetchPlayerProjectConfig";
        private static string _getPlayerQueryName = "GetPlayer";
        private static string _syncPlayerQueryName = "SyncPlayer";
        private static string _fetchPlayerFieldsQueryName = "FetchPlayerFields";

        public static async Task<AllConfigData> GetConfig()
        {
            Debug.Log("Get config");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchConfigQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            string results = await graphQL.Send(
                query.ToRequest(new Dictionary<string, object>()),
                null, Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            //Debug.Log(resultObject.ToString());
            Debug.Log(resultObject["serverTime"].ToString());

            AllConfigData configData = resultObject.ToObject<AllConfigData>();

            return configData;
            //CoreSDK.SetConfig(configData);
        }

        public static async Task<JObject> GetPlayer(GetPlayerInput input, bool withToken)
        {
            Debug.Log("Get player");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_getPlayerQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            //Debug.Log(queryTuple.Item2);

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", "EN");
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            Debug.Log(resultObject.ToString());

            return resultObject;
            //CoreSDK.player.SetPlayerData(resultObject);
        }

        public static async Task<JObject> SyncPlayer(SyncPlayerInput input, bool withToken)
        {
            Debug.Log("Sync player");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_syncPlayerQueryName, "result", OperationType.Mutation);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", "EN");
            variables.Add("withToken", withToken);

            Debug.Log(queryTuple.Item2);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            //Debug.Log(resultObject.ToString());

            return resultObject;
        }

        public static async Task<List<PlayerField>> FetchPlayerFields(bool withToken)
        {
            Debug.Log("Fetch Player Fields");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchPlayerFieldsQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", "EN");
            variables.Add("withToken", withToken);

            //Debug.Log(queryTuple.Item2);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            //Debug.Log(results);

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            List<PlayerField> playerFields = resultObject["items"].ToObject<List<PlayerField>>();

            return playerFields;
        }

        public static async void Ping(string token)
        {
            //Debug.Log($"Try Ping: {_apiURL}/ping?t={token}");
            UnityWebRequest pingRequest = await GetRequest($"{_apiURL}/ping?t={token}");
            Debug.Log($"Ping: {pingRequest.result}");
        }

        private static async Task<UnityWebRequest> GetRequest(string url)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                await Task.Yield();
            }
            return webRequest;
        }

        public static async void RequestExample()
        {
            string URL = "https://example.com";

            UnityWebRequest webRequest = await GetRequest(URL);

            string[] pages = URL.Split('/');
            int page = pages.Length - 1;

            switch (webRequest.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError(pages[page] + ": Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError(pages[page] + ": HTTP Error: " + webRequest.error);
                    break;
                case UnityWebRequest.Result.Success:
                    Debug.Log(pages[page] + ":\nReceived: " + webRequest.downloadHandler.text);
                    break;
            }
        }

    }


}
