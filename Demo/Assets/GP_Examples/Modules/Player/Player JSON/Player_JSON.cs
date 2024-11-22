using UnityEngine;
using UnityEngine.UI;
using TMPro;

using GamePush;

namespace Examples.Player.JSON
{
    public class Player_JSON : MonoBehaviour
    {
        [SerializeField] private TMP_Text _gold;
        [SerializeField] private TMP_Text _level;
        [SerializeField] private Toggle _vip;

        [Space(15)]
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _setButton;
        [SerializeField] private Button _saveButton;

        private SaveData _saveData = new SaveData();
        private SaveSystem _saveSystem = new SaveSystem();


        private void OnEnable()
        {
            _addButton.onClick.AddListener(Add);
            _setButton.onClick.AddListener(Set);
            _saveButton.onClick.AddListener(Save);
            _vip.onValueChanged.AddListener(VIP);
        }

        private void OnDisable()
        {
            _addButton.onClick.RemoveListener(Add);
            _setButton.onClick.RemoveListener(Set);
            _saveButton.onClick.RemoveListener(Save);
            _vip.onValueChanged.RemoveListener(VIP);
        }


        private void Awake()
        {
            //Get_Player_Data_JSON();
        }


        private void Get_Player_Data_JSON()
        {
            _saveData = _saveSystem.Load();

            _gold.text = "GOLD: " + _saveData.Gold;
            _level.text = "LEVEL: " + _saveData.Level;
            _vip.isOn = _saveData.VIP;
        }


        public void Add()
        {
            _saveData.Gold += 50;
            UpdateTexts();
        }
        public void Set()
        {
            _saveData.Level = 10;
            UpdateTexts();
        }
        public void VIP(bool value) => _saveData.VIP = value;
        public void Save() => _saveSystem.Save(_saveData);



        private void UpdateTexts()
        {
            _gold.text = "GOLD: " + _saveData.Gold;
            _level.text = "LEVEL: " + _saveData.Level;
        }
    }
}