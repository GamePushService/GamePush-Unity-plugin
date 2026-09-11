using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GamePush;

namespace GamePush.Native
{
    public static class NativeGamesCollections
    {
        public static void Fetch(string idOrTag)
        {
            NativeRun.Go(async () =>
            {
                var data = await Load(idOrTag);
                NativeMainThread.Run(() => GP_GamesCollections.NativeFireFetch(idOrTag, data));
            }, () => GP_GamesCollections.NativeFireFetchError(), "gamesCollections");
        }

        public static void FetchForOverlay(string idOrTag, Action<GamesCollectionsFetchData> onDone,
            Action<string> onError)
        {
            NativeRun.Go(async () =>
            {
                var data = await Load(idOrTag);
                NativeMainThread.Run(() => onDone?.Invoke(data));
            }, error => onError?.Invoke(error), "gamesCollections");
        }

        static async Task<GamesCollectionsFetchData> Load(string idOrTag)
        {
            var input = new Dictionary<string, object>();
            if (int.TryParse(idOrTag, out var id) && id > 0)
                input["id"] = id;
            else
                input["tag"] = string.IsNullOrEmpty(idOrTag) ? "ANY" : idOrTag;

            var json = await NativeCore.Client.Fetch(NativeQueries.FetchGamesCollection, input,
                new Dictionary<string, object> { ["url"] = NativeCore.PlatformType ?? "" });
            NativeRun.ThrowIfProblem(json);
            var result = GpJson.GetObject(json, "result");

            var games = new List<Games>();
            foreach (var item in GpJson.GetObjectArray(result, "games"))
            {
                games.Add(new Games
                {
                    id = GpJson.GetInt(item, "id"),
                    name = Text(item, "name"),
                    description = Text(item, "description"),
                    icon = ReadIcon(item),
                    url = Text(item, "url")
                });
            }

            return new GamesCollectionsFetchData
            {
                id = GpJson.GetInt(result, "id"),
                tag = Text(result, "tag"),
                name = Text(result, "name"),
                description = Text(result, "description"),
                games = games.ToArray()
            };
        }

        static string ReadIcon(string gameJson)
        {
            var assets = GpJson.GetObject(gameJson, "assets");
            var icon = assets != null ? GpJson.GetObject(assets, "icon") : null;
            if (icon == null)
                return "";
            foreach (var resource in GpJson.GetObjectArray(icon, "resources"))
            {
                if (GpJson.TryGetString(resource, "src", out var src) && !string.IsNullOrEmpty(src))
                    return src;
            }
            return "";
        }

        static string Text(string json, string key) =>
            GpJson.TryGetString(json, key, out var value) ? value ?? "" : "";
    }
}
