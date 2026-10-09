using System.Runtime.InteropServices;
using UnityEngine;
using GamePush.Data;
using GamePush.Native;

namespace GamePush
{
    public class GP_Platform : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Platform);

#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
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
        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_Platform_IsSupportsCloudSaves();
        [DllImport("__Internal")]
        private static extern string GP_Platform_IsBackendAllowed();
        [DllImport("__Internal")]
        private static extern string GP_Platform_IsChatAvailable();
        #endif
#endif

        public static Platform Type()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return PlatformTypes.ConvertToEnum(GP_Platform_Type());
#else
            if (GamePushHost.UseNativeCore)
                return PlatformTypes.ConvertToEnum(NativeCore.PlatformType);
            if (GP_Play2Web.TryGet("PlatformType", out var live))
                return PlatformTypes.ConvertToEnum(live);
            Platform platform = GP_Settings.instance.GetFromPlatformSettings().PlatformToEmulate;
            return platform;
#endif
        }

        public static string TypeAsString()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_Type();
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.PlatformType;
            if (GP_Play2Web.TryGet("PlatformType", out var live))
                return live;
            Platform platform = GP_Settings.instance.GetFromPlatformSettings().PlatformToEmulate;
            return platform.ToString();
#endif
        }

        
        public static string Tag()
        {
            if(Type() != Platform.CUSTOM)
                return "";
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_Tag();
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.PlatformTag ?? "";
            if (GP_Play2Web.TryGet("PlatformTag", out var live))
                return live;
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
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_HasIntegratedAuth() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.Auth.HasIntegratedAuth;
            if (GP_Play2Web.TryGetBool("PlatformHasIntegratedAuth", out var live))
                return live;
            return GP_Settings.instance.GetFromPlatformSettings().HasIntegratedAuth;
#endif
        }

        
        public static bool IsLogoutAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_IsLogoutAvailable() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.Auth.IsLogoutAvailable;
            if (GP_Play2Web.TryGetBool("PlatformIsLogoutAvailable", out var live))
                return live;
            return GP_Settings.instance.GetFromPlatformSettings().IsLogoutAvailable;
#endif
        }


        public static bool IsExternalLinksAllowed()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_IsExternalLinksAllowed() == "true";
#else
            if (GP_Play2Web.TryGetBool("PlatformIsExternalLinksAllowed", out var live))
                return live;
            return GP_Settings.instance.GetFromPlatformSettings().IsExternalLinksAllowed;
#endif
        }

        public static bool IsSecretCodeAuthAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_IsSecretCodeAuthAvailable() == "true";
#else
            if (GP_Play2Web.TryGetBool("PlatformIsSecretCodeAuthAvailable", out var live))
                return live;
            return GP_Settings.instance.GetFromPlatformSettings().IsSecretCodeAuthAvailable;
#endif
        }

        
        public static bool IsSupportsCloudSaves()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_IsSupportsCloudSaves() == "true";
#else
            if (GP_Play2Web.TryGetBool("PlatformIsSupportsCloudSaves", out var live))
                return live;
            return GP_Settings.instance.GetFromPlatformSettings().IsSupportsCloudSaves;
#endif
        }

        public static bool IsBackendAllowed()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_IsBackendAllowed() == "true";
#else
            if (GP_Play2Web.TryGetBool("PlatformIsBackendAllowed", out var live))
                return live;
            return GP_Settings.instance.GetFromPlatformSettings().IsBackendAllowed;
#endif
        }

        public static bool IsChatAvailable()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_Platform_IsChatAvailable() == "true";
#else
            if (GP_Play2Web.TryGetBool("PlatformIsChatAvailable", out var live))
                return live;
            return GP_Settings.instance.GetFromPlatformSettings().IsChatAvailable;
#endif
        }

        public static bool IsAlwaysSyncPublicFields()
        {
            //return CoreSDK.platform.alwaysSyncPublicFields;
            return false;
        }

    }
}

