using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using GamePush;
using UnityEngine.Networking;

namespace GamePushEditor
{
    public static class GP_ProductCatalogClient
    {
        private const string Endpoint = "https://api.gamepush.com/gs/api/graphql";
        private const int PageSize = 100;

        private const string Query = @"
query fetchProducts($input: FetchProductsInput!) {
  result: FetchProducts(input: $input) {
    __typename
    ... on Problem { message }
    ... on ProductsList {
      items {
        id
        tag
        isSubscription
        trialPeriod
        period
        icon
        names { en ru }
        descriptions { en ru }
        prices { YANDEX VK OK TELEGRAM CUSTOM PARTNER GOOGLE_PLAY CRAZY_GAMES }
        realPrices { RUB USD EUR }
      }
    }
  }
}";

        public static async Task<List<FetchProducts>> FetchProducts(int projectId, string apiSecret, Platform platform)
        {
            var all = new List<FetchProducts>();
            int offset = 0;
            while (true)
            {
                string body = BuildBody(projectId, offset);
                string json = await Post(body, projectId, apiSecret);
                string typename = ReadTypename(json);
                if (string.Equals(typename, "Problem", StringComparison.Ordinal))
                    throw new InvalidOperationException(ReadProblemMessage(json));

                string items = ExtractItemsArray(json);
                List<FetchProducts> page = GP_ProductCatalogMapper.MapGraphqlItems(items, platform);
                all.AddRange(page);
                if (page.Count < PageSize)
                    break;
                offset += PageSize;
            }

            return all;
        }

        private static string BuildBody(int projectId, int offset)
        {
            string variables = "{\"input\":{\"projectId\":" + projectId + ",\"limit\":" + PageSize + ",\"offset\":" + offset + "}}";
            return "{\"query\":" + JsonEscape(Query) + ",\"variables\":" + variables + "}";
        }

        private static async Task<string> Post(string body, int projectId, string apiSecret)
        {
            using var request = new UnityWebRequest(Endpoint, UnityWebRequest.kHttpVerbPOST);
            request.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            request.SetRequestHeader("X-API-Secret", apiSecret);
            request.SetRequestHeader("X-Project-ID", projectId.ToString());

            UnityWebRequestAsyncOperation operation = request.SendWebRequest();
            while (!operation.isDone)
                await Task.Yield();

            if (request.result != UnityWebRequest.Result.Success)
                throw new InvalidOperationException("GamePush API request failed.");

            return request.downloadHandler.text ?? string.Empty;
        }

        private static string ReadTypename(string json)
        {
            return ReadJsonString(json, "__typename") ?? string.Empty;
        }

        private static string ReadProblemMessage(string json)
        {
            return ReadJsonString(json, "message") ?? "GamePush API returned an error.";
        }

        private static string ReadJsonString(string json, string key)
        {
            string token = "\"" + key + "\"";
            int keyIndex = json.IndexOf(token, StringComparison.Ordinal);
            if (keyIndex < 0)
                return null;
            int colon = json.IndexOf(':', keyIndex + token.Length);
            if (colon < 0)
                return null;
            int start = json.IndexOf('"', colon + 1);
            if (start < 0)
                return null;
            int end = json.IndexOf('"', start + 1);
            return end < 0 ? null : json.Substring(start + 1, end - start - 1);
        }

        private static string ExtractItemsArray(string json)
        {
            int items = json.IndexOf("\"items\"", StringComparison.Ordinal);
            if (items < 0)
                return "[]";
            int start = json.IndexOf('[', items);
            if (start < 0)
                return "[]";

            int depth = 0;
            bool quoted = false;
            for (int i = start; i < json.Length; i++)
            {
                char ch = json[i];
                if (ch == '"' && (i == 0 || json[i - 1] != '\\'))
                    quoted = !quoted;
                if (quoted)
                    continue;
                if (ch == '[')
                    depth++;
                else if (ch == ']')
                {
                    depth--;
                    if (depth == 0)
                        return json.Substring(start, i - start + 1);
                }
            }

            return "[]";
        }

        private static string JsonEscape(string value)
        {
            var sb = new StringBuilder(value.Length + 16);
            sb.Append('"');
            foreach (char ch in value)
            {
                if (ch == '"' || ch == '\\')
                    sb.Append('\\').Append(ch);
                else if (ch == '\n')
                    sb.Append("\\n");
                else if (ch == '\r')
                    sb.Append("\\r");
                else if (ch == '\t')
                    sb.Append("\\t");
                else
                    sb.Append(ch);
            }

            sb.Append('"');
            return sb.ToString();
        }
    }
}
