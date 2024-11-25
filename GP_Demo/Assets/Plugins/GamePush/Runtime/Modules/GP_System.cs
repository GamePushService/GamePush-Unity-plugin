﻿using System.Runtime.InteropServices;

namespace GamePush
{
    public class GP_System : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.System);

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("__Internal")]
        private static extern string GP_IsDev();
        [DllImport("__Internal")]
        private static extern string GP_IsAllowedOrigin();
#endif

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