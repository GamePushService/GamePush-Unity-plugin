using System;
using UnityEngine;

namespace GamePush.Core
{
    public static class DataHolder
    {
        private static string _secretCode;

        private static string SECRETCODE_STATE_KEY = "secretCode";
        private static string SAVE_MODIFICATOR = "xSaveState_";
        private static string SECRETCODE_SAVE_KEY = SAVE_MODIFICATOR + SECRETCODE_STATE_KEY;

        public static void SetSecretCode(string code)
        {
            PlayerPrefs.SetString(SECRETCODE_SAVE_KEY, code);
            _secretCode = code;
        }

        public static string GetSecretCode()
        {
            if (_secretCode == "")
                return GetSavedSecretCode();
            else
                return _secretCode;
        }

        public static void ResetSecretCode()
        {
            _secretCode = "";
            PlayerPrefs.SetString(SECRETCODE_SAVE_KEY, "");
        }

        public static string GetSavedSecretCode()
        {
            if (PlayerPrefs.HasKey(SECRETCODE_SAVE_KEY))
            {
                return PlayerPrefs.GetString(SECRETCODE_SAVE_KEY);
            }

            return "";
        }
    }
}
