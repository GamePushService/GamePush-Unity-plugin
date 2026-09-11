using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using GamePush;

namespace GamePush.Native
{
    public sealed class GraphQLClient
    {
        readonly int _projectId;
        readonly string _publicToken;
        readonly string[] _apiRoots;
        string _lang = "EN";
        string _platformType = "NONE";
        string _platformKey = "";
        string _playerDataB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes("{}"));

        public int ProjectId => _projectId;
        public IReadOnlyList<string> ApiRoots => _apiRoots;

        public GraphQLClient(int projectId, string publicToken, params string[] apiRoots)
        {
            _projectId = projectId;
            _publicToken = publicToken ?? "";
            _apiRoots = apiRoots != null && apiRoots.Length > 0
                ? apiRoots
                : new[] { "https://api.eponesh.com/gs/api", "https://api.gamepush.com/gs/api" };
        }

        public void SetLang(string lang) => _lang = string.IsNullOrEmpty(lang) ? "EN" : lang.ToUpperInvariant();
        public void SetPlatform(string type, string key)
        {
            _platformType = string.IsNullOrEmpty(type) ? "NONE" : type;
            _platformKey = key ?? "";
        }

        public void SetPlayerData(Dictionary<string, object> data)
        {
            var json = GpJson.SortedStringifyObject(data ?? new Dictionary<string, object>());
            _playerDataB64 = Convert.ToBase64String(Encoding.UTF8.GetBytes(json));
        }

        public async Task Ping(string token)
        {
            foreach (var root in _apiRoots)
            {
                var url = root.TrimEnd('/') + "/ping?t=" + UnityWebRequest.EscapeURL(token ?? "");
                using var req = UnityWebRequest.Get(url);
                var op = req.SendWebRequest();
                while (!op.isDone)
                    await Task.Yield();
                if (req.result == UnityWebRequest.Result.Success)
                    return;
            }
        }

        public async Task<string> Fetch(string query, Dictionary<string, object> input = null, Dictionary<string, object> extraVariables = null)
        {
            var variables = new Dictionary<string, object>();
            if (query.IndexOf("$lang", StringComparison.Ordinal) >= 0)
                variables["lang"] = _lang;
            if (extraVariables != null)
            {
                foreach (var pair in extraVariables)
                    variables[pair.Key] = pair.Value;
            }
            if (input != null)
                variables["input"] = input;

            var body = UnsortedVariablesJson(query, variables);
            var hash = Sign(input);
            Exception last = null;
            foreach (var root in _apiRoots)
            {
                var url = root.TrimEnd('/') + "/graphql/" + _projectId;
                try
                {
                    return await Post(url, body, hash);
                }
                catch (Exception exception)
                {
                    last = exception;
                    GP_Logger.Warn("GraphQL", url + ": " + exception.Message);
                }
            }
            throw last ?? new Exception("connection_error");
        }

        public async Task<string> PostRaw(string pathAndQuery, string jsonBody)
        {
            Exception last = null;
            foreach (var root in _apiRoots)
            {
                var url = root.TrimEnd('/') + "/" + (pathAndQuery ?? "").TrimStart('/');
                try
                {
                    using var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
                    req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(jsonBody ?? "{}"));
                    req.downloadHandler = new DownloadHandlerBuffer();
                    req.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
                    req.SetRequestHeader("X-Platform", _platformType);
                    req.SetRequestHeader("X-Platform-Key", _platformKey);
                    req.SetRequestHeader("X-Project-ID", _projectId.ToString());
                    req.SetRequestHeader("X-Project-Token", _publicToken);
                    req.SetRequestHeader("X-Language", _lang);
                    req.SetRequestHeader("X-Player-Data", _playerDataB64);
                    var op = req.SendWebRequest();
                    while (!op.isDone)
                        await Task.Yield();
                    if (req.result != UnityWebRequest.Result.Success)
                        throw new Exception(req.responseCode + ". " + req.error + " " + req.downloadHandler.text);
                    return req.downloadHandler.text ?? "";
                }
                catch (Exception exception)
                {
                    last = exception;
                    GP_Logger.Warn("GraphQL", url + ": " + exception.Message);
                }
            }
            throw last ?? new Exception("connection_error");
        }

        string UnsortedVariablesJson(string query, Dictionary<string, object> variables)
        {
            var sb = new StringBuilder();
            sb.Append("{\"query\":");
            sb.Append(GpJson.Quote(query));
            sb.Append(",\"variables\":");
            sb.Append(VariablesToJson(variables));
            sb.Append('}');
            return sb.ToString();
        }

        string VariablesToJson(Dictionary<string, object> variables)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            var first = true;
            foreach (var pair in variables)
            {
                if (!first) sb.Append(',');
                first = false;
                sb.Append(GpJson.Quote(pair.Key));
                sb.Append(':');
                if (pair.Value is Dictionary<string, object> nested)
                    sb.Append(DictionaryToJsonUnsorted(nested));
                else
                    sb.Append(GpJson.Stringify(pair.Value));
            }
            sb.Append('}');
            return sb.ToString();
        }

        string DictionaryToJsonUnsorted(Dictionary<string, object> source)
        {
            var sb = new StringBuilder();
            sb.Append('{');
            var first = true;
            foreach (var pair in source)
            {
                if (pair.Value == null)
                    continue;
                if (!first) sb.Append(',');
                first = false;
                sb.Append(GpJson.Quote(pair.Key));
                sb.Append(':');
                if (pair.Value is Dictionary<string, object> nested)
                    sb.Append(DictionaryToJsonUnsorted(nested));
                else
                    sb.Append(GpJson.Stringify(pair.Value));
            }
            sb.Append('}');
            return sb.ToString();
        }

        string Sign(Dictionary<string, object> input)
        {
            var ready = "{\"projectId\":" + _projectId +
                        ",\"query\":" + (input == null ? "null" : GpJson.SortedStringifyObject(input)) +
                        ",\"token\":" + GpJson.Quote(_publicToken) + "}";
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(ready));
            var sb = new StringBuilder(bytes.Length * 2);
            foreach (var b in bytes)
                sb.Append(b.ToString("x2"));
            return sb.ToString();
        }

        async Task<string> Post(string url, string body, string hash)
        {
            using var req = new UnityWebRequest(url, UnityWebRequest.kHttpVerbPOST);
            req.uploadHandler = new UploadHandlerRaw(Encoding.UTF8.GetBytes(body));
            req.downloadHandler = new DownloadHandlerBuffer();
            req.SetRequestHeader("Content-Type", "application/json;charset=utf-8");
            req.SetRequestHeader("X-Transaction-Token", hash);
            req.SetRequestHeader("X-Platform", _platformType);
            req.SetRequestHeader("X-Platform-Key", _platformKey);
            req.SetRequestHeader("X-Project-ID", _projectId.ToString());
            req.SetRequestHeader("X-Project-Token", _publicToken);
            req.SetRequestHeader("X-Language", _lang);
            req.SetRequestHeader("X-Player-Data", _playerDataB64);
            var op = req.SendWebRequest();
            while (!op.isDone)
                await Task.Yield();
            if (req.result != UnityWebRequest.Result.Success)
                throw new Exception(req.responseCode + ". " + req.error + " " + req.downloadHandler.text);
            var text = req.downloadHandler.text;
            if (GpJson.TryGetString(text, "message", out var message) && text.Contains("\"errors\""))
                throw new Exception(message);
            var data = GpJson.GetObject(text, "data");
            if (string.IsNullOrEmpty(data))
                return text ?? "";
            var keys = GpJson.ObjectKeys(data);
            if (keys.Count == 1)
            {
                var inner = GpJson.GetObject(data, keys[0]);
                if (!string.IsNullOrEmpty(inner))
                    return inner;
            }
            return data;
        }
    }
}
