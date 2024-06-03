using System.Collections.Generic;
using UnityEngine;

using GamePush;
using Examples.Console;
using UnityEngine.UI;

namespace Examples.Players
{
    [System.Serializable]
    public class PlayersData
    {
        public PlayerState state;
        public PlayerAchievements[] achievements;
        public PlayerPurchases[] purchasesList;
    }

    [System.Serializable]
    public class PlayerState
    {
        // Добавлять глобальные значения которые создали в панели "Игроки" на сайте GamePush например public bool isOnline;
        public string avatar;
        public string credentials;
        public int id;
        public string name;
        public string platformType;
        public int projectId;
        public int score;
    }

    [System.Serializable]
    public class PlayerAchievements
    {
        public int id;
        public string tag;
        public string createdAt;
    }

    [System.Serializable]
    public class PlayerPurchases
    {
        public int productId;
        public string createdAt;
        public string expiredAt;
        public bool subscribed;
        public bool gift;
        public string tag;
    }

    [System.Serializable]
    public class LeaderBoardFetchData
    {
        public string avatar;
        public int id;
        public int score;
        public string name;
        public int position;
    }


    public class Players : MonoBehaviour
    {
        // Один из примеров как получить данные игроков

        // 1 - Фетчим данные лидерборда 
        // 2 - Полуаем данные игроков с лидерборда, нам нужны только ID игроков в виде массива
        // 3 - Фетчим данные игроков указав полученный массив ID

        [SerializeField] private Button _fetchButton;

        private List<int> _playersId = new List<int>();

        private void OnEnable()
        {
            _fetchButton.onClick.AddListener(FetchLeaderboard);

            GP_Leaderboard.OnFetchSuccess += OnLeaderboardFetch;
        }
        private void OnDisable()
        {
            _fetchButton.onClick.RemoveListener(FetchLeaderboard);

            GP_Leaderboard.OnFetchSuccess -= OnLeaderboardFetch;
        }


        // -= 1 =-
        public void FetchLeaderboard() => GP_Leaderboard.Fetch("Level - Gold", "level,gold", Order.DESC, 15);

        // -= 2 =-
        private void OnLeaderboardFetch(string tag, GP_Data data)
        {
            var leaderboardData = data.GetList<LeaderBoardFetchData>();

            for (int i = 0; i < leaderboardData.Count; i++)
            {
                _playersId.Add(leaderboardData[i].id);
            }

            // -= 3 =-
            FetchPlayers(_playersId);
        }

        // -= 3 =-
        private void FetchPlayers(List<int> playersId)
        {
            GP_Players.Fetch(playersId, OnPlayersFetchSuccess, OnPlayersFetchError);
        }


        private void OnPlayersFetchSuccess(GP_Data data)
        {
            var players = data.GetList<PlayersData>();

            for (int i = 0; i < players.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("PLAYER STATE: AVATAR " + players[i].state.avatar);
                ConsoleUI.Instance.Log("PLAYER STATE: CREDITIALS " + players[i].state.credentials);
                ConsoleUI.Instance.Log("PLAYER STATE: ID " + players[i].state.id);
                ConsoleUI.Instance.Log("PLAYER STATE: NAME " + players[i].state.name);
                ConsoleUI.Instance.Log("PLAYER STATE: PLATFORM TYPE " + players[i].state.platformType);
                ConsoleUI.Instance.Log("PLAYER STATE: PROJECT ID " + players[i].state.projectId);
                ConsoleUI.Instance.Log("PLAYER STATE: SCORE " + players[i].state.score);

                ConsoleUI.Instance.Log(" ");

                for (int a = 0; a < players[i].achievements.Length; a++)
                {
                    ConsoleUI.Instance.Log("PLAYER ACHIEVEMENTS: ID " + players[i].achievements[a].id);
                    ConsoleUI.Instance.Log("PLAYER ACHIEVEMENTS: TAG " + players[i].achievements[a].tag);
                    ConsoleUI.Instance.Log("PLAYER ACHIEVEMENTS: CREATED AT " + players[i].achievements[a].createdAt);
                }

                ConsoleUI.Instance.Log(" ");

                for (int x = 0; x < players[i].purchasesList.Length; x++)
                {
                    ConsoleUI.Instance.Log("PLAYER PURCHASES: CREATED AT " + players[i].purchasesList[x].createdAt);
                    ConsoleUI.Instance.Log("PLAYER PURCHASES: EXPIRED AT " + players[i].purchasesList[x].expiredAt);
                    ConsoleUI.Instance.Log("PLAYER PURCHASES: GIFT " + players[i].purchasesList[x].gift);
                    ConsoleUI.Instance.Log("PLAYER PURCHASES: PRODUCT ID " + players[i].purchasesList[x].productId);
                    ConsoleUI.Instance.Log("PLAYER PURCHASES: SUBSCRIBED " + players[i].purchasesList[x].subscribed);
                    ConsoleUI.Instance.Log("PLAYER PURCHASES: TAG " + players[i].purchasesList[x].tag);
                }

                ConsoleUI.Instance.Log("-- -- --");
            }
        }
        private void OnPlayersFetchError() => ConsoleUI.Instance.Log("PLAYERS FETCH: ERROR");
    }
}