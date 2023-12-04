using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Language : MonoBehaviour
    {
        public static event UnityAction<Language> OnChangeLanguage;
        public static event Action<Language> _onChangeLanguage;

        private static string English = "en";
        private static string Russian = "ru";
        private static string Turkish = "tr";
        private static string French = "fr";
        private static string Italian = "it";
        private static string German = "de";
        private static string Spanish = "es";
        private static string Chineese = "zh";
        private static string Portuguese = "pt";
        private static string Korean = "ko";
        private static string Japanese = "ja";
        private static string Arab = "ar";
        private static string Hindi = "hi";
        private static string Indonesian = "id";

        [DllImport("__Internal")]
        private static extern string GP_Current_Language();
        public static Language Current()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return ConvertToEnum(GP_Current_Language());
#else
            if (GP_ConsoleController.Instance.LanguageConsoleLogs)
                Console.Log("LANGUAGE CURRENT: ", Language.English.ToString());
            return Language.English;
#endif
        }

        [DllImport("__Internal")]
        private static extern void GP_ChangeLanguage(string lang);
        public static void Change(Language lang, Action<Language> onLanguageChange = null)
        {
            _onChangeLanguage = onLanguageChange;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_ChangeLanguage(ConvertToString(lang));
#else
            if (GP_ConsoleController.Instance.LanguageConsoleLogs)
                Console.Log("LANGUAGE CHANGE: ", lang.ToString());
            OnChangeLanguage?.Invoke(lang);
            _onChangeLanguage?.Invoke(lang);
#endif
        }

        private void CallChangeLanguage(string lang) { _onChangeLanguage?.Invoke(ConvertToEnum(lang)); OnChangeLanguage?.Invoke(ConvertToEnum(lang)); }

        private static Language ConvertToEnum(string lang)
        {
            if (lang == English)
                return Language.English;

            if (lang == Russian)
                return Language.Russian;

            if (lang == Turkish)
                return Language.Turkish;

            if (lang == French)
                return Language.French;

            if (lang == Italian)
                return Language.Italian;

            if (lang == German)
                return Language.German;

            if (lang == Spanish)
                return Language.Spanish;

            if (lang == Chineese)
                return Language.Chineese;

            if (lang == Portuguese)
                return Language.Portuguese;

            if (lang == Korean)
                return Language.Korean;

            if (lang == Japanese)
                return Language.Japanese;

            if (lang == Arab)
                return Language.Arab;

            if (lang == Hindi)
                return Language.Hindi;

            if (lang == Indonesian)
                return Language.Indonesian;

            return Language.English;
        }

        private static string ConvertToString(Language lang)
        {
            if (lang == Language.English)
                return English;

            if (lang == Language.Russian)
                return Russian;

            if (lang == Language.Turkish)
                return Turkish;

            if (lang == Language.French)
                return French;

            if (lang == Language.Italian)
                return Italian;

            if (lang == Language.German)
                return German;

            if (lang == Language.Spanish)
                return Spanish;

            if (lang == Language.Chineese)
                return Chineese;

            if (lang == Language.Portuguese)
                return Portuguese;

            if (lang == Language.Korean)
                return Korean;

            if (lang == Language.Japanese)
                return Japanese;

            if (lang == Language.Arab)
                return Arab;

            if (lang == Language.Hindi)
                return Hindi;

            if (lang == Language.Indonesian)
                return Indonesian;

            return English;
        }
    }

    public enum Language : byte
    {
        English,
        Russian,
        Turkish,
        French,
        Italian,
        German,
        Spanish,
        Chineese,
        Portuguese,
        Korean,
        Japanese,
        Arab,
        Hindi,
        Indonesian,
    }
}