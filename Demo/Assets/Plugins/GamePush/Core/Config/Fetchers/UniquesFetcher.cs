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

namespace GamePush.Core
{
    public class UniquesFetcher
    {
        private static string _configName = "GP_GraphQL";
        private static string _resultOperation = "result";

        private static string _checkUnique = "CheckUniques";
        private static string _deleteUnique = "DeleteUniques";
        private static string _registerUnique = "RegisterUniques";

        private static string SUCCESS_TAG = "gp_success";
        private static string ERROR_TAG = "gp_error";

        public static async Task<TagValueData> CheckUniqueValue(object input, bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_checkUnique, _resultOperation, OperationType.Query);

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
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_registerUnique, _resultOperation, OperationType.Mutation);

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

            UniquesData resultValue = resultObject["success"].ToObject<UniquesData>();
            result = new TagValueData(SUCCESS_TAG, resultValue);

            return result;
        }

        public static async Task<TagValueData> DeleteUniqueValue(object input, bool withToken)
        {
            GraphQLConfig config = Resources.Load<GraphQLConfig>(_configName);
            var graphQL = new GraphQLClient(config);
            Query query = graphQL.FindQuery(_deleteUnique, _resultOperation, OperationType.Mutation);

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
    }
}
