using System.Runtime.InteropServices;
using UnityEngine;
using GamePush.Data;

namespace GamePush
{
    public class GP_Platform : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Platform);

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_Platform_Type();
        [DllImport("__Internal")]
        private static extern string GP_Platform_Tag();
        [DllImport("__Internal")]
        private static extern string GP_Platform_HasIntegratedAuth();
        [DllImport("__Internal")]
        private static extern string GP_Platform_IsLogoutAvailable();
        [DllImport("__Internal")]
        private static extern string GP_Platform_IsExternalLinksAllowed();
        [DllImport("__Internal")]
        private static extern string GP_Platform_IsSecretCodeAuthAvailable();
        [DllImport("__Internal")]
        private static extern string GP_Platform_IsSupportsCloudSaves();
#endif

        public static Platform Type()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return PlatformTypes.ConvertToEnum(GP_Platform_Type());
#else
            Platform platform = GP_Settings.instance.GetFromPlatformSettings().PlatformToEmulate;
            //return PlatformTypes.ConvertToEnum(CoreSDK.platform.type);
            return platform;
#endif
        }

        public static string TypeAsString()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_Type();
#else
            Platform platform = GP_Settings.instance.GetFromPlatformSettings().PlatformToEmulate;
            //return CoreSDK.platform.type;
            return platform.ToString();
#endif
        }

        
        public static string Tag()
        {
            if(Type() != Platform.CUSTOM)
                return "";
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_Tag();
#else
            //return CoreSDK.platform.tag;
            return "";
#endif
        }

        public static string ProgressSaveFormat()
        {
            //return CoreSDK.platform.progressSaveFormat;
            return "";
        }

        //public static SyncStorageType PrefferedSyncType()
        //{
        //   return CoreSDK.platform.prefferedSyncType;
        //}

        
        public static bool HasIntegratedAuth()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_HasIntegratedAuth() == "true";
#else
            //return CoreSDK.platform.hasIntegratedAuth;
            
            return GP_Settings.instance.GetFromPlatformSettings().HasIntegratedAuth;
#endif
        }

        
        public static bool IsLogoutAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsLogoutAvailable() == "true";
#else
            //return CoreSDK.platform.isLogoutAvailable;
            return GP_Settings.instance.GetFromPlatformSettings().IsLogoutAvailable;
#endif
        }


        public static bool IsExternalLinksAllowed()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsExternalLinksAllowed() == "true";
#else
            //return CoreSDK.platform.isExternalLinksAllowed;
            return GP_Settings.instance.GetFromPlatformSettings().IsExternalLinksAllowed;
#endif
        }

        public static bool IsSecretCodeAuthAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsSecretCodeAuthAvailable() == "true";
#else
            //return CoreSDK.platform.isSecretCodeAuthAvailable;
            return GP_Settings.instance.GetFromPlatformSettings().IsSecretCodeAuthAvailable;
#endif
        }

        
        public static bool IsSupportsCloudSaves()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsSupportsCloudSaves() == "true";
#else
            //return CoreSDK.platform.isSupportsCloudSaves;
            return GP_Settings.instance.GetFromPlatformSettings().IsSupportsCloudSaves;
#endif
        }

        public static bool IsAlwaysSyncPublicFields()
        {
            //return CoreSDK.platform.alwaysSyncPublicFields;
            return false;
        }

    }
}

