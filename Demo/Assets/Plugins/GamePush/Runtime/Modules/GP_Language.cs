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

        private void OnEnable()
        {
            CoreSDK.Language.OnChangeLanguage += (Language lang) => CallChangeLanguageEnum(lang);
        }

        private void CallChangeLanguageEnum(Language lang)
        {
            _onChangeLanguage?.Invoke(lang);
            OnChangeLanguage?.Invoke(lang);
        }

        private void CallChangeLanguage(string lang)
        {
            _onChangeLanguage?.Invoke(LanguageTypes.ConvertToEnum(lang));
            OnChangeLanguage?.Invoke(LanguageTypes.ConvertToEnum(lang));
        }

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
            return CoreSDK.Language.Current();
#endif
        }

        public static string CurrentISO()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Current_Language();
#else
            return CoreSDK.Language.CurrentISO();
#endif
        }

        public static void Change(Language lang, Action<Language> onLanguageChange = null)
        {
            _onChangeLanguage = onLanguageChange;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_ChangeLanguage(LanguageTypes.ConvertToString(lang));
#else
            CoreSDK.Language.Change(lang);
#endif
        }

        public static void Change(string lang, Action<Language> onLanguageChange = null)
        {
            _onChangeLanguage = onLanguageChange;
#if !UNITY_EDITOR && UNITY_WEBGL
            GP_ChangeLanguage(lang);
#else
            CoreSDK.Language.Change(lang);
#endif
        }
       
    }
}