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
            //return PlatformTypes.ConvertToEnum(CoreSDK.platform.type);
            return Platform.NONE;
#endif
        }

        public static string TypeAsString()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_Type();
#else
            //return CoreSDK.platform.type;
            return "";
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
            return false;
#endif
        }

        
        public static bool IsLogoutAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsLogoutAvailable() == "true";
#else
            //return CoreSDK.platform.isLogoutAvailable;
            return false;
#endif
        }


        public static bool IsExternalLinksAllowed()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsExternalLinksAllowed() == "true";
#else
            //return CoreSDK.platform.isExternalLinksAllowed;
            return false;
#endif
        }

        public static bool IsSecretCodeAuthAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsSecretCodeAuthAvailable() == "true";
#else
            //return CoreSDK.platform.isSecretCodeAuthAvailable;
            return false;
#endif
        }

        
        public static bool IsSupportsCloudSaves()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_Platform_IsSupportsCloudSaves() == "true";
#else
            //return CoreSDK.platform.isSupportsCloudSaves;
            return false;
#endif
        }

        public static bool IsAlwaysSyncPublicFields()
        {
            //return CoreSDK.platform.alwaysSyncPublicFields;
            return false;
        }

    }
}

