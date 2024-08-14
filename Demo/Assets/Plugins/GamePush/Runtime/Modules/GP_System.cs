using System.Runtime.InteropServices;

namespace GamePush
{
    public class GP_System : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.System);

        [DllImport("__Internal")]
        private static extern string GP_IsDev();
        public static bool IsDev()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsDev() == "true";
#else
            bool isVal = GP_Settings.instance.GetFromPlatformSettings().IsDev;
            ConsoleLog("IS DEV: " + isVal);
            return isVal;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_IsAllowedOrigin();
        public static bool IsAllowedOrigin()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsAllowedOrigin() == "true";
#else
            bool isVal = GP_Settings.instance.GetFromPlatformSettings().IsAllowedOrigin;
            ConsoleLog("IS ALLOWED ORIGIN: " + isVal);
            return isVal;
#endif
        }


    }

}