using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleGraphQL;
using UnityEngine;
using UnityEngine.Serialization;
using GamePush.Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GamePush.Core
{
    public class DataFetcher
    {
        private static string _configName = "GP_GraphQL";
        private static string _fetchConfigQueryName = "FetchPlayerProjectConfig";
        private static string _getPlayerQueryName = "GetPlayer";
        private static string _syncPlayerQueryName = "SyncPlayer";
        private static string _fetchPlayerFieldsQueryName = "FetchPlayerFields";

        public static async Task GetConfig()
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

            //Debug.Log(results);

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            AllConfigData configData = resultObject.ToObject<AllConfigData>();

            CoreSDK.SetConfig(configData);
        }

        public static async Task GetPlayer(GetPlayerInput input, bool withToken)
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

            //Debug.Log(results);

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            //TODO return to player
            CoreSDK.player.SetPlayerData(resultObject);
        }

        public static async Task SyncPlayer(SyncPlayerInput input, bool withToken)
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

            //Debug.Log(results);

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            //TODO return to player
            CoreSDK.player.SetPlayerData(resultObject);
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

            Debug.Log(queryTuple.Item2);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            Debug.Log(results);

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            List<PlayerField> playerFields = resultObject["items"].ToObject<List<PlayerField>>();

            return playerFields;
        }

    }

    
}
