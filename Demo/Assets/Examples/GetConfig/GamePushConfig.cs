using Newtonsoft.Json;
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.UI;
using SimpleGraphQL;
using GamePush.Data;
using GP_Utilities;
using TMPro;
using System.Windows;

namespace GamePush.Test{

    public class GamePushConfig : MonoBehaviour
    {
        public GraphQLConfig Config;
        public TMP_Text resultText;
        public TMP_InputField inputID;
        public TMP_InputField inputToken;

        private void Start()
        {
            //string hash = Hash.GetDataHash(null);

            //GetConfig(hash);
        }

        public void GetProjectConfig()
        {
            int id;
            int.TryParse(inputID.text, out id);
            GetConfig(Hash.GetProjectHash(id, null, inputToken.text));
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
                { "X-Project-ID", inputID.text },
                { "X-Project-Token", inputToken.text },
                { "X-Language", "en" },
                { "X-Player-Data", "" },
                }
            );

            Debug.Log(results);
            

            ConfigData configData = GP_JSON.Get<ConfigData>(results);

            CoreSDK.SetConfig(configData.data.result);
            resultText.text = JsonUtility.ToJson(configData.data.result.project);
        }

        public void SetText()
        {
            string text = " ";
            resultText.text = text;
        }

        /*
        static void PrintDictionary(Dictionary<string, object> dictionary, string indent = "")
        {
            foreach (var kvp in dictionary)
            {
                Debug.Log($"{indent}{kvp.Key}:");
                if (kvp.Value is Dictionary<string, object>)
                {
                    PrintDictionary((Dictionary<string, object>)kvp.Value, indent + "  ");
                }
                else
                {
                    Debug.Log($"{indent}  {kvp.Value}");
                }
            }
        }
        */
    }

}
