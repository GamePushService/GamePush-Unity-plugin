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
        private const string API_URL = "https://api.gamepush.com/gs/api";
        private const string ConfigName = "GP_GraphQL";
        private static string _resultOperation = "result";

        private static string _fetchConfigQueryName = "FetchPlayerProjectConfig";
        private static string _getPlayerQueryName = "GetPlayer";
        private static string _syncPlayerQueryName = "SyncPlayer";
        private static string _fetchPlayerFieldsQueryName = "FetchPlayerFields";
        private static string _purchaseProductQueryName = "PurchasePlayerPurchase";
        private static string _consumeProductQueryName = "ConsumePlayerPurchase";
        
        public static async void Ping(string token)
        {
            UnityWebRequest pingRequest = await GetRequest($"{API_URL}/ping?t={token}");
        }

        private static async Task<UnityWebRequest> GetRequest(string url)
        {
            UnityWebRequest webRequest = UnityWebRequest.Get(url);
            await webRequest.SendWebRequest();
            //webRequest.SendWebRequest();

            while (!webRequest.isDone)
            {
                await Task.Delay(1);
            }

            return webRequest;
        }

        private static string GetLang()
        {
            string lang;
            if (CoreSDK.Language != null)
                lang = CoreSDK.Language.CurrentISO().ToUpper();
            else
                lang = CoreSDK.currentLang.ToUpper();

            if (lang == "" || lang == null) lang = "EN";
            return lang;
        }

        public static Query GetQuery(string queryName, OperationType operationType)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(queryName, _resultOperation, operationType);
            return query;
        }

        private static async Task<JObject> SendQueryRequest(string queryName, OperationType operationType, object input, bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(queryName, "result", operationType);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            if (results == "" || results == null) return null;

            JObject root = JObject.Parse(results);
            Debug.Log(root.ToString());
            if ((JObject)root["data"].ToObject<object>() == null)
            {
                string error = root["errors"][0]["message"].ToObject<string>();
                throw new Exception(error);
            }
            JObject resultObject = (JObject)root["data"]["result"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                throw new Exception(error);
            }

            return resultObject;
        }

        public static async Task<AllConfigData> GetConfig()
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchConfigQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            string results = await graphQL.Send(
                query.ToRequest(new Dictionary<string, object>()),
                null, Headers.GetHeaders(queryTuple.Item1)
            );

            if (string.IsNullOrEmpty(results)) 
                return null;

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            // Debug.Log(resultObject["platformConfig"].ToString());

            AllConfigData configData = resultObject.ToObject<AllConfigData>();

            return configData;
        }

        #region PlayerFetches

        public static async Task<JObject> GetPlayer(GetPlayerInput input, bool withToken)
        {
            //Debug.Log("Get player");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_getPlayerQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", "EN");
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            if (string.IsNullOrEmpty(results)) 
                return null;

            JObject root = JObject.Parse(results);
            if ((JObject)root["data"].ToObject<object>() == null)
            {
                string error = root["errors"][0]["message"].ToObject<string>();
                throw new Exception(error);
            }
            JObject resultObject = (JObject)root["data"]["result"];
            return resultObject;
        }


        public static async Task<JObject> SyncPlayer(SyncPlayerInput input, bool withToken)
        {
            JObject resultObject = await SendQueryRequest(_syncPlayerQueryName, OperationType.Mutation, input, withToken);

            //Debug.Log("Sync result");
            //Debug.Log(resultObject["uniques"].ToString());

            return resultObject;
        }

        public static async Task<List<PlayerField>> FetchPlayerFields(bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchPlayerFieldsQueryName, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            List<PlayerField> playerFields = resultObject["items"].ToObject<List<PlayerField>>();

            return playerFields;
        }

        public static async Task<List<GameVariable>> FetchPlayerProjectVariables(bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchPlayerProjectVariables, "result", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                throw new Exception(error);
            }

            List<GameVariable> playerGameVariables = resultObject["items"].ToObject<List<GameVariable>>();

            return playerGameVariables;
        }

        #endregion

        #region LeaderboardFetches

        private static string _fetchTopQueryName = "FetchTopQuery";
        private static string _fetchTopScopedQueryName = "FetchTopScopedQuery";
        private static string _playerPublishRating = "PlayerPublishRating";

        public static async Task<AllRatingData> GetRating(object input, bool withMe)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchTopQueryName, _fetchTopQueryName, OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());
            variables.Add("withMe", withMe);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];
            JObject playerResultObject = (JObject)root["data"]["playerResult"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                Logger.Error(error);
                return null;
            }

            Debug.Log(playerResultObject.ToString());

            AllRatingData allRatingData = new AllRatingData();
            allRatingData.ratingData = resultObject.ToObject<RatingData>();
            allRatingData.playerRatingData = playerResultObject.ToObject<PlayerRatingData>();

            return allRatingData;
        }

        public static async Task<PlayerRatingData> GetPlayerRating(GetLeaderboardQuery input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchTopQueryName, _fetchTopQueryName, OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());
            variables.Add("withMe", true);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject playerResultObject = (JObject)root["data"]["playerResult"];

            //Debug.Log(playerResultObject.ToString());

            PlayerRatingData ratingData = playerResultObject.ToObject<PlayerRatingData>();

            return ratingData;
        }

        public static async Task<AllRatingData> GetRatingVariant(GetLeaderboardVariantQuery input, bool withMe)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchTopScopedQueryName, _fetchTopScopedQueryName, OperationType.Query);

            object filterInput;
            if (int.TryParse(input.idOrTag, out int id))
                filterInput = new GetLeaderboardVariantQueryID(id, input);
            else
                filterInput = new GetLeaderboardVariantQueryTAG(input.idOrTag, input);

            Tuple<string, object> queryTuple = Hash.SingQuery(filterInput);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());
            variables.Add("withMe", withMe);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);

            Debug.Log(root.ToString());
            JObject resultObject = (JObject)root["data"]["result"];
            JObject playerResultObject = (JObject)root["data"]["playerResult"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                Logger.Error(error);
                return null;
            }

            Debug.Log(resultObject.ToString());

            AllRatingData allRatingData = new AllRatingData();
            allRatingData.ratingData = resultObject.ToObject<RatingData>();
            allRatingData.playerRatingData = playerResultObject.ToObject<PlayerRatingData>();

            return allRatingData;
        }

        public static async Task<PlayerRatingData> GetPlayerRatingVariant(GetLeaderboardVariantQuery input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_fetchTopScopedQueryName, _fetchTopScopedQueryName, OperationType.Query);

            object filterInput;
            if (int.TryParse(input.idOrTag, out int id))
                filterInput = new GetLeaderboardVariantQueryID(id, input);
            else
                filterInput = new GetLeaderboardVariantQueryTAG(input.idOrTag, input);

            Tuple<string, object> queryTuple = Hash.SingQuery(filterInput);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());
            variables.Add("withMe", true);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
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

        public static async Task<PlayerRecordData> PublishRecord(PublishRecordQuery input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_playerPublishRating, _playerPublishRating, OperationType.Mutation);
          
            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());

            Debug.Log(JsonConvert.SerializeObject(variables));

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                Logger.Error(error);
                return null;
            }

            Debug.Log(root.ToString());

            //PurchaseOutput purchaseOutput = resultObject.ToObject<PurchaseOutput>();
            PlayerRecordData data = resultObject.ToObject<PlayerRecordData>();

            return data;
        }

        #endregion

        #region UniquesFetches

        private static string SUCCESS_TAG = "gp_success";
        private static string ERROR_TAG = "gp_error";

        private static string _fetchPlayerProjectVariables = "FetchPlayerProjectVariables";
        private static string _playerUniquesValues = "PlayerUniquesValues";

        public static async Task<TagValueData> CheckUniqueValue(object input, bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_playerUniquesValues, "Check", OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            TagValueData result;

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                result = new TagValueData(ERROR_TAG, error);
                return result;
                //throw new Exception(error);
            }

            bool resultValue = resultObject["success"].ToObject<bool>();
            result = new TagValueData(SUCCESS_TAG, resultValue);

            return result;
        }

        public static async Task<TagValueData> RegisterUniqueValue(object input, bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_playerUniquesValues, "Register", OperationType.Mutation);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];
            //Debug.Log(resultObject);
            TagValueData result;

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                result = new TagValueData(ERROR_TAG, error);
                return result;
                //throw new Exception(error);
            }

            UniquesData resultValue = resultObject.ToObject<UniquesData>();
            result = new TagValueData(SUCCESS_TAG, resultValue);

            return result;
        }

        public static async Task<TagValueData> DeleteUniqueValue(object input, bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_playerUniquesValues, "Delete", OperationType.Mutation);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("withToken", withToken);

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            TagValueData result;

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                result = new TagValueData(ERROR_TAG, error);
                return result;
                //throw new Exception(error);
            }

            bool resultValue = resultObject["success"].ToObject<bool>();
            result = new TagValueData(SUCCESS_TAG, resultValue);

            return result;
        }

        #endregion

        #region AchievementsFetchers

        private static string _unlockAchivement = "UnlockPlayerAchievement";
        private static string _setAchivementProgress = "PlayerSetAchievementProgress";

        public static async Task<Achievement> UnlockAchievement(UnlockPlayerAchievementInput input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_unlockAchivement, _unlockAchivement, OperationType.Mutation);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());

            Debug.Log(JsonConvert.SerializeObject(variables));

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                Logger.Error(error);
                return null;
            }

            Debug.Log(resultObject.ToString());
            Debug.Log(resultObject["id"].ToString());

            Achievement data = resultObject.ToObject<Achievement>();

            return data;
        }

        public static async Task<PlayerAchievement> SetAchievemntProgress(PlayerSetAchievementProgressInput input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_setAchivementProgress, _setAchivementProgress, OperationType.Mutation);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", GetLang());

            Debug.Log(JsonConvert.SerializeObject(variables));

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            if (resultObject["__typename"].ToObject<string>() == "Problem")
            {
                string error = resultObject["message"].ToObject<string>();
                Logger.Error(error);
                return null;
            }

            Debug.Log(root.ToString());

            //PurchaseOutput purchaseOutput = resultObject.ToObject<PurchaseOutput>();
            PlayerAchievement data = resultObject.ToObject<PlayerAchievement>();

            return data;
        }

        #endregion

        #region GameCollectionsFetchers

        private const string FetchGamesCollectionQuery = "FetchGamesCollection";
        public static async Task<FetchPlayerGamesCollectionOutput> FetchGamesCollection(FetchPlayerGamesCollectionInput input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            
            Query query = graphQL.FindQuery(FetchGamesCollectionQuery, FetchGamesCollectionQuery, OperationType.Query);
            
            Tuple<string, object> queryTuple = Hash.SingQuery(input);
            
            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", GetLang() },
                { "url", input.urlFrom}
            };

            // Debug.Log(JsonConvert.SerializeObject(variables));

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]?["result"];

            if (resultObject["__typename"]?.ToObject<string>() == "Problem")
            {
                string error = resultObject["message"]?.ToObject<string>();
                Logger.Error(error);
                return null;
            }

            // Debug.Log(root.ToString());

            FetchPlayerGamesCollectionOutput data = resultObject.ToObject<FetchPlayerGamesCollectionOutput>();
            return data;
        }

        #endregion

        #region DocumentFetchers

        private const string FetchDocumentQuery = "FetchDocument";
        public static async Task<DocumentData> FetchDocument(FetchDocumentInput input)
        {
            Debug.Log("FetchDocument");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Debug.Log(graphQL);
            
            Query query = graphQL.FindQuery(FetchDocumentQuery, FetchDocumentQuery, OperationType.Query);
            Debug.Log(query);
            
            Tuple<string, object> queryTuple = Hash.SingQuery(input);
            
            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", GetLang() },
                { "format", input.Format },
            };

            Debug.Log(JsonConvert.SerializeObject(variables));
            Debug.Log(queryTuple.Item1);
            
            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );

            JObject root = JObject.Parse(results);
            Debug.Log(root.ToString());
            
            JObject resultObject = (JObject)root["data"]?["result"];

            if (resultObject["__typename"]?.ToObject<string>() == "Problem")
            {
                string error = resultObject["message"]?.ToObject<string>();
                Logger.Error(error);
                return null;
            }
            
            DocumentData data = resultObject.ToObject<DocumentData>();
            return data;
        }


        #endregion

        #region PurchaseFetchers
        public static async Task<PurchaseOutput> PurchaseProduct(PurchasePlayerPurchaseInput input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_purchaseProductQueryName, "result", OperationType.Mutation);
        
            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", "EN");

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );


            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            Debug.Log(resultObject.ToString());

            //PurchaseOutput purchaseOutput = resultObject.ToObject<PurchaseOutput>();

            //return purchaseOutput;
            return null;
        }

        public static async Task<PurchaseOutput> ConsumeProduct(ConsumePlayerPurchaseInput input)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Debug.Log(graphQL.ToString());

            Query query = graphQL.FindQuery(_consumeProductQueryName, "result", OperationType.Mutation);
            Debug.Log(query.ToString());

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            Dictionary<string, object> variables = new Dictionary<string, object>();

            variables.Add("input", queryTuple.Item2);
            variables.Add("lang", "EN");

            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(queryTuple.Item1)
            );


            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            Debug.Log(resultObject.ToString());

            //PurchaseOutput purchaseOutput = resultObject.ToObject<PurchaseOutput>();

            //return purchaseOutput;
            return null;
        }
        #endregion


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
