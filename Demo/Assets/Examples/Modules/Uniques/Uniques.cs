using UnityEngine;
using UnityEngine.UI;
using TMPro;
using GamePush;

using Examples.Console;

namespace Examples.Uniques
{
    public class Uniques : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _tagInput;
        [SerializeField] private TMP_InputField _valueInput;
        [Space]
        [SerializeField] private Button _buttonRegister;
        [SerializeField] private Button _buttonGet;
        [SerializeField] private Button _buttonList;
        [SerializeField] private Button _buttonCheck;
        [SerializeField] private Button _buttonDelete;

        private void OnEnable()
        {
            _buttonRegister.onClick.AddListener(Register);
            _buttonGet.onClick.AddListener(Get);
            _buttonList.onClick.AddListener(List);
            _buttonCheck.onClick.AddListener(Check);
            _buttonDelete.onClick.AddListener(Delete);
        }

        private void OnDisable()
        {
            _buttonRegister.onClick.RemoveListener(Register);
            _buttonGet.onClick.RemoveListener(Get);
            _buttonList.onClick.RemoveListener(List);
            _buttonCheck.onClick.RemoveListener(Check);
            _buttonDelete.onClick.RemoveListener(Delete);
        }

        public void Register()
        {
            ConsoleUI.Instance.Log($"Register {_tagInput.text}, value: {_valueInput.text}");
            GP_Uniques.Register(_tagInput.text, _valueInput.text, OnRegister, OnError);
        }

        private void OnRegister(UniquesData data) => ConsoleUI.Instance.Log("Register: SUCCESS: " + data.tag + ": " + data.value);

        public void Get()
        {
            ConsoleUI.Instance.Log($"Get {_tagInput.text}:");
            UniquesData data = GP_Uniques.Get(_tagInput.text);
            ConsoleUI.Instance.Log($"TAG: {data.tag}, VALUE: {data.value}");
        }

        public void List()
        {
            ConsoleUI.Instance.Log($"Get List:");
            UniquesData[] uniques = GP_Uniques.List();
            foreach(UniquesData data in uniques)
            {
                ConsoleUI.Instance.Log($"TAG: {data.tag}, VALUE: {data.value}");
            }
            
        }

        public void Check()
        {
            ConsoleUI.Instance.Log($"Check {_tagInput.text}, value: {_valueInput.text}");
            GP_Uniques.Check(_tagInput.text, _valueInput.text, OnCheck, OnError);
        }

        private void OnCheck(UniquesData data) => ConsoleUI.Instance.Log("Check: SUCCESS: " + data.tag + ": " + data.value);

        public void Delete()
        {
            ConsoleUI.Instance.Log($"Delete {_tagInput.text}");
            GP_Uniques.Delete(_tagInput.text, OnDelete, OnError);
        }

        private void OnDelete(string tag) => ConsoleUI.Instance.Log("Delete: SUCCESS: " + tag);

        private void OnError(string error) => ConsoleUI.Instance.Log("ERROR: " + error);
    }
}
