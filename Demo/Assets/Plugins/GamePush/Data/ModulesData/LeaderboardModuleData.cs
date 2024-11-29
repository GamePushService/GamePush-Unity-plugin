using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GamePush.Data;

namespace GamePush
{
    public enum Order : byte
    {
        DESC,
        ASC
    }

    public enum WithMe : byte
    {
        none,
        first,
        last
    }
    public class GetLeaderboardQuery
    {
        public string[] orderBy;
        public int? limit;
        public string[] includeFields;
        //public string[] displayFields;
        public string order; // "ASC" or "DESC"
        //public string withMe; // "none", "first", or "last"
        public int? showNearest;

        public GetLeaderboardQuery(
            string orderBy = "score",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 0,
            //WithMe withMe = WithMe.none,
            string includeFields = "")
        {
            this.orderBy = orderBy.Trim().Split(",");
            this.limit = limit;
            this.includeFields = includeFields.Trim().Split(",");
            this.order = order.ToString();
            //this.withMe = "None";//withMe.ToString();
            this.showNearest = showNearest;
        }
    }

    public class GetLeaderboardVariantQuery
    {
        public string variant;
        public int? id;
        public string tag;
        public int? limit;
        public string[] includeFields;
        public string[] displayFields;
        public string withMe; // "none", "first", or "last"
        public string order; // "ASC" or "DESC"
        public int? showNearest;
    }

    public class PublishRecordQuery
    {
        public string variant;
        public Dictionary<string, object> record;
        public bool? overrideFlag;
        public int? id;
        public string tag;
    }

    public class GetPlayerRatingQuery
    {
        public string[] orderBy;
        public int? limit;
        public string[] includeFields;
        public string[] displayFields;
        public string order; // "ASC" or "DESC"
        public int? showNearest;
    }

    public class GetPlayerRatingVariantQuery
    {
        public string variant;
        public int? id;
        public string tag;
        public int? limit;
        public string[] includeFields;
        public string[] displayFields;
        public string order; // "ASC" or "DESC"
        public int? showNearest;
    }

    public class Leaderboard
    {
        public string name;
        public string description;
        public string shareText;
        public bool isAuthorizedOnly;
        public int? limit;
    }

    public class RatingData
    {
        public Leaderboard leaderboard;
        public List<PlayerState> players;
        public List<PlayerState> topPlayers;
        public List<PlayerState> abovePlayers;
        public List<PlayerState> belowPlayers;
        public PlayerState player;
        public List<PlayerField> fields;
        public int? countOfPlayersAbove;
    }

    public class PlayerRatingData
    {
        public PlayerState player;
        public List<PlayerField> fields;
        public List<PlayerState> abovePlayers;
        public List<PlayerState> belowPlayers;
    }

    public class PlayerRecordData
    {
        public Dictionary<string, object> record;
        public List<PlayerField> fields;
    }

    public class FetchPlayerFieldsOutput
    {
        public List<PlayerField> items;
    }

}