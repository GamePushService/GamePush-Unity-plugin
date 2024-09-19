using System.Runtime.InteropServices;
using UnityEngine;

namespace GamePush
{
    public class GP_Platform : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Platform);

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
        private static string BEELINE = "BEELINE";
        private static string APP_GALLERY = "APP_GALLERY";
        private static string GALAXY_STORE = "GALAXY_STORE";
        private static string ONE_STORE = "ONE_STORE";
        private static string AMAZON_APPSTORE = "AMAZON_APPSTORE";
        private static string XIAOMI_GETAPPS = "XIAOMI_GETAPPS";
        private static string APTOIDE = "APTOIDE";
        private static string RUSTORE = "RUSTORE";
        private static string ANDROID = "ANDROID";


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

        [DllImport("__Internal")]
        private static extern string GP_Platform_Tag();
        public static string Tag()
        {
            if(Type() != Platform.CUSTOM)
                return "";
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_Tag();
#else

            return "Editor";
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
        private static extern string GP_Platform_IsLogoutAvailable();
        public static bool IsLogoutAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsLogoutAvailable() == "true";
#else
            bool value = GP_Settings.instance.GetFromPlatformSettings().IsLogoutAvailable;

            ConsoleLog("Is Logout Available: " + value.ToString());
            return value;
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

        [DllImport("__Internal")]
        private static extern string GP_Platform_IsSecretCodeAuthAvailable();
        public static bool IsSecretCodeAuthAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsSecretCodeAuthAvailable() == "true";
#else
            bool value = GP_Settings.instance.GetFromPlatformSettings().IsSecretCodeAuthAvailable;

            ConsoleLog("Is SecretCode Auth Available: " + value.ToString());
            return value;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_Platform_IsSupportsCloudSaves();
        public static bool IsSupportsCloudSaves()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsSupportsCloudSaves() == "true";
#else
            bool value = GP_Settings.instance.GetFromPlatformSettings().IsSupportsCloudSaves;

            ConsoleLog("Is Supports Cloud Saves: " + value.ToString());
            return value;
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

            if (platform == BEELINE)
                return Platform.BEELINE;

            if (platform == APP_GALLERY)
                return Platform.APP_GALLERY;

            if (platform == GALAXY_STORE)
                return Platform.GALAXY_STORE;

            if (platform == ONE_STORE)
                return Platform.ONE_STORE;

            if (platform == AMAZON_APPSTORE)
                return Platform.AMAZON_APPSTORE;

            if (platform == XIAOMI_GETAPPS)
                return Platform.XIAOMI_GETAPPS;

            if (platform == XIAOMI_GETAPPS)
                return Platform.XIAOMI_GETAPPS;

            if (platform == APTOIDE)
                return Platform.APTOIDE;

            if (platform == RUSTORE)
                return Platform.RUSTORE;

            if (platform == ANDROID)
                return Platform.ANDROID;

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
        CUSTOM = 15,
        BEELINE = 16,
        APP_GALLERY = 17,
        GALAXY_STORE = 18,
        ONE_STORE = 19,
        AMAZON_APPSTORE = 20,
        XIAOMI_GETAPPS = 21,
        APTOIDE = 22,
        RUSTORE = 23,
        ANDROID = 24
    }
}

