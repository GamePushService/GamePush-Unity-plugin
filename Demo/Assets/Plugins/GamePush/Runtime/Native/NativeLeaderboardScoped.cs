using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public static class NativeLeaderboardScoped
    {
        public static void Fetch(string idOrTag, string variant, string order, int limit, int showNearest,
            string includeFields, string withMe)
        {
            NativeRun.Go(async () =>
            {
                var result = await Load(idOrTag, variant, order, limit, showNearest, includeFields, withMe);
                NativeMainThread.Run(() => GP_LeaderboardScoped.NativeFireFetch(result));
            }, () => GP_LeaderboardScoped.NativeFireFetchError(), "leaderboardScoped");
        }

        public static void FetchForOverlay(string idOrTag, string variant, string order, int limit, int showNearest,
            string includeFields, string withMe, Action<NativeLeaderboardResult> onDone, Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var result = await Load(idOrTag, variant, order, limit, showNearest, includeFields, withMe);
                NativeMainThread.Run(() => onDone?.Invoke(result));
            }, error => onError?.Invoke(error), "leaderboardScoped");
        }

        public static void FetchPlayerRating(string idOrTag, string variant, string includeFields)
        {
            NativeRun.Go(async () =>
            {
                var input = ScopedInput(idOrTag, variant, "DESC", 10, 0, includeFields);
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchPlayerRatingScoped, input);
                NativeRun.ThrowIfProblem(json);
                var player = NativeLeaderboard.ParseEntry(GpJson.GetObject(GpJson.GetObject(json, "result"), "player"));
                var position = player?.position ?? 0;
                NativeMainThread.Run(() =>
                    GP_LeaderboardScoped.NativeFirePlayerRating(idOrTag, variant, position));
            }, () => GP_LeaderboardScoped.NativeFirePlayerRatingError(), "leaderboardScoped");
        }

        public static void PublishRecord(string idOrTag, string variant, bool over, Dictionary<string, double> record)
        {
            NativeRun.Go(async () =>
            {
                var input = Target(idOrTag);
                input["variant"] = variant ?? "";
                input["override"] = over;
                var values = new Dictionary<string, object>();
                if (record != null)
                {
                    foreach (var pair in record)
                    {
                        if (!string.IsNullOrEmpty(pair.Key))
                            values[pair.Key] = pair.Value;
                    }
                }
                input["record"] = values;
                var json = await NativeCore.Client.Fetch(NativeQueries.PublishRecord, input);
                NativeRun.ThrowIfProblem(json);
                NativeMainThread.Run(() => GP_LeaderboardScoped.NativeFirePublishRecord());
            }, () => GP_LeaderboardScoped.NativeFirePublishRecordError(), "leaderboardScoped");
        }

        static async Task<NativeLeaderboardResult> Load(string idOrTag, string variant, string order, int limit,
            int showNearest, string includeFields, string withMe)
        {
            var wantsMe = !string.IsNullOrEmpty(withMe) && withMe != "none";
            var input = ScopedInput(idOrTag, variant, order, limit, showNearest, includeFields);
            var json = await NativeCore.Client.Fetch(NativeQueries.FetchTopScoped, input,
                new Dictionary<string, object> { ["withMe"] = wantsMe });
            NativeRun.ThrowIfProblem(json);
            var result = NativeLeaderboard.Compose(json, wantsMe, withMe, showNearest);
            result.tag = idOrTag ?? "";
            result.variant = variant ?? "";
            return result;
        }

        static Dictionary<string, object> ScopedInput(string idOrTag, string variant, string order, int limit,
            int showNearest, string includeFields)
        {
            var input = Target(idOrTag);
            input["variant"] = variant ?? "";
            input["order"] = string.IsNullOrEmpty(order) ? "DESC" : order;
            input["limit"] = limit > 0 ? limit : 10;
            input["includeFields"] = NativeLeaderboard.SplitList(includeFields);
            if (showNearest > 0)
                input["showNearest"] = showNearest;
            return input;
        }

        static Dictionary<string, object> Target(string idOrTag)
        {
            var input = new Dictionary<string, object>();
            if (int.TryParse(idOrTag, out var id) && id > 0)
                input["id"] = id;
            else
                input["tag"] = idOrTag ?? "";
            return input;
        }
    }
}
