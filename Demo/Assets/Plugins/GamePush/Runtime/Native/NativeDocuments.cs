using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public static class NativeDocuments
    {
        public const string DefaultType = "PLAYER_PRIVACY_POLICY";

        public static void Fetch(string type = DefaultType, string format = "TXT")
        {
            NativeRun.Go(async () =>
            {
                var content = await Load(type, format);
                NativeMainThread.Run(() => GP_Documents.NativeFireFetch(content));
            }, () => GP_Documents.NativeFireFetchError(), "documents");
        }

        public static void FetchForOverlay(string type, string format, Action<string> onDone, Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var content = await Load(type, format);
                NativeMainThread.Run(() => onDone?.Invoke(content));
            }, error => onError?.Invoke(error), "documents");
        }

        static async Task<string> Load(string type, string format)
        {
            var json = await NativeCore.Client.Fetch(NativeQueries.FetchDocument,
                new Dictionary<string, object> { ["type"] = string.IsNullOrEmpty(type) ? DefaultType : type },
                new Dictionary<string, object> { ["format"] = string.IsNullOrEmpty(format) ? "TXT" : format });
            NativeRun.ThrowIfProblem(json);
            var result = GpJson.GetObject(json, "result");
            return GpJson.TryGetString(result, "content", out var content) ? content ?? "" : "";
        }
    }
}
