using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;
using Examples.Console;

namespace Examples.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private TMP_Text _id;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _playerAvatar;

        [Space(15)]
        [SerializeField] private TMP_Text _gold;
        [SerializeField] private TMP_Text _level;
        [SerializeField] private Toggle _vip;

        [Space(15)]
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _setButton;
        [SerializeField] private Button _saveButton;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private Button _statsButton;


        private void OnEnable()
        {
            GP_Player.OnConnect += OnConnect;

            _addButton.onClick.AddListener(Add);
            _setButton.onClick.AddListener(Set);
            _saveButton.onClick.AddListener(Save);
            _loginButton.onClick.AddListener(Login);
            _resetButton.onClick.AddListener(ResetPlayer);
            _removeButton.onClick.AddListener(Remove);
            _statsButton.onClick.AddListener(PlayerStats);
        }



        private void OnDisable()
        {
            GP_Player.OnConnect -= OnConnect;

            _addButton.onClick.RemoveListener(Add);
            _setButton.onClick.RemoveListener(Set);
            _saveButton.onClick.RemoveListener(Save);
            _loginButton.onClick.RemoveListener(Login);
            _resetButton.onClick.RemoveListener(ResetPlayer);
            _removeButton.onClick.RemoveListener(Remove);
            _statsButton.onClick.RemoveListener(PlayerStats);
        }


        private void Awake()
        {
            Get_Player_Data();
        }


        private void Get_Player_Data()
        {
            _id.text = "ID: " + GP_Player.GetID();
            _name.text = "NAME: " + GP_Player.GetName();

            // #if !UNITY_EDITOR && UNITY_WEBGL
            //                 GP_Player.GetAvatar(_playerAvatar);
            // #endif

            _gold.text = "SCORE: " + GP_Player.GetScore();
            //_level.text = "LEVEL: " + GP_Player.GetInt("level");
            //_vip.isOn = GP_Player.GetBool("vip");
        }


        #region Button Methods
        public void Add()
        {
            GP_Player.Add("coins", 50);
            UpdateGoldText();
        }
        public void Set()
        {
            GP_Player.Set("level", 10);
            GP_Player.Set("vip", true);
            UpdateLevelText();
        }
        public void Save() => GP_Player.Sync();

        public void ResetPlayer() => GP_Player.ResetPlayer();
        public void Remove() => GP_Player.Remove();
        public void Login() => GP_Player.Login();

        public void PlayerStats()
        {
            ConsoleUI.Instance.Log("PLAYER STATS:");
            ConsoleUI.Instance.Log("Active Days:" + GP_Player.GetActiveDays());
            ConsoleUI.Instance.Log("Active Days Consecutive:" + GP_Player.GetActiveDaysConsecutive());
            ConsoleUI.Instance.Log("Playtime Today:" + GP_Player.GetPlaytimeToday());
            ConsoleUI.Instance.Log("Playtime All:" + GP_Player.GetPlaytimeAll());
        }
        #endregion


        private int _goldCount;
        private void UpdateGoldText() { _goldCount += 50; _gold.text = "SCORE: " + _goldCount; }
        private void UpdateLevelText() => _level.text = "LEVEL: " + 10;

        private void OnConnect()
        {
            ConsoleUI.Instance.Log("Connect");
        }
    }
}