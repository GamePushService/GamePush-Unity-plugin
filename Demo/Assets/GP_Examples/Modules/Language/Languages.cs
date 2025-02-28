using UnityEngine;
using UnityEngine.UI;
using GamePush;
using TMPro;

using Examples.Console;

namespace Examples.Languages
{
    public class Languages : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _langs;
        [Space]
        [SerializeField] private Button _buttonCurrent;
        [SerializeField] private Button _buttonChange;

        private void OnEnable()
        {
            _buttonCurrent.onClick.AddListener(Current);
            _buttonChange.onClick.AddListener(Change);
        }

        private void OnDisable()
        {
            _buttonCurrent.onClick.RemoveListener(Current);
            _buttonChange.onClick.RemoveListener(Change);
        }

        public void Current()
        {
            string lang = GP_Language.Current().ToString();
            ConsoleUI.Instance.Log($"LANGUAGE : CURRENT: {lang}");
        }

        public void Change()
        {
            Language langToChange = (Language)_langs.value;
            ConsoleUI.Instance.Log($"LANGUAGE : CHANGE TO: {langToChange}");
            GP_Language.Change(langToChange, OnChange);
        }

        private void OnChange(Language language)
        {
            ConsoleUI.Instance.Log("LANGUAGE : ON CHANGE: " + language);
        }
    }
}