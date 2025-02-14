using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using SimpleGraphQL;
using Newtonsoft.Json.Linq;
using UnityEngine;

namespace GamePush.Core
{
    public class RewardsFetcher
    {
        private const string GiveRewardQuery = "GiveReward";
        
        public async Task<AllRewardData> GiveReward(GivePlayerRewardInput input)
        {
            Query query = DataFetcher.GetQuery(GiveRewardQuery, OperationType.Mutation);
            
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
                
                PlayerReward playerReward = resultObject.ToObject<PlayerReward>();
                RewardData rewardData = resultObject["reward"].ToObject<RewardData>();

                return new AllRewardData(rewardData, playerReward);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }
    }
}