using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

using GamePush;
using GamePush.Data;
using Examples.Console;

namespace Examples.Player
{
    public class Player : MonoBehaviour
    {
        [SerializeField] private TMP_Text _id;
        [SerializeField] private TMP_Text _name;
        [SerializeField] private Image _playerAvatar;

        [Space(15)]
        [SerializeField] private TMP_InputField _value;
        [SerializeField] private TMP_InputField _key;
        [SerializeField] private Toggle _flag;
        
        [Space(15)]
        [SerializeField] private Button _getIDButton;
        [SerializeField] private Button _getScoreButton;
        [SerializeField] private Button _getNameButton;
        [SerializeField] private Button _getAvatarButton;
        [SerializeField] private Button _isLoggedButton;
        [SerializeField] private Button _hasCredsButton;

        [Space(15)]
        [SerializeField] private Button _getIntButton;
        [SerializeField] private Button _getFloatButton;
        [SerializeField] private Button _getBoolButton;
        [SerializeField] private Button _getStringButton;

        [SerializeField] private Button _setButton;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _hasButton;

        [SerializeField] private Button _syncButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private Button _loginButton;
        [SerializeField] private Button _statsButton;
        [SerializeField] private Button _isStabButton;

        [SerializeField] private Button _fetchButton;


        private void OnEnable()
        {
            GP_Player.OnConnect += OnConnect;
            GP_Player.OnLoadComplete += OnLoad;
            GP_Player.OnSyncComplete += OnSync;
            GP_Player.OnLoginComplete += OnLoginComplete;
            GP_Player.OnLoginError += OnLoginError;

            GP_Player.OnPlayerChange += OnPlayerChange;

            _getIDButton.onClick.AddListener(GetID);
            _getScoreButton.onClick.AddListener(GetScore);
            _getNameButton.onClick.AddListener(GetScore);
            _getAvatarButton.onClick.AddListener(GetAvatarURL);
            _isLoggedButton.onClick.AddListener(IsLoggedIn);
            _hasCredsButton.onClick.AddListener(HasCreds);

            _getIntButton.onClick.AddListener(GetInt);
            _getFloatButton.onClick.AddListener(GetFloat);
            _getBoolButton.onClick.AddListener(GetBool);
            _getStringButton.onClick.AddListener(GetString);

            _addButton.onClick.AddListener(Add);
            _setButton.onClick.AddListener(Set);
            _hasButton.onClick.AddListener(Has);

            _syncButton.onClick.AddListener(Sync);
            _loginButton.onClick.AddListener(Login);
            _resetButton.onClick.AddListener(ResetPlayer);
            _removeButton.onClick.AddListener(Remove);
            _statsButton.onClick.AddListener(PlayerStats);
            _isStabButton.onClick.AddListener(IsStab);

            _fetchButton.onClick.AddListener(FetchFields);
        }



        private void OnDisable()
        {
            GP_Player.OnConnect -= OnConnect;
            GP_Player.OnLoadComplete -= OnLoad;
            GP_Player.OnSyncComplete -= OnSync;
            GP_Player.OnLoginComplete -= OnLoginComplete;
            GP_Player.OnLoginError -= OnLoginError;

            GP_Player.OnPlayerChange -= OnPlayerChange;

            _getIDButton.onClick.RemoveListener(GetID);
            _getScoreButton.onClick.RemoveListener(GetScore);
            _getNameButton.onClick.RemoveListener(GetScore);
            _getAvatarButton.onClick.RemoveListener(GetAvatarURL);
            _isLoggedButton.onClick.RemoveListener(IsLoggedIn);
            _hasCredsButton.onClick.RemoveListener(HasCreds);

            _getIntButton.onClick.RemoveListener(GetInt);
            _getFloatButton.onClick.RemoveListener(GetFloat);
            _getBoolButton.onClick.RemoveListener(GetBool);
            _getStringButton.onClick.RemoveListener(GetString);

            _addButton.onClick.RemoveListener(Add);
            _setButton.onClick.RemoveListener(Set);
            _hasButton.onClick.RemoveListener(Has);

            _syncButton.onClick.RemoveListener(Sync);
            _loginButton.onClick.RemoveListener(Login);
            _resetButton.onClick.RemoveListener(ResetPlayer);
            _removeButton.onClick.RemoveListener(Remove);
            _statsButton.onClick.RemoveListener(PlayerStats);
            _isStabButton.onClick.RemoveListener(IsStab);

            _fetchButton.onClick.RemoveListener(FetchFields);
        }


        private async void Start()
        {
            await GP_Init.Ready;
            Get_Player_Data();
        }


        private void Get_Player_Data()
        {
            _id.text = "ID: " + GP_Player.GetID();
            _name.text = "NAME: " + GP_Player.GetName();

        #if !UNITY_EDITOR && UNITY_WEBGL
            GP_Player.GetAvatar(_playerAvatar);
        #endif

            //_gold.text = "SCORE: " + GP_Player.GetScore();
            //_level.text = "LEVEL: " + GP_Player.GetInt("level");
            //_vip.isOn = GP_Player.GetBool("vip");
        }


        #region Button Methods

        public void GetID() => ConsoleUI.Instance.Log($"\nID: {GP_Player.GetID()}");
        public void GetScore() => ConsoleUI.Instance.Log($"\nScore: {GP_Player.GetScore()}");
        public void GetName() => ConsoleUI.Instance.Log($"\nName: {GP_Player.GetName()}");
        public void GetAvatarURL() => ConsoleUI.Instance.Log($"\nAvatar URL: {GP_Player.GetAvatarUrl()}");
        public void IsLoggedIn() => ConsoleUI.Instance.Log($"\nIs logged in: {GP_Player.IsLoggedIn()}");
        public void HasCreds() => ConsoleUI.Instance.Log($"\nHas any credentials: {GP_Player.HasAnyCredentials()}");

        public void GetInt()
        {
            int value = GP_Player.GetInt(_key.text);
            ConsoleUI.Instance.Log($"\nGet int {_key.text}: {value}");
        }

        public void GetFloat()
        {
            float value = GP_Player.GetFloat(_key.text);
            ConsoleUI.Instance.Log($"\nGet float {_key.text}: {value}");
        }

        public void GetBool()
        {
            bool value = GP_Player.GetBool(_key.text);
            ConsoleUI.Instance.Log($"\nGet bool {_key.text}: {value}");
        }

        public void GetString()
        {
            string value = GP_Player.GetString(_key.text);
            ConsoleUI.Instance.Log($"\nGet string {_key.text}: {value}");
        }
        

        public void Set()
        {
            ConsoleUI.Instance.Log($"\nSet {_key.text}: {_value.text}");
            GP_Player.Set(_key.text, _value.text);
        }

        public void Add()
        {
            ConsoleUI.Instance.Log($"\nAdd {_key.text}: {_value.text}");
            float.TryParse(_value.text, out float value);
            GP_Player.Add(_key.text, value);
        }

        public void Has()
        {
            bool has = GP_Player.Has(_key.text);
            ConsoleUI.Instance.Log($"\nHas {_key.text}: {has}");
        }

        public void Sync()
        {
            ConsoleUI.Instance.Log($"\nSync player");
            GP_Player.Sync();
        }

        public void ResetPlayer()
        {
            ConsoleUI.Instance.Log($"\nReset player");
            GP_Player.ResetPlayer();
        }
        public void Remove()
        {
            ConsoleUI.Instance.Log($"\nRemove player");
            GP_Player.Remove();
        }
        public void Login()
        {
            ConsoleUI.Instance.Log($"\nTry to login");
            GP_Player.Login();
        }

        public void PlayerStats()
        {
            ConsoleUI.Instance.Log("\nPLAYER STATS:");
            ConsoleUI.Instance.Log("Active Days:" + GP_Player.GetActiveDays());
            ConsoleUI.Instance.Log("Active Days Consecutive:" + GP_Player.GetActiveDaysConsecutive());
            ConsoleUI.Instance.Log("Playtime Today:" + GP_Player.GetPlaytimeToday());
            ConsoleUI.Instance.Log("Playtime All:" + GP_Player.GetPlaytimeAll());
        }

        public void IsStab()
        {
            bool stab = GP_Player.IsStub();
            ConsoleUI.Instance.Log($"\nIs Stab: {stab}");
        }

        public void FetchFields()
        {
            ConsoleUI.Instance.Log("\nFetch player fields");
            GP_Player.FetchFields(OnFetchFields);
        }
        #endregion

        private void OnFetchFields(List<PlayerFetchFieldsData> playerFetchFields)
        {
            ConsoleUI.Instance.Log("\nPLAYER FIELDS:");
            foreach (PlayerFetchFieldsData field in playerFetchFields)
            {
                ConsoleUI.Instance.Log($"\nField key: {field.key}");
                ConsoleUI.Instance.Log($"Field name: {field.name}");
                ConsoleUI.Instance.Log($"Field type: {field.type}");
                ConsoleUI.Instance.Log($"Field important: {field.important}");

                ConsoleUI.Instance.Log($"Default value: {field.defaultValue}");
             
                foreach (PlayerFieldVariant variant in field.variants)
                {
                    ConsoleUI.Instance.Log($" variant:");
                    ConsoleUI.Instance.Log($"  name: {variant.name}");
                    ConsoleUI.Instance.Log($"  value: {variant.value}");
                }
               
            }
        }

        private void OnConnect() => ConsoleUI.Instance.Log("Player Connect");

        private void OnLoad() => ConsoleUI.Instance.Log("Player Load");

        private void OnLoginComplete() => ConsoleUI.Instance.Log("Login Complete");
        private void OnLoginError() => ConsoleUI.Instance.Log("Login Error");

        private void OnSync() => ConsoleUI.Instance.Log("Sync Complete");

        private void OnPlayerChange() => ConsoleUI.Instance.Log("Player Change");
    }
}