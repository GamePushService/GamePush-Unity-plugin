using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Leaderboard
{
    [System.Serializable]
    public class LeaderboardFetchData
    {
        public string avatar;
        public int id;
        public int score;
        public string name;
        public int position;

        public int gold;
        public int level;
    }

    public class Leaderboard : MonoBehaviour
    {
        [SerializeField] private Button _openButton;
        [SerializeField] private Button _fetchButton;
        [SerializeField] private Button _fetchPlayerRatingButton;
        [Space(40)]
        [SerializeField] private TMP_InputField _tagInput;
        [SerializeField] private TMP_InputField _orderByInput;
        [Space]
        [SerializeField] private TMP_Dropdown _orderDropdown;
        [SerializeField] private TMP_Dropdown _withMeDropdown;
        [Space]
        [SerializeField] private TMP_InputField _limitInput;
        [SerializeField] private TMP_InputField _nearestInput;
        [Space]
        [SerializeField] private TMP_InputField _includeInput;
        [SerializeField] private TMP_InputField _displayInput;



        private void OnEnable()
        {
            _openButton.onClick.AddListener(Open);
            _fetchButton.onClick.AddListener(Fetch);
            _fetchPlayerRatingButton.onClick.AddListener(FetchPlayerRating);

            GP_Leaderboard.OnLeaderboardOpen += OnOpen;
            GP_Leaderboard.OnLeaderboardClose += OnClose;

            GP_Leaderboard.OnFetchSuccess += OnFetchSuccess;
            GP_Leaderboard.OnFetchPlayerRatingSuccess += OnFetchPlayerRatingSuccess;
        }

        private void OnDisable()
        {
            _openButton.onClick.RemoveListener(Open);
            _fetchButton.onClick.RemoveListener(Fetch);
            _fetchPlayerRatingButton.onClick.RemoveListener(FetchPlayerRating);

            GP_Leaderboard.OnLeaderboardOpen -= OnOpen;
            GP_Leaderboard.OnLeaderboardClose -= OnClose;

            GP_Leaderboard.OnFetchSuccess -= OnFetchSuccess;
            GP_Leaderboard.OnFetchPlayerRatingSuccess -= OnFetchPlayerRatingSuccess;
        }


        public void Open() =>
            GP_Leaderboard.Open(
                _orderByInput.text,
                (Order)_orderDropdown.value,
                int.Parse(_limitInput.text),
                int.Parse(_nearestInput.text),
                (WithMe)_withMeDropdown.value,
                _includeInput.text,
                _displayInput.text
            );
        

        public void Fetch() =>
            GP_Leaderboard.Fetch(
                _tagInput.text,
                _orderByInput.text,
                (Order)_orderDropdown.value,
                int.Parse(_limitInput.text),
                int.Parse(_nearestInput.text),
                (WithMe)_withMeDropdown.value,
                _includeInput.text
                );

        public void FetchPlayerRating() =>
            GP_Leaderboard.FetchPlayerRating(
                            _tagInput.text,
                            _orderByInput.text,
                            (Order)_orderDropdown.value
                           );
        
            


        private void OnOpen() => ConsoleUI.Instance.Log("LEADERBOARD: ON OPEN");
        private void OnClose() => ConsoleUI.Instance.Log("LEADERBOARD: ON CLOSE");

        private void OnFetchSuccess(string fetchTag, GP_Data data)
        {
            var players = data.GetList<LeaderboardFetchData>();

            for (int i = 0; i < players.Count; i++)
            {
                ConsoleUI.Instance.Log("PLAYER: AVATAR: " + players[i].avatar);
                ConsoleUI.Instance.Log("PLAYER: ID: " + players[i].id);
                ConsoleUI.Instance.Log("PLAYER: SCORE: " + players[i].score);
                ConsoleUI.Instance.Log("PLAYER: NAME: " + players[i].name);
                ConsoleUI.Instance.Log("PLAYER: POSITION: " + players[i].position);
                ConsoleUI.Instance.Log("PLAYER: GOLD: " + players[i].gold);
                ConsoleUI.Instance.Log("PLAYER: LEVEL: " + players[i].level);
            }
        }

        private void OnFetchPlayerRatingSuccess(string fetchTag, int position) =>
            ConsoleUI.Instance.Log("LEADERBOARD: " + fetchTag + " PLAYER POSITION: " + position);
        
    }
}