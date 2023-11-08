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


        private void OnEnable()
        {
            GP_Player.OnConnect += OnConnect;

            _addButton.onClick.AddListener(Add);
            _setButton.onClick.AddListener(Set);
            _saveButton.onClick.AddListener(Save);
            _loginButton.onClick.AddListener(Login);
            _resetButton.onClick.AddListener(ResetPlayer);
            _removeButton.onClick.AddListener(Remove);
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

            _gold.text = "GOLD: " + GP_Player.GetInt("GOLD");
            _level.text = "LEVEL: " + GP_Player.GetInt("LEVEL");
            _vip.isOn = GP_Player.GetBool("VIP");
        }


        #region Button Methods
        public void Add()
        {
            GP_Player.Add("GOLD", 50);
            UpdateGoldText();
        }
        public void Set()
        {
            GP_Player.Set("LEVEL", 10);
            GP_Player.Set("VIP", true);
            UpdateLevelText();
        }
        public void Save() => GP_Player.Sync();

        public void ResetPlayer() => GP_Player.ResetPlayer();
        public void Remove() => GP_Player.Remove();
        public void Login() => GP_Player.Login();
        #endregion


        private int _goldCount;
        private void UpdateGoldText() { _goldCount += 50; _gold.text = "GOLD: " + _goldCount; }
        private void UpdateLevelText() => _level.text = "LEVEL: " + 10;

        private void OnConnect()
        {
            ConsoleUI.Instance.Log("Connect");
        }
    }
}