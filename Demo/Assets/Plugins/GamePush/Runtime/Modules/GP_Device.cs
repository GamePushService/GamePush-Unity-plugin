using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;

using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Device : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Device);

        public static event UnityAction OnChangeOrientation;
        private void CallChangeOrientation() => OnChangeOrientation?.Invoke();

        [DllImport("__Internal")]
        private static extern string GP_IsMobile();
        public static bool IsMobile()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "true";
#else
            bool isMobile = GP_Settings.instance.GetFromPlatformSettings().IsMobile;
            
                ConsoleLog("IS MOBILE: " + isMobile.ToString());
            return isMobile;
#endif
        }
        public static bool IsDesktop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "false";
#else
            bool isDesktop = !GP_Settings.instance.GetFromPlatformSettings().IsMobile;
            
                ConsoleLog("IS DESKTOP: " + isDesktop.ToString());
            return isDesktop;
#endif
        }

        [DllImport("__Internal")]
        private static extern string GP_IsPortrait();
        public static bool IsPortrait()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsPortrait() == "true";
#else
            bool isPortrait = GP_Settings.instance.GetFromPlatformSettings().IsPortrait;
            
                ConsoleLog("IS PORTRAIT: " + isPortrait.ToString());
            return isPortrait;
#endif
        }
    }

}