using System.Collections;
using System.Collections.Generic;
using SimpleGraphQL;
using UnityEngine;
using GamePush.Data;

namespace GamePush.Config
{
    public class ConfigFetcher : MonoBehaviour
    {
        public GraphQLConfig Config;

        private void Start()
        {
            string hash = Hash.GetQueryHash(null);

            GetConfig(hash);
        }

        public async void GetConfig(string hash)
        {
            var graphQL = new GraphQLClient(Config);
            Query query = graphQL.FindQuery("FetchPlayerProjectConfig", "result", OperationType.Query);

            string results = await graphQL.Send(
                query.ToRequest(new Dictionary<string, object>()),
                null,
                new Dictionary<string, string>()
                {
                { "X-Transaction-Token", hash },
                { "X-Platform", "NONE" },
                { "X-Platform-Key", "" },
                { "X-Project-ID", CoreSDK.projectId.ToString() },
                { "X-Project-Token", CoreSDK.token.ToString() },
                { "X-Language", "en" },
                { "X-Player-Data", "" },
                }
            );

            Debug.Log(results);

            ConfigData configData = JsonUtility.FromJson<ConfigData>(results);

            CoreSDK.SetConfig(configData.data.result);

            Debug.Log(JsonUtility.ToJson(configData.data.result));

            string vars = JsonUtility.ToJson(CoreSDK.variables.GetData());
            Debug.Log(vars);
        }
    }

}
