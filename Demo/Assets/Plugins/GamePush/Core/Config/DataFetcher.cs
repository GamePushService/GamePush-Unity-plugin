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
        
        public static readonly PlayersFetcher Players = new PlayersFetcher();
        public static readonly UniquesFetcher Uniques = new UniquesFetcher();
        public static readonly LeaderboardFetcher Leaderboards = new LeaderboardFetcher();
        public static readonly EventsFetcher Events = new EventsFetcher();
        public static readonly RewardsFetcher Rewards = new RewardsFetcher();

        private static string _fetchConfigQueryName = "FetchPlayerProjectConfig";
        private static string _getPlayerQueryName = "GetPlayer";
        private static string SyncPlayerQuery = "SyncPlayer";
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

        public static string GetLang()
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
            Query query = graphQL.FindQuery(queryName, queryName, operationType);
            return query;
        }

        public static async Task<JObject> SendQueryRequest(Query query, string headers, Dictionary<string, object> variables)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            
            Debug.Log(JsonConvert.SerializeObject(variables["input"]));
            string results = await graphQL.Send(
                query.ToRequest(variables),
                null,
                Headers.GetHeaders(headers)
            );

            if (string.IsNullOrEmpty(results)) 
                return null;

            JObject root = JObject.Parse(results);
            return root;
        }

        private static JObject RootToResult(JObject root)
        {
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
            Query query = graphQL.FindQuery(_fetchConfigQueryName, _fetchConfigQueryName, OperationType.Query);

            Tuple<string, object> queryTuple = Hash.SingQuery(null);

            string results = await graphQL.Send(
                query.ToRequest(new Dictionary<string, object>()),
                null, Headers.GetHeaders(queryTuple.Item1)
            );

            if (string.IsNullOrEmpty(results)) 
                return null;

            JObject root = JObject.Parse(results);
            JObject resultObject = (JObject)root["data"]["result"];

            // Debug.Log(resultObject["events"].ToString());
            // Debug.Log(resultObject.ToString());

            AllConfigData configData = resultObject.ToObject<AllConfigData>();

            return configData;
        }

        #region PlayerFetches

        private static string FetchPlayerProjectVariablesQuery = "FetchPlayerProjectVariables";
        public static async Task<JObject> GetPlayer(GetPlayerInput input, bool withToken)
        {
            //Debug.Log("Get player");
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_getPlayerQueryName, "result", OperationType.Query);

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
            Query query = GetQuery(SyncPlayerQuery, OperationType.Mutation);

            Tuple<string, object> queryTuple = Hash.SingQuery(input);
            string headers = queryTuple.Item1;
            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", GetLang() },
                { "withToken", withToken }
            };

            JObject root = await SendQueryRequest(query, headers, variables);

            JObject resultObject = RootToResult(root);

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
            Query query = graphQL.FindQuery(FetchPlayerProjectVariablesQuery, "result", OperationType.Query);

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
        public static async Task<DocumentData> FetchDocument(FetchDocumentInput input, string Format)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(ConfigName);
            var graphQL = new GraphQLClient(config);
           
            Query query = graphQL.FindQuery(FetchDocumentQuery, FetchDocumentQuery, OperationType.Query);
            
            Tuple<string, object> queryTuple = Hash.SingQuery(input);
            
            Dictionary<string, object> variables = new Dictionary<string, object>
            {
                { "input", queryTuple.Item2 },
                { "lang", GetLang() },
                { "format",Format },
            };
            
            try
            {
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

                DocumentData data = resultObject.ToObject<DocumentData>();
                return data;
            }
            catch (Exception ex)
            {
                Debug.Log(ex.Message);
                return null;
            }
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
