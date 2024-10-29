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

        private void OnEnable()
        {
            CoreSDK.device.OnChangeOrientation += CallChangeOrientation;
        }

        private void OnDisable()
        {
            CoreSDK.device.OnChangeOrientation -= CallChangeOrientation;
        }

#if !UNITY_EDITOR && UNITY_WEBGL
        [DllImport("libARWrapper.so")]
        private static extern string GP_IsMobile();
        
        [DllImport("libARWrapper.so")]
        private static extern string GP_IsPortrait();
#endif

        public static bool IsMobile()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "true";
#else
            return CoreSDK.device.isMobile;
#endif
        }
        public static bool IsDesktop()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsMobile() == "false";
#else
            return !CoreSDK.device.isMobile;
#endif
        }

        public static bool IsPortrait()
        {
#if !UNITY_EDITOR && UNITY_WEBGL
            return GP_IsPortrait() == "true";
#else
            return CoreSDK.device.isPortrait;
#endif
        }
    }

}