using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GamePush.Config
{
    public static class Headers
    {
        private const string
            X_Transaction_Token = "X-Transaction-Token",
            X_Platform = "X-Platform",
            X_Platform_Key = "X-Platform-Key",
            X_Project_ID = "X-Project-ID",
            X_Project_Token = "X-Project-Token",
            X_Language = "X-Language",
            X_Player_Data = "X-Player-Data";


        public static Dictionary<string, string> GetConfigHeaders()
        {
            string hash = Hash.GetQueryHash(null);

            return new Dictionary<string, string>()
            {
            { X_Transaction_Token, hash},
            { X_Platform, "NONE" },
            { X_Platform_Key, "" },
            { X_Project_ID, CoreSDK.projectId.ToString() },
            { X_Project_Token, CoreSDK.projectToken.ToString() },
            { X_Language, "en" },
            { X_Player_Data, "" },
            };
        }
    }
}
