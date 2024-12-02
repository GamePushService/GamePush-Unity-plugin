using System;
using System.Collections.Generic;
using UnityEngine;
using GamePush.UI;
using GamePush.Data;
using System.Threading.Tasks;
using System.Linq;
using Newtonsoft.Json;

namespace GamePush.Core
{
    public class LeaderboardModule
    {
        public event Action<string, GP_Data> OnFetchSuccess;
        public event Action<string, GP_Data> OnFetchTopPlayers;
        public event Action<string, GP_Data> OnFetchAbovePlayers;
        public event Action<string, GP_Data> OnFetchBelowPlayers;
        public event Action<string, GP_Data> OnFetchPlayer;
        public event Action OnFetchError;

        public event Action<string, int> OnFetchPlayerRatingSuccess;
        public event Action OnFetchPlayerRatingError;

        public event Action OnLeaderboardOpen;
        public event Action OnLeaderboardClose;

        public event Action<string, GP_Data> OnScopedFetchSuccess;
        public event Action<string, GP_Data> OnScopedFetchTopPlayers;
        public event Action<string, GP_Data> OnScopedFetchAbovePlayers;
        public event Action<string, GP_Data> OnScopedFetchBelowPlayers;
        public event Action<string, GP_Data> OnScopedFetchPlayer;
        public event Action<string, string, GP_Data> OnScopedFetchTagVariant;
        public event Action OnScopedFetchError;

        public event Action<string, int> OnScopedFetchPlayerRating;
        public event Action<string, string, int> OnScopedFetchPlayerRatingTagVariant;
        public event Action OnScopedFetchPlayerRatingError;

        public event Action OnScopedOpen;
        public event Action OnScopedClose;

        public event Action OnScopedPublishRecordComplete;
        public event Action OnScopedPublishRecordError;

        public LeaderboardModule() { }

        private void ActionInvoke(Action<string, GP_Data> action, string tag, object data)
        {
            GP_Data gpData = new GP_Data(JsonConvert.SerializeObject(data, Formatting.Indented));
            action?.Invoke(tag, gpData);
        }

        public async void Open(string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 5, WithMe withMe = WithMe.none, string includeFields = "", string displayFields = "")
        {
            //Logger.Log("OPEN");
            RatingData data = await Fetch(orderBy, order, limit, showNearest, withMe, includeFields);
            OverlayCanvas.Controller.OpenLeaderboard(data, OnLeaderboardOpen, OnLeaderboardClose);
        }

        public async void SimpleFetch(
            string tag = "",
            string orderBy = "score",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 0,
            WithMe withMe = WithMe.none,
            string includeFields = ""
            )
        {
            RatingData data = await Fetch(orderBy, order, limit, showNearest, withMe, includeFields);
            if (data == null)
            {
                OnFetchError?.Invoke();
                return;
            }

            ActionInvoke(OnFetchSuccess, tag, data.players);
            ActionInvoke(OnFetchTopPlayers, tag, data.topPlayers);
            ActionInvoke(OnFetchAbovePlayers, tag, data.abovePlayers);
            ActionInvoke(OnFetchBelowPlayers, tag, data.belowPlayers);
            ActionInvoke(OnFetchPlayer, tag, data.player);
        }


        public async Task<RatingData> Fetch(string orderBy = "score", Order order = Order.DESC, int limit = 10, int showNearest = 0, WithMe withMe = WithMe.none, string includeFields = "")
        {
            //Logger.Log("FETCH");
            GetLeaderboardQuery input = new GetLeaderboardQuery(orderBy, order, limit, showNearest, includeFields);
            AllRatingData data = await DataFetcher.GetRating(input, withMe: true);

            ProcessLeaderboardResult(data.ratingData, data.playerRatingData, showNearest, withMe.ToString(), limit);

            return data.ratingData;
        }

        public async void SimpleFetchPlayerRating(
            string tag = "",
            string orderBy = "score",
            Order order = Order.DESC)
        {
            PlayerRatingData data = await FetchPlayerRating(orderBy, order);
            if(data == null)
            {
                OnFetchPlayerRatingError?.Invoke();
                return;
            }


            int playerPosition = 0;

            if (data.player.TryGetValue("position", out var positionValue))
            {
                if (positionValue is int position)
                    playerPosition = position;
                else if (positionValue is long longPosition)
                    playerPosition = (int)longPosition;
                else if (positionValue is string positionString && int.TryParse(positionString, out int parsedPosition))
                    playerPosition = parsedPosition;
            }

            OnFetchPlayerRatingSuccess?.Invoke(tag, playerPosition);
        }

        public async Task<PlayerRatingData> FetchPlayerRating(string orderBy = "score", Order order = Order.DESC)
        {
            //Logger.Log("FETCH PLAYER RATING");
            GetLeaderboardQuery input = new GetLeaderboardQuery(orderBy, order);
            PlayerRatingData data = await DataFetcher.GetPlayerRating(input);

            return data;
        }

        public async void OpenScoped(string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, int showNearest = 5, string includeFields = "", string displayFields = "", WithMe withMe = WithMe.first)
        {
            RatingData data = await FetchScoped(idOrTag, variant, order, limit, showNearest, includeFields, withMe);
            OverlayCanvas.Controller.OpenLeaderboard(data, OnScopedOpen, OnScopedClose);
        }

        public async void SimpleFetchScoped(
            string tag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "",
            WithMe withMe = WithMe.none)

        {
            RatingData data = await FetchScoped(tag, variant, order, limit, showNearest, includeFields, withMe);
            if (data == null)
            {
                OnScopedFetchError?.Invoke();
                return;
            }

            ActionInvoke(OnScopedFetchSuccess, tag, data.players);
            ActionInvoke(OnScopedFetchTopPlayers, tag, data.topPlayers);
            ActionInvoke(OnScopedFetchAbovePlayers, tag, data.abovePlayers);
            ActionInvoke(OnScopedFetchBelowPlayers, tag, data.belowPlayers);
            ActionInvoke(OnScopedFetchPlayer, tag, data.player);

            GP_Data gpData = new GP_Data(JsonConvert.SerializeObject(data.players, Formatting.Indented));
            OnScopedFetchTagVariant?.Invoke(tag, variant, gpData);
        }

        public async Task<RatingData> FetchScoped(string idOrTag = "", string variant = "some_variant", Order order = Order.DESC, int limit = 10, int showNearest = 5, string includeFields = "", WithMe withMe = WithMe.none)
        {
            //Logger.Log("FETCH");
            GetLeaderboardVariantQuery input = new GetLeaderboardVariantQuery(idOrTag, variant, order, limit, showNearest, includeFields);
            AllRatingData data = await DataFetcher.GetRatingVariant(input, withMe: true);
            if (data == null) return null;

            ProcessLeaderboardResult(data.ratingData, data.playerRatingData, showNearest, withMe.ToString(), limit);

            return data.ratingData;
        }

        public async void SimpleFetchScopedPlayerRating(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "")
        {
            PlayerRatingData data = await FetchScopedPlayerRating(idOrTag, variant, order, limit, showNearest, includeFields);
            if (data == null)
            {
                OnScopedFetchPlayerRatingError?.Invoke();
                return;
            }

            int playerPosition = 0;

            if (data.player.TryGetValue("position", out var positionValue))
            {
                if (positionValue is int position)
                    playerPosition = position;
                else if (positionValue is long longPosition)
                    playerPosition = (int)longPosition;
                else if (positionValue is string positionString && int.TryParse(positionString, out int parsedPosition))
                    playerPosition = parsedPosition;
            }

            OnScopedFetchPlayerRating?.Invoke(idOrTag, playerPosition);
            OnScopedFetchPlayerRatingTagVariant?.Invoke(idOrTag, variant, playerPosition);
        }

        public async Task<PlayerRatingData> FetchScopedPlayerRating(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = "")
        {
            GetLeaderboardVariantQuery input = new GetLeaderboardVariantQuery(idOrTag, variant, order, limit, showNearest, includeFields);
            PlayerRatingData data = await DataFetcher.GetPlayerRatingVariant(input);

            return data;
        }

        public async void PublishRecord(string idOrTag = "", string variant = "some_variant", bool Override = true, Dictionary<string, object> record = null)
        {
            //Logger.Log("PUBLICH RECORD");
            PlayerRecordData data = await PublishRecordReturn(idOrTag, variant, Override, record);
            if(data == null)
            {
                OnScopedPublishRecordError?.Invoke();
                return;
            }

            OnScopedPublishRecordComplete?.Invoke();
        }

        public async Task<PlayerRecordData> PublishRecordReturn(string idOrTag = "", string variant = "some_variant", bool Override = true, Dictionary<string, object> record = null)
        {
            //Logger.Log("PUBLICH RECORD");
            PublishRecordQuery input = new PublishRecordQuery(idOrTag, variant, Override, record);
            PlayerRecordData data = await DataFetcher.PublishRecord(input);

            return data;
        }

        #region Utils


        public static void ProcessLeaderboardResult(
            RatingData result,
            PlayerRatingData playerResult,
            int showNearest,
            string withMe,
            int limit)
        {
            result.countOfPlayersAbove = GetSizeOfUniquePlayers(result.players, playerResult?.abovePlayers ?? new List<Dictionary<string, object>>());
            result.topPlayers = new List<Dictionary<string, object>>(result.players);
            result.abovePlayers = new List<Dictionary<string, object>>(playerResult?.abovePlayers ?? new List<Dictionary<string, object>>());
            result.belowPlayers = new List<Dictionary<string, object>>(playerResult?.belowPlayers ?? new List<Dictionary<string, object>>());
            result.player = playerResult?.player;

            result.players = PlayersMixMe(
                result.players,
                GetWithMeValue(showNearest, withMe),
                GetLimitValue(limit, result.leaderboard),
                playerResult?.player,
                playerResult?.abovePlayers,
                playerResult?.belowPlayers,
                GetShowNearestValue(showNearest)
            );
        }

        public static List<Dictionary<string, object>> PlayersMixMe(
            List<Dictionary<string, object>> players,
            string withMe,
            int limit,
            Dictionary<string, object> myPlayer = null,
            List<Dictionary<string, object>> abovePlayers = null,
            List<Dictionary<string, object>> belowPlayers = null,
            int? showNearest = null)
        {
            if (myPlayer == null || string.IsNullOrEmpty(withMe) || withMe == "none")
            {
                return players;
            }

            if (players.Any(p => (int)p["id"] == (int)myPlayer["id"] && (int)p["position"] == (int)myPlayer["position"]))
            {
                return players.Select(p => (int)p["id"] == (int)myPlayer["id"] ? myPlayer : p).ToList();
            }

            int topLength = players.Count;
            int myIndex = (int)myPlayer["position"] - 1;

            // Adjust positions if myPlayer is cached in the top but position has shifted out of top
            if (myIndex > topLength)
            {
                int myCachedIndex = players.FindIndex(p => (int)p["id"] == (int)myPlayer["id"]);
                if (myCachedIndex >= 0)
                {
                    for (int i = myCachedIndex; i < players.Count; i++)
                    {
                        players[i]["position"] = (int)players[i]["position"] - 1;
                    }
                }
            }

            if (abovePlayers != null && abovePlayers.Count > 0 && myIndex > limit - 1)
            {
                players = abovePlayers.Concat(players).ToList();
            }

            if (belowPlayers != null && belowPlayers.Count > 0 && myIndex > limit - 1)
            {
                players = players.Concat(belowPlayers).ToList();
            }

            if (myIndex < topLength)
            {
                players.Insert(myIndex, myPlayer);
                for (int i = myIndex + 1; i < players.Count; i++)
                {
                    players[i]["position"] = (int)players[i]["position"] + 1;
                }

                if (players.Count > topLength)
                {
                    players.RemoveAt(players.Count - 1);
                }
            }
            else
            {
                switch (withMe)
                {
                    case "first":
                        players.Insert(0, myPlayer);
                        break;
                    case "last":
                        players.Add(myPlayer);
                        break;
                }
            }

            players = players
                .GroupBy(p => (int)p["id"])
                .Select(group => group.First())
                .ToList();

            if (showNearest.HasValue)
            {
                players = players.OrderBy(p => (int)p["position"]).ToList();
            }

            return players;
        }


        public static string GetWithMeValue(int showNearest, string withMe)
        {
            return showNearest > 0 && (string.IsNullOrEmpty(withMe) || withMe == "none") ? "last" : withMe;
        }

        public static int GetShowNearestValue(int showNearest)
        {
            return Math.Min(showNearest, 10);
        }

        public static int GetLimitValue(int limit, Leaderboard leaderboard)
        {
            return limit > 0 ? limit : leaderboard.limit ?? 0;
        }

        public static int GetSizeOfUniquePlayers(List<Dictionary<string, object>> players, List<Dictionary<string, object>> nearestPlayers)
        {
            var uniquePlayers = new HashSet<int>();

            foreach (var player in players.Concat(nearestPlayers))
            {
                if (player.TryGetValue("id", out var idValue) && idValue is int id)
                {
                    uniquePlayers.Add(id);
                }
            }

            foreach (var player in players)
            {
                if (player.TryGetValue("id", out var idValue) && idValue is int id)
                {
                    uniquePlayers.Remove(id);
                }
            }

            return uniquePlayers.Count;
        }


        #endregion
    }
}
