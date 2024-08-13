using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush;

using Examples.Console;

namespace Examples.Storage
{
    public class Storage : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _keyInput;
        [SerializeField] private TMP_InputField _valueInput;
        [SerializeField] private TMP_Dropdown _storage;
        [Space]
        [SerializeField] private Button _buttonGet;
        [SerializeField] private Button _buttonSet;
        [SerializeField] private Button _buttonGetGlobal;
        [SerializeField] private Button _buttonSetGlobal;
        [SerializeField] private Button _buttonSetStorage;

        private void OnEnable()
        {
            _buttonGet.onClick.AddListener(Get);
            _buttonSet.onClick.AddListener(Set);
            _buttonGetGlobal.onClick.AddListener(GetGlobal);
            _buttonSetGlobal.onClick.AddListener(SetGlobal);
            _buttonSetStorage.onClick.AddListener(SetStorage);
        }

        private void OnDisable()
        {
            _buttonGet.onClick.RemoveListener(Get);
            _buttonSet.onClick.RemoveListener(Set);
            _buttonGetGlobal.onClick.RemoveListener(GetGlobal);
            _buttonSetGlobal.onClick.RemoveListener(SetGlobal);
            _buttonSetStorage.onClick.RemoveListener(SetStorage);
        }

        public void Get()
        {
            ConsoleUI.Instance.Log($"Get {_keyInput.text}:");
            GP_Storage.Get(_keyInput.text, OnGet);
        }
        public void Set()
        {
            ConsoleUI.Instance.Log($"Set {_keyInput.text}:");
            GP_Storage.Set(_keyInput.text, _valueInput.text, OnSet);
        }
        public void GetGlobal()
        {
            ConsoleUI.Instance.Log($"Get Global {_keyInput.text}:");
            GP_Storage.Get(_keyInput.text, OnGetGlobal);
        }
        public void SetGlobal()
        {
            ConsoleUI.Instance.Log($"Set Global {_keyInput.text}:");
            GP_Storage.Set(_keyInput.text, _valueInput.text, OnSetGlobal);
        }

        public void SetStorage()
        {
            ConsoleUI.Instance.Log($"Set {_keyInput.text}:");
            GP_Storage.SetStorage((GP_Storage.SaveStorage)_storage.value);
        }
       

        private void OnGet(object value) => ConsoleUI.Instance.Log("Get Value: " + value.ToString());
        private void OnSet(object value) => ConsoleUI.Instance.Log("Set Value: " + value.ToString());
        private void OnGetGlobal(object value) => ConsoleUI.Instance.Log("Get Value: " + value.ToString());
        private void OnSetGlobal(object value) => ConsoleUI.Instance.Log("Set Value: " + value.ToString());
    }
}

