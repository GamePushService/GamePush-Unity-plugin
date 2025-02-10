using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using SimpleGraphQL;
using UnityEngine;
using GamePush.Data;
using Newtonsoft.Json.Linq;

namespace GamePush.Core
{
    public class PlayersFetcher
    {
        private const string FetchPlayersQuery = "FetchPlayers";
        public static async Task<List<PlayerOutput>> FetchPlayers(FetchPlayersInput input, string Format)
        {

            Query query = DataFetcher.GetQuery(FetchPlayersQuery, OperationType.Query);
            
            Tuple<string, object> queryTuple = Hash.SingQuery(input);
            string headers = queryTuple.Item1;
            
            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", DataFetcher.GetLang() }
            };
            
            try
            {
                JObject root = await DataFetcher.SendQueryRequest(query, headers, variables);
                
                JObject resultObject = (JObject)root["data"]?["result"];

                if (resultObject["__typename"]?.ToObject<string>() == "Problem")
                {
                    string error = resultObject["message"]?.ToObject<string>();
                    Logger.Error(error);
                    return null;
                }

                List<PlayerOutput> data = resultObject.ToObject<List<PlayerOutput>>();
                return data;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return null;
            }
        }
    }
}