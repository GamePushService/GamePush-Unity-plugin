using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

namespace GamePush
{
    public class GP_Language : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Language);

        public static event UnityAction<Language> OnChangeLanguage;
        private static event Action<Language> _onChangeLanguage;

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_Current_Language();
        [DllImport("__Internal")]
        private static extern void GP_ChangeLanguage(string lang);
#endif
        public static Language Current()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return LanguageTypes.ConvertToEnum(GP_Current_Language());
#else

            ConsoleLog("CURRENT: " + GP_Settings.instance.GetLanguage().ToString());
            return GP_Settings.instance.GetLanguage();
#endif
        }

        public static string CurrentISO()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Current_Language();
#else

            ConsoleLog("CURRENT: " + GP_Settings.instance.GetLanguage().ToString());
            return LanguageTypes.ConvertToString(GP_Settings.instance.GetLanguage());
#endif
        }

        public static void Change(Language lang, Action<Language> onLanguageChange = null)
        {
            _onChangeLanguage = onLanguageChange;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_ChangeLanguage(LanguageTypes.ConvertToString(lang));
#else

            ConsoleLog("CHANGE: " + lang.ToString());
            OnChangeLanguage?.Invoke(lang);
            _onChangeLanguage?.Invoke(lang);
#endif
        }

        public static void Change(string lang, Action<Language> onLanguageChange = null)
        {
            _onChangeLanguage = onLanguageChange;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_ChangeLanguage(lang);
#else

            ConsoleLog("CHANGE: " + lang);
            OnChangeLanguage?.Invoke(LanguageTypes.ConvertToEnum(lang));
            _onChangeLanguage?.Invoke(LanguageTypes.ConvertToEnum(lang));
#endif
        }

        private void CallChangeLanguage(string lang) {
            _onChangeLanguage?.Invoke(LanguageTypes.ConvertToEnum(lang));
            OnChangeLanguage?.Invoke(LanguageTypes.ConvertToEnum(lang));
        }

       
    }
}