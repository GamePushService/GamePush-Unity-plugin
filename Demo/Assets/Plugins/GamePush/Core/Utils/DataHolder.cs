using System;
using UnityEngine;

namespace GamePush.Core
{
    public static class DataHolder
    {
        private static string _secretCode;

        private static string SECRETCODE_SAVE_KEY = "secretCode";
        private static string ID_SAVE_KEY = "playerID";
        private static string SAVE_MODIFICATOR = "xSaveState_";


        private static string GetSaveKey(string key) => SAVE_MODIFICATOR + key;


        public static void SetPlayerID(int id)
        {
            PlayerPrefs.SetInt(GetSaveKey(ID_SAVE_KEY), id);
        }

        public static int GetPlayerID()
        {
            if (PlayerPrefs.HasKey(GetSaveKey(ID_SAVE_KEY)))
            {
                return PlayerPrefs.GetInt(GetSaveKey(ID_SAVE_KEY));
            }

            return 0;
        }

        public static void SetSecretCode(string code)
        {
            PlayerPrefs.SetString(GetSaveKey(SECRETCODE_SAVE_KEY), code);
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
            PlayerPrefs.SetString(GetSaveKey(SECRETCODE_SAVE_KEY), "");
        }

        public static string GetSavedSecretCode()
        {
            if (PlayerPrefs.HasKey(GetSaveKey(SECRETCODE_SAVE_KEY)))
            {
                return PlayerPrefs.GetString(GetSaveKey(SECRETCODE_SAVE_KEY));
            }

            return "";
        }
    }
}
