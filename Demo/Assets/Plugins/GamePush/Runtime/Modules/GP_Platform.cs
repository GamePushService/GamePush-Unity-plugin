﻿using System.Runtime.InteropServices;
using UnityEngine;
using GamePush.Data;

namespace GamePush
{
    public class GP_Platform : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Platform);

        [DllImport("libARWrapper.so")]
        private static extern string GP_Platform_Type();
        public static Platform Type()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return ConvertToEnum(GP_Platform_Type());
#else
            return ConvertToEnum(CoreSDK.platform.type);
#endif
        }

        public static string TypeAsString()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_Type();
#else
            return CoreSDK.platform.type;
#endif
        }

        [DllImport("libARWrapper.so")]
        private static extern string GP_Platform_Tag();
        public static string Tag()
        {
            if(Type() != Platform.CUSTOM)
                return "";
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_Tag();
#else

            return CoreSDK.platform.tag;
#endif
        }

        public static string ProgressSaveFormat()
        {
            return CoreSDK.platform.progressSaveFormat;
        }

        public static SyncStorageType PrefferedSyncType()
        {
            return CoreSDK.platform.prefferedSyncType;
        }


        [DllImport("libARWrapper.so")]
        private static extern string GP_Platform_HasIntegratedAuth();
        public static bool HasIntegratedAuth()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_HasIntegratedAuth() == "true";
#else
            bool auth = false;

            ConsoleLog("HAS INTEGRATED AUTH: " + auth.ToString());
            return auth;
#endif
        }

        [DllImport("libARWrapper.so")]
        private static extern string GP_Platform_IsLogoutAvailable();
        public static bool IsLogoutAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsLogoutAvailable() == "true";
#else
            //bool value = GP_Settings.instance.GetFromPlatformSettings().IsLogoutAvailable;
            bool value = false;

            ConsoleLog("Is Logout Available: " + value.ToString());
            return value;
#endif
        }


        [DllImport("libARWrapper.so")]
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

        [DllImport("libARWrapper.so")]
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

        [DllImport("libARWrapper.so")]
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

        public static bool IsAlwaysSyncPublicFields()
        {
            return CoreSDK.platform.alwaysSyncPublicFields;
        }

        private static Platform ConvertToEnum(string platform)
        {
            if (platform == PlatformTypes.YANDEX)
                return Platform.YANDEX;

            if (platform == PlatformTypes.VK)
                return Platform.VK;

            if (platform == PlatformTypes.CRAZY_GAMES)
                return Platform.CRAZY_GAMES;

            if (platform == PlatformTypes.GAME_DISTRIBUTION)
                return Platform.GAME_DISTRIBUTION;

            if (platform == PlatformTypes.GAME_MONETIZE)
                return Platform.GAME_MONETIZE;

            if (platform == PlatformTypes.OK)
                return Platform.OK;

            if (platform == PlatformTypes.SMARTMARKET)
                return Platform.SMARTMARKET;

            if (platform == PlatformTypes.GAMEPIX)
                return Platform.GAMEPIX;

            if (platform == PlatformTypes.POKI)
                return Platform.POKI;

            if (platform == PlatformTypes.VK_PLAY)
                return Platform.VK_PLAY;

            if (platform == PlatformTypes.WG_PLAYGROUND)
                return Platform.WG_PLAYGROUND;

            if (platform == PlatformTypes.KONGREGATE)
                return Platform.KONGREGATE;

            if (platform == PlatformTypes.GOOGLE_PLAY)
                return Platform.GOOGLE_PLAY;

            if (platform == PlatformTypes.PLAYDECK)
                return Platform.PLAYDECK;

            if (platform == PlatformTypes.CUSTOM)
                return Platform.CUSTOM;

            return Platform.None;

            //return platform switch
            //{
            //    PlatformTypes.YANDEX => Platform.YANDEX,
            //    _ => Platform.None
            //};
        }

    }
}

