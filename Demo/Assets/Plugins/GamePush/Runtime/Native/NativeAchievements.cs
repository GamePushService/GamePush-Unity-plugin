using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using GamePush;

namespace GamePush.Native
{
    public static class NativeAchievements
    {
        static readonly List<AchievementsFetch> Catalog = new List<AchievementsFetch>();
        static readonly List<AchievementsFetchGroups> Groups = new List<AchievementsFetchGroups>();
        static readonly Dictionary<int, AchievementsFetchPlayer> Progress =
            new Dictionary<int, AchievementsFetchPlayer>();

        static bool _catalogLoaded;
        static Task _catalogTask;

        public static IReadOnlyList<AchievementsFetch> Achievements => Catalog;
        public static IReadOnlyList<AchievementsFetchGroups> AchievementGroups => Groups;
        public static bool CatalogLoaded => _catalogLoaded;

        public static void Fetch()
        {
            NativeRun.Go(async () =>
            {
                await EnsureCatalog();
                var list = new List<AchievementsFetch>(Catalog);
                var groups = new List<AchievementsFetchGroups>(Groups);
                var player = PlayerList();
                NativeMainThread.Run(() =>
                {
                    GP_Achievements.NativeFireFetch(list);
                    GP_Achievements.NativeFireFetchGroups(groups);
                    GP_Achievements.NativeFireFetchPlayer(player);
                });
            }, () => GP_Achievements.NativeFireFetchError(), "achievements");
        }

        /// <summary>Loads the achievement catalog once and keeps it cached for the session.</summary>
        public static Task EnsureCatalog()
        {
            if (_catalogLoaded)
                return Task.CompletedTask;
            return _catalogTask ?? (_catalogTask = LoadCatalog());
        }

        static async Task LoadCatalog()
        {
            try
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.FetchAchievementsConfig);
                NativeRun.ThrowIfProblem(json);
                var result = GpJson.GetObject(json, "result");
                Catalog.Clear();
                foreach (var item in GpJson.GetObjectArray(result, "achievements"))
                    Catalog.Add(ParseAchievement(item));
                Groups.Clear();
                foreach (var item in GpJson.GetObjectArray(result, "achievementsGroups"))
                    Groups.Add(ParseGroup(item));
                _catalogLoaded = true;
            }
            finally
            {
                _catalogTask = null;
            }
        }

        public static void Unlock(string idOrTag)
        {
            NativeRun.Go(async () =>
            {
                var json = await NativeCore.Client.Fetch(NativeQueries.UnlockAchievement, Target(idOrTag, 2));
                NativeRun.ThrowIfProblem(json);
                var result = GpJson.GetObject(json, "result");
                ApplyPlayerEntry(result);
                NativeMainThread.Run(() => GP_Achievements.NativeFireUnlock(idOrTag));
            }, error => GP_Achievements.NativeFireUnlockError(error), "achievements");
        }

        public static void SetProgress(string idOrTag, int progress)
        {
            NativeRun.Go(async () =>
            {
                var input = Target(idOrTag, 0);
                input["progress"] = progress;
                var json = await NativeCore.Client.Fetch(NativeQueries.SetAchievementProgress, input);
                NativeRun.ThrowIfProblem(json);
                ApplyPlayerEntry(GpJson.GetObject(json, "result"));
                NativeMainThread.Run(() => GP_Achievements.NativeFireProgress(idOrTag));
            }, () => GP_Achievements.NativeFireProgressError(), "achievements");
        }

        public static bool Has(string idOrTag)
        {
            var achievement = Find(idOrTag);
            if (achievement == null)
                return false;
            return Progress.TryGetValue(achievement.id, out var entry) && entry.unlocked;
        }

        public static int GetProgress(string idOrTag)
        {
            var achievement = Find(idOrTag);
            if (achievement == null)
                return 0;
            return Progress.TryGetValue(achievement.id, out var entry) ? entry.progress : 0;
        }

        public static AchievementsFetch Find(string idOrTag)
        {
            if (string.IsNullOrEmpty(idOrTag))
                return null;
            var isId = int.TryParse(idOrTag, out var id);
            foreach (var item in Catalog)
            {
                if (isId && item.id == id)
                    return item;
                if (!isId && item.tag == idOrTag)
                    return item;
            }
            return null;
        }

        public static List<AchievementsFetchPlayer> PlayerList()
        {
            var list = new List<AchievementsFetchPlayer>(Progress.Count);
            foreach (var entry in Progress.Values)
                list.Add(entry);
            return list;
        }

        public static AchievementsFetchPlayer PlayerEntry(int achievementId) =>
            Progress.TryGetValue(achievementId, out var entry) ? entry : null;

        /// <summary>Called by NativePlayer whenever a player response carries achievementsList.</summary>
        public static void ApplyPlayerList(string arrayJson)
        {
            Progress.Clear();
            foreach (var item in GpJson.SplitArray(arrayJson))
                Store(ParsePlayerAchievement(item));
        }

        static void ApplyPlayerEntry(string result)
        {
            if (string.IsNullOrEmpty(result))
                return;
            Store(ParsePlayerAchievement(result));
            var achievement = GpJson.GetObject(result, "achievement");
            if (string.IsNullOrEmpty(achievement))
                return;
            var parsed = ParseAchievement(achievement);
            for (var i = 0; i < Catalog.Count; i++)
            {
                if (Catalog[i].id != parsed.id)
                    continue;
                Catalog[i] = parsed;
                return;
            }
            Catalog.Add(parsed);
        }

        static void Store(AchievementsFetchPlayer entry)
        {
            if (entry != null && entry.achievementId > 0)
                Progress[entry.achievementId] = entry;
        }

        static Dictionary<string, object> Target(string idOrTag, int version)
        {
            var input = new Dictionary<string, object>();
            if (int.TryParse(idOrTag, out var id))
                input["id"] = id;
            else
                input["tag"] = idOrTag ?? "";
            if (version > 0)
                input["v"] = version;
            return input;
        }

        static AchievementsFetch ParseAchievement(string json)
        {
            return new AchievementsFetch
            {
                id = GpJson.GetInt(json, "id"),
                tag = Text(json, "tag"),
                name = Text(json, "name"),
                description = Text(json, "description"),
                icon = Text(json, "icon"),
                iconSmall = Text(json, "iconSmall"),
                lockedIcon = Text(json, "lockedIcon"),
                lockedIconSmall = Text(json, "lockedIconSmall"),
                rare = Text(json, "rare"),
                maxProgress = GpJson.GetInt(json, "maxProgress"),
                progressStep = GpJson.GetInt(json, "progressStep"),
                lockedVisible = GpJson.GetBool(json, "isLockedVisible"),
                lockedDescriptionVisible = GpJson.GetBool(json, "isLockedDescriptionVisible")
            };
        }

        static AchievementsFetchGroups ParseGroup(string json)
        {
            var ids = new List<int>();
            foreach (var raw in GpJson.SplitArray(GpJson.GetObject(json, "achievements")))
            {
                if (int.TryParse(raw, out var id))
                    ids.Add(id);
            }
            return new AchievementsFetchGroups
            {
                id = GpJson.GetInt(json, "id"),
                tag = Text(json, "tag"),
                name = Text(json, "name"),
                description = Text(json, "description"),
                achievements = ids.ToArray()
            };
        }

        static AchievementsFetchPlayer ParsePlayerAchievement(string json)
        {
            return new AchievementsFetchPlayer
            {
                achievementId = GpJson.GetInt(json, "achievementId"),
                createdAt = Text(json, "createdAt"),
                progress = GpJson.GetInt(json, "progress"),
                unlocked = GpJson.GetBool(json, "unlocked")
            };
        }

        static string Text(string json, string key) =>
            GpJson.TryGetString(json, key, out var value) ? value ?? "" : "";
    }
}
