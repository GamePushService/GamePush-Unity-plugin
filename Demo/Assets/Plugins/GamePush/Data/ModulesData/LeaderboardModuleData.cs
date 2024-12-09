using System;
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

    public class GetOpenLeaderboardQuery
    {
        public List<string> orderBy;
        public int limit;
        public List<string> includeFields;
        public List<string> displayFields;
        public string order; // "ASC" or "DESC"
        public string withMe; // "none", "first", or "last"
        public int showNearest;

        public GetOpenLeaderboardQuery(
            string orderBy = "score",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 0,
            WithMe withMe = WithMe.none,
            string includeFields = "",
            string displayFields = "")
        {
            this.orderBy = new List<string>(orderBy.Trim().Split(","));
            this.limit = limit;
            this.includeFields = new List<string>(includeFields.Trim().Split(","));
            this.displayFields = new List<string>(displayFields.Trim().Split(","));
            this.order = order.ToString();
            this.withMe = withMe.ToString();
            this.showNearest = showNearest;
        }
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
            //this.withMe = withMe.ToString();
            this.showNearest = showNearest;
        }
    }

    public class GetPlayerRatingQuery
    {
        public string[] orderBy;
        public int? limit;
        public string[] includeFields;
        public string order;
        public int? showNearest;

        public GetPlayerRatingQuery(
            string orderBy = "score",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 0,
            string includeFields = "")
        {
            this.orderBy = orderBy.Trim().Split(",");
            this.limit = limit;
            this.includeFields = includeFields.Trim().Split(",");
            this.order = order.ToString();
            this.showNearest = showNearest;
        }
    }

    public class GetLeaderboardVariantQueryID : GetLeaderboardVariantQuery
    {
        public int id;

        public GetLeaderboardVariantQueryID(int id)
        {
            this.id = id;
        }
    }

    public class GetLeaderboardVariantQueryTAG : GetLeaderboardVariantQuery
    {
        public string tag;

        public GetLeaderboardVariantQueryTAG(string tag)
        {
            this.tag = tag;
        }
    }

    public class GetLeaderboardVariantQuery
    {
        public string variant;
        //public int? id;
        //public string tag;
        [NonSerialized]
        public string idOrTag;
        public int? limit;
        public string[] includeFields;
        //public string[] displayFields;
        //public string withMe; // "none", "first", or "last"
        public string order; // "ASC" or "DESC"
        public int? showNearest;

        public GetLeaderboardVariantQuery(
            string idOrTag = "",
            string variant = "some_variant",
            Order order = Order.DESC,
            int limit = 10,
            int showNearest = 5,
            string includeFields = ""
         )
        {
            //if (int.TryParse(idOrTag, out int id))
            //    this.id = id;
            //else
            //    this.tag = idOrTag;

            this.idOrTag = idOrTag;

            this.variant = variant;
            this.limit = limit;
            this.includeFields = includeFields.Trim().Split(",");
            this.order = order.ToString();
            this.showNearest = showNearest;
        }
    }



    
    public class PublishRecordQuery
    {
        public string variant;
        public Dictionary<string, string> @record;
        public bool? @override;
        public int? id;
        public string tag;

        public PublishRecordQuery(
            string idOrTag = "",
            string variant = "some_variant",
            bool Override = true,
            Dictionary<string, object> record = null
            )
        {
            if (int.TryParse(idOrTag, out int id))
                this.id = id;
            else
                this.tag = idOrTag;

            this.@record = new Dictionary<string, string>();
            foreach (string key in record.Keys)
            {
                Debug.Log(key);
                this.@record.Add(key, record[key].ToString());
            }

            this.variant = variant;
            @override = Override;
        }
    }

    
    //public class GetPlayerRatingQuery
    //{
    //    public string[] orderBy;
    //    public int? limit;
    //    public string[] includeFields;
    //    public string[] displayFields;
    //    public string order; // "ASC" or "DESC"
    //    public int? showNearest;

    //    public GetPlayerRatingQuery(
    //        string orderBy = "score",
    //        Order order = Order.DESC,
    //        int limit = 10,
    //        int showNearest = 0,
    //        string includeFields = ""
    //        )
    //    {
    //        this.orderBy = orderBy.Trim().Split(",");
    //        this.order = order.ToString();
    //        this.limit = limit;
    //        this.includeFields = includeFields.Trim().Split(",");
    //        this.showNearest = showNearest;
    //    }
    //}

    //public class GetPlayerRatingVariantQuery
    //{
    //    public string variant;
    //    public int? id;
    //    public string tag;
    //    public int? limit;
    //    public string[] includeFields;
    //    public string[] displayFields;
    //    public string order; // "ASC" or "DESC"
    //    public int? showNearest;
    //}

    public class Leaderboard
    {
        public string name;
        public string description;
        public string shareText;
        public bool isAuthorizedOnly;
        public int? limit;
    }

    public class AllRatingData
    {
        public RatingData ratingData;
        public PlayerRatingData playerRatingData;
    }

    [Serializable]
    public class PlayersList
    {
        public Dictionary<string, object>[] players;

        public PlayersList(List<Dictionary<string, object>> players)
        {
            this.players = players.ToArray();
        }
    }

    public class RatingData
    {
        public Leaderboard leaderboard;
        public List<Dictionary<string, object>> players;
        public List<Dictionary<string, object>> topPlayers;
        public List<Dictionary<string, object>> abovePlayers;
        public List<Dictionary<string, object>> belowPlayers;
        public Dictionary<string, object> player;
        public List<PlayerField> fields;
        public int? countOfPlayersAbove;
    }

    public class PlayerRatingData
    {
        public Dictionary<string, object> player;
        public List<PlayerField> fields;
        public List<Dictionary<string, object>> abovePlayers;
        public List<Dictionary<string, object>> belowPlayers;
    }

    [Serializable]
    public class PlayerRatingState
    {
        public int id;
        public string name;
        public string avatar;
        public long score;
        public int position;
        public string fields;

        public static PlayerRatingState FromDictionary(Dictionary<string, object> dict)
        {
            var player = new PlayerRatingState();

            var additionalFields = new List<string>();

            if (dict.TryGetValue("id", out var idValue))
            {
                if (idValue is int id)
                    player.id = id;
                else if (idValue is long longId)
                    player.id = (int)longId;
                else if (idValue is string idString && int.TryParse(idString, out int parsedId))
                    player.id = parsedId;
            }

            if (dict.TryGetValue("score", out var scoreValue))
            {
                if (scoreValue is long score)
                    player.score = (long)score;
                else if (scoreValue is string scoreString && long.TryParse(scoreString, out long parsedScore))
                    player.score = parsedScore;
            }

            if (dict.TryGetValue("position", out var positionValue))
            {
                if (positionValue is int position)
                    player.position = position;
                else if (positionValue is long longPosition)
                    player.position = (int)longPosition;
                else if (positionValue is string positionString && int.TryParse(positionString, out int parsedPosition))
                    player.position = parsedPosition;
            }

            if (dict.TryGetValue("name", out var nameValue) && nameValue is string name)
                player.name = name;

            if (player.name == "")
                player.name = "#" + player.id.ToString();

            if (dict.TryGetValue("avatar", out var avatarValue) && avatarValue is string avatar)
                player.avatar = avatar;


            foreach (var kvp in dict)
            {
                if (kvp.Key == "id" || kvp.Key == "name" || kvp.Key == "avatar" || kvp.Key == "position")
                    continue;

                string valueStr = kvp.Value switch
                {
                    int intValue => intValue.ToString(),
                    string strValue => strValue,
                    bool boolValue => boolValue.ToString(),
                    _ => kvp.Value?.ToString() ?? "null"
                };

                additionalFields.Add($"{kvp.Key} : {valueStr}");
            }

            player.fields = string.Join(", ", additionalFields);

            return player;
        }
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