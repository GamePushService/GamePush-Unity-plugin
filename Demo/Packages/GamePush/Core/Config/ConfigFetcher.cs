using System.Collections;
using System.Collections.Generic;
using SimpleGraphQL;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Config
{
    public class ConfigFetcher
    {
        private static string _configName = "GP_GraphQL";
        private static string _queryName = "FetchPlayerProjectConfig";

        public static async void GetConfig()
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_queryName, "result", OperationType.Query);

            string results = await graphQL.Send(
                query.ToRequest(new Dictionary<string, object>()),
                null, Headers.GetConfigHeaders()
            );

            Debug.Log(results);

            ConfigData configData = JsonUtility.FromJson<ConfigData>(results);
            CoreSDK.SetConfig(configData.data.result);

            //Debug.Log(JsonUtility.ToJson(configData.data.result));
        }
    }

}
