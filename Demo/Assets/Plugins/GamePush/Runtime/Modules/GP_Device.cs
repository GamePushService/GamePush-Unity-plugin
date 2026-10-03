using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Events;
using GamePush.Native;

using GamePush.ConsoleController;

namespace GamePush
{
    public class GP_Device : GP_Module
    {
        private static void ConsoleLog(string log) => GP_Logger.ModuleLog(log, ModuleName.Device);

        public static event UnityAction OnChangeOrientation;
        private void CallChangeOrientation() => OnChangeOrientation?.Invoke();

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_IsMobile();
        #endif
        public static bool IsMobile()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_IsMobile() == "true";
#else
            if (GamePushHost.UseNativeCore)
                return Application.isMobilePlatform;
            if (GP_Play2Web.TryGetBool("IsMobile", out var live))
                return live;
            bool isMobile = GP_Settings.instance.GetFromPlatformSettings().IsMobile;
            
                ConsoleLog("IS MOBILE: " + isMobile.ToString());
            return isMobile;
#endif
        }
        public static bool IsDesktop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_IsMobile() == "false";
#else
            if (GamePushHost.UseNativeCore)
                return !Application.isMobilePlatform;
            if (GP_Play2Web.TryGetBool("IsMobile", out var liveMobile))
                return !liveMobile;
            bool isDesktop = !GP_Settings.instance.GetFromPlatformSettings().IsMobile;
            
                ConsoleLog("IS DESKTOP: " + isDesktop.ToString());
            return isDesktop;
#endif
        }

        #if UNITY_WEBGL && !UNITY_EDITOR
        [DllImport("__Internal")]
        private static extern string GP_IsPortrait();
        #endif
        public static bool IsPortrait()
        {
#if !UNITY_EDITOR && UNITY_WEBGL && !GP_NATIVE_WEBGL
            return GP_IsPortrait() == "true";
#else
            if (GP_Play2Web.TryGetBool("IsPortrait", out var live))
                return live;
            bool isPortrait = GP_Settings.instance.GetFromPlatformSettings().IsPortrait;
            
                ConsoleLog("IS PORTRAIT: " + isPortrait.ToString());
            return isPortrait;
#endif
        }
    }

}