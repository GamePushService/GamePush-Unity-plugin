using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public sealed class NativeLeaderboardField
    {
        public string key = "";
        public string name = "";
        public string type = "";
        public bool important;
    }

    public sealed class NativeLeaderboardEntry
    {
        public int id;
        public int position;
        public string name = "";
        public string avatar = "";
        public double score;
        public string json = "{}";

        public string Get(string key) => GpJson.TryGetString(json, key, out var value) ? value ?? "" : "";
    }

    public sealed class NativeLeaderboardResult
    {
        public string tag = "";
        public string variant = "";
        public string name = "";
        public List<NativeLeaderboardEntry> players = new List<NativeLeaderboardEntry>();
        public List<NativeLeaderboardEntry> topPlayers = new List<NativeLeaderboardEntry>();
        public List<NativeLeaderboardEntry> abovePlayers = new List<NativeLeaderboardEntry>();
        public List<NativeLeaderboardEntry> belowPlayers = new List<NativeLeaderboardEntry>();
        public NativeLeaderboardEntry player;
        public List<NativeLeaderboardField> fields = new List<NativeLeaderboardField>();

        public string playersJson = "[]";
        public string topPlayersJson = "[]";
        public string abovePlayersJson = "[]";
        public string belowPlayersJson = "[]";
        public string playerJson = "{}";
    }

    public static class NativeLeaderboard
    {
        public static void Fetch(string tag, string orderBy, string order, int limit, int showNearest,
            string withMe, string includeFields)
        {
            NativeRun.Go(async () =>
            {
                var result = await Load(tag, orderBy, order, limit, showNearest, withMe, includeFields);
                NativeMainThread.Run(() => GP_Leaderboard.NativeFireFetch(result));
            }, () => GP_Leaderboard.NativeFireFetchError(), "leaderboard");
        }

        public static void FetchForOverlay(string tag, string orderBy, string order, int limit, int showNearest,
            string withMe, string includeFields, Action<NativeLeaderboardResult> onDone, Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var result = await Load(tag, orderBy, order, limit, showNearest, withMe, includeFields);
                NativeMainThread.Run(() => onDone?.Invoke(result));
            }, error => onError?.Invoke(error), "leaderboard");
        }

        public static void FetchPlayerRating(string tag, string orderBy, string order)
        {
            NativeRun.Go(async () =>
            {
                var input = TopInput(orderBy, order, 10, 0, "");
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchPlayerRating, input);
                NativeRun.ThrowIfProblem(json);
                var result = GpJson.GetObject(json, "result");
                var player = ParseEntry(GpJson.GetObject(result, "player"));
                var position = player?.position ?? 0;
                NativeMainThread.Run(() => GP_Leaderboard.NativeFirePlayerRating(tag, position));
            }, () => GP_Leaderboard.NativeFirePlayerRatingError(), "leaderboard");
        }

        static async Task<NativeLeaderboardResult> Load(string tag, string orderBy, string order, int limit,
            int showNearest, string withMe, string includeFields)
        {
            var wantsMe = !string.IsNullOrEmpty(withMe) && withMe != "none";
            var input = TopInput(orderBy, order, limit, showNearest, includeFields);
            var json = await NativeCore.Client.Fetch(NativeQueries.FetchTop, input,
                new Dictionary<string, object> { ["withMe"] = wantsMe });
            NativeRun.ThrowIfProblem(json);
            var result = Compose(json, wantsMe, withMe, showNearest);
            result.tag = tag ?? "";
            return result;
        }

        internal static Dictionary<string, object> TopInput(string orderBy, string order, int limit,
            int showNearest, string includeFields)
        {
            var input = new Dictionary<string, object>
            {
                ["orderBy"] = SplitList(string.IsNullOrEmpty(orderBy) ? "score" : orderBy),
                ["order"] = string.IsNullOrEmpty(order) ? "DESC" : order,
                ["limit"] = limit > 0 ? limit : 10,
                ["includeFields"] = SplitList(includeFields)
            };
            if (showNearest > 0)
                input["showNearest"] = showNearest;
            return input;
        }

        internal static List<string> SplitList(string source)
        {
            var list = new List<string>();
            if (string.IsNullOrEmpty(source))
                return list;
            foreach (var part in source.Split(','))
            {
                var trimmed = part.Trim();
                if (trimmed.Length > 0)
                    list.Add(trimmed);
            }
            return list;
        }

        /// <summary>
        /// Merges the FetchTop and FetchPlayerRating halves into the single shape the JS SDK exposes:
        /// the top list, optionally followed by the nearest block, with the player pinned per withMe.
        /// </summary>
        internal static NativeLeaderboardResult Compose(string json, bool wantsMe, string withMe, int showNearest)
        {
            var result = new NativeLeaderboardResult();
            var top = GpJson.GetObject(json, "result");
            var leaderboard = GpJson.GetObject(top, "leaderboard");
            if (!string.IsNullOrEmpty(leaderboard))
                result.name = GpJson.TryGetString(leaderboard, "name", out var name) ? name ?? "" : "";

            result.topPlayersJson = GpJson.GetObject(top, "players") ?? "[]";
            result.topPlayers = ParseList(result.topPlayersJson);
            result.fields = ParseFields(GpJson.GetObjectArray(top, "fields"));

            var playerResult = GpJson.GetObject(json, "playerResult");
            if (!string.IsNullOrEmpty(playerResult))
            {
                result.playerJson = GpJson.GetObject(playerResult, "player") ?? "{}";
                result.abovePlayersJson = GpJson.GetObject(playerResult, "abovePlayers") ?? "[]";
                result.belowPlayersJson = GpJson.GetObject(playerResult, "belowPlayers") ?? "[]";
                result.player = ParseEntry(result.playerJson);
                result.abovePlayers = ParseList(result.abovePlayersJson);
                result.belowPlayers = ParseList(result.belowPlayersJson);
            }

            var merged = new List<NativeLeaderboardEntry>(result.topPlayers);
            if (showNearest > 0 && result.player != null && !Contains(merged, result.player.id))
            {
                // The nearest block overlaps the top whenever the player sits just below it,
                // so every neighbour goes in by id instead of wholesale.
                foreach (var above in result.abovePlayers)
                    AddUnique(merged, above);
                AddUnique(merged, result.player);
                foreach (var below in result.belowPlayers)
                    AddUnique(merged, below);
            }
            else if (wantsMe && result.player != null && !Contains(merged, result.player.id))
            {
                if (withMe == "first")
                    merged.Insert(0, result.player);
                else
                    merged.Add(result.player);
            }

            result.players = merged;
            result.playersJson = Serialize(merged);
            return result;
        }

        public static List<NativeLeaderboardField> VisibleFields(IReadOnlyList<NativeLeaderboardField> fields,
            string displayFields)
        {
            var source = new List<NativeLeaderboardField>();
            if (fields != null)
            {
                foreach (var field in fields)
                {
                    if (field == null || IsIdentityField(field.key))
                        continue;
                    source.Add(field);
                }
            }

            var keys = SplitList(displayFields);
            if (keys.Count > 0)
            {
                var ordered = new List<NativeLeaderboardField>();
                foreach (var key in keys)
                {
                    if (IsIdentityField(key))
                        continue;
                    NativeLeaderboardField match = null;
                    foreach (var field in source)
                    {
                        if (field.key != key)
                            continue;
                        match = field;
                        break;
                    }
                    ordered.Add(match ?? new NativeLeaderboardField { key = key, name = key });
                }
                return ordered.Count > 0 ? ordered : ScoreOnly();
            }

            return source.Count > 0 ? source : ScoreOnly();
        }

        static bool IsIdentityField(string key)
        {
            return key == "id" || key == "name" || key == "avatar" || key == "position";
        }

        static List<NativeLeaderboardField> ScoreOnly()
        {
            return new List<NativeLeaderboardField>
            {
                new NativeLeaderboardField { key = "score", name = "score" }
            };
        }

        static void AddUnique(List<NativeLeaderboardEntry> list, NativeLeaderboardEntry entry)
        {
            if (entry != null && !Contains(list, entry.id))
                list.Add(entry);
        }

        internal static bool Contains(List<NativeLeaderboardEntry> list, int id)
        {
            foreach (var entry in list)
            {
                if (entry.id == id)
                    return true;
            }
            return false;
        }

        internal static string Serialize(List<NativeLeaderboardEntry> list)
        {
            var sb = new StringBuilder();
            sb.Append('[');
            for (var i = 0; i < list.Count; i++)
            {
                if (i > 0)
                    sb.Append(',');
                sb.Append(list[i].json);
            }
            sb.Append(']');
            return sb.ToString();
        }

        internal static List<NativeLeaderboardEntry> ParseList(string arrayJson)
        {
            var list = new List<NativeLeaderboardEntry>();
            foreach (var item in GpJson.SplitArray(arrayJson))
            {
                var entry = ParseEntry(item);
                if (entry != null)
                    list.Add(entry);
            }
            return list;
        }

        internal static NativeLeaderboardEntry ParseEntry(string json)
        {
            if (string.IsNullOrEmpty(json) || GpJson.IsEmptyObject(json))
                return null;
            return new NativeLeaderboardEntry
            {
                id = GpJson.GetInt(json, "id"),
                position = GpJson.GetInt(json, "position"),
                name = GpJson.TryGetString(json, "name", out var name) ? name ?? "" : "",
                avatar = GpJson.TryGetString(json, "avatar", out var avatar) ? avatar ?? "" : "",
                score = ParseDouble(json, "score"),
                json = json
            };
        }

        static double ParseDouble(string json, string key)
        {
            if (!GpJson.TryGetString(json, key, out var raw) || string.IsNullOrEmpty(raw))
                return 0;
            return double.TryParse(raw, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : 0;
        }

        internal static List<NativeLeaderboardField> ParseFields(List<string> items)
        {
            var fields = new List<NativeLeaderboardField>();
            if (items == null)
                return fields;
            foreach (var item in items)
            {
                fields.Add(new NativeLeaderboardField
                {
                    key = GpJson.TryGetString(item, "key", out var key) ? key ?? "" : "",
                    name = GpJson.TryGetString(item, "name", out var name) ? name ?? "" : "",
                    type = GpJson.TryGetString(item, "type", out var type) ? type ?? "" : "",
                    important = GpJson.GetBool(item, "important")
                });
            }
            return fields;
        }
    }
}
