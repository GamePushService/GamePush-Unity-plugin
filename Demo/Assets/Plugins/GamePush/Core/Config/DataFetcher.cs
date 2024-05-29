using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleGraphQL;
using UnityEngine;
using UnityEngine.Serialization;
using GamePush.Data;
using Newtonsoft.Json;

namespace GamePush.Core
{
    public class DataFetcher
    {
        private static string _configName = "GP_GraphQL";
        private static string _configQueryName = "FetchPlayerProjectConfig";
        private static string _getPlayerQueryName = "GetPlayer";
        private static string _syncPlayerQueryName = "SyncPlayer";

        public static async Task GetConfig()
        {
            Debug.Log("Get config");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_configQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            string results = await graphQL.Send(
                query.ToRequest(new Dictionary<string, object>()),
                null, Headers.GetHeaders(queryTuple.Item1)
            );

            Debug.Log(results);

            ConfigData configData = JsonUtility.FromJson<ConfigData>(results);
            CoreSDK.SetConfig(configData.data.result);

            //Debug.Log(JsonUtility.ToJson(configData.data.result));
        }

        public static async Task GetPlayer(SyncPlayerInput input, bool withToken)
        {
            Debug.Log("Get player");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_getPlayerQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            string results = await graphQL.Send(
                query.ToRequest(new Dictionary<string, object>()),
                null, Headers.GetHeaders(queryTuple.Item1)
            );

            Debug.Log(results);

            ConfigData configData = JsonUtility.FromJson<ConfigData>(results);
            CoreSDK.SetConfig(configData.data.result);

            //Debug.Log(JsonUtility.ToJson(configData.data.result));
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

            Debug.Log(results);

            ConfigData configData = JsonUtility.FromJson<ConfigData>(results);
            //CoreSDK.SetConfig(configData.data.result);

            //Debug.Log(JsonUtility.ToJson(configData.data.result));
        }


    }

    
}
