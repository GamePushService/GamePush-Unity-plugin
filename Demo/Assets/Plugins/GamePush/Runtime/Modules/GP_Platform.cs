using System.Runtime.InteropServices;
using UnityEngine;

namespace GamePush
{
    public class GP_Platform : GP_Module
    {
        private void OnValidate() => SetModuleName(ModuleName.Platform);

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
        private static string WG_PLAYGROUND = "WG_PLAYGROUND";
        private static string KONGREGATE = "KONGREGATE";
        private static string GOOGLE_PLAY = "GOOGLE_PLAY";
        private static string PLAYDECK = "PLAYDECK";
        private static string CUSTOM = "CUSTOM";


        [DllImport("__Internal")]
        private static extern string GP_Platform_Type();
        public static Platform Type()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return ConvertToEnum(GP_Platform_Type());
#else
            Platform platform = GP_Settings.instance.GetFromPlatformSettings().PlatformToEmulate;

            ConsoleLog("TYPE: " + platform.ToString());
            return platform;
#endif
        }

        public static string TypeAsString()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_Type();
#else
            Platform platform = GP_Settings.instance.GetFromPlatformSettings().PlatformToEmulate;

            ConsoleLog("TYPE: " + platform.ToString());
            return platform.ToString();
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Platform_HasIntegratedAuth();
        public static bool HasIntegratedAuth()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_HasIntegratedAuth() == "true";
#else
            bool auth = GP_Settings.instance.GetFromPlatformSettings().HasIntegratedAuth;

            ConsoleLog("HAS INTEGRATED AUTH: " + auth.ToString());
            return auth;
#endif
        }


        [DllImport("__Internal")]
        private static extern string GP_Platform_IsExternalLinksAllowed();
        public static bool IsExternalLinksAllowed()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsExternalLinksAllowed() == "true";
#else
            bool linkAllow = GP_Settings.instance.GetFromPlatformSettings().IsExternalLinksAllowed;

            ConsoleLog("IS EXTERNAL LINKS ALLOWED: " + linkAllow.ToString());
            return linkAllow;
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

            if (platform == WG_PLAYGROUND)
                return Platform.WG_PLAYGROUND;

            if (platform == KONGREGATE)
                return Platform.KONGREGATE;

            if (platform == GOOGLE_PLAY)
                return Platform.GOOGLE_PLAY;

            if (platform == PLAYDECK)
                return Platform.PLAYDECK;

            if (platform == CUSTOM)
                return Platform.CUSTOM;

            return Platform.None;
        }

    }

    public enum Platform : byte
    {
        None = 0,
        YANDEX = 1,
        VK = 2,
        CRAZY_GAMES = 3,
        GAME_DISTRIBUTION = 4,
        GAME_MONETIZE = 5,
        OK = 6,
        SMARTMARKET = 7,
        GAMEPIX = 8,
        POKI = 9,
        VK_PLAY = 10,
        WG_PLAYGROUND = 11,
        KONGREGATE = 12,
        GOOGLE_PLAY = 13,
        PLAYDECK = 14,
        CUSTOM = 15
    }
}

