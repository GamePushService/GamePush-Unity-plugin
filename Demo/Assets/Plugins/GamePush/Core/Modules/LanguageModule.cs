using System;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using GamePush.Localization;

namespace GamePush.Core
{
    public class LanguageModule
    {
        private string TRANSLATES_PATH = "Translates";

        private string _currentLang = "en";
        private Language _currentLangEnum;
        public LocalizationData localization { get; private set; }

        public event Action<Language> OnChangeLanguage;

        public void Init(string lang)
        {
            SetLang(lang);
        }

        private void SetLang(string lang)
        {
            _currentLang = lang;
            _currentLangEnum = LanguageTypes.ConvertToEnum(lang);

            SetLangData();
        }

        private void SetLang(Language lang)
        {
            _currentLang = LanguageTypes.ConvertToString(lang);
            _currentLangEnum = lang;

            SetLangData();
        }

        private void SetLangData()
        {
            string path = TRANSLATES_PATH + "/" + _currentLang;
            //Debug.Log(path);
            TextAsset jsonFile = Resources.Load<TextAsset>(path);
            //Debug.Log(jsonFile.text);
            localization = JsonConvert.DeserializeObject<LocalizationData>(jsonFile.text);
        }

        public Language Current() => _currentLangEnum;
        public string CurrentISO() => _currentLang;

        public void Change(Language lang)
        {
            SetLang(lang);
            OnChangeLanguage?.Invoke(lang);
        }

        public void Change(string lang)
        {
            SetLang(lang);
            OnChangeLanguage?.Invoke(LanguageTypes.ConvertToEnum(lang));
        }
    }
}
