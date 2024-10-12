using System.Collections.Generic;

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

            return new Dictionary<string, string>()
            {
            { X_Transaction_Token, hash},
            { X_Platform, "ANDROID" },
            { X_Platform_Key, "" },
            { X_Project_ID, CoreSDK.projectId.ToString() },
            { X_Project_Token, CoreSDK.projectToken.ToString() },
            { X_Language, "EN" },
            { X_Player_Data, base64 },
            };
        }

        private static string GetEncodeString(string secret) => $"{{\"secretCode\":\"{secret}\"}}";

        private static string GetBase64()
        {
            string secret = DataHolder.GetSecretCode();
            return Hash.Base64Encode(GetEncodeString(secret));
        }

    }
}
