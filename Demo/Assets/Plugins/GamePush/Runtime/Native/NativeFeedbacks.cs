using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public sealed class NativeFeedbacksPage
    {
        public FeedbackData[] items = Array.Empty<FeedbackData>();
        public bool canLoadMore;
    }

    public static class NativeFeedbacks
    {
        static int _offset;
        static int _total;

        public static void Send(FeedbackData data)
        {
            NativeRun.Go(async () =>
            {
                var input = new Dictionary<string, object>
                {
                    ["text"] = data?.text ?? ""
                };
                if (!string.IsNullOrEmpty(data?.type))
                    input["type"] = data.type;
                if (data?.files != null && data.files.Length > 0)
                    input["files"] = data.files;

                var json = await NativeCore.Client.Fetch(NativeQueries.CreateFeedback, input);
                NativeRun.ThrowIfProblem(json);
                var feedback = ParseFeedback(GpJson.GetObject(json, "result"));
                NativeMainThread.Run(() => GP_Feedbacks.NativeFireSend(feedback));
            }, error => GP_Feedbacks.NativeFireSendError(error), "feedbacks");
        }

        public static void Fetch(string type, string status, int limit)
        {
            NativeRun.Go(async () =>
            {
                var page = await Load(type, status, limit, 0);
                NativeMainThread.Run(() => GP_Feedbacks.NativeFireFetch(page));
            }, error => GP_Feedbacks.NativeFireFetchError(error), "feedbacks");
        }

        public static void FetchMore(string type, string status, int limit)
        {
            NativeRun.Go(async () =>
            {
                var page = await Load(type, status, limit, _offset);
                NativeMainThread.Run(() => GP_Feedbacks.NativeFireFetchMore(page));
            }, error => GP_Feedbacks.NativeFireFetchMoreError(error), "feedbacks");
        }

        public static void FetchForOverlay(string type, string status, int limit, int offset,
            Action<NativeFeedbacksPage> onDone, Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var page = await Load(type, status, limit, offset);
                NativeMainThread.Run(() => onDone?.Invoke(page));
            }, error => onError?.Invoke(error), "feedbacks");
        }

        public static void FetchOne(string feedbackId, Action<FeedbackData> onDone, Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchFeedback,
                    new Dictionary<string, object> { ["feedbackId"] = feedbackId ?? "" });
                NativeRun.ThrowIfProblem(json);
                var feedback = ParseFeedback(GpJson.GetObject(json, "result"));
                NativeMainThread.Run(() => onDone?.Invoke(feedback));
            }, error => onError?.Invoke(error), "feedbacks");
        }

        public static void SendMessage(FeedbackMessageData data, Action<FeedbackMessageData> onDone = null,
            Action<string> onError = null)
        {
            NativeRun.Go(async () =>
            {
                var input = new Dictionary<string, object>
                {
                    ["feedbackId"] = data?.feedbackId ?? "",
                    ["text"] = data?.text ?? ""
                };
                if (data?.files != null && data.files.Length > 0)
                    input["files"] = data.files;

                var json = await NativeCore.Client.Fetch(NativeQueries.SendFeedbackMessage, input);
                NativeRun.ThrowIfProblem(json);
                var message = ParseMessage(GpJson.GetObject(json, "result"));
                NativeMainThread.Run(() =>
                {
                    GP_Feedbacks.NativeFireSendMessage(message);
                    onDone?.Invoke(message);
                });
            }, error =>
            {
                GP_Feedbacks.NativeFireSendMessageError(error);
                onError?.Invoke(error);
            }, "feedbacks");
        }

        static async Task<NativeFeedbacksPage> Load(string type, string status, int limit, int offset)
        {
            var take = limit > 0 ? limit : 20;
            var input = new Dictionary<string, object>
            {
                ["limit"] = take,
                ["offset"] = offset
            };
            if (!string.IsNullOrEmpty(type))
                input["type"] = type;
            if (!string.IsNullOrEmpty(status))
                input["status"] = status;

            var json = await NativeCore.Client.Fetch(NativeQueries.FetchFeedbacks, input);
            NativeRun.ThrowIfProblem(json);
            var result = GpJson.GetObject(json, "result");

            var items = new List<FeedbackData>();
            foreach (var item in GpJson.GetObjectArray(result, "feedbacks"))
                items.Add(ParseFeedback(item));

            _total = GpJson.GetInt(result, "totalCount");
            _offset = offset + items.Count;
            return new NativeFeedbacksPage
            {
                items = items.ToArray(),
                canLoadMore = _offset < _total
            };
        }

        static FeedbackData ParseFeedback(string json)
        {
            var messages = new List<FeedbackMessageData>();
            foreach (var item in GpJson.GetObjectArray(json, "messages"))
                messages.Add(ParseMessage(item));

            return new FeedbackData
            {
                id = Text(json, "id"),
                type = Text(json, "type"),
                text = Text(json, "text"),
                status = Text(json, "status"),
                files = Strings(json, "files"),
                messages = messages.ToArray(),
                playerId = GpJson.GetInt(json, "playerId"),
                projectId = GpJson.GetInt(json, "projectId"),
                platformId = GpJson.GetInt(json, "platformId"),
                createdAt = Text(json, "createdAt"),
                updatedAt = Text(json, "updatedAt")
            };
        }

        static FeedbackMessageData ParseMessage(string json)
        {
            // The API names the timestamp "time" on messages while the plugin DTO calls it createdAt.
            var created = Text(json, "createdAt");
            if (string.IsNullOrEmpty(created))
                created = Text(json, "time");
            return new FeedbackMessageData
            {
                id = Text(json, "id"),
                text = Text(json, "text"),
                files = Strings(json, "files"),
                attachments = Strings(json, "attachments"),
                author = Text(json, "author"),
                feedbackId = Text(json, "feedbackId"),
                createdAt = created
            };
        }

        static string[] Strings(string json, string key)
        {
            var raw = GpJson.GetObject(json, key);
            if (string.IsNullOrEmpty(raw))
                return Array.Empty<string>();
            var items = GpJson.SplitArray(raw);
            var result = new string[items.Count];
            for (var i = 0; i < items.Count; i++)
                result[i] = Unquote(items[i]);
            return result;
        }

        static string Unquote(string token)
        {
            if (string.IsNullOrEmpty(token))
                return "";
            token = token.Trim();
            if (token.Length >= 2 && token[0] == '"' && token[token.Length - 1] == '"')
                return token.Substring(1, token.Length - 2).Replace("\\\"", "\"").Replace("\\\\", "\\");
            return token;
        }

        static string Text(string json, string key) =>
            GpJson.TryGetString(json, key, out var value) ? value ?? "" : "";
    }
}
