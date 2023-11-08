using UnityEngine;
using UnityEngine.UI;

using GamePush;
using Examples.Console;
using System;

namespace Examples.LeaderboardScoped
{
    [System.Serializable]
    public class LeaderboardScopedFetchData
    {
        public string avatar;
        public int id;
        public int score;
        public string name;
        public int position;
        public int level;
    }

    public class LeaderboardScoped : MonoBehaviour
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _publishRecordButton;
        [SerializeField] private Button _fetchPlayerRatingButton;
        [SerializeField] private Button _fetchButton;

        private void OnEnable()
        {
            _openButton.onClick.AddListener(Open);
            _publishRecordButton.onClick.AddListener(PublishRecord);
            _fetchPlayerRatingButton.onClick.AddListener(FetchPlayerRating);
            _fetchButton.onClick.AddListener(Fetch);

            GP_LeaderboardScoped.OnOpen += OnOpen;
            GP_LeaderboardScoped.OnClose += OnClose;

            GP_LeaderboardScoped.OnPublishRecordComplete += OnPublishRecordComplete;
            GP_LeaderboardScoped.OnPublishRecordError += OnPublishRecordError;

            GP_LeaderboardScoped.OnFetchPlayerRating += OnFetchPlayerRating;
            GP_LeaderboardScoped.OnFetchPlayerRatingError += OnFetchPlayerRatingError;

            GP_LeaderboardScoped.OnFetchSuccess += OnFetchSuccess;
            GP_LeaderboardScoped.OnFetchError += OnFetchError;
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(Open);
            _publishRecordButton.onClick.RemoveListener(PublishRecord);
            _fetchPlayerRatingButton.onClick.RemoveListener(FetchPlayerRating);
            _fetchButton.onClick.RemoveListener(Fetch);

            GP_LeaderboardScoped.OnOpen -= OnOpen;
            GP_LeaderboardScoped.OnClose -= OnClose;

            GP_LeaderboardScoped.OnPublishRecordComplete -= OnPublishRecordComplete;
            GP_LeaderboardScoped.OnPublishRecordError -= OnPublishRecordError;

            GP_LeaderboardScoped.OnFetchPlayerRating -= OnFetchPlayerRating;
            GP_LeaderboardScoped.OnFetchPlayerRatingError -= OnFetchPlayerRatingError;

            GP_LeaderboardScoped.OnFetchSuccess -= OnFetchSuccess;
            GP_LeaderboardScoped.OnFetchError -= OnFetchError;
        }


        public void Open() => GP_LeaderboardScoped.Open("BEST_PLAYER", "LEVELS", Order.DESC, 15);
        public void PublishRecord() => GP_LeaderboardScoped.PublishRecord("BEST_PLAYER", "LEVELS", true, "level", 5);
        public void FetchPlayerRating() => GP_LeaderboardScoped.FetchPlayerRating("BEST_PLAYER", "LEVELS");
        public void Fetch() => GP_LeaderboardScoped.Fetch("BEST_PLAYER", "LEVELS", Order.DESC, 15);



        private void OnOpen() => ConsoleUI.Instance.Log("LEADERBOARD SCOPED: ON OPEN");
        private void OnClose() => ConsoleUI.Instance.Log("LEADERBOARD SCOPED: ON CLOSE");

        private void OnPublishRecordComplete() => ConsoleUI.Instance.Log("LEADERBOARD SCOPED: ON PUBLISH RECORD: COMPLETE");
        private void OnPublishRecordError() => ConsoleUI.Instance.Log("LEADERBOARD SCOPED: ON PUBLISH RECORD: ERROR");

        private void OnFetchPlayerRating(string fetchTag, int position) => ConsoleUI.Instance.Log("LEADERBOARD SCOPED: " + fetchTag + ": PLAYER POSITION: " + position);
        private void OnFetchPlayerRatingError() => ConsoleUI.Instance.Log("LEADERBOARD SCOPED: ON FETCH PLAYER RATING: ERROR");

        private void OnFetchSuccess(string fetchTag, GP_Data data)
        {
            var players = data.GetList<LeaderboardScopedFetchData>();

            ConsoleUI.Instance.Log("FETCH TAG: " + fetchTag);

            for (int i = 0; i < players.Count; i++)
            {
                ConsoleUI.Instance.Log(" ");
                ConsoleUI.Instance.Log("PLAYER: AVATAR: " + players[i].avatar);
                ConsoleUI.Instance.Log("PLAYER: ID: " + players[i].id);
                ConsoleUI.Instance.Log("PLAYER: SCORE: " + players[i].score);
                ConsoleUI.Instance.Log("PLAYER: NAME: " + players[i].name);
                ConsoleUI.Instance.Log("PLAYER: POSITION: " + players[i].position);
                ConsoleUI.Instance.Log("PLAYER: LEVEL: " + players[i].level);
                ConsoleUI.Instance.Log(" ");
            }
        }
        private void OnFetchError() => ConsoleUI.Instance.Log("LEADERBOARD SCOPED: ON FETCH: ERROR");


    }
}