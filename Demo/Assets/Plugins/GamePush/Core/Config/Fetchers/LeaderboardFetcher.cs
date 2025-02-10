using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using SimpleGraphQL;
using UnityEngine;

namespace GamePush.Core
{
    public class LeaderboardFetcher
    {
        private const string FetchTopQuery = "FetchTopQuery";
        private const string FetchTopScopedQuery = "FetchTopScopedQuery";
        private const string PlayerPublishRating = "PlayerPublishRating";

        public async Task<AllRatingData> GetRating(object input, bool withMe)
        {
            Query query = DataFetcher.GetQuery(FetchTopQuery, OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", DataFetcher.GetLang() },
                { "withMe", withMe }
            };

            try
            {
                JObject root = await DataFetcher.SendQueryRequest(query, queryTuple.Item1, variables);
                JObject resultObject = (JObject)root["data"]["result"];
                JObject playerResultObject = (JObject)root["data"]["playerResult"];

                if (resultObject["__typename"].ToObject<string>() == "Problem")
                {
                    string error = resultObject["message"].ToObject<string>();
                    Logger.Error(error);
                    return null;
                }

                Debug.Log(playerResultObject.ToString());

                AllRatingData allRatingData = new AllRatingData
                {
                    ratingData = resultObject.ToObject<RatingData>(),
                    playerRatingData = playerResultObject.ToObject<PlayerRatingData>()
                };

                return allRatingData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
            
        }

        public async Task<PlayerRatingData> GetPlayerRating(GetLeaderboardQuery input)
        {
            Query query = DataFetcher.GetQuery(FetchTopQuery, OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", DataFetcher.GetLang() },
                { "withMe", true }
            };

            try
            {
                JObject root = await DataFetcher.SendQueryRequest(query, queryTuple.Item1, variables);
                JObject playerResultObject = (JObject)root["data"]["playerResult"];
                
                PlayerRatingData ratingData = playerResultObject.ToObject<PlayerRatingData>();
                return ratingData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        public async Task<AllRatingData> GetRatingVariant(GetLeaderboardVariantQuery input, bool withMe)
        {
            Query query = DataFetcher.GetQuery(FetchTopScopedQuery, OperationType.Query);

            object filterInput;
            if (int.TryParse(input.idOrTag, out int id))
                filterInput = new GetLeaderboardVariantQueryID(id, input);
            else
                filterInput = new GetLeaderboardVariantQueryTAG(input.idOrTag, input);

            Tuple<string, object> queryTuple = Hash.SingQuery(filterInput);

            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", DataFetcher.GetLang() },
                { "withMe", withMe }
            };

            try
            {
                JObject root = await DataFetcher.SendQueryRequest(query, queryTuple.Item1, variables);

                JObject resultObject = (JObject)root["data"]["result"];
                JObject playerResultObject = (JObject)root["data"]["playerResult"];

                if (resultObject["__typename"].ToObject<string>() == "Problem")
                {
                    string error = resultObject["message"].ToObject<string>();
                    Logger.Error(error);
                    return null;
                }
                
                AllRatingData allRatingData = new AllRatingData();
                allRatingData.ratingData = resultObject.ToObject<RatingData>();
                allRatingData.playerRatingData = playerResultObject.ToObject<PlayerRatingData>();

                return allRatingData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        public async Task<PlayerRatingData> GetPlayerRatingVariant(GetLeaderboardVariantQuery input)
        {
            Query query = DataFetcher.GetQuery(FetchTopScopedQuery, OperationType.Query);

            object filterInput;
            if (int.TryParse(input.idOrTag, out int id))
                filterInput = new GetLeaderboardVariantQueryID(id, input);
            else
                filterInput = new GetLeaderboardVariantQueryTAG(input.idOrTag, input);

            Tuple<string, object> queryTuple = Hash.SingQuery(filterInput);

            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", DataFetcher.GetLang() },
                { "withMe", true }
            };

            try
            {
                JObject root = await DataFetcher.SendQueryRequest(query, queryTuple.Item1, variables);
                JObject playerResultObject = (JObject)root["data"]["playerResult"];

                if (playerResultObject["__typename"].ToObject<string>() == "Problem")
                {
                    string error = playerResultObject["message"].ToObject<string>();
                    Logger.Error(error);
                    return null;
                }

                Debug.Log(playerResultObject.ToString());

                PlayerRatingData ratingData = playerResultObject.ToObject<PlayerRatingData>();

                return ratingData;
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message);
                return null;
            }
        }

        public async Task<PlayerRecordData> PublishRecord(PublishRecordQuery input)
        {
            Query query = DataFetcher.GetQuery(PlayerPublishRating, OperationType.Mutation);
          
            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", DataFetcher.GetLang() }
            };

            try
            {
                JObject root = await DataFetcher.SendQueryRequest(query, queryTuple.Item1, variables);

            JObject resultObject = (JObject)root["data"]["result"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                Logger.Error(error);
                return null;
            }

            PlayerRecordData data = resultObject.ToObject<PlayerRecordData>();

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