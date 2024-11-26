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
        public string[] orderBy { get; set; }
        public int? limit { get; set; }
        public string[] includeFields { get; set; }
        public string[] displayFields { get; set; }
        public string order { get; set; } // "ASC" or "DESC"
        public string withMe { get; set; } // "none", "first", or "last"
        public int? showNearest { get; set; }
    }

    public class GetLeaderboardVariantQuery
    {
        public string variant { get; set; }
        public int? id { get; set; }
        public string tag { get; set; }
        public int? limit { get; set; }
        public string[] includeFields { get; set; }
        public string[] displayFields { get; set; }
        public string withMe { get; set; } // "none", "first", or "last"
        public string order { get; set; } // "ASC" or "DESC"
        public int? showNearest { get; set; }
    }

    public class PublishRecordQuery
    {
        public string variant { get; set; }
        public Dictionary<string, object> record { get; set; }
        public bool? overrideFlag { get; set; }
        public int? id { get; set; }
        public string tag { get; set; }
    }

    public class GetPlayerRatingQuery
    {
        public string[] orderBy { get; set; }
        public int? limit { get; set; }
        public string[] includeFields { get; set; }
        public string[] displayFields { get; set; }
        public string order { get; set; } // "ASC" or "DESC"
        public int? showNearest { get; set; }
    }

    public class GetPlayerRatingVariantQuery
    {
        public string variant { get; set; }
        public int? id { get; set; }
        public string tag { get; set; }
        public int? limit { get; set; }
        public string[] includeFields { get; set; }
        public string[] displayFields { get; set; }
        public string order { get; set; } // "ASC" or "DESC"
        public int? showNearest { get; set; }
    }

    public class Leaderboard
    {
        public string name { get; set; }
        public string description { get; set; }
        public string shareText { get; set; }
        public bool isAuthorizedOnly { get; set; }
        public int? limit { get; set; }
    }

    public class RatingData
    {
        public Leaderboard leaderboard { get; set; }
        public List<PlayerState> players { get; set; }
        public List<PlayerState> topPlayers { get; set; }
        public List<PlayerState> abovePlayers { get; set; }
        public List<PlayerState> belowPlayers { get; set; }
        public PlayerState player { get; set; }
        public List<PlayerField> fields { get; set; }
        public int? countOfPlayersAbove { get; set; }
    }

    public class PlayerRatingData
    {
        public PlayerState player { get; set; }
        public List<PlayerField> fields { get; set; }
        public List<PlayerState> abovePlayers { get; set; }
        public List<PlayerState> belowPlayers { get; set; }
    }

    public class PlayerRecordData
    {
        public Dictionary<string, object> record { get; set; }
        public List<PlayerField> fields { get; set; }
    }

    public class FetchPlayerFieldsOutput
    {
        public List<PlayerField> items { get; set; }
    }

}