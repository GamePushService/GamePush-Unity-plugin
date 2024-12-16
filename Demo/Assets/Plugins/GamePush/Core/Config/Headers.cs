using System.Collections.Generic;
using GamePush.Data;

namespace GamePush.Core
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

        public static Dictionary<string, string> GetHeaders(string hash)
        {
            string base64 = GetBase64();

            //Logger.Log("Platform", GetPlatform());
            //Logger.Log("Lang", GetLang());

            return new Dictionary<string, string>()
            {
            { X_Transaction_Token, hash},
            { X_Platform, GetPlatform() },
            { X_Platform_Key, "" },
            { X_Project_ID, CoreSDK.projectId.ToString() },
            { X_Project_Token, CoreSDK.projectToken.ToString() },
            { X_Language, GetLang() },
            { X_Player_Data, base64 },
            };
        }

        private static string GetPlatform()
        {
#if UNITY_ANDROID
            return ProjectData.BUILD_PLATFORM;
#elif UNITY_EDITOR
            return CoreSDK.targetPlatform == null ? ProjectData.BUILD_PLATFORM : CoreSDK.targetPlatform;
#else
            return PlatformTypes.NONE;
#endif
        }

        private static string GetLang()
        {
            string lang = CoreSDK.currentLang.ToUpper();
            if (lang == "" || lang == null) lang = "EN";
            return lang;
        }

        private static string GetEncodeString(string secret) => $"{{\"secretCode\":\"{secret}\"}}";

        private static string GetBase64()
        {
            string secret = DataHolder.GetSecretCode();
            return Hash.Base64Encode(GetEncodeString(secret));
        }

    }
}
