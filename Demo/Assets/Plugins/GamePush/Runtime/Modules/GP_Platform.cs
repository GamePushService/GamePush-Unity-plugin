using System.Runtime.InteropServices;
using UnityEngine;

using GP_Utilities.Console;

namespace GamePush
{
    public class GP_Platform : MonoBehaviour
    {
        private static string YANDEX = "YANDEX";
        private static string VK = "VK";
        private static string CRAZY_GAMES = "CRAZY_GAMES";
        private static string GAME_DISTRIBUTION = "GAME_DISTRIBUTION";
        private static string GAME_MONETIZE = "GAME_MONETIZE";
        private static string OK = "OK";
        private static string SMARTMARKET = "SMARTMARKET";
        private static string GAMEPIX = "GAMEPIX";
        private static string POKI = "POKI";
        private static string VK_PLAY = "VK_PLAY";

        [DllImport("__Internal")]
        private static extern string GP_Platform_Type();
        public static Platform Type()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return ConvertToEnum(GP_Platform_Type());
#else
            if (GP_ConsoleController.Instance.PlatformConsoleLogs)
                Console.Log("PLATFORM: TYPE: ", Platform.None.ToString());
            return Platform.None;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Platform_HasIntegratedAuth();
        public static bool HasIntegratedAuth()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_HasIntegratedAuth() == "true";
#else
            if (GP_ConsoleController.Instance.PlatformConsoleLogs)
                Console.Log("PLATFORM: HAS INTEGRATED AUTH: ", "TRUE");
            return true;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Platform_IsExternalLinksAllowed();
        public static bool IsExternalLinksAllowed()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsExternalLinksAllowed() == "true";
#else
            if (GP_ConsoleController.Instance.PlatformConsoleLogs)
                Console.Log("PLATFORM: IS EXTERNAL LINKS ALLOWED: ", "TRUE");
            return true;
#endif
        }

        private static Platform ConvertToEnum(string platform)
        {
            if (platform == YANDEX)
                return Platform.YANDEX;

            if (platform == VK)
                return Platform.VK;

            if (platform == CRAZY_GAMES)
                return Platform.CRAZY_GAMES;

            if (platform == GAME_DISTRIBUTION)
                return Platform.GAME_DISTRIBUTION;

            if (platform == GAME_MONETIZE)
                return Platform.GAME_MONETIZE;

            if (platform == OK)
                return Platform.OK;

            if (platform == SMARTMARKET)
                return Platform.SMARTMARKET;

            if (platform == GAMEPIX)
                return Platform.GAMEPIX;

            if (platform == POKI)
                return Platform.POKI;

            if (platform == VK_PLAY)
                return Platform.VK_PLAY;

            return Platform.None;
        }

    }

    public enum Platform : byte
    {
        YANDEX,
        VK,
        CRAZY_GAMES,
        GAME_DISTRIBUTION,
        GAME_MONETIZE,
        OK,
        SMARTMARKET,
        GAMEPIX,
        POKI,
        VK_PLAY,
        None,
    }
}

