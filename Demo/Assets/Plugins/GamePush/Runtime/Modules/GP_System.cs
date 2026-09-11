using System.Runtime.InteropServices;
using GamePush.Native;

namespace GamePush
{
    public class GP_System : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.System);

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_IsDev();
        #endif
        public static bool IsDev()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsDev() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.IsDev;
            if (GP_Play2Web.TryGetBool("IsDev", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetFromPlatformSettings().IsDev;
            ConsoleLog("IS DEV: " + isVal);
            return isVal;
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_IsAllowedOrigin();
        #endif
        public static bool IsAllowedOrigin()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsAllowedOrigin() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return NativeCore.IsAllowedOrigin;
            if (GP_Play2Web.TryGetBool("IsAllowedOrigin", out var live))
                return live;
            bool isVal = GP_Settings.instance.GetFromPlatformSettings().IsAllowedOrigin;
            ConsoleLog("IS ALLOWED ORIGIN: " + isVal);
            return isVal;
#endif
        }


    }

}