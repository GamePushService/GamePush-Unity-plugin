using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush.Data;
using SimpleGraphQL;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GamePush.Core
{
    public class EventsFetcher
    {
        private const string JoinEventQuery = "JoinEvent";
        
        public async Task<PlayerEvent> JoinEvent(PlayerJoinEventInput input)
        {
            Query query = DataFetcher.GetQuery(JoinEventQuery, OperationType.Mutation);
            
            Tuple<string, object> queryTuple = Hash.SingQuery(input);
            string headers = queryTuple.Item1;
            
            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 }
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

                PlayerEvent data = resultObject.ToObject<PlayerEvent>();
                return data;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }
    }
}